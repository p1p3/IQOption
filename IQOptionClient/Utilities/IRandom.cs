namespace IQOptionClient.Utilities
{
    public interface IRandomNumbers
    {
        int GenerateValue(int min, int max);

        //Generate a value between 0 and 2147483647
        int GenerateValue();
    }
}