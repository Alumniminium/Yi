// -------- Yi --------
// Project: Library File: GuildRequest.cs 
// Created: 27/10/2015/2015 at 3:09 PM
// Last Edit: 08/12/2015 at 12:31 PM
// By: Buddha

namespace YiX.Enums
{
    public enum GuildRequest
    {
        None = 0,
        ApplyJoin = 1, // ÉêÇë¼ÓÈëºÚÉç»á, id
        InviteJoin = 2, // ÑûÇë¼ÓÈëºÚÉç»á, id
        LeaveSyn = 3, // ÍÑÀëºÚÉç»á
        KickOutMember = 4, // ¿ª³ýºÚÉç»á³ÉÔ±, name
        QuerySynName = 6, // ²éÑ¯°ïÅÉÃû×Ö
        SetAlly = 7, // ½áÃË				// to client, npc(npc_id is syn_id, same follow)
        ClearAlly = 8, // ½â³ý½áÃË			// to client, npc
        SetEnemy = 9, // Ê÷µÐ				// to client, npc
        ClearAntagonize = 10, // ½â³ýÊ÷µÐ			// to client, npc
        DonateMoney = 11, // °ïÖÚ¾èÇ®
        QuerySynAttr = 12, // ²éÑ¯°ïÅÉÐÅÏ¢		// to server
        SetSyn = 14, // Ìí¼Ó°ïÅÉID		// to client
        UniteSubSyn = 15, // ºÏ²¢ÌÃ¿Ú // to client // dwData±»ºÏ²¢, targetÊÇÖ÷ÈË
        UniteSyn = 16, // ºÏ²¢°ïÅÉ // to client // dwData±»ºÏ²¢, targetÊÇÖ÷ÈË
        SetWhiteSyn = 17, // °×°ï°ïÅÉID // Î´±»Õ¼ÁìÇë·¢ID_NONE
        SetBlackSyn = 18, // ºÚ°ï°ïÅÉID // Î´±»Õ¼ÁìÇë·¢ID_NONE
        DestroySyn = 19, // ÊÀ½ç¹ã²¥£¬É¾³ý°ïÅÉ¡£
        SetMantle = 20, // ÊÀ½ç¹ã²¥£¬Åû·ç // add huang 2004.1.1       

        //_APPLY_ALLY = 21,			// ÉêÇë½áÃË			// to server&client, idTarget=SynLeaderID
        //_CLEAR_ALLY = 22,			// Çå³ý½áÃË			// to server

        //_SET_ANTAGONIZE = 23,			//Ê÷µÐ client to server
        //_CLEAR_ANTAGONIZE = 24,			//½â³ýÊ÷µÐ client to server

        //NPCMSG_CREATE_SYN = 101,			// Í¨ÖªNPC·þÎñÆ÷Ìí¼Ó°ïÅÉ	// to npc
        //NPCMSG_DESTROY_SYN = 102,			// Í¨ÖªNPC·þÎñÆ÷É¾³ý°ïÅÉ	// to npc
        //KICKOUT_MEMBER_INFO_QUERY = 110,	//°ïÖ÷²éÑ¯ÉêÇë¿ª³ýµÄ³ÉÔ±
        //KICKOUT_MEMBER_AGREE = 111,	//°ïÖ÷Í¬Òâ¿ª³ý»áÔ±
        //KICKOUT_MEMBER_NOAGREE = 112,	//°ïÖ÷²»Í¬Òâ¿ª³ý»áÔ±
        //SYNMEMBER_ASSIGN = 113,			//°ïÅÉ³ÉÔ±±àÖÆ	
        //SYN_CHANGE_NAME = 114,			// °ïÅÉ¸ÄÃû
        //SYN_CHANGE_SUBNAME = 114,		//·ÖÍÅ¸ÄÃû
        //SYN_CHANGE_SUBSUBNAME = 115,		//·Ö¶Ó¸ÄÃû
        //SYN_DEMISE = 116,		//ìøÈÃ
        //SYN_SET_ASSISTANT = 117,		//ÉèÖÃ¸±°ïÖ÷
        //SYN_SET_TEAMLEADER = 118,		//ÉèÖÃ°ïÅÉ¶Ó³¤
        //SYN_SET_PUBLISHTIME = 119,		//ÉèÖÃ¹«¸æÊ±¼ä
    }
}