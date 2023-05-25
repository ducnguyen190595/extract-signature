using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using ES.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;

namespace ES.WebUI.Controllers
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Upload([FromForm]UploadModel model)
        {

            string imagePath = "D:\\WORKPLACE\\PROJECTS\\extract-signature\\ExtractSignature\\Assets\\1.png";
            Mat originalImage = CvInvoke.Imread(imagePath, ImreadModes.Unchanged);

            // Tạo ảnh mask chỉ giữ lại các pixel có màu đen (0, 0, 0) và giữ lại alpha channel
            Mat mask = new Mat();
            CvInvoke.InRange(originalImage, new ScalarArray(new MCvScalar(0, 0, 0)), new ScalarArray(new MCvScalar(0, 0, 0)), mask);

            // Tìm bounding rectangle của vùng chứa chữ ký bằng cách xác định giới hạn của mask
            Rectangle signatureRect = CvInvoke.BoundingRectangle(mask);

            // Cắt chữ ký từ ảnh gốc và tạo ảnh mới
            Mat signatureImage = new Mat(originalImage, signatureRect);

            // Tạo một ảnh mới với kích thước và định dạng phù hợp
            Mat transparentSignatureImage = new Mat(signatureImage.Size, DepthType.Cv8U, 4);

            // Sao chép dữ liệu từ ảnh chữ ký vào ảnh mới với kênh alpha tương ứng
            CvInvoke.CvtColor(signatureImage, transparentSignatureImage, ColorConversion.Bgr2Bgra);

            // Lưu ảnh chữ ký thành file mới
            string signatureImagePath = "D:\\WORKPLACE\\PROJECTS\\extract-signature\\ExtractSignature\\Assets\\demo.png";
            signatureImage.Save(signatureImagePath);

            return RedirectToAction("Index");
        }
    }
}