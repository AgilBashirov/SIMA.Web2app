using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace SIMA.WebUI.QrCodeHelper
{
    public class QrCodeGenerate
    {
        public static string GenerateQrCode(string content, string imageName)
        {
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData qrData = qr.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode result = new QRCode(qrData);
            Bitmap bitmap = result.GetGraphic(20);

            var Image = $"{imageName}.png";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "QrImages", Image);
            bitmap.Save(path, ImageFormat.Png);

            return $"/qr/{Image}";
        }
    }
}
