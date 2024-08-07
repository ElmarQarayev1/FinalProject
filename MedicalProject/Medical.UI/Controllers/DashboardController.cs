using System;
using Medical.UI.Filter;
using Medical.UI.Service;
using Microsoft.AspNetCore.Mvc;

namespace Medical.UI.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class DashboardController
	{
        private HttpClient _client;
        private readonly ICrudService _crudService;

        public DashboardController(ICrudService crudService)
        {
            _crudService = crudService;
            _client = new HttpClient();
        }

    }
}

