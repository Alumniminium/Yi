namespace YiX.Structures
{
    public struct LevelExp
    {
        public ulong ExpReq;
        public int AllLevTime;

        public LevelExp(ulong expReq, int levtime)
        {
            AllLevTime = levtime;
            ExpReq = expReq;
        }
    }
}