using System;

namespace IQOptionClient.Ws.Models
{
    public class BuyInputModel
    {
        public BuyInputModel(double price, Active active, Direction direction, Balance balance, TimeSpan operationTime)
        {
            Price = price;
            Active = active;
            Direction = direction;
            Balance = balance;
            OperationTime = operationTime;
        }

        public double Price { get; }
        public Active Active { get; }
        public Direction Direction { get; }
        public Balance Balance { get; }

        public TimeSpan OperationTime { get; }
    }

}
