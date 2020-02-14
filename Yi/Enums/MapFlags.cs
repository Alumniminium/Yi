// -------- Yi --------
// Project: Library File: MapFlags.cs 
// Created: 27/10/2015/2015 at 3:09 PM
// Last Edit: 08/12/2015 at 12:31 PM
// By: Buddha

using System;

namespace Yi.Enums
{
    [Flags]
    public enum MapFlags
    {
        None = 0x0000,
        NoPkpNoFlash = 0x0001, //No PKPoints, Not Flashing...
        RecordDisable = 0x0004, //Do not save this position, save the previous
        NoPk = 0x0008, //Can't PK
        EnablePlayerShop = 0x0010, //Can create booth
        DisableTeams = 0x0020, //Can't create team
        DisableScrolls = 0x0040, //Can't use scroll
        GuildMap = 0x0080, //Syndicate MapId
        Prison = 0x0100, //Prison MapId
        DisableFly = 0x0200, //Can't fly
        Family = 0x0400, //Family MapId
        Mine = 0x0800, //Mine MapId
        NewbieProtect = 0x4000 //Newbie protection
    }
}