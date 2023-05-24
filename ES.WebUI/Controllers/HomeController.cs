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

            using (var image = new Image<Gray, byte>("D:\\WORKPLACE\\PROJECTS\\extract-signature\\ExtractSignature\\Assets\\3.bmp"))
            {
                // Chuyển ảnh sang ảnh grayscale để xử lý dễ dàng hơn
                var grayImage = image.Convert<Gray, byte>();

                // Áp dụng thresholding để tách chữ ký từ nền
                CvInvoke.Threshold(grayImage, grayImage, 200, 255, ThresholdType.Binary);

                // Tìm kiếm các contours trong ảnh
                var contours = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(grayImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                // Tìm contour có diện tích lớn nhất, đó chính là chữ ký
                double maxArea = 0;
                int maxAreaContourIndex = -1;
                for (int i = 0; i < contours.Size; i++)
                {
                    double area = CvInvoke.ContourArea(contours[i]);
                    if (area > maxArea)
                    {
                        maxArea = area;
                        maxAreaContourIndex = i;
                    }
                }

                // Nếu tìm thấy contour chữ ký
                if (maxAreaContourIndex >= 0)
                {
                    // Tạo bounding rectangle xung quanh chữ ký
                    var signatureRect = CvInvoke.BoundingRectangle(contours[maxAreaContourIndex]);

                    // Cắt ảnh chữ ký từ ảnh gốc
                    var signatureImage = image.GetSubRect(signatureRect);

                    // Thay đổi kích thước của ảnh chữ ký để có cùng width với chữ ký ban đầu
                    var resizedSignatureImage = signatureImage.Resize(image.Width, signatureImage.Height, Inter.Linear);

                    // Lưu ảnh chữ ký đã cắt và thay đổi kích thước vào tệp đầu ra
                    resizedSignatureImage.Save("D:\\WORKPLACE\\PROJECTS\\extract-signature\\ExtractSignature\\Assets\\demo.bmp");
                }
            }

            return RedirectToAction("Index");
        }
    }
}