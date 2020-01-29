namespace YiX.Enums
{
    public enum MsgFloorItemType
    {
        None = 0,
        Create = 1, // to client
        Delete = 2, // to client
        Pick = 3, // to server, client: perform action of pick

        DisplayEffect = 10, // to client
        SynchroTrap = 11, // to client
        RemoveEffect = 12 // to client, id=trap_id
    }
}