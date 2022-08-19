using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Net;

namespace SimaIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public async Task<HttpStatusCode> Auth()
        {
            var body = new
            {
                SignableContainer = new
                {
                    ProtoInfo = new
                    {
                        Name = "web2app",
                        Version = "1.0"
                    },
                    OperationInfo = new
                    {
                        Type = "Auth",
                        OperationId = "123456789",
                        NbfUTC = 1649721600,
                        ExpUTC = 1650326400,
                        Assignee = new List<string>()
                    },
                    ClientInfo = new
                    {
                        ClientId = 292,
                        IconURI = "https://img.icons8.com/color/search/96",
                        Callback = "https://testapi-sima.rabita.az/api/FileSigning/callback"
                    }

                },
                Header = new
                {
                    AlgName = "HMACSHA256",
                    Signature = "nQxNMasxoL1WuLJ2x1kRhFAmwFTbDxyjMGnF1Tycyr0="
                }

            };
            string jsonBody = System.Text.Json.JsonSerializer.Serialize(body);



            var client = new RestClient("https://scanme.sima.az/");
            var req = new RestRequest(Method.POST);
            //req.AddHeader("AlgName", "HMACSHA256");
            //req.AddHeader("Signature", "nQxNMasxoL1WuLJ2x1kRhFAmwFTbDxyjMGnF1Tycyr0=");
            //req.AddJsonBody(jsonBody);

            //req.RequestFormat = DataFormat.Json;
            IRestResponse response = await client.ExecuteAsync(req);
            var isOK = response.StatusCode;

            return isOK;
        }

        [HttpPost("/callback")]
        public void Callback(string tsquery)
        {

        }
    }
}
