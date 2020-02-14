namespace YiX.Items
{
    public class Product
    {
        public int Owner { get; set; }
        public int ShopId { get; set; }
        public int Price { get; set; }
        public Item Item { get; set; }

        public Product(int owner, int price, int shopId, Item item)
        {
            ShopId = shopId;
            Owner = owner;
            Price = price;
            Item = item;
        }
    }
}
