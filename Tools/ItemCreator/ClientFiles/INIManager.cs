using System.IO;

namespace ItemCreator.ClientFiles
{
    public static class INIManager
    {

        public static void WeaponINI()
        {
            using (var writer = new StreamWriter(Config.ConquerPath + "weapon.ini", true))
            {
                var id = Equipment.Instance.Id;
                var mesh = Equipment.Instance.Mesh;
                var texture = Equipment.Instance.TextureF;
                writer.WriteLine("");
                writer.WriteLine($"[{id}]");
                writer.WriteLine("Part=1");
                writer.WriteLine($"Mesh0={mesh}");
                writer.WriteLine($"Texture={texture}");
                writer.WriteLine("MixTex0=0"); // constant
                writer.WriteLine("MixOpt0=0"); // constant
                writer.WriteLine("Asb0=5"); // constant
                writer.WriteLine("Adb0=6"); // constant
                writer.WriteLine("Material0=default"); // constant
            }
        }

        public static void Texture3D()
        {
            using (var writer = new StreamWriter(Config.ConquerPath + "3dtexture.ini", true))
            {
                var texture = Equipment.Instance.TextureF;
                writer.WriteLine($"{texture}=c3\\texture\\{texture}.dds");
            }
        }

        public static void Action3Deffect()
        {
            using (var writer = new StreamWriter(Config.ConquerPath + "Action3DEffect.ini", true))
            {
                var glow = Equipment.Instance.GlowEffect;
                var id = Equipment.Instance.Id%10000;
                var type = Equipment.Instance.Id%1000;
                writer.WriteLine($"999.999.{type}.{id}=c3\\texture\\{glow}");
            }
        }

        public static void ItemMinIcon()
        {
            using (var writer = new StreamWriter(Config.AniPath + "ItemMinIcon.ini", true))
            {
                var id = Equipment.Instance.Id;
                var inventoryIcon = Equipment.Instance.InvIconImg;
                writer.WriteLine("");
                writer.WriteLine($"[{id}]");
                writer.WriteLine("FrameAmount=1"); //constant
                writer.WriteLine($"Frame0=data/ItemMinIcon/{inventoryIcon}.dds");
            }
        }

        public static void MapItemIcon()
        {
            using (var writer = new StreamWriter(Config.AniPath + "ItemMinIcon.ini", true))
            {
                var id = Equipment.Instance.Id;
                var mapIcon = Equipment.Instance.InvIconImg;
                writer.WriteLine("");
                writer.WriteLine($"[{id}]");
                writer.WriteLine("FrameAmount=1"); //constant
                writer.WriteLine($"Frame0=data/MapItemIcon/{mapIcon}.dds");
            }
        }
    }
}