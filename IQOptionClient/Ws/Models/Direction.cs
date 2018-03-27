using System.ComponentModel;

namespace IQOptionClient.Ws.Models
{
    public enum Direction
    {
        [Description("put")]
        Put = 0,
        [Description("call")]
        Call = 1,
    }
}
