using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleSales.Models;

namespace VehicleSales.Services.interfaces
{
    public interface IUploadService
    {
        IEnumerable<VehicleSale> Process(IFormFile uploadedFile);
        string GetVehicleTopSale();
    }
}
