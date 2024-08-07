using OfficeOpenXml;
using System.Linq;
using System.Threading.Tasks;
using Medical.Data;
using Microsoft.EntityFrameworkCore;

namespace Medical.Service.Excel
{
    public class ExcelExportService
    {
        private readonly AppDbContext _context;

        public ExcelExportService(AppDbContext context)
        {
            _context = context;

           
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");


                worksheet.Cells[1, 1].Value = "Full Name";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Total Price";
                worksheet.Cells[1, 4].Value = "Status";

                for (int i = 0; i < orders.Count; i++)
                {
                    var order = orders[i];
                    var totalPrice = order.OrderItems.Sum(oi => oi.SalePrice * oi.Count);

                    worksheet.Cells[i + 2, 1].Value = order.FullName;
                    worksheet.Cells[i + 2, 2].Value = order.Email;
                    worksheet.Cells[i + 2, 3].Value = totalPrice;
                    worksheet.Cells[i + 2, 4].Value = order.Status.ToString();
                }

                return package.GetAsByteArray();
            }
        }
        public async Task<byte[]> ExportAllTablesAsync()
        {
            using (var package = new ExcelPackage())
            {
                await AddWorksheetForTableAsync(package, "Sliders", await _context.Sliders.ToListAsync());
                await AddWorksheetForTableAsync(package, "Features", await _context.Features.ToListAsync());
                await AddWorksheetForTableAsync(package, "Appointments", await _context.Appointments.ToListAsync());
                await AddWorksheetForTableAsync(package, "AppUsers", await _context.AppUsers.ToListAsync());
                await AddWorksheetForTableAsync(package, "Categories", await _context.Categories.ToListAsync());
                await AddWorksheetForTableAsync(package, "Feeds", await _context.Feeds.ToListAsync());
                await AddWorksheetForTableAsync(package, "Doctors", await _context.Doctors.ToListAsync());
                await AddWorksheetForTableAsync(package, "MedicineImages", await _context.MedicineImages.ToListAsync());
                await AddWorksheetForTableAsync(package, "MedicineReviews", await _context.MedicineReviews.ToListAsync());
                await AddWorksheetForTableAsync(package, "Medicines", await _context.Medicines.ToListAsync());
                await AddWorksheetForTableAsync(package, "BasketItems", await _context.BasketItems.ToListAsync());
                await AddWorksheetForTableAsync(package, "Departments", await _context.Departments.ToListAsync());
                await AddWorksheetForTableAsync(package, "Settings", await _context.Settings.ToListAsync());
                await AddWorksheetForTableAsync(package, "OrderItems", await _context.OrderItems.ToListAsync());
                await AddWorksheetForTableAsync(package, "Orders", await _context.Orders.ToListAsync());
                await AddWorksheetForTableAsync(package, "Services", await _context.Services.ToListAsync());

                return package.GetAsByteArray();
            }
        }

        private async Task AddWorksheetForTableAsync<T>(ExcelPackage package, string sheetName, List<T> data) where T : class
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            worksheet.Cells["A1"].LoadFromCollection(data, true);
        }
    }

}
