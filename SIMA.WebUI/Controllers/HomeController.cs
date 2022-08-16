using Microsoft.AspNetCore.Mvc;
using QRCoder;
using SIMA.WebUI.Models;
using SIMA.WebUI.QrCodeHelper;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace SIMA.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ContractModel model)
        {

            if (model == null)
                return null;

            var json = JsonSerializer.Serialize(model);
            string base64EncodedExternalAccount = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            byte[] byteArray = Convert.FromBase64String(base64EncodedExternalAccount);
            string fileFormBase64 = Convert.ToBase64String(byteArray);

            var encodedContract = fileFormBase64;

            var text = $"https://scanme.sima.az/Home/GetFile/?tsquery={encodedContract}";

            string url = QrCodeGenerate.GenerateQrCode(text, model.SignableContainer.ClientInfo.IconURI);

            if (url is not null)
            return Redirect(url);

            return View(model);
        }

        


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}