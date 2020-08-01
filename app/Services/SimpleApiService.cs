using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using app.Models;
using Newtonsoft.Json;

namespace app.Services
{
  public class SimpleApiService : IApiService
  {
    private HttpClient client = new HttpClient();
    private readonly ITokenService tokenService;
    public SimpleApiService(ITokenService tokenService)
    {
      this.tokenService = tokenService;
    }

    public async Task<IList<string>> GetValues()
    {
      List<string> values = new List<string>();
      var token = await tokenService.GetToken();
      client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      var res = await client.GetAsync("http://localhost:5000/api/values");
      if(res.IsSuccessStatusCode)
      {
        var json = res.Content.ReadAsStringAsync().Result;
        values = JsonConvert.DeserializeObject<List<string>>(json);
      }
      else
      {
        values = new List<string>{res.StatusCode.ToString(), res.ReasonPhrase};
      }
      return values;
    }
  }
}