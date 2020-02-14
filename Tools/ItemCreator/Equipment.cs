using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ItemCreator.ClientFiles;
using ItemCreator.CoreDLL.IO.Ini;
using ItemCreator.Enums;
using ItemCreator.Pages;
using ItemCreator.Properties;

namespace ItemCreator
{
    public class Equipment : INotifyPropertyChanged
    {
        #region Privates

        private static Equipment _instance;
        private string _mapIconImg;
        private string _invIconImg;
        private string _textureF;
        private int _id;
        private string _description;
        private string _name;
        private byte _level;
        private uint _priceBaseLine;
        private ushort _magicDefense;
        private int _glowEffect;
        private byte _durability;
        private short _potHp;
        private short _potMp;
        private short _dodge;
        private int _mesh;
        private ushort _minimumAttack;
        private ushort _maximumAttack;
        private ushort _magicAttack;
        private short _frequency;
        private byte _range;
        private short _defense;
        private short _requiredAgility;
        private byte _requiredStrenght;
        private ushort _requredSpeed;
        private byte _requiredProfession;
        private byte _requiredSex;
        private string _textureM;

        public static Equipment Instance
        {
            get { return _instance ?? (_instance = new Equipment()); }
            set { _instance = value; }

        }

        #endregion

        public ItemType.Entry ImportedFrom { get; set; }
        public int RequiredSpeed
        {
            get { return _requredSpeed; }
            set
            {
                if (value == _requredSpeed) return;
                _requredSpeed = (ushort) value;
                OnPropertyChanged();
            }
        }
        public int RequiredProfession
        {
            get { return _requiredProfession; }
            set
            {
                if (value == _requiredProfession) return;
                _requiredProfession = (byte)value;
                OnPropertyChanged();
            }
        }
        public int RequiredSex
        {
            get { return _requiredSex; }
            set
            {
                if (value == _requiredSex) return;
                _requiredSex = (byte)value;
                OnPropertyChanged();
            }
        }
        public int Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged();
                OnPropertyChanged("Quality");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public byte Level
        {
            get { return _level; }
            set
            {
                if (value == _level) return;
                _level = value;
                OnPropertyChanged();
            }
        }

        public ushort MinimumAttack
        {
            get { return _minimumAttack; }
            set
            {
                _minimumAttack = value;
                OnPropertyChanged();
            }
        }

        public ushort MaximumAttack
        {
            get { return _maximumAttack; }
            set
            {
                _maximumAttack = value; 
                OnPropertyChanged();
            }
        }

        public ushort MagicAttack
        {
            get { return _magicAttack; }
            set
            {
                _magicAttack = value;
                OnPropertyChanged();
            }
        }

        public short Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                OnPropertyChanged();
            }
        }

        public byte Range
        {
            get { return _range; }
            set
            {
                _range = value;
                OnPropertyChanged();
            }
        }

        public short Defense
        {
            get { return _defense; }
            set
            {
                _defense = value;
                OnPropertyChanged();
            }
        }

        public byte RequiredStrenght
        {
            get { return _requiredStrenght; }
            set
            {
                _requiredStrenght = value;
                OnPropertyChanged();
            }
        }

        public short RequiredAgility
        {
            get { return _requiredAgility; }
            set
            {
                _requiredAgility = value;
                OnPropertyChanged();
            }
        }

        public uint PriceBaseLine
        {
            get { return _priceBaseLine; }
            set
            {
                _priceBaseLine = value;
                OnPropertyChanged();
            }
        }

        public byte Durability
        {
            get { return _durability; }
            set
            {
                _durability = value;
                OnPropertyChanged();
            }
        }

        public short PotHp
        {
            get { return _potHp; }
            set
            {
                _potHp = value; 
                OnPropertyChanged();
            }
        }

        public short PotMp
        {
            get { return _potMp; }
            set
            {
                _potMp = value; 
                OnPropertyChanged();
            }
            
        }

        public short Dodge
        {
            get { return _dodge; }
            set
            {
                _dodge = value;
                OnPropertyChanged();
            }
        }

        public int Mesh
        {
            get { return _mesh; }
            set
            {
                _mesh = value; 
                OnPropertyChanged();
            }
        }

        public int GlowEffect
        {
            get { return _glowEffect; }
            set
            {
                _glowEffect = value; 
                OnPropertyChanged();
            }
        }


        public ushort MagicDefense
        {
            get { return _magicDefense; }
            set
            {
                _magicDefense = value; 
                OnPropertyChanged();
            }
        }

        public string TextureF
        {
            get { return _textureF; }
            set
            {
                if (value == _textureF) return;
                _textureF = value;
                OnPropertyChanged();
                Equip3D.Instance.UpdateTextureF(value);
            }
        }
        public string TextureM
        {
            get { return _textureM; }
            set
            {
                if (value == _textureM) return;
                _textureM = value;
                OnPropertyChanged();
                Equip3D.Instance.UpdateTextureM(value);
            }
        }

        public string InvIconImg
        {
            get { return _invIconImg; }
            set
            {
                if (value == _invIconImg) return;
                _invIconImg = value;
                OnPropertyChanged();
                Equip3D.Instance.UpdateInvIcon(value);
            }
        }

        public string MapIconImg
        {
            get { return _mapIconImg; }
            set
            {
                if (value == _mapIconImg) return;
                _mapIconImg = value;
                OnPropertyChanged();
                Equip3D.Instance.UpdateMapIcon(value);
            }
        }

        public ItemQuality Quality => (ItemQuality) (Id%10);

        public unsafe void CopyFrom(ItemType.Entry entry)
        {
            if (ImportedFrom.ID == entry.ID)
                return;
            Id = entry.ID;
            ImportedFrom = entry;
            PriceBaseLine = entry.Price;
            Defense = entry.Defense;
            MinimumAttack = entry.MinAttack;
            MaximumAttack = entry.MaxAttack;
            MagicAttack = entry.MagicAttack;
            MagicDefense = entry.MagicDefence;
            Level = entry.RequiredLevel;
            Range = (byte) entry.Range;
            Dodge = entry.Dodge;
            RequiredAgility = entry.Dexterity;
            PotMp = entry.Mana;
            PotHp = entry.Life;
            RequiredSpeed = entry.RequiredSpeed;
            RequiredSex = entry.RequiredSex;
            RequiredProfession = entry.RequiredProfession;
            Name = "";
            Description = "";
            for (byte i = 0; i < 16; i++)
                Name += (char) entry.Name[i];
            for (byte i = 0; i < 128; i++)
                Description += (char) entry.Desc[i];
            Name = Name.Replace("\0", "");
            Description = Description.Replace("\0", "");

            if (FindItemMiniImage(entry, out string invIcon))
                InvIconImg = invIcon;

            if (FindMapItemImage(entry, out string mapIcon))
                MapIconImg = mapIcon;

            var textureF = GetFemaleTexturePath(entry);
            if (File.Exists(textureF))
                TextureF = textureF;

            var textureM = GetMaleTexturePath(entry);
            if (File.Exists(textureM))
                TextureM = textureM;
        }

        private string GetFemaleTexturePath(ItemType.Entry entry)
        {
            var textureF = Config.ConquerPath + $@"\c3\texture\{entry.ID}.dds";
            if (!File.Exists(textureF))
                textureF = Config.ConquerPath + $@"\c3\texture\{entry.ID / 10 * 10}.dds";
            if (!File.Exists(textureF))
                textureF = Config.ConquerPath + $@"\c3\texture\00{entry.ID / 10 + 100000}0.dds";
            if (File.Exists(textureF))
                return textureF;

            if (WeaponReader.Entries.TryGetValue(entry.ID, out Weapon wep))
                textureF = Config.ConquerPath + $@"\c3\texture\{wep.TextureId}.dds";

            if (File.Exists(textureF))
                return textureF;

            if (ArmetReader.Entries.TryGetValue(entry.ID / 10 + 1100000, out Armet arm))
                textureF = Config.ConquerPath + $@"\c3\texture\{arm.TextureId}.dds";

            return textureF;
        }
        private string GetMaleTexturePath(ItemType.Entry entry)
        {
            var textureF = Config.ConquerPath + $@"\c3\texture\{entry.ID}.dds";
            if (!File.Exists(textureF))
                textureF = Config.ConquerPath + $@"\c3\texture\{entry.ID / 10 * 10}.dds";
            if (!File.Exists(textureF))
                textureF = Config.ConquerPath + $@"\c3\texture\00{entry.ID / 10 + 200000}0.dds";
            if (File.Exists(textureF))
                return textureF;

            if (WeaponReader.Entries.TryGetValue(entry.ID, out Weapon wep))
                textureF = Config.ConquerPath + $@"\c3\texture\{wep.TextureId}.dds";

            if (File.Exists(textureF))
                return textureF;

            if (ArmetReader.Entries.TryGetValue(entry.ID / 10 + 2100000, out Armet arm))
                textureF = Config.ConquerPath + $@"\c3\texture\{arm.TextureId}.dds";

            return textureF;
        }

        private static bool FindItemMiniImage(ItemType.Entry entry, out string image)
        {
            if (ItemMiniIcons.Entries.TryGetValue(entry.ID, out ItemMiniIcon itemMiniIcon))
            {
                image = Config.ConquerPath + itemMiniIcon.Frames.Values.FirstOrDefault();
                if (File.Exists(image))
                    return true;
                MessageBox.Show("Item Mini Icon referenced in ItemMinIcon.ani but not found at specified path!\r\n" + image, "FATAL!");
            }
            image = Config.ConquerPath + @"\data\ItemMinIcon\" + entry.ID + ".dds";
            if (File.Exists(image))
                return true;
            image = Config.ConquerPath + @"\data\ItemMinIcon\" + entry.ID / 10 * 10 + ".dds";
            return File.Exists(image);
        }
        private static bool FindMapItemImage(ItemType.Entry entry, out string image)
        {
            if (MapItemIcons.Entries.TryGetValue(entry.ID, out MapItemIcon mapItemIcon))
            {
                image = Config.ConquerPath + mapItemIcon.Frames.Values.FirstOrDefault();
                if (File.Exists(image))
                    return true;
                MessageBox.Show("Item Map Icon referenced in ItemMinIcon.ani but not found at specified path!\r\n" + image, "FATAL!");
            }
            image = Config.ConquerPath + @"\data\MapItemIcon\" + entry.ID + ".dds";
            if (File.Exists(image))
                return true;
            image = Config.ConquerPath + @"\data\MapItemIcon\" + entry.ID / 10 * 10 + ".dds";
            return File.Exists(image);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
