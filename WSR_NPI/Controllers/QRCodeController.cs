using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WSR_NPI.Models;

namespace WSR_NPI.Controllers
{
    public class QRCodeController : Controller
    {
        // GET: QRCode
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Picture pic, HttpPostedFileBase uploadImage)
        {
            if (uploadImage != null)
            {
                var fileName = System.Guid.NewGuid().ToString() + System.IO.Path.GetExtension(uploadImage.FileName);
                string physicalPath = Server.MapPath("~/Images/Uploads" + fileName);

                uploadImage.SaveAs(physicalPath);
                byte[] imageData = null;
                // считываем переданный файл в массив байтов
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
                // установка массива байтов
                pic.Image = imageData;

                pic.IsView = true;

                ExportDataJson exportData;
                QRCodeDecoder decoder = new QRCodeDecoder(); // создаём "раскодирование изображения"
                try
                {
                    var data = decoder.decode(new QRCodeBitmapImage(new Bitmap(physicalPath)));

                    exportData = new ExportDataJson { existQr = true, qrData = data };

                }
                catch
                {
                    exportData = new ExportDataJson { existQr = false, qrData = null };
                    
                }
                pic.Name = JsonConvert.SerializeObject(exportData);
                return View(pic);
                //return PartialView("Scan", JsonConvert.SerializeObject(exportData));
            }

            return View(pic);
        }

        
    }
    public class ExportDataJson
    {
        public bool existQr { get; set; }
        public string qrData { get; set; }
    }
}