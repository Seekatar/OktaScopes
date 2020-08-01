using System.Threading.Tasks;

namespace app.Services
{
    public interface ITokenService
    {
         Task<string> GetToken();
    }
}