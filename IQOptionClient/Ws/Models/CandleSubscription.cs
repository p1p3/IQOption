namespace IQOptionClient.Ws.Models
{
    public class CandleSubscription
    {
        public CandleSubscription(Active active, int sizeInSeconds)
        {
            Active = active;
            SizeInSeconds = sizeInSeconds;
        }

        public Active Active { get; }
        public int SizeInSeconds { get; }
    }
}
