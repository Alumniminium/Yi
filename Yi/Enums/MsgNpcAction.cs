namespace Yi.Enums
{
    public enum MsgNpcAction : short
    {
        BeActived = 0, // to server		// ´¥·¢
        AddNpc = 1, // no use
        LeaveMap = 2, // to client		// É¾³ý
        DelNpc = 3, // to server
        ChangePos = 4, // to client/server
        LayNpc = 5, // to client(id=region,data=lookface), answer MsgNpcInfo(CMsgPlayer for statuary)
                   //8 for GC, 9 for Statue, 25/26 for Furniture.
        //GC=8,
        //Statue=9,
        //Furniture1=25,
        //Furniture2=26
    }
}