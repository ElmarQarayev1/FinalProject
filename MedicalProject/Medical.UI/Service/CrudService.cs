using System;
using Medical.UI.Exception;
using Medical.UI.Models;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Medical.UI.Service
{
    public class CrudService : ICrudService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _client;
        private const string baseUrl = "https://localhost:7061/api/admin/";
        public CrudService(IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<YearlyAppointmentsResponse> GetMonthlyAppointmentsCountAsync()
        {
            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "monthly-count"))
            {
                var json = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("API Response JSON: " + json); 

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonSerializer.Deserialize<YearlyAppointmentsResponse>(json);
                    return data;
                }
                else
                {
                    //Console.WriteLine("Error: " + json); 
                    throw new HttpException(response.StatusCode, json);
                }
            }
        }
        public async Task<OrderStatusCountsResponse> GetOrderStatusCountsAsync()
        {
            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "order-status-counts"))
            {
                var json = await response.Content.ReadAsStringAsync();
                //Console.WriteLine("API Response JSON: " + json); 

                if (response.IsSuccessStatusCode)
                {
                    
                    var data = JsonSerializer.Deserialize<OrderStatusCountsResponse>(json);
                    if (data == null)
                    {
                        throw new HttpException(response.StatusCode);
                    }
                    return data;
                }
                else
                {
                    throw new HttpException(response.StatusCode, json);
                }
            }
        }


        public async Task<int> GetDailyAppointmentsCountAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "daily-count"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var count = await response.Content.ReadAsStringAsync();
                    return int.Parse(count);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<int> GetYearlyAppointmentsCountAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "yearly-count"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var count = await response.Content.ReadAsStringAsync();
                    return int.Parse(count);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<int> GetTodayOrdersCountAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "today/orders/count"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var count = await response.Content.ReadAsStringAsync();
                    return int.Parse(count);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<double> GetTodayOrdersTotalPriceAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "today/orders/total-price"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var totalPrice = await response.Content.ReadAsStringAsync();
                    return double.Parse(totalPrice);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<int> GetMonthlyOrdersCountAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "monthly/orders/count"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var count = await response.Content.ReadAsStringAsync();
                    return int.Parse(count);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<double> GetMonthlyOrdersTotalPriceAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "monthly/orders/total-price"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var totalPrice = await response.Content.ReadAsStringAsync();
                    return double.Parse(totalPrice);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<byte[]> ExportAllTablesAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "excel/DownloadAllTables"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<byte[]> ExportOrdersAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.GetAsync(baseUrl + "excel/DownloadExcel"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<CreateResponse> Create<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }







        public async Task<CreateResponse> CreateFromForm<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            MultipartFormDataContent content = new MultipartFormDataContent();
            foreach (var prop in request.GetType().GetProperties())
            {
                var val = prop.GetValue(request);

                if (val is IFormFile file)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, prop.Name, file.FileName);
                }
                else if (val is List<IFormFile> listF)
                {
                    foreach (var item in listF)
                    {

                        var fileContent = new StreamContent(item.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(item.ContentType);
                        content.Add(fileContent, prop.Name, item.FileName);

                    }
                }
                else if (val is List<int> list)
                {
                    // content.Add(new StringContent(JsonSerializer.Serialize(list)), prop.Name);

                    foreach (var item in list)
                    {
                        content.Add(new StringContent(item.ToString()), prop.Name);
                    }
                }
                else if (val is not null)
                {
                    content.Add(new StringContent(val.ToString()), prop.Name);
                }
            }

            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }






        public async Task EditFromForm<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            MultipartFormDataContent content = new MultipartFormDataContent();
            foreach (var prop in request.GetType().GetProperties())
            {
                var val = prop.GetValue(request);

                if (val is IFormFile file)
                {
                    var fileContent = new StreamContent(file.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, prop.Name, file.FileName);
                }
                else if (val is List<IFormFile> listF)
                {
                    foreach (var item in listF)
                    {

                        var fileContent = new StreamContent(item.OpenReadStream());
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(item.ContentType);
                        content.Add(fileContent, prop.Name, item.FileName);

                    }
                }
                else if (val is List<int> list)
                {
                    foreach (var item in list)
                    {
                        content.Add(new StringContent(item.ToString()), $"{prop.Name}[]");
                    }
                }
                else if (val is not null)
                {
                    content.Add(new StringContent(val.ToString()), prop.Name);
                }
            }
            using (HttpResponseMessage response = await _client.PutAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    // return JsonSerializer.Deserialize<CreateResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }






        public async Task Delete(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.DeleteAsync(baseUrl + path))
            {
                if (!response.IsSuccessStatusCode)
                    throw new HttpException(response.StatusCode);
            }
        }







        public async Task<TResponse> Get<TResponse>(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else { throw new HttpException(response.StatusCode); }
            }
        }


        public async Task<PaginatedResponse<TResponse>> GetAllPaginated<TResponse>(string path, int page, int size = 10)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync($"{baseUrl}{path}?page={page}&size={size}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<PaginatedResponse<TResponse>>(await response.Content.ReadAsStringAsync(), options);
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new HttpException(response.StatusCode, errorResponse?.Message);
                }
            }
        }
        public async Task<PaginatedResponse<TResponse>> GetAllPaginatedForAppointment<TResponse>(string path, int page, int size = 10, int? doctorId = null)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var uri = $"{baseUrl}{path}?page={page}&size={size}";
            if (doctorId.HasValue)
            {
                uri += $"&doctorId={doctorId.Value}";
            }

            using (var response = await _client.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<PaginatedResponse<TResponse>>(await response.Content.ReadAsStringAsync(), options);
                }
                else
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new HttpException(response.StatusCode, errorResponse?.Message);
                }
            }
        }


        public async Task Update<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

           var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PutAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                    }
                    else
                    {
                        throw new HttpException(response.StatusCode, errorResponse.Message);
                    }
                }
            }
        }

        async Task<CreateResponseForAdmins> ICrudService.CreateForAdmins<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponseForAdmins>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }


        public async Task Status(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.PutAsync(baseUrl + path, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new HttpException(response.StatusCode, errorResponse?.Message);
                }
            }
        }
    }
}

