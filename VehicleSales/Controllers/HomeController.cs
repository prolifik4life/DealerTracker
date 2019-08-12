using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleSales.Models;
using VehicleSales.Services.interfaces;

namespace VehicleSales.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUploadService _uploadService;
        public HomeController(IUploadService uploadsedrvice)
        {
            _uploadService = uploadsedrvice;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile upload)
        {
            IEnumerable<VehicleSale> model = null;
            if(upload == null)
            {
                ModelState.AddModelError("File", "You have not selected a file to upload.");
                return View();
            }
            if (upload.FileName.EndsWith(".csv"))
            {
                ViewBag.FileName = upload.FileName;
                model = _uploadService.Process(upload);
                ViewBag.MostSoldVehicle = _uploadService.GetVehicleTopSale();
            }
            else
            {
                ModelState.AddModelError("File", "This file format is not supported");
                return View();
            }
            
            return View(model);
        }
    }
}