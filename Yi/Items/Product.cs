namespace Yi.Items
{
    public class Product
    {
        public int Owner { get; set; }
        public int ShopId { get; set; }
        public int Price { get; set; }
        public byte Amount{ get; set; }

        public Product(int owner, int price, int shopId)
        {
            ShopId = shopId;
            Owner = owner;
            Price = price;
        }
    }
}
