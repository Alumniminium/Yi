// -------- Yi --------
// Project: Library File: MsgActionType.cs 
// Created: 27/10/2015/2015 at 3:09 PM
// Last Edit: 08/12/2015 at 12:31 PM
// By: Buddha

namespace Yi.Enums
{
    public enum MsgActionType : short
    {
        MapShow = 74,
        Hotkeys = 75,
        ConfirmFriends = 76,
        ConfirmProf = 77,
        ConfirmSkills = 78,
        ChangeDirection = 79,
        Action = 81,
        Portal = 85,
        ChangeMap = 86,
        Leveled = 92,
        DeleteChar = 95,
        EndXpList = 93,
        CharDeletion = 95,
        ChangePkMode = 96,
        ConfirmGuild = 97,
        QueryTeamLeader = 101,
        EntitySpawn = 102,
        CompleteMapChange = 104,
        QueryTeamMemberPos = 106,
        CorrectCords = 108,
        DropMagic = 109, // to client only, data is skill/spell ID
        DropSkill = 110,
        Shop = 111,
        OpenShop = 113,
        StartVending = 111,
        // ¿ªÊ¼°ÚÌ¯ to server = unPosX,unPosY: playerpos; unDir:dirofbooth; to client = idTarget:idnpc;
        StopVending = 114,
        RemoteCommands = 116,
        // SubType 1 = Quit, 2 = IG Quit, 39 = MsgTeam thing, 43 kick GS Member, 44 donate money, 46 join guild, 47 quit guild, 48 join guild, 53 help , 54 freinds, 55 change chat + name in to box, 56 send freind msg, 57 delete freind,  60 set hotkeys, 61 furniture, 64 fux with window, 68 view msg, 69 send msg.., 71 selling price, 78 hawk msg, 80 black list, 88 remove gem warning, 89 show/hide names, 90 show/hide exp, 93 weird dialog box...never seen it before :x, 94 delete enemy, 98 font colour, 101 conqueronline.com, 105 show/hide counter, 109 exp ball, 111 closed the client! fuxors!, 113 ask to vend,
        ViewOthersEquip = 117,
        EndTransform = 118,
        PickupCashEffect = 121,
        QueryEnemyInfo = 123,
        Dialog = 126,
        PetJump = 129,
        OnTeleport = 130,
        SpawnEffect = 131,
        EntityRemove = 132,
        Jump = 133,
        RemoveWeaponMesh = 135,
        RemoveWeaponMesh2 = 136,
        FinishTeleport = 137,
        Sync = 138,
        QueryFriendInfo = 140,
        ChangeFace = 142,
        Mine = 99,
        Revive = 94,
        EndFly = 120,
            Pathfinding= 162,
            QueryFriendEquip = 310,
        QueryStatInfo = 408,
    }
}