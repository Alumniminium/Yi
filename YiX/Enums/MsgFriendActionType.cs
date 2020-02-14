namespace YiX.Enums
{
    public enum MsgFriendActionType : byte
    {
        None = 0,
        FriendApply = 10,
        FriendAccept = 11,
        FriendOnline = 12,
        FriendOffline = 13,
        FriendBreak = 14,
        GetInfo = 15,
        EnemyOnline = 16, //To client
        EnemyOffline = 17, //To client
        EnemyDel = 18, //To client & to server
        EnemyAdd = 19
    }
}