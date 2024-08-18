using System;
using Medical.Service.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "admin_v1")]
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
        [HttpGet("api/admin/excel/DownloadAllTables")]
        public async Task<IActionResult> DownloadAllTables()
        {
            var fileContent = await _excelExportService.ExportAllTablesAsync();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AllTables.xlsx");
        }
    }
}

