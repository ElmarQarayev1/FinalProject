using System;
using Medical.UI.Models;

namespace Medical.UI.Service
{
	public interface ICrudService
	{
        Task<PaginatedResponse<TResponse>> GetAllPaginated<TResponse>(string path, int page, int size = 10);
        Task<TResponse> Get<TResponse>(string path);
        Task<CreateResponse> Create<TRequest>(TRequest request, string path);
        Task<CreateResponseForAdmins> CreateForAdmins<TRequest>(TRequest request, string path);
        Task Update<TRequest>(TRequest request, string path);
        Task Delete(string path);
        Task<CreateResponse> CreateFromForm<TRequest>(TRequest request, string path);
        Task EditFromForm<TRequest>(TRequest request, string path);
        Task<PaginatedResponse<TResponse>> GetAllPaginatedForAppointment<TResponse>(string path, int page, int size = 10, int? doctorId = null);
        Task<byte[]> ExportOrdersAsync();
        Task<byte[]> ExportAllTablesAsync();
        Task Status(string path);
        Task<int> GetTodayOrdersCountAsync();
        Task<double> GetTodayOrdersTotalPriceAsync();
        Task<int> GetMonthlyOrdersCountAsync();
        Task<double> GetMonthlyOrdersTotalPriceAsync();
        Task<int> GetDailyAppointmentsCountAsync();
        Task<int> GetYearlyAppointmentsCountAsync();
        Task<YearlyAppointmentsResponse> GetMonthlyAppointmentsCountAsync();

    }
}

