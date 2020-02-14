namespace Yi.Enums
{
    public enum MsgStorageAction : byte
    {
        List = 0,
        Add = 1,
        Remove = 2,
    }
    public enum MsgStorageType : byte
    {
        None =0,
        Storage =10,
        Trunk = 20,
        Chest = 30
    }
}