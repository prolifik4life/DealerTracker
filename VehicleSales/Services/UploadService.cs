using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VehicleSales.Models;
using VehicleSales.Services.interfaces;


namespace VehicleSales.Services
{
    public class UploadService : IUploadService
    {
        private IList<string> Vehicles { get; set; }

        public string GetVehicleTopSale()
        {
            return GetMostSoldVehicle(this.Vehicles);
        }

        public IEnumerable<VehicleSale> Process(IFormFile uploadedFile)
        {
            try
            {
                var sales = new List<VehicleSale>();
                List<string> vehicles = new List<string>();
                
                var stream = uploadedFile.OpenReadStream();
                if (stream.CanRead)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var headerLine = reader.ReadLine();

                        string dealerNumber = string.Empty, 
                            customer = string.Empty, 
                            dealership = string.Empty, 
                            vehicle = string.Empty, 
                            price = string.Empty;
                        DateTime date = new DateTime();

                        while (!reader.EndOfStream)
                        {
                            int dealerNumberIndx = 0;
                            int customerNameIndx = 1;
                            int dealershipnameIndx = 2;
                            int vehicleIndx = 3;
                            int priceIndx = 4;
                            int dateIndx = 5;

                            
                            var row = reader.ReadLine();

                            var rowvalues = row.Split(',');
                            for(int i = 0; i < rowvalues.Length; i++)
                            {
                                if (i == dealerNumberIndx)
                                    dealerNumber = rowvalues[i];
                                else if (i == customerNameIndx)
                                    customer = rowvalues[i];
                                else
                                {
                                    if (i == dealershipnameIndx)
                                    {
                                        if (rowvalues[i].StartsWith('"'))
                                        {
                                            var endQuoteIndex = GetIndexOfNextQuote(rowvalues, i);
                                            dealership = GetRwoColumnValue(rowvalues, i, endQuoteIndex);

                                            i = endQuoteIndex + 1;
                                            vehicleIndx = i;
                                            priceIndx = vehicleIndx + 1;
                                        }
                                        else
                                        {
                                            dealership = rowvalues[i];
                                        }
                                        vehicles.Add(rowvalues[vehicleIndx]);
                                    }
                                    else if (i == priceIndx)
                                    {
                                        if (rowvalues[i].StartsWith('"'))
                                        {
                                            var endQuoteIndex = GetIndexOfNextQuote(rowvalues, i);
                                            price = GetRwoColumnValue(rowvalues, i, endQuoteIndex);

                                            i = endQuoteIndex;
                                            dateIndx = i + 1;
                                        }
                                        else
                                        {
                                            price = rowvalues[i];
                                        }

                                    }
                                    else if (i == dateIndx)
                                    {
                                        var dateparts = rowvalues[i].Split('/');
                                        date = new DateTime(int.Parse(dateparts[2]), int.Parse(dateparts[0]), int.Parse(dateparts[1]));
                                    }
                                }

                            }
                            
                            sales.Add(new VehicleSale {
                                DealNumber = int.Parse(dealerNumber),
                                CustomerName = customer,
                                DealershipName = dealership,
                                Vehicle = rowvalues[vehicleIndx],
                                Price = price,
                                Date = date
                            });
                        }
                    }
                }
                this.Vehicles = vehicles;
                return sales;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private int GetIndexOfNextQuote(string[] rowvalues, int firstIndex)
        {
            int index = 0;
            for(int i = firstIndex +1; i<rowvalues.Length; i++)
            {
                if (rowvalues[i].EndsWith('"'))
                {
                    index = i;
                    break;
                }  
            }
            return index;
        }

        private string GetRwoColumnValue(string[] rowvalues, int startIndx, int endIndx)
        {
            var strValue = rowvalues[startIndx];

            for(int i = startIndx + 1; i <= endIndx; i++)
            {
                strValue = $"{strValue}, {rowvalues[i]}".Replace('"', ' ').Trim();
            }

            return strValue;
        }

        private string GetMostSoldVehicle(IList<string> vehicleSales)
        {
            try
            {
                string mostSoldvehicle = string.Empty;

                var counts = new Dictionary<string, int>();
                foreach (string vehicle in vehicleSales)
                {
                    int count;
                    counts.TryGetValue(vehicle, out count);
                    count++;
                    counts[vehicle] = count;
                }
                int occurrences = 0;
                foreach (var pair in counts)
                {
                    if (pair.Value > occurrences)
                    {
                        occurrences = pair.Value;
                        mostSoldvehicle = pair.Key;
                    }
                }

                return mostSoldvehicle;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
