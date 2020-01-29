namespace YiX.Structures
{
    public struct Statpoint
    {
        public short Spirit;
        public short Vitality;
        public short Agility;
        public short Strength;

        public Statpoint(short strength, short agility, short vitality, short spirit)
        {
            Spirit = spirit;
            Vitality = vitality;
            Agility = agility;
            Strength = strength;
        }
    }
}