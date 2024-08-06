using System;
using Medical.Service.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiController]
    public class ExcelController : Controller
    {
        private readonly ExcelExportService _excelExportService;
        public ExcelController(ExcelExportService excelExportService)
        {
            _excelExportService = excelExportService;
        }

        [HttpGet("api/admin/excel/DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            var fileContent = await _excelExportService.ExportOrdersAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
        }
    }
}

