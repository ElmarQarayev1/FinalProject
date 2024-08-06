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
    }
}
