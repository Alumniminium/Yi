namespace YiX.Enums
{
    public enum MsgRemoteMaintenanceType : byte
    {
        StartBackup = 0,
        OptimizeMemory = 1,
        Shutdown = 2,
        DownloadDatabase = 3
    }
}