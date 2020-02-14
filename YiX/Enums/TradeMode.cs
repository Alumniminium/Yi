// -------- Yi --------
// Project: Library File: TradeMode.cs 
// Created: 27/10/2015/2015 at 3:09 PM
// Last Edit: 08/12/2015 at 12:31 PM
// By: Buddha

namespace YiX.Enums
{
    public enum TradeMode 
    {
        RequestNewTrade = 1,
        RequestCloseTrade = 2,
        ShowTradeWindow = 3,
        CloseTradeWindowSuccess = 4,
        CloseTradeWindowFail = 5,
        RequestAddItemToTrade = 6,
        RequestAddMoneyToTrade = 7,
        DisplayMoney = 8,
        DisplayMoneyAdd = 9,
        RequestCompleteTrade = 10,
        ReturnItem = 11
    }
}