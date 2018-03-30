namespace IQOptionClient.Ws.Models
{
    public class CandleSubscription
    {
        public CandleSubscription(Active active, double sizeInSeconds)
        {
            Active = active;
            SizeInSeconds = sizeInSeconds;
        }

        public Active Active { get; }
        public double SizeInSeconds { get; }
    }
}
