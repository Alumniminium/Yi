using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YiX.AI.Workers;
using YiX.Calculations;
using YiX.Database;
using YiX.Entities;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Sockets;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;

namespace YiX.Network.Packets.Conquer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct MsgText
    {
        public ushort Size;
        public ushort Id;
        public int Color;
        public MsgTextType Channel;
        public short Style;
        public int SenderUniqueId;
        public int Look1;
        public int Look2;
        public byte StringCount;
        public unsafe fixed byte Data[304];

        public unsafe string From()
        {
            fixed (byte* p = Data)
            {
                var fromLen = p[0];
                var txtBytes = new byte[fromLen];
                for (var i = 0; i < txtBytes.Length; i++)
                    txtBytes[i] = p[1 + i];
                return Encoding.ASCII.GetString(txtBytes);
            }
        }

        public unsafe string To()
        {
            fixed (byte* p = Data)
            {
                var fromLen = p[0];
                var toLen = p[1 + fromLen];
                var txtBytes = new byte[toLen];
                for (var i = 0; i < txtBytes.Length; i++)
                    txtBytes[i] = p[2 + fromLen + i];
                return Encoding.ASCII.GetString(txtBytes);
            }
        }

        public unsafe string Message()
        {
            fixed (byte* p = Data)
            {
                var fromLen = p[0];
                var toLen = p[1 + fromLen];
                var emoLen = p[2 + fromLen + toLen];
                var msgLen = p[3 + emoLen + fromLen + toLen];
                var txtBytes = new byte[msgLen];
                for (var i = 0; i < txtBytes.Length; i++)
                    txtBytes[i] = p[4 + fromLen + toLen + emoLen + i];
                return Encoding.ASCII.GetString(txtBytes);
            }
        }

        public static unsafe byte[] Create(string from, string to, string message, MsgTextType type)
        {
            from = from.Replace("\0", "");
            to = to.Replace("\0", "");
            message = message.Replace("\0", "");


            // ReSharper disable once UseObjectOrCollectionInitializer
            var packet = new MsgText
            {
                Size = (ushort)(29 + from.Length + to.Length + message.Length),
                Id = 1004,
                StringCount = 4,
                Channel = type,
                SenderUniqueId = Environment.TickCount,
                Color = 0x00FF00FF,
            };

            if (GameWorld.Find(to, out YiObj found))
                packet.Look1 = (int)found.Look;
            if (GameWorld.Find(from, out YiObj found1))
                packet.Look2 = (int)found1.Look;


            packet.Data[0] = (byte)from.Length;
            for (var i = 0; i < from.Length; i++)
                packet.Data[1 + i] = (byte)from[i];
            packet.Data[1 + from.Length] = (byte)to.Length;
            for (var i = 0; i < to.Length; i++)
                packet.Data[2 + from.Length + i] = (byte)to[i];
            packet.Data[2 + from.Length + to.Length] = 0;
            packet.Data[3 + from.Length + to.Length] = (byte)message.Length;
            for (var i = 0; i < message.Length; i++)
                packet.Data[4 + from.Length + to.Length + i] = (byte)message[i];
            return packet;
        }

        public static async void Handle(Player player, MsgText packet)
        {
            try
            {

                Output.WriteLine($"[{DateTime.UtcNow:G}] | [{packet.Channel}] {player.Name.Replace("\0", "")} - {packet.To().Replace("\0", "")} |=> {packet.Message().Replace("\0", "")}");

                ChatLog.WriteLine(player, packet.To(), packet.Message(), packet.Channel);

                switch (packet.Channel)
                {
                    case MsgTextType.Ghost:
                        GhostChat(player, ref packet);
                        break;
                    case MsgTextType.Talk:
                        TalkChat(player, ref packet);
                        break;
                    case MsgTextType.Whisper:
                        WhisperChat(player, ref packet);
                        break;
                    case MsgTextType.Team:
                        TeamChat(player, ref packet);
                        break;
                    case MsgTextType.Guild:
                        GuildChat(player, ref packet);
                        break;
                    case MsgTextType.Friend:
                        FriendChat(player, ref packet);
                        break;
                    case MsgTextType.Broadcast:
                        BroadcastChat(player, ref packet);
                        break;
                    case MsgTextType.Service:
                        ServiceChat(player, ref packet);
                        break;
                    case MsgTextType.VendorHawk:
                        VendorHawk(player, ref packet);
                        break;
                    case MsgTextType.TradeBoard:
                        break;
                    case MsgTextType.FriendBoard:
                        break;
                    case MsgTextType.TeamBoard:
                        break;
                    case MsgTextType.GuildBulletin:
                        GuildBulletin(player, ref packet);
                        break;
                    case MsgTextType.GuildBoard:
                        GuildBoard(player, ref packet);
                        break;
                    case MsgTextType.OthersBoard:
                        break;
                    default:
                        Output.WriteLine("Unknown ChatType: " + packet.Channel);
                        Output.WriteLine(((byte[])packet).HexDump());
                        break;
                }


                var msg = packet.Message();
                var frm = packet.From();
                var to = packet.To();
                var command = msg.Split(' ');
                switch (command[0])
                {
                    case "scroll":
                        {
                            switch (command[1].ToLower())
                            {
                                case "tc":
                                    player.Teleport(430, 380, 1002);
                                    break;
                                case "pc":
                                    player.Teleport(195, 260, 1011);
                                    break;
                                case "ac":
                                case "am":
                                    player.Teleport(566, 563, 1020);
                                    break;
                                case "dc":
                                    player.Teleport(500, 645, 1000);
                                    break;
                                case "bi":
                                    player.Teleport(723, 573, 1015);
                                    break;
                                case "pka":
                                    player.Teleport(050, 050, 1005);
                                    break;
                                case "ma":
                                    player.Teleport(211, 196, 1036);
                                    break;
                                case "ja":
                                    player.Teleport(100, 100, 6000);
                                    break;
                            }
                            break;
                        }
                    //case "testsave":
                    //    var stopwatch = Stopwatch.StartNew();
                    //    var serializer = new JsonSerializer
                    //    {
                    //        DefaultValueHandling = DefaultValueHandling.Include,
                    //        NullValueHandling = NullValueHandling.Ignore,
                    //        TypeNameHandling = TypeNameHandling.Auto,
                    //        Formatting = Formatting.Indented,
                    //        Converters = { new BoolConverter() }
                    //    };

                    //    var compressed = new byte[1];

                    //    using (var memStream = new MemoryStream())
                    //    {
                    //        using (var stream = new StreamWriter(memStream))
                    //        {
                    //            serializer.Serialize(stream, player, player.GetType());
                    //            stream.Flush();
                    //            var data = memStream.GetBuffer();
                    //            compressed = Content.QuickLz.Compress(data, 0, (uint) data.Length);
                    //            memStream.Seek(0, SeekOrigin.Begin);
                    //        }
                    //    }
                    //    using (var compressedStream = new MemoryStream(compressed))
                    //    {
                    //        TcpClient client = null;
                    //        NetworkStream netstream = null;
                    //        try
                    //        {
                    //            client = new TcpClient(YiCore.ServerIp, 9959);
                    //            netstream = client.GetStream();
                    //            var noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(compressedStream.Length) / Convert.ToDouble(1024)));
                    //            var totalLength = (int)compressedStream.Length;
                    //            for (var i = 0; i < noOfPackets; i++)
                    //            {
                    //                int currentPacketLength;
                    //                if (totalLength > 1024)
                    //                {
                    //                    currentPacketLength = 1024;
                    //                    totalLength = totalLength - currentPacketLength;
                    //                }
                    //                else
                    //                    currentPacketLength = totalLength;
                    //                var sendingBuffer = new byte[currentPacketLength];

                    //                compressedStream.Read(sendingBuffer, 0, currentPacketLength);
                    //                netstream.Write(sendingBuffer, 0, sendingBuffer.Length);
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Console.WriteLine(ex.Message);
                    //        }
                    //        finally
                    //        {
                    //            netstream?.Close();
                    //            client?.Close();
                    //        }

                    //        stopwatch.Stop();

                    //        player.GetMessage("SYSTEM", "ALLUSERS", $"Serializing & Compression took {stopwatch.Elapsed.TotalMilliseconds:F1}ms.", MsgTextType.Action);
                    //    }
                    //    break;
                    case "rev":
                        MsgAction.Handle(player, MsgAction.Create(player, player.UniqueId, MsgActionType.MapShow));
                        player.Respawn();
                        break;
                    case "tp":
                        {
                            player.Teleport(ushort.Parse(command[1]), ushort.Parse(command[2]), ushort.Parse(command[3]));
                            break;
                        }
                    case "gc":
                        YiCore.CompactLoh();
                        //NativeMethods.MinimizeFootprint();
                        GCSettings.LatencyMode = GCLatencyMode.Interactive;
                        break;
                    case "dura+":
                        player.Equipment.AddDura(MsgItemPosition.LeftWeapon);
                        break;
                    case "dura-":
                        player.Equipment.RemoveDura(MsgItemPosition.LeftWeapon);
                        break;
                    case "resetcolor":
                        player.Send(MsgColor.Create(player, 0));
                        break;
                    case "getweather":
                        WeatherSystem.Start();
                        break;
                    case "weather":
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.Atoms, 150, 40, 0));
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.AutumnLeaves, 150, 40, 0));
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.BlowingCotten, 150, 40, 0));
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.CherryBlossomPetals, 150, 40, 0));
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.CherryBlossomPetalsWind, 150, 40, 0));
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.Rain, 150, 40, 0));
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.RainWind, 150, 40, 0));
                        Thread.Sleep(5000);
                        player.Send(MsgWeather.Create(WeatherType.Snow, 150, 40, 0));
                        break;
                    case "color":
                        if (command.Length < 2)
                        {
                            player.Send(Create("Server", player.Name, "Usage: color HEX", MsgTextType.Talk));
                            break;
                        }
                        player.Send(MsgColor.Create(player, command[1].ToHex()));
                        player.Send(Create("Server", player.Name, $"Color: {command[1].ToHex()}", MsgTextType.Talk));
                        break;
                    case "color2":
                        player.Send(MsgColor.Create(player, System.Drawing.Color.Aqua));
                        player.Send(Create("Server", player.Name, $"Color: {System.Drawing.Color.Aqua}", MsgTextType.Talk));
                        break;
                    case "locate":
                        {
                            await Task.Run(() =>
                            {
                                foreach (var o in from o in GameWorld.Maps[player.MapId].Entities.Values.OfType<Monster>() let mob = o where mob.Name.Contains("Guard") select o)
                                {
                                    player.Send(MsgAction.Create(Environment.TickCount, o.UniqueId, 0, o.Location.X, o.Location.Y, 0, MsgActionType.QueryTeamMemberPos));
                                    Thread.Sleep(1000);
                                }
                            });
                            break;
                        }
                    case "snow":
                        player.Skills.Add(SkillId.Snow, new Skill((ushort)SkillId.Snow, 6, 0));
                        break;
                    case "npctest":
                        player.ForceSend(MsgNpc.Create(player, ushort.Parse(command[1]), MsgNpcAction.LayNpc), 16);
                        break;
                    case "guild":
                        player.Guild = new Guild(player, "Equinox");
                        break;
                    case "dc":
                        player.Disconnect();
                        break;
                    case "awake":
                        foreach (var obj in ScreenSystem.GetEntities(player))
                            (obj as Monster)?.Brain?.Think();
                        break;
                    case "transfer":
                        ConcurrentQueue<byte[]> ignored;
                        OutgoingPacketQueue.PacketQueue.TryRemove(player, out ignored);
                        player.ForceSend(MsgAction.Create(Environment.TickCount, player.UniqueId, 0, 0, 0, 0, MsgActionType.ChangeMap), 24);
                        player.ForceSend(LegacyPackets.MsgTransfer(player.UniqueId, player.UniqueId, 5816), 32);
                        break;
                    case "transferback":
                        OutgoingPacketQueue.PacketQueue.TryRemove(player, out ignored);
                        player.ForceSend(MsgAction.Create(Environment.TickCount, player.UniqueId, 0, 0, 0, 0, MsgActionType.ChangeMap), 24);
                        player.ForceSend(LegacyPackets.MsgTransfer(player.UniqueId, player.UniqueId, 9958), 32);
                        break;
                    case "path":
                        var sw = Stopwatch.StartNew();
                        foreach (var mob in GameWorld.Maps[player.MapId].Entities.Values.OfType<Monster>())
                            GameWorld.Maps[mob.MapId].Path(mob, mob.Location.X, mob.Location.Y);
                        sw.Stop();
                        Console.Title = $"Pathing for {GameWorld.Maps[player.MapId].Entities.Values.OfType<Monster>().Count()} took {sw.Elapsed.TotalMilliseconds} ms";
                        break;
                    case "status":
                        player.RemoveStatusEffect(StatusEffect.Die);
                        break;
                    case "nostatus":
                        player.RemoveStatusEffect(StatusEffect.SuperMan);
                        break;
                    case "money":
                        player.Money += 1000000;
                        break;
                    case "lvl":
                        byte.TryParse(command[1], out var newlvl);
                        player.Level = newlvl;
                        EntityLogic.Recalculate(player);
                        break;
                    case "job":
                        byte.TryParse(command[1], out var newclass);
                        player.Class = newclass;
                        EntityLogic.Recalculate(player);
                        break;
                    case "tc":
                        player.Teleport(438, 377, 1002);
                        break;
                    case "bv":
                        player.Teleport(650, 650, 1015);
                        break;
                    case "pet":
                        var pet = new Monster();
                        pet.Id = 900;
                        pet.Look = 920;
                        pet.UniqueId = 700000 + player.UniqueId - 1000000;
                        pet.MapId = player.MapId;
                        pet.Location.X = player.Location.X;
                        pet.Location.Y = player.Location.Y;
                        player.Pet = pet;
                        GameWorld.Maps[pet.MapId].Enter(pet);
                        ScreenSystem.Create(player.Pet);
                        ScreenSystem.Update(player.Pet);
                        ScreenSystem.SendSpawn(pet);
                        player.Send(MsgAssignPet.Create(player, pet.UniqueId));
                        break;
                    case "testreborn":
                        {
                            if (command.Length == 2)
                            {
                                byte.TryParse(command[1], out var newjob);
                                PlayerReborn.FirstReborn(player, newjob);
                            }
                            break;
                        }
                    case "skill":
                        player.Skills = new Dictionary<SkillId, Skill>
                        {
                            [SkillId.Thunder] = new Skill(1000, 0, 0),
                            [SkillId.Fire] = new Skill(1001, 0, 0),
                            [SkillId.Tornado] = new Skill(1002, 0, 0),
                            [SkillId.Cyclone] = new Skill(1110, 0, 0),
                            [SkillId.Shield] = new Skill(1020, 0, 0),
                            [SkillId.Superman] = new Skill(1025, 0, 0),
                            [SkillId.AdvancedDispel] = new Skill(1070, 0, 0),
                            [SkillId.Invisibility] = new Skill(1075, 0, 0),
                            [SkillId.MagicShield] = new Skill(1090, 1, 0),
                            [SkillId.RapidFire] = new Skill(8000, 5, 0),
                            [SkillId.Scatter] = new Skill(8001, 5, 0),
                            [SkillId.Fly] = new Skill(8002, 0, 0),
                            [SkillId.AdvancedFly] = new Skill(8003, 0, 0),
                            [SkillId.ArrowRain] = new Skill(8030, 0, 0),
                            [SkillId.Penetration] = new Skill((ushort)SkillId.Penetration, 0, 0),
                            [SkillId.Snow] = new Skill((ushort)SkillId.Snow, 0, 0),
                        };
                        player.Skills.Add(SkillId.FireCircle, new Skill((ushort)SkillId.FireCircle, 2, 0));
                        player.MaximumMp = 10000;
                        player.CurrentMp = 10000;
                        player.MaximumHp = 10000;
                        player.CurrentHp = 10000;
                        player.Class = 135;
                        player.Spirit = 100;
                        player.Stamina = byte.MaxValue;

                        foreach (var skill in player.Skills)
                            player.Send(MsgSkill.Create(skill.Value));

                        break;
                    case "prof":
                        player.Profs = new Dictionary<int, Prof> { [134] = new Prof(134, 10, 0), [133] = new Prof(133, 10, 0), [130] = new Prof(130, 10, 0), [131] = new Prof(131, 10, 0), [117] = new Prof(117, 10, 0), [114] = new Prof(114, 10, 0), [113] = new Prof(113, 10, 0), [118] = new Prof(118, 10, 0), [111] = new Prof(111, 10, 0), [121] = new Prof(121, 10, 0), [120] = new Prof(120, 10, 0), [150] = new Prof(150, 10, 0), [151] = new Prof(151, 10, 0), [152] = new Prof(152, 10, 0), [160] = new Prof(160, 10, 0), [900] = new Prof(900, 10, 0), [400] = new Prof(400, 10, 0), [410] = new Prof(410, 10, 0), [420] = new Prof(420, 10, 0), [421] = new Prof(421, 10, 0), [430] = new Prof(430, 10, 0), [440] = new Prof(440, 10, 0), [450] = new Prof(450, 10, 0), [460] = new Prof(460, 10, 0), [480] = new Prof(480, 10, 0), [481] = new Prof(481, 10, 0), [490] = new Prof(490, 10, 0), [500] = new Prof(500, 20, 0), [510] = new Prof(510, 10, 0), [530] = new Prof(530, 10, 0), [540] = new Prof(540, 10, 0), [560] = new Prof(560, 10, 0), [561] = new Prof(561, 10, 0), [562] = new Prof(562, 10, 0), [580] = new Prof(580, 10, 0) };
                        foreach (var prof in player.Profs)
                            player.Send(MsgProf.Create(prof.Value));
                        break;
                    case "item":
                        {
                            try
                            {
                                int.TryParse(command[1], out var id);
                                byte.TryParse(command[2], out var plus);
                                byte.TryParse(command[3], out var bless);
                                byte.TryParse(command[4], out var enchant);
                                byte.TryParse(command[5], out var gem1);
                                byte.TryParse(command[6], out var gem2);

                                if (Collections.Items.ContainsKey(id))
                                {
                                    var cloned = ItemFactory.Create(id);
                                    cloned.Plus = Math.Min(plus, (byte)9);
                                    cloned.Bless = Math.Min(bless, (byte)7);
                                    cloned.Enchant = Math.Min(enchant, (byte)255);
                                    cloned.Gem1 = Math.Min(gem1, (byte)255);
                                    cloned.Gem2 = Math.Min(gem2, (byte)255);
                                    player.Inventory.AddItem(cloned);
                                    break;
                                }

                                Helpers.Message.SendTo(player, "Your id was invalid.", MsgTextType.Center);
                                break;
                            }
                            catch
                            {
                                Helpers.Message.SendTo(player, "Your item command had null values. Use 'item ID PLUS BLESS ENCHANT GEM1 GEM2'", MsgTextType.Center);
                                break;
                            }
                        }
                    case "craft":
                        player.Send(MsgAction.Create(player, 1088, MsgActionType.RemoteCommands)); //Crafting Window
                        break;
                    case "bottest":
                        {
                            var bot = new Bot
                            {
                                UniqueId = player.UniqueId + 10,
                                Look = player.Look,
                                Name = "Im_a_Bot",
                                Partner = "Im_a_Bot",
                                Location =
                            {
                                X = player.Location.X,
                                Y = player.Location.Y
                            },
                                MapId = player.MapId
                            };
                            GameWorld.Maps[bot.MapId].LoadInEntity(bot);
                            ScreenSystem.SendSpawn(bot);

                            var workerAI = new WorkerAI(bot);

                            workerAI.WorkerQueue.Enqueue(new WorkerTask(new Vector2(439, 379), false, null));

                            new Thread(() =>
                            {
                                while (true)
                                {
                                    workerAI.Think();
                                    Thread.Sleep(1000);
                                }
                            }).Start();

                            break;
                        }
                    case "test":
                        //for (uint i = 0; i < 10; i++)
                        //{
                        //    for (uint j = 0; j < 10; j++)
                        //    {
                        //    }
                        //    Thread.Sleep(1000);
                        //}
                        //player.ForceSend(MsgAction.Create(player, player.MapId,MsgActionType.ChangeMap), 24);
                        player.ForceSend(MsgUpdate.Create(player, 1, (MsgUpdateType)24), 24);
                        player.ForceSend(MsgAction.Pathfinding(player), 24);
                        player.ForceSend(MsgAction.Create(player, 280, (MsgActionType)280), 24);
                        player.ForceSend(MsgUpdate.Create(player, 0, MsgUpdateType.Nobility), 24);
                        //player.Send(MsgAction.CreateFor(player, player.BoothId, MsgActionType.Shop));
                        //StorageSystem.NewStorageTest(player, 8);
                        //StorageSystem.NewStorageTest(player, 8);
                        //StorageSystem.NewStorageTest(player, 8);

                        //foreach (var playerList in SelectorSystem.Players.Values)
                        //{
                        //    foreach (var pl in playerList)
                        //    {
                        //        GameWorld.Maps[pl.MapId].LoadInEntity(pl);
                        //    }
                        //}

                        //foreach (var item1 in Collections.Items)
                        //{
                        //    item1.Type.CustomTextId = 1000000001;
                        //}
                        //foreach (var yiObj in Collections.Npcs)
                        //{
                        //    if (yiObj.Type.Inventory?.Count > 0)
                        //        BoothSystem.Create(yiObj.Type);
                        //}
                        //Task.Run(() =>
                        //{
                        //    for (int i = 0; i < 1000; i++)
                        //    {
                        //        player.Send(new MsgNpc
                        //        {
                        //            Action = MsgNpcAction.LayNpc,
                        //            UniqId = player.UniqueId,
                        //            Param = (uint)i,
                        //        });
                        //        player.Send(CreateFor(player.Name, player.Name, $"Param: {i}", MsgTextType.Talk));
                        //        Thread.Sleep(100);
                        //    }
                        //});
                        //Task.Run(() =>
                        //{
                        //    for (int i = 27; i < 1000; i++)
                        //    {
                        //        player.Send(MsgUpdate.CreateFor(player, 123, (MsgUpdateType) i));
                        //        player.Send(CreateFor(player.Name, player.Name, $"Param: {i}", MsgTextType.Talk));
                        //        Thread.Sleep(1000);
                        //    }
                        //});
                        //Task.Run(() =>
                        //{
                        //    for (ushort i = 1150; i < 5200; i++)
                        //    {
                        //        player.Send(MsgAction.Create2(player, i, MsgActionType.RemoteCommands));
                        //        player.Send(CreateFor(player.Name, player.Name, $"Param: {i}", MsgTextType.Talk));
                        //        Thread.Sleep(100);
                        //    }
                        //});
                        //var owner = Collections.Npcs[1];
                        //owner.Inventory = new Inventory(owner);
                        //for (int i = 0; i < 100; i++)
                        //{
                        //    owner.Inventory.AddBypass(Item.Factory.CreateFor(ItemNames.Dragonball));
                        //}
                        //BoothSystem.CreateFor(owner);
                        //var count = 0;
                        //foreach (var kvp in owner.Inventory)
                        //{
                        //    count++;
                        //    BoothSystem.Add(owner, kvp.Key, (uint)count);
                        //}
                        break;
                    case "suck":
                        BlackHoleSystem.Create(player);
                        BlackHoleSystem.Suck();
                        break;
                    case "removemob":
                        Collections.Monsters.TryRemove(player.CurrentTarget.UniqueId, out var idc);
                        Db.Serialize("Monsters", Collections.Monsters);
                        ScreenSystem.Update(player);
                        break;
                }
            }
            catch (Exception e)
            {
                Output.WriteLine(e);
            }
        }

        private static void GuildChat(Player player, ref MsgText packet)
        {
            if (player.Guild == null)
                return;

            foreach (var guildMember in player.Guild.Members)
            {
                if (GameWorld.Find(guildMember, out Player found))
                    found.GetMessage(packet.From(), packet.To(), packet.Message(), packet.Channel);
            }
        }

        private static void FriendChat(Player player, ref MsgText packet)
        {
            foreach (var friendUniqueId in player.Friends)
            {
                if (GameWorld.Find(friendUniqueId, out Player friend))
                    friend.GetMessage(packet.From(), packet.To(), packet.Message(), packet.Channel);
            }
        }

        private static void GhostChat(Player player, ref MsgText packet)
        {
            foreach (var entity in ScreenSystem.GetEntities(player))
            {
                if (entity.Class > 139 && entity.Class < 150)
                    entity.GetMessage(player.Name, packet.To(), packet.Message(), MsgTextType.Ghost);
            }
        }

        private static void TalkChat(Player player, ref MsgText packet)
        {
            foreach (var entity in ScreenSystem.GetEntities(player))
                entity.GetMessage(player.Name, packet.To(), packet.Message(), MsgTextType.Talk);
        }

        private static void WhisperChat(Player player, ref MsgText packet)
        {
            if (GameWorld.Find(packet.To().Trim(), out Player to))
            {
                MsgText msg = Create(player.Name.Trim('\0'), packet.To().Trim('\0'), packet.Message().Trim('\0'), packet.Channel);
                to.ForceSend(msg, msg.Size);
            }
            else
                Helpers.Message.SendTo(player, "Target offline.", MsgTextType.Action);
        }

        private static void TeamChat(Player player, ref MsgText packet)
        {
            if (!TeamSystem.Teams.ContainsKey(player.UniqueId))
                return;

            foreach (var member in TeamSystem.Teams[player.UniqueId].Members.Where(member => member.Value is Player && member.Key != player.UniqueId))
                (member.Value as Player)?.ForceSend(packet, packet.Size);
        }

        private static void BroadcastChat(Player player, ref MsgText packet)
        {
            foreach (var p in GameWorld.Maps.Values.SelectMany(map => map.Entities.Values.OfType<Player>()).Where(p => p.UniqueId != player.UniqueId))
                p.ForceSend(packet, packet.Size);
        }

        private static void ServiceChat(Player player, ref MsgText packet)
        {
            foreach (var map in GameWorld.Maps.Values)
                foreach (var p in map.Entities.Values.OfType<Player>().Where(p => p.UniqueId != player.UniqueId))
                    p.ForceSend(packet, packet.Size);
        }

        private static void VendorHawk(Player player, ref MsgText packet)
        {
            foreach (var p in GameWorld.Maps[player.MapId].Entities.Values.OfType<Player>())
                p.ForceSend(packet, packet.Size);
        }

        private static void GuildBulletin(Player player, ref MsgText packet)
        {
            player.GetMessage("SYSTEM", "ALLUSERS", player.Guild.Bulletin, packet.Channel);
            player.Guild.Bulletin = packet.Message();
        }

        private static void GuildBoard(Player player, ref MsgText packet)
        {
        }

        public static unsafe implicit operator byte[](MsgText msg)
        {
            var buffer = BufferPool.GetBuffer();
            fixed (byte* p = buffer)
                *(MsgText*)p = *&msg;
            return buffer;
        }

        public static unsafe implicit operator MsgText(byte[] msg)
        {
            fixed (byte* p = msg)
                return *(MsgText*)p;
        }
    }
}