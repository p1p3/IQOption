using System.Threading.Tasks;

namespace IQOptionClient.Http.Resources
{
    public interface ILogin<TResult> where TResult : class
    {
        Task<TResult> Login(string username, string password);
    }
}