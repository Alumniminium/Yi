using System;

namespace YiX.Enums
{
    [Flags]
    public enum StorageAccess
    {
        None = 0x000000,
        ItemSee = 0x000001,
        ItemAdd = 0x000010,
        ItemTake = 0x000100,
        MoneySee = 0x001000,
        MoneyAdd = 0x010000,
        MoneyTake = 0x1000000,

        Browse = MoneySee | ItemSee,
        TakeItems = ItemSee | ItemTake,
        TakeMoney = MoneySee | MoneyTake,

        AddItems = ItemSee | ItemAdd,
        AddMoney = MoneySee | MoneyAdd,

        Take = TakeItems | TakeMoney,
        Add = AddItems | AddMoney,

        Owner = Take | Add,
    }
}