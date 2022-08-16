using QRCoder;
using SimaTest.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SimaTest.QrCodeHelper
{
    public class QrCodeGenerate
    {
        //public static string GenerateQrCode(string content, string imageName)
        //{
        //    QRCodeGenerator qr = new QRCodeGenerator();
        //    QRCodeData qrData = qr.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        //    QRCode result = new QRCode(qrData);
        //    Bitmap bitmap = result.GetGraphic(20);

        //    var Image = $"{imageName}.png";
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "QrImages", Image);
        //    bitmap.Save(path, ImageFormat.Png);

        //    return $"/qr/{Image}";qrCodes
        //}

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static String GetHMAC(String text, String key)
        {
            var enc = Encoding.Default;
            HMACSHA256 hmac = new HMACSHA256(enc.GetBytes(key));
            hmac.Initialize();

            byte[] buffer = enc.GetBytes(text);
            return BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
        }

        public static string Signature(SignableContainer model, string secretKey)
        {
            var json = JsonSerializer.Serialize(model);
            string base64EncodedExternalAccount = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            byte[] byteArray = Convert.FromBase64String(base64EncodedExternalAccount);
            string fileFormBase64 = Convert.ToBase64String(byteArray);

            var CH = QrCodeGenerate.ComputeSha256Hash(fileFormBase64);

            var S = QrCodeGenerate.GetHMAC(CH, secretKey);

            return S;
        }

        public static string EncodeContract(ContractModel model)
        {
            var json = JsonSerializer.Serialize(model);
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            byte[] byteArray = Convert.FromBase64String(base64);
            string fileFormBase64 = Convert.ToBase64String(byteArray);

            return fileFormBase64;
        }

    }
}
