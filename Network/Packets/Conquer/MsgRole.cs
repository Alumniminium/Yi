using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YiX.Calculations;
using YiX.Enums;
using YiX.Helpers;
using YiX.Items;
using YiX.Network.Sockets;
using YiX.SelfContainedSystems;
using YiX.Structures;
using YiX.World;
using Player = YiX.Entities.Player;

namespace YiX.Network.Packets.Conquer
{
    public class MsgRole
    {
        private static readonly ushort[] Models = { 1004, 2004, 1002, 2002, 1003, 2003, 2001, 1001 };
        private static readonly byte[] Jobs = { 10, 20, 40, 100 };
        public static Player Player;
        public static string AccountId, CharName;
        public static ushort Model;
        public static byte Job;
        public static bool NameAvailable;

        public static void Handle(Player player, byte[] packet)
        {
            AccountId = Encoding.Default.GetString(packet, 4, 16).Trim((char)0x0000);
            CharName = Encoding.Default.GetString(packet, 20, 16).Trim((char)0x0000);
            Model = BitConverter.ToUInt16(packet, 52);
            Job = (byte)BitConverter.ToUInt16(packet, 54);
            NameAvailable = GameWorld.NameAvailable(CharName);

            Player = SelectorSystem.GetOrCreatePlayer(AccountId, player.Password);
            Player.GameSocket = player.GameSocket;
            Process();
            BufferPool.RecycleBuffer(packet);
        }

        private static Player CreateCharacter()
        {
            var character = new Player(true)
            {
                UniqueId = UniqueIdGenerator.GetNext(EntityType.Player),
                Name = CharName.Size16(),
                Partner = "None",
                Level = 1,
                Money = 1000,
                Look = Model,
                Class = Job,
                MapId = 1010,
                Profs = new Dictionary<int, Prof>
                {
                    [134] = new Prof(134, 0, 0),
                    [133] = new Prof(133, 0, 0),
                    [130] = new Prof(130, 0, 0),
                    [131] = new Prof(131, 0, 0),
                    [117] = new Prof(117, 0, 0),
                    [114] = new Prof(114, 0, 0),
                    [113] = new Prof(113, 0, 0),
                    [118] = new Prof(118, 0, 0),
                    [111] = new Prof(111, 0, 0),
                    [121] = new Prof(121, 0, 0),
                    [120] = new Prof(120, 0, 0),
                    [150] = new Prof(150, 0, 0),
                    [151] = new Prof(151, 0, 0),
                    [152] = new Prof(152, 0, 0),
                    [160] = new Prof(160, 0, 0),
                    [900] = new Prof(900, 0, 0),
                    [400] = new Prof(400, 0, 0),
                    [410] = new Prof(410, 0, 0),
                    [420] = new Prof(420, 0, 0),
                    [421] = new Prof(421, 0, 0),
                    [430] = new Prof(430, 0, 0),
                    [440] = new Prof(440, 0, 0),
                    [450] = new Prof(450, 0, 0),
                    [460] = new Prof(460, 0, 0),
                    [480] = new Prof(480, 0, 0),
                    [481] = new Prof(481, 0, 0),
                    [490] = new Prof(490, 0, 0),
                    [500] = new Prof(500, 0, 0),
                    [510] = new Prof(510, 0, 0),
                    [530] = new Prof(530, 0, 0),
                    [540] = new Prof(540, 0, 0),
                    [560] = new Prof(560, 0, 0),
                    [561] = new Prof(561, 0, 0),
                    [562] = new Prof(562, 0, 0),
                    [580] = new Prof(580, 0, 0)
                }
            };

            GiveStarterItems(character);

            EntityLogic.Recalculate(character);
            character.CurrentHp = character.MaximumHp;
            character.CurrentMp = character.MaximumMp;
            character.Money = 1000;
            character.Location = new Vector2(61, 109);
            return character;
        }

        private static void GiveStarterItems(Player character)
        {
            character.Equipment.AddOrUpdate(MsgItemPosition.Armor, ItemFactory.Create(ItemNames.Coat));
            character.Inventory.AddItem(ItemFactory.Create(ItemNames.Stancher), 3);
            if (Job == 100)
            {
                character.Equipment.AddOrUpdate(MsgItemPosition.RightWeapon, ItemFactory.Create(ItemNames.LuckyBacksword));
                character.Inventory.AddItem(ItemFactory.Create(ItemNames.Agrypnotic), 3);
            }

            if (Job == 40)
            {
                character.Equipment.AddOrUpdate(MsgItemPosition.RightWeapon, ItemFactory.Create(ItemNames.LuckyBow));
                character.Inventory.AddItem(ItemFactory.Create(ItemNames.LuckyArrow), 3);
            }

            if (Job == 10 || Job == 20)
            {
                character.Equipment.AddOrUpdate(MsgItemPosition.RightWeapon, ItemFactory.Create(ItemNames.LuckyBlade));
            }
        }

        private static void Process()
        {
            if (VerifyJob(Job) && VerifyModel(Model) && VerifyName(CharName) && NameAvailable)
            {
                var character = CreateCharacter();
                character.GameSocket = Player.GameSocket;
                character.GameSocket.Ref = character;
                character.AccountId = Player.AccountId;
                character.Password = Player.Password;
                Player = character;
                Player.LoginType = LoginType.Login;
                SelectorSystem.AddCharacterTo(Player.AccountId, Player);
                Player.ForceSend(MsgText.Create(Constants.System, Constants.Allusers, Constants.AnswerOk, MsgTextType.Dialog), 29 + Constants.System.Length + Constants.Allusers.Length + Constants.AnswerOk.Length);
            }
            else
                Player.ForceSend(MsgText.Create(Constants.System, Constants.Allusers, "Taken or invalid name. (A-Y | 0-9)", MsgTextType.Dialog), 29 + Constants.System.Length + Constants.Allusers.Length + "Taken or invalid name. (A-Y | 0-9)".Length);
        }

        private static bool VerifyJob(byte job) => Jobs.Contains(job);
        private static bool VerifyModel(ushort model) => Models.Contains(model);
        private static bool VerifyName(string charName) => Regex.IsMatch(charName, "^[a-zA-Z0-9]+$", RegexOptions.Compiled);
    }
}