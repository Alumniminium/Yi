// -------- Yi --------
// Project: Library File: MsgNameType.cs 
// Created: 27/10/2015/2015 at 3:09 PM
// Last Edit: 08/12/2015 at 12:31 PM
// By: Buddha

namespace YiX.Enums
{
    public enum MsgNameType : byte
    {
        None = 0,
        Fireworks = 1,
        CreateSyn = 2,
        Syndicate = 3,
        ChangeTitle = 4,
        DelRole = 5,
        Spouse = 6,
        QueryNpc = 7, //To client, To Server
        Wanted = 8, //To client
        MapEffect = 9, //To client
        RoleEffect = 10, //To client
        MemberList = 11, //To client, To Server, dwData is index
        KickOutSynMem = 12,
        QueryWanted = 13,
        QueryPoliceWanted = 14,
        PoliceWanted = 15,
        QuerySpouse = 16,
        AddDicePlayer = 17, //BcastClient(INCLUDE_SELF) Ôö¼Ó÷»×ÓÍæ¼Ò// dwDataÎª÷»×ÓÌ¯ID // To Server ¼ÓÈë ÐèÒªÔ­ÏûÏ¢·µ»Ø
        DelDicePlayer = 18, //BcastClient(INCLUDE_SELF) É¾³ý÷»×ÓÍæ¼Ò// dwDataÎª÷»×ÓÌ¯ID // To Server Àë¿ª ÐèÒªÔ­ÏûÏ¢·µ»Ø
        DiceBonus = 19, //dwDataÎªMoney
        Sound = 20,
        SynEnemie = 21,
        SynAlly = 22,
        Bavarder = 26,
    }
}