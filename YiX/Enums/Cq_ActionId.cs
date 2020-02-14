namespace YiX.Enums
{
    public enum Cq_ActionId
    {
        ACTION_SYS_FIRST = 100,

        ACTION_MENUTEXT =
            101, // menu text. data: display the number of rows (default is 0), param = "text"(optional): display text, this type can contain spaces, but also for the blank lines. 

        ACTION_MENULINK =
            102, // menu hyperlink. "text task_id align": align (optional): Alignment mode (default / 0: left-justified; 5: center; 9: Right-aligned; a (1-9) b (2-8): not-for - line, from the a / b Xingkuan Department is beginning to show, 9 for the right-aligned) 

        ACTION_MENUEDIT =
            103, // menu input box. "len task_id text": len: length can be entered; text (optional): the text that appears, align: alignment mode (default: text in the left, positive integer: input box embedded in the text the first few characters Department) . ¡ï Description: For a number of input box, from the interface with a round button distinction, only to upload a.

        ACTION_MENUPIC =
            104, // menu picture. "xy pic_id task_id": task_id (optional): that the picture can be "by." Pictures do not show the text of the region.
        ACTION_MENUBUTTON = 110, // menu button, the format with hyperlink. 

        ACTION_MENULISTPART =
            111, // menu list item. "task_id iter text ...", click on the list of players, it will trigger task_id, and iter will be filled to the players% iter variables. 
        ACTION_MENUCREATE = 120, // menu to create. "cancel_task_id" (optional): forcibly shut down menu triggered TASK
        ACTION_RAND = 121, // Detection of random rate. "data1 data2". "10,100," said 1 / 10 the chance is true. 

        ACTION_RANDACTION =
            122, // Random Action "action0 action1 ... action7" a total of eight randomly pick an executive

        ACTION_CHKTIME =
            123, // data for the time to type 0 - check the details of the current server time "% d-% d-% d% d:% d% d-% d-% d% d:% d"; 1 - check in One day time "% d-% d% d:% d% d-% d% d:% d", 2 - to check the time on one day "% d% d:% d% d% d:% d", 3 - check Zhou days "% d% d:% d% d% d:% d", 4 - to check on time "% d:% d% d:% d", 5 - check the hours "% d % d "(the first few minutes of each hour in the end a few minutes) 
        ACTION_POSTCMD = 124, // interface to the client to send commands, data for the order number
        ACTION_BROCASTMSG = 125, // full-text news broadcast servers, data for the channel, para content

        ACTION_MESSAGEBOX =
            126, // so that the client pop-up dialog box.data for the dialog box type(0-99), param = text string parameters.
        ACTION_SYS_LIMIT = 199,

// Npc part, need npc ptr ------------------------------------------ -------------------------------- 
        ACTION_NPC_FIRST = 200,

        ACTION_NPC_ATTR =
            201, // modify or testing the properties of NPC tasks. "attr opt data npc_id", at least three parameters. If you specify npc_id, the NPC must be in the map group.attr choose "ownerid "(=,==)," ownertype "(=,==)," lookface "(=,==)," data ?"(=,+=,==,<,<= ,>,>=, pass.pass allows field Datestamp maintained a certain degree of increase or decrease in the number of days,. data can "accept"), "datastr "(=,==)," life "(=)," maxlife "(=) 

// ACTION_NPC_REQUESTSHIFT = 202, // inform the client of a pan NPC.param = "idNextTask". 
// ACTION_NPC_SHIFT = 203, // shift the current NPC.Limited mobility with the map. Limited dynamic NPC.
        ACTION_NPC_ERASE =
            205, // delete the current NPC. Limited dynamic NPC.Note: deleted, not to operate this NPC.dwData not to 0, said to delete all of this map type for dwData the NPC.param = "idMap type": the deletion of certain designated map NPC.

        ACTION_NPC_MODIFY =
            206, // cross-thread modify the attributes of the tasks assigned to NPC. "npc_id attr opt data". attr choose "lookface "(=)," data ?"(=)," datastr" (=) 

        ACTION_NPC_RESETSYNOWNER =
            207, // reset gangs map owner.Gang signs only for NPC.Statistics gangs fill the first record OWNER_ID, at the same time to remove all record. Automatically stop all attacks. (Not to suspend the fighting map logo)

        ACTION_NPC_FIND_NEXT_TABLE =
            208, // Find a list of items, will ID write TASK_ITERATOR.Only with a list of the NPC. param = "type", corresponding to the type field cq_table. 

        ACTION_NPC_ADD_TABLE =
            209, // in the list to add one, type and the same will be idKey pre-delete (idKey for 0:00, do not delete). Only with a list of the NPC.param = "type idKey data0 data1 data2 data3 szData", at least two parameters.

        ACTION_NPC_DEL_TABLE =
            210, // delete from the list of all eligible, non-existent also return true. Only with a list of the NPC.param = "type idKey data0 data1 data2 data3 szData", at least two parameters to 0 that does not match, all is not to 0 with a list of exactly the same will delete.No param, deleted the current record (iterator designated record), always return true. 

        ACTION_NPC_DEL_INVALID =
            211, // from the list to remove all expired items, non-existent also return true. Only with a list of the NPC.param = "type idx", idx that date(% date_stamp) in which data stored in. [For example, that the date of idx 3 stored in data3, all data3 the date the item is less than today's date will be deleted. ] 

        ACTION_NPC_TABLE_AMOUNT =
            212, // check list of items,> = data return false, <data return true. param meaningless.

        ACTION_NPC_SYS_AUCTION =
            213, // LW let NPC system auction started, DATA for the NPC's ID, param officially began for the system prompts 
        ACTION_NPC_DRESS_SYNCLOTHING = 214, //´©°ïto send clothing
        ACTION_NPC_TAKEOFF_SYNCLOTHING = 215, // off gang clothing

        ACTION_NPC_AUCTIONING =
            216, // determine whether there are auction items DATA for the NPC's ID, PARAM: type: 0. That view the system auction items, 1. That the players view the items 
        ACTION_NPC_LIMIT = 299,

// Map part, need curr map ------------------------------------------ -------------------------------- 
        ACTION_MAP_FIRST = 300,

        ACTION_MAP_MOVENPC =
            301, // the npc move to the designated map, location(only for fixed NPC), data for a specific npc's ID, param for "idMap nPosX nPosY". Note: to move to the map (0,0) coordinates, in order to hide the NPC. 

        ACTION_MAP_MAPUSER =
            302, // determine the map of the designated number of users, data for the specified map ID, param for "cmd opt data",

// Cmd support "map_user" and "alive_user", opt for "==, <=,> =", data for the number of
        ACTION_MAP_BROCASTMSG = 303, // radio news, data for the map id, szParam for broadcasting news
        ACTION_MAP_DROPITEM = 304, // map have designated items, szParam for "idItemType idMap nPosX nPosY" 

        ACTION_MAP_SETSTATUS =
            305, // set map, and support EVENT.param = "mapid status_bit data", status_bit = (STATUS_WAR = 1,), data = 0 or 1. 

        ACTION_MAP_ATTRIB =
            306, // check, modify the properties of the map.param = "field opt data idMap", at least three parameters, the default for the current map.field = "synid" (opt ="==","=")¡£ field = "status" (opt = "test", "set", "reset"). field = "type" (opt = "test"). field = "res_lev" (opt ="=","==","<")¡£ field = "mapdoc" (opt ="=","=="), portal0_x(=), portal0_y(=), field = "castle" (opt ="==") 

        ACTION_MAP_REGION_MONSTER =
            307, // check the map or the current map designation of a number of the region monster.param = "map_id region_x region_y region_cx region_cy monster_type opt data". 0:00 said map_id check for the current map, monster_type of 0 indicated that he did not check the type, opt to support the "==" and "<." 


        ACTION_MAP_CHANGEWEATHER =
            310, // modify where players REGION weather.param = "Type Intensity Dir Color KeepSecs", Type, Intensity = 0 ~ 999, Dir = 0 ~ 359, Color = 0x00RRGGBB, KeepSecs = seconds

        ACTION_MAP_CHANGELIGHT =
            311, // modify the brightness of players map.param = "idmap light secs", light = 0xAARRGGBB(0xFFFFFFFF, said the restoration), secs to 0: that a permanent change

        ACTION_MAP_MAPEFFECT =
            312, // in the designated map shows the location map of the designated special effects, param = "idMap xy EffectName" 

        ACTION_MAP_CREATEMAP =
            313, // create a map link to the current NPC's (npc must LINK_NPC), the needs of the target audience. param = "name owner_type owner_id mapdoc type portal_x portal_y reborn_map reborn_portal res_lev". partal refers to the point of entry coordinates, res_lev that map hierarchy (for upgrade). 
        ACTION_MAP_FIREWORKS = 314, // put fireworks


        ACTION_MAP_LIMIT = 399,

// Item action only part, need pUser + pItem ptr -------------------------------------- ------------------------------------ 
        ACTION_ITEMONLY_FIRST = 400,

        ACTION_ITEM_REQUESTLAYNPC =
            401, // notify the client to place a NPC.param = "idNextTask type sort lookface region", at least four parameters.region of the type that cq_region

        ACTION_ITEM_COUNTNPC =
            402, // check with the map of the NPC number.param = "field data opt num", field = "name" (by name), "type" (by type), "all" (all NPC), "furniture" (furniture), data = to the name or the type of statistics(all and furniture fill 0), opt ="<","=="¡£ 

        ACTION_ITEM_LAYNPC =
            403, // create a NPC, creating successful, the NPC is the immediate task of NPC, owner_id will be automatically set to gang ID or player ID.param = "name type sort lookface ownertype life region base linkid task0 task0 ... task7 data0 data1 data2 data3 datastr". At least five parameters.Data3 on target in the hierarchy.
        ACTION_ITEM_DELTHIS = 498, // delete the current task items.Note: the need for the last ACTION.
        ACTION_ITEMONLY_LIMIT = 499,

// User item part, need user ptr ----------------------------------------- --------------------------------- 
        ACTION_ITEM_FIRST = 500,

        ACTION_ITEM_ADD =
            501, // Add items.data = itemtype_id, param = "amount amount_limit ident gem1 gem2 magic1 magic2 magic3 data warghostexp gemtype availabletime", param can be omitted, all the default value is 0 (that is not revised) 

        ACTION_ITEM_DEL =
            502, // delete items.data = itemtype_id, param not 0:00, items can be superimposed at the same time to delete a number.Or data for 0, param said you want to delete the items were.

        ACTION_ITEM_CHECK =
            503, // Detection items.data = itemtype_id, param not be 0:00, while the number of items to check(or durability), the goods must meet the requirements of the number(or durability) do. Or data for 0, param items that were looking for. 
        ACTION_ITEM_HOLE = 504, // weapons holes.param support "ChkHole HoleNum" or "MakeHole HoleNum", Num for 1 or 2 
        ACTION_ITEM_REPAIR = 505, // equipment repair.data for the specified location of equipment.

        ACTION_ITEM_MULTIDEL =
            506, // delete multiple items, param for "idType0 idType1 num", that is, delete num months idType0-idType1 items.

        ACTION_ITEM_MULTICHK =
            507, // detection of a variety of items, param for "idType0 idType1 num", that is, detection num months idType0-idType1 items.
        ACTION_ITEM_LEAVESPACE = 508, // check the remaining space backpack.param = "space weight packtype" 
// Which packtype range of 50 ~ 53 
// 50: ordinary items backpack 
// 51:Ä§»êjewel backpack 
// 52: imaginary animals eggs backpack 
// 53: imaginary animals backpack


        ACTION_ITEM_UPEQUIPMENT = 509, // equipment operation, param format "cmd position",
// Cmd support "up_lev", "up_quality", "recover_dur" 
// Position for equipment location, defined as follows 
/* ITEMPOSITION_HELMET = 1
ITEMPOSITION_NECKLACE = 2; 
ITEMPOSITION_ARMOR = 3; 
ITEMPOSITION_WEAPONR = 4; 
ITEMPOSITION_WEAPONL = 5; 
ITEMPOSITION_RINGR = 6; 
ITEMPOSITION_RINGL = 7; 
ITEMPOSITION_SHOES = 8; 
ITEMPOSITION_MOUNT = 9 */


        ACTION_ITEM_EQUIPTEST = 510, // goods quality inspection,

// Param "equip_pos cmd opt num",
// Equip_pos Ibid position definition 
// Cmd support "level", "quality", "durability", "max_dur" 
// Opt to support the "==,> =," = ", 
// Num data, cmd for "durability" and "max_dur" when -1 for the maximum
        ACTION_ITEM_EQUIPEXIST = 511, // the existence of test equipment, data for the equipment location

        ACTION_ITEM_EQUIPCOLOR =
            512, // equipment change color, param = "equip_pos color", equip_pos support is as follows 

/* ITEMPOSITION_HELMET = 1; 
ITEMPOSITION_ARMOR = 3; 
ITEMPOSITION_WEAPONL = 5; */ // ITEMPOSITION_WEAPONL must only work as a shield
        ACTION_ITEM_FIND =
            513, // Find an item, type the existence of user in the iterator.data = itemtype_id.Or data for 0, param that people want to find items.
        ACTION_ENCASH_CHIP = 514, // use of chips for cash, the amount of money in the Item in the Data field
        ACTION_ITEM_LIMIT = 599,

// User npc only part, need not user ptr --------------------------------------- ----------------------------------- 
        ACTION_NPCONLY_FIRST = 600,

        ACTION_NPCONLY_CREATENEW_PET =
            601, // create a MONSTER, OWNERID, OWNERTYPE the same with the NPC.param = "xy generator_id type data name", at least four parameters, if name is changed.monster generator used to control the scope of activities, cq_generator the type meaningless.x, y coordinates of the map's absolute. 

        ACTION_NPCONLY_DELETE_PET =
            602, // Delete the map all the MONSTER, OWNERID, OWNERTYPE the same with the NPC.param = "type data name", at least one parameter, data does not match the 0 at the same time data, if any name while at the same time matching names.

        ACTION_NPCONLY_MAGICEFFECT =
            603, // NPC issued a magic effect.param = "source_id magic_type magic_level target_id data" 

        ACTION_NPCONLY_MAGICEFFECT2 =
            604, // NPC issued by one magic effect.param = "source_id magic_type magic_level xy target_id data", at least five parameters.
        ACTION_NPCONLY_LIMIT = 699,

// User syndicate part --------------------------------------------- ----------------------------- 
        ACTION_SYN_FIRST = 700,

//////////////////////////////////////////////////////// 
// Gangs collate Action
        ACTION_SYN_CREATE =
            701, // a help, the player must enter the names of gangs.param = "level money leave_money", the three parameters that need grading players need to cash the number of gangs in cash after the establishment of the remaining few.

        ACTION_SYN_DESTROY =
            702, // dissolution.The implementation of Action for the head of the players, sub-head when the sub-captain, respectively, said the dissolution of gangs, sub-groups, detachments
        ACTION_SYN_DONATE = 703, // contributions, the need for an input box.

        ACTION_SYN_CREATE_SUB =
            708, // a sub-help(Legion long to implement is to create sub - groups, sub - head of the implementation is to create units),

// Players to enter the gang name(the length of not more than 16BYTE). 
        ACTION_SYN_COMBINE_SUB =
            710, // merge sub-gangs.Executive Action is the son of the players gang°ïÖ÷, merged into the parent gang

        ACTION_SYN_ATTR =
            717, // check and modify the attributes gangs, parameter is not less than 3, the default is the current player ID gang gang ID.

// Param = "szField szOpt data syn_id", szField Optional: 
// Fund: "money" (opt optional "+=", "<"),
// Prestige: "repute" (opt optional "+=", "<"),
// Number: "membernum" (opt for "<"),
// Father of gang: "fealty" (opt for "=="),
// Grade: "level" (opt optional "=", "+=", "<", "==") 
        ACTION_SYN_ALLOCATE_SYNFUND =
            729, // distribution gangs Fund.Players need a specific amount of data(up to no more than 50 percent of total funds)
        ACTION_SYN_RENAME = 731, // rename gangs.Must be sub-gangs, gangs from sub-°ïÖ÷implementation
//////////////////////////////////////////////////////// 

        ACTION_SYN_DEMISE =
            704, // shanrang, allowing only a long Legion shanrang, sub-head and sub-captain are not allowed.

// Players to enter Bangzhong name.param = "level", said players need to accept the demise of the hierarchy
        ACTION_SYN_SET_ASSISTANT = 705, // promoted to vice°ïÖ÷, players have to enter Bangzhong name.
        ACTION_SYN_CLEAR_RANK = 706, // relieved of his duties, the player must enter Bangzhong name.

        ACTION_SYN_PRESENT_MONEY =
            707, //ËÍÇ®¸øother gangs.The main input of money to help the number of the ID for other gang TASK_ITERATOR(see ACTION_SYN_FIND_BY_NAME). Money should not be less than 10000 

        ACTION_SYN_CHANGE_LEADER =
            709, // update the sub-gangs°ïÖ÷.°ïÖ÷and sub-gang new°ïÖ÷team, enter the name of sub-gangs.param = level, the requirements of the new grading°ïÖ÷ 
        ACTION_SYN_ANTAGONIZE = 711, // enemies, players must enter the names of gangs.
        ACTION_SYN_CLEAR_ANTAGONIZE = 712, // clear the enemies, players must enter the names of gangs.
        ACTION_SYN_ALLY = 713, // alliance, to ask the two team°ïÖ÷ 
        ACTION_SYN_CLEAR_ALLY = 714, // lift the alliance, the player must enter the names of gangs.
        ACTION_SYN_KICKOUT_MEMBER = 715, // by name expelled Bangzhong, players have to enter Bangzhong name. 

        ACTION_SYN_CREATENEW_PET =
            716, // (void) to create a gang to protect animals. param = "generator_id type data", at least two parameters, if any accept the name. monster generator used to control the scope of activities, cq_generator the type meaningless. 

        ACTION_SYN_CHANGESYN =
            718, // Bangzhong lesson mouth.Church mouth to mouth Church, Church to help the mouth and the total conversion.Bangzhong andÌÃÖ÷need (or°ïÖ÷) teams, one to one person.Need to enter into the church I want to name (or gang name). Prior to the past, jobs would be automatically canceled.

        ACTION_SYN_CHANGE_SUBNAME =
            719, // modify parishes were limited to the names of more than 6 bytes of parishes.Otherwise return FALSE. (Provisional function)

        ACTION_SYN_FIND_NEXT_SYN = 720, // Find next gangs will ID write TASK_ITERATOR

        ACTION_SYN_FIND_BY_NAME =
            721, // by name to find gangs, the names of players to enter the gang. ID will be written into the TASK_ITERATOR
        ACTION_SYN_FIND_NEXT_SYNMEMBER = 722, // Find next Bangzhong will ID write TASK_ITERATOR
        ACTION_SYN_SAINT = 724, // St.Knights of the escalation of the operation "=,> =" 

        ACTION_SYN_RANK =
            726, // modify RANK, ACCEPT = "rank name".Only modify RANK = 50 and below. param = "RANK50 the level of restrictions RANK40 30 of 20 10", param is empty is not restricted. 


        ACTION_SYN_UPMEMBERLEVEL = 728,
        ACTION_SYN_APPLLY_ATTACKSYN = 730, // application to attack gang


        ACTION_SYN_LIMIT = 799,

// Monster part ---------------------------------------------- ---------------------------- 
        ACTION_MST_FIRST = 800,

        ACTION_MST_DROPITEM =
            801, // monster killed off after the death of goods or money, param "dropitem itemtype" or "dropmoney money" 

// monster killed off the trap of death, param "droptrap traptype lifeperiod". 
        ACTION_MST_MAGIC = 802, // check magic. 

// Param "check type" (studied type types of magic), 
// "Check type level" (studied type types of magic, and the rating for a level-class), 
// "Learn type" (Institute of type-type magic, grade for 0), 
// "Uplevel type" (type 1 or type of magic) 
        ACTION_MST_LIMIT = 899,

// User attr part --------------------------------------------- ----------------------------- 
        ACTION_USER_FIRST = 1000,
        ACTION_USER_ATTR = 1001, // players attribute the changes and checks. "attr opt data". attr can choose
// "Life "(+=,==,<), 
// "Mana "(+=,==,<), 
// "Money "(+=,==,<), 
// "Exp "(+=,==,<), 
// "Pk "(+=,==,<), 
// "Profession "(==, set,> =, <=), 
// "Level ",(+=,==,<), 
// "Force ",(+=,==,<), 
// "Dexterity ",(+=,==,<) 
// "Speed ",(+=,==,<), 
// "Health ",(+=,==,<), 
// "Soul ",(+=,==,<), 
// "Rank ",(==,<), 
// "Rankshow ",(==,<), 
// "Iterator ",(=, <=, + =, ==), 
// "Crime" (==, set) 
// "Gamecard "(==,> =, <=) 
// "Gamecard2 "(==,> =, <=) 
// "Xp "(+=) 
// "Metempsychosis "(==, <) 
// // "Nobility_rank "(==, <, + =, =) // 
// "Mercenary_rank "(==, <, + =) // Bounty grade
// "Mercenary_exp "(==, <, + =) // Bounty experience
// "Exploit "(==, <, + =) // Gongxun value
// "Maxlifepercent "(+=,==,<) // maximum of life per thousand 
// "Turor_exp "(==,<,+=,=) 
// "Tutor_level "(==,<,+=,=) 
// "Syn_proffer "(<,+=,=) // gang contribution
// "Maxeudemon "(<,+=,=) // maximum number of calls imaginary animals

        ACTION_USER_FULL = 1002, // will fill a player's attributes. "attr". attr optional "life", "mana" 

        ACTION_USER_CHGMAP =
            1003, // cut map param "idMap nPosX nPosY bPrisonChk", bPrisonChk for optional parameters, default can not be a prison, is set to 1 can be a
        ACTION_USER_RECORDPOINT = 1004, // record-keeping point param "idMap nMapX nMapY" 
        ACTION_USER_HAIR = 1005, // "color num" 

// "Style num" 
        ACTION_USER_CHGMAPRECORD = 1006, // cut map to record points
        ACTION_USER_CHGLINKMAP = 1007, // cut to the NPC link map map.The need for NPC object. 

        ACTION_USER_TALK = 1010, // message to the players MSGTALK fat.param for the news content, data for the channel,

// Const unsigned short _TXTATR_NORMAL = 2000; 
// Const unsigned short _TXTATR_ACTION = _TXTATR_NORMAL + 2; // action 
// Const unsigned short _TXTATR_SYSTEM = _TXTATR_NORMAL + 5; // system 
// Const unsigned short _TXTATR_TALK = _TXTATR_NORMAL + 7; // chat 
// Const unsigned short _TXTATR_GM = _TXTATR_NORMAL + 11; // GM Channel
// Const unsigned short _TXTATR_WEBPAGE = _TXTATR_NORMAL + 105; // Open URL
        ACTION_USER_MAGIC = 1020, // check magic.param can be as follows: 

// "Check type" (players learned type types of magic), 
// "Check type level" (players learned type types of magic, and the rating for a level-class), 
// "Learn type" (players learn to type type magic, grade for 0), 
// "Uplevel type" (a player's type 1 or type of magic) 
// "Addexp type exp" (a player's type type magic experience points to increase exp) 
        ACTION_USER_WEAPONSKILL =
            1021, // "check type level", check the weapons of the type and level of skills, whether or not> = grade 

// "Learn type level", specified the type and level of learning skills
        ACTION_USER_LOG =
            1022, // Save the specified information to trigger gm log and into the Information(name and id), the information specified in the param

// For example, "% s to complete the task and gemstones Sky Sword", param in% s is the preservation of the location of the trigger Information
        ACTION_USER_BONUS = 1023, // get a prize.
        ACTION_USER_DIVORCE = 1024, // divorce
        ACTION_USER_MARRIAGE = 1025, // marriage inspection, married to return to 1, unmarried return 0 
        ACTION_USER_SEX = 1026, // sex check, M to return to 1, women return 0 

        ACTION_USER_EFFECT =
            1027, // trigger action figures specified additional effects, param to "opt effect", opt to support the "self", "couple", "team", "target", effect to effect the name of

        ACTION_USER_TASKMASK =
            1028, // task mask related to the operation, param to "opt idx", opt for the operation, support for "chk", "add", "clr", idx mission number value(0-31)

        ACTION_USER_MEDIAPLAY =
            1029, // media player, param to "opt media", opt to support the "play, broacast", "media" for the media file name

        ACTION_USER_SUPERMANLIST =
            1030, // query unique list, start value in the existence of TASK_ITERATOR.param = "idNextTask number", idNextTask next TASK value, number is the number of each list item downlink.
        ACTION_USER_CHKIN_CARD = 1031, // delete the players onto the game card items, add a game card records
        ACTION_USER_CHKOUT_CARD = 1032, // add an item to the card game players, game cards to delete a record

        ACTION_USER_CREATEMAP =
            1033, // create a map link to home_id players, the needs of the target audience. param = "name owner_type owner_id mapdoc type portal_x portal_y reborn_map reborn_portal res_lev".partal refers to the point of entry coordinates, res_lev that map hierarchy (for upgrade). 
        ACTION_USER_ENTER_HOME = 1034, // return to their home.
        ACTION_USER_ENTER_MATE_HOME = 1035, // back to their spouses home.
        ACTION_USER_CHKIN_CARD2 = 1036, // delete the players onto the game card 2 items, add a game card 2 records
        ACTION_USER_CHKOUT_CARD2 = 1037, // add a game card 2 items to the players, to delete a game card 2 records

        ACTION_USER_FLY_NEIGHBOR =
            1038, // on the map to find a group _ROLE_NEIGHBOR_DOOR types of NPC, immediately cut screen to the NPC Office.param = "serial", serial refers data3 value.

        ACTION_USER_UNLEARN_MAGIC =
            1039, // reincarnation, the forgotten magic skills, the skills of the future could be "epiphany." param = "type1 type2 ...", at least one parameter, a maximum of 20 parameters.

        ACTION_USER_REBIRTH =
            1040, // reincarnation.If you have not turned or grade, and will fail.Check whether the player has been transferred, occupational requirements, level.Automatically modify player career, class, body and equipment levels, the redistribution of points.param = "nProf nLook" 

// The following features completed by other ACTION: Task Award; 15,40,100 class inaugural awards; mounts incentives; at any time back to town; skills incentives.
        ACTION_USER_WEBPAGE = 1041, // notify the client to open the page.param = "http:// ....."

        ACTION_USER_BBS =
            1042, // in the BBS bulletin boards, add a SYSTEM news channel, message the name of human players. The need for USER objects, retaining only one of each USER. param is the message. 

        ACTION_USER_UNLEARN_SKILL =
            1043, // reincarnation, the forgotten all weapons skills, the skills of the future could be "epiphany." 

        ACTION_USER_DROP_MAGIC =
            1044, // reincarnation, deleted magic skills.param = "type1 type2 ...", at least one parameter, a maximum of 20 parameters.

        ACTION_USER_OPEN_DIALOG =
            1046, // notify the client to open an interface. data = idDialog.param = "task_id0 task_id1 task_id2 task_id3 ...", can no param, a maximum of 20 task_id, task_id not to 0, allowing the client to choose the next TASK.Non-param when the client can only upload "the client can trigger the TASK". Param when there is, cq_task.client_active must be to 0. 
        ACTION_USER_CHGMAP_REBORN = 1047, // cut screen to the resurrection point. 

// ACTION_USER_ADD_WPG_BADGE = 1048, // add PK Cup keepsake weeks, according to found the first thing to add token types. The items must be superimposed.A maximum of only two.
        ACTION_USER_DEL_WPG_BADGE = 1049, // delete all week keepsake PK tournament. 

        ACTION_USER_CHK_WPG_BADGE =
            1050, // check that there is only one player who param types of goods (the number can only have one), no other week PK tournament keepsake.param is empty that it can not have any weeks PK Cup keepsake.

        ACTION_USER_TAKESTUDENTEXP =
            1051, // extract contributions apprentice experience. PszAccept specified the need for players to return to the experience of extracting value, automatically deducted from the experience of instructors.

        ACTION_USER_CHGTO_MAINMAP = 1052, // to the main point of the revival of the revival of the map

        ACTION_USER_CHGTO_RANDOMPOS =
            1053, // characters randomly fly any of the current map coordinates (the points can not mask)


//--- Mandate system to record the details of the task --- begin 

        ACTION_USER_TASK_MANAGER = 1080, /////////////////////////////////////////////// // 
// // Data: the task number 
// Param: 'new' (to create a new record) 
// 'Delete' (delete records) 
// 'Isexit' (the existence of the task) 
///////////////////////////////////////////////// 

        ACTION_USER_TASK_OPE = 1081, /////////////////////////////////////////////// // 

// data: the task number, if data == -1, then the following operation is carried out against FindNext
// Param: 'ope opt data', data(value)
// Ope(phase) opt(> =, ==, +=,=) mission operation phase 
// Ope(completenum) opt(> =, ==, +=,=) task is completed on the number of operations
// Ope(begintime) opt(> =, ==, +=,=, reset) start time of the mission to operate, for + = time in seconds parameter; for when ">=,==,=" "yyyy -mm-dd hh: mm: ss "for the format
// Reset the start of the mandate that it will set-up time for the current time
        ACTION_USER_TASK_LOCALTIME = 1082, ///////////////////////////////////////////////////////// 
// Data: the task number 
// param: 'seconds', the current time with the task start time comparison action; if the current time with the task start time is greater than the difference between the param, then return true. Otherwise, return false 
////////////////////////////////////////////////////////////////////// 

        ACTION_USER_TASK_FIND =
            1083, // for gamers tasks inquiries, records are in accordance with the userid, taskid ascending collection
// param: 'find taskid phase completenum'; in accordance with the task ID, phase, the completion of a specific record number of inquiries; phase with cocompletenum at the same time -1, the only record of inquiry in line with the taskid
// 'Findnext'; inquiries under a record
//--- Mandate system to record the details of the task --- end 


// Team part. ¡ï no team will return false. The following ACTION must be triggered by the captain,
// Operator for each team (usually does not include the captain), team members must be in the framework of a screen.
// NOTE: All team members must be true are only return true; otherwise return false 
// ¡ï ----------------------------------------------- ---------------- 
        ACTION_TEAM_BROADCAST = 1101, // to force a message broadcast channels.param = news.
        ACTION_TEAM_ATTR = 1102, // check or operating team attributes.

// Param = "field opt data",
// Field = "money "(+=,<,>,==),
// Field = "level "(<,>,==),
// Field = "count"(the number of players including captain,<,==),
// Field = "count_near"(the number of players including captain, the map must be alive,<,==),
// Field = "mate"(only need to field, must be alive),
// Field = "friend"(only need to field, must be alive),
        ACTION_TEAM_LEAVESPACE = 1103, // check the remaining space backpack, param = "space weight packtype".

// Packtype for the need to check the backpack type, range 50 ~ 53 
        ACTION_TEAM_ITEM_ADD = 1104, // Add items. data = itemtype_id
        ACTION_TEAM_ITEM_DEL = 1105, // delete items. data = itemtype_id
        ACTION_TEAM_ITEM_CHECK = 1106, // Detection items. data = itemtype_id

        ACTION_TEAM_CHGMAP =
            1107, // team all screens (including captain), only for a map with the group cut screen, all to be alive. param = "mapid x y"

        ACTION_TEAM_CHK_ISLEADER = 1501, // check whether the captain, no parameters
        ACTION_USER_LIMIT = 1999,

// Event part, need not any ptr ----------------------------------------- --------------------------------- 
        ACTION_EVENT_FIRST = 2000,

        ACTION_EVENT_SETSTATUS =
            2001, // set up a state map.param = "mapid status_bit data", status_bit = (STATUS_WAR = 1,), data = 0 or 1. 
        ACTION_EVENT_DELNPC_GENID = 2002, // (void) Delete MONSTER.param = "idMap idGen".

        ACTION_EVENT_COMPARE =
            2003, // compare various attributes. "data1 opt data2". data1, data2 for belt% of the general parameters, by comparison with several symbols.optional opt "==","<","<=" 

        ACTION_EVENT_COMPARE_UNSIGNED =
            2004, // compare various attributes. "data1 opt data2". data1, data2 for belt% of the general parameters, according to the number of unsigned comparison.optional opt "==","<","<=" 

        ACTION_EVENT_CHANGEWEATHER =
            2005, // modify designated REGION weather. param = "idMap idRegion Type Intensity Dir Color KeepSecs", Type, Intensity = 0 ~ 999, Dir = 0 ~ 359, Color = 0x00RRGGBB, KeepSecs = seconds

        ACTION_EVENT_CREATEPET =
            2006, // create a MONSTER.param = "nOwnerType idOwner idMap nPosX nPosY idGen idType nData szName", at least seven parameters, if any accept the name, otherwise named by name. monster generator used to control the scope of activities, cq_generator the type meaningless. idOwner for 0:00, do not save. 

        ACTION_EVENT_CREATENEW_NPC =
            2007, // create a NPC.param = "name type sort lookface ownertype ownerid mapid posx posy life base linkid task0 task0 ... task7 data0 data1 data2 data3 datastr".At least 9 parameters.

        ACTION_EVENT_COUNTMONSTER =
            2008, // check with the number of maps MONSTER.param = "idMap field data opt num", field = "name"(by name), "gen_id" (by type), data = to statistics name or type, opt = "<","=="¡£ 

        ACTION_EVENT_DELETEMONSTER =
            2009, // delete a map of the MONSTER. param = "idMap type data name", at least two parameters. If the data does not match the 0 at the same time data, if a name is at the same time matching name. 

        ACTION_EVENT_BBS =
            2010, // in the BBS bulletin boards, add a SYSTEM news channel, message man-made "SYSTEM". param is the message. 

        ACTION_EVENT_ERASE =
            2011, // delete the specified NPC. Limited dynamic NPC.Note: deleted, not to operate on such NPC. param = "idMap type": delete the specified map for the type of all types of NPC.
        ACTION_EVENT_LIMIT = 2099,

// Event part, need not any ptr ----------------------------------------- --------------------------------- 
        ACTION_TRAP_FIRST = 2100,
        ACTION_TRAP_CREATE = 2101, // create a trap.param = "type look owner_id map_id pos_x pos_y data".

        ACTION_TRAP_ERASE =
            2102, // delete a trap.param = "", delete the current trap. Note: Do not delete after the operation of the trap.

        ACTION_TRAP_COUNT =
            2103, // Detection of the type of trap type number, less than the count to return to true. param = "map_id pos_x pos_y pos_cx pos_cy count type".

        ACTION_TRAP_ATTR =
            2104, // modify the properties of traps (not save). param = "id field opt num".field: "type" (opt: "="), "look" (opt: "=")¡£ 
        ACTION_TRAP_LIMIT = 2199,

// Wanted list part --------------------------------------------- ----------------------------- 
        ACTION_WANTED_FIRST = 3000,
        ACTION_WANTED_NEXT = 3001, // search for the next reward will be TASK_ITERATOR write idx

        ACTION_WANTED_NAME =
            3002, // players to return to reward those who have been designated pszAccept name, and ACTION_WANTED_NEW GC.
        ACTION_WANTED_BONUTY = 3003, // players to return to the amount specified pszAccept, and ACTION_WANTED_NEW GC.

        ACTION_WANTED_NEW =
            3004, // through CUser:: m_WantedInfo reward the production of new records, and the joint use of two action.
        ACTION_WANTED_ORDER = 3005, // receive pszAccept specified reward
        ACTION_WANTED_CANCEL = 3006, // 2 times the price of the abolition of pszAccept specified reward
        ACTION_WANTED_MODIFYID = 3007, // players to return to the specified changes pszAccept reward id.

        ACTION_WANTED_SUPERADD =
            3008, // players to return to the designated pszAccept additional reward money, with the joint use of ACTION_WANTED_ID.
        ACTION_POLICEWANTED_NEXT = 3010, // search for the next official reward will idx write TASK_ITERATOR
        ACTION_POLICEWANTED_ORDER = 3011, //½Ò°ñ(pszAccept specified number)
        ACTION_POLICEWANTED_CHECK = 3012, // check whether the trigger was wanted by officials
        ACTION_WANTED_LIMIT = 3099,
    }
}