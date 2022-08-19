using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SimaTest.Models;
using SimaTest.QrCodeHelper;
using SimaTest.VmModels;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Web2App.Interfaces;
using Web2App.Models;

namespace SimaTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQrGenerator _qrGenerator;

        public HomeController(ILogger<HomeController> logger, IQrGenerator qrGenerator)
        {
            _logger = logger;
            _qrGenerator = qrGenerator;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContractVm vm)
        {

            string DomainName = Request.GetDisplayUrl();
            ContractModel model = new ContractModel();
            model.SignableContainer.ProtoInfo.Name = "web2app";
            model.SignableContainer.ProtoInfo.Version = vm.Version;

            model.SignableContainer.OperationInfo.Type = vm.Type;
            model.SignableContainer.OperationInfo.OperationId = vm.OperationId;
            model.SignableContainer.OperationInfo.NbfUTC = new DateTimeOffset(vm.NbfUTC).ToUnixTimeSeconds();
            model.SignableContainer.OperationInfo.ExpUTC = new DateTimeOffset(vm.ExpUTC).ToUnixTimeSeconds();

            if (vm.Assignee.Contains(" "))
            {
                foreach (var item in vm.Assignee.Split(" "))
                {
                    model.SignableContainer.OperationInfo.Assignee.Add(item);
                }
            }
            else
            {
                model.SignableContainer.OperationInfo.Assignee.Add(vm.Assignee);
            }

            model.SignableContainer.ClientInfo.ClientId = vm.ClientId;
            model.SignableContainer.ClientInfo.IconURI = vm.IconURI;
            model.SignableContainer.ClientInfo.Callback = vm.Callback;

            //Header
            model.Header.Signature = QrCodeGenerate.Signature(model.SignableContainer, vm.SecretKey);
            model.Header.AlgName = "HMACSHA256";


            var encodedContract = QrCodeGenerate.EncodeContract(model);

            var qRtext = $"{DomainName}Home/GetFile/?tsquery={encodedContract}";


            //generate qr code
            var url = await _qrGenerator.GenerateQr(qRtext, model.SignableContainer.ClientInfo.IconURI);


            
            //file
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp", "test.pdf");
            if (url is not null)
            {
                using (FileStream reader = new FileStream(path, FileMode.Open))
                {
                    var buffer = new byte[reader.Length];
                    await reader.ReadAsync(buffer, 0, buffer.Length);

                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", vm.OperationId + ".pdf");
                    using (FileStream filestream = new FileStream(path, FileMode.Create))
                    {
                        await filestream.WriteAsync(buffer, 0, buffer.Length);
                    }

                }

                return Redirect(url);
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult GetFile([FromBody] string tsQuery)
        {
            var headers = GetHeaders(HttpContext.Request.Headers);
            var tcCertResult = HttpContext.Request.Headers.TryGetValue("ts-cert", out StringValues tcCert);
            var tcSignResult = HttpContext.Request.Headers.TryGetValue("ts-sign", out StringValues tcSign);
            var tcSignAlgResult = HttpContext.Request.Headers.TryGetValue("ts-sign-alg", out StringValues tcAlg);


            try
            {
                string str = Encoding.UTF8.GetString(Convert.FromBase64String(tsQuery));
                var container = JsonSerializer.Deserialize<ContractModel>(str);

                if (container is null)
                {
                    throw new NullReferenceException(nameof(tsQuery));
                }

                string base64 = String.Empty;
                if (container.SignableContainer.OperationInfo.Type == OperationType.Auth)
                {
                    var challange = Guid.NewGuid().ToString();
                    base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(challange));

                }
                else
                {
                    //You need to find and get correct base64 of file (I will return random file)
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", container.SignableContainer.OperationInfo.OperationId + ".pdf");

                    using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        base64 = Convert.ToBase64String(buffer);
                    }

                }

                return Json(new
                {
                    filename = container.SignableContainer.OperationInfo.Type == OperationType.Auth ? "challange" : container.SignableContainer.OperationInfo.OperationId + ".pdf",
                    data = base64
                });
            }
            catch (Exception exp)
            {

                return Json(new
                {
                    errormessage = exp.Message
                });
            }

        }

        [HttpPost]
        public IActionResult Callback([FromBody] CallbackPostModel model)
        {
            var headers = GetHeaders(HttpContext.Request.Headers);

            //get request body bytes
            var json = JsonSerializer.Serialize(model);
            var bodyBuffer = Encoding.UTF8.GetBytes(json);

            #region Model Validation and OperationId Update
            try
            {
                if (model != null && model.OperationId != null)
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", model.OperationId + ".pdf"));

                    using (FileStream stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", model.OperationId + ".pdf"), FileMode.Create))
                    {
                        var buffer = Convert.FromBase64String(model.DataSignature);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }

                return Json(new
                {
                    status = "success"
                });
            }
            catch (Exception exp)
            {
                return Json(new
                {
                    status = exp.Message
                });
            }
            #endregion
        }


        private string GetHeaders(IHeaderDictionary headersDictionary)
        {
            string headers = String.Empty;
            foreach (var header in headersDictionary)
            {
                headers += header.Key + ":" + header.Value + ",";
            }

            headers = headers.Remove(headers.LastIndexOf(","), 1);
            return headers;
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