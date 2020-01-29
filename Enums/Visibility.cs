// -------- Yi --------
// Project: Library File: Visibility.cs 
// Created: 27/10/2015/2015 at 3:09 PM
// Last Edit: 08/12/2015 at 12:31 PM
// By: Buddha

using System;

namespace YiX.Enums
{
    [Flags]
    public enum Visibility : byte
    {
        Everyone = 1,
        Owner = 2,
        Mobs = 3,
        Bots = 4,
        Npcs = 5,
        Players = 6,
        Friends = 7,
        Enemies = 8,
        Guild = 9,
        Team = 10,
        Partner = 11,
        Staff = 12
    }
}