namespace IQOptionClient.Ws.Models
{
    public class BuyInputModel
    {
        public BuyInputModel(double price, Active active, Direction direction)
        {
            Price = price;
            Active = active;
            Direction = direction;
        }

        public double Price { get; }
        public Active Active { get; }
        public Direction Direction { get; }
    }
}
