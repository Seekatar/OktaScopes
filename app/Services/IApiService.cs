using System.Collections.Generic;
using System.Threading.Tasks;

namespace app.Services
{
    public interface IApiService
    {
         Task<IList<string>> GetValues();
    }
}