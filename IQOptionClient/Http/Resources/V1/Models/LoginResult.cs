namespace IQOptionClient.Http.Resources.V1.Models
{
    public class LoginResult
    {
        public LoginResult(string ssid)
        {
            Ssid = ssid;
        }

        public string Ssid { get; }
    }
}