using System;
using Medical.UI.Models.AdminModels;

namespace Medical.UI.Service
{
	public interface ILayoutService
	{
        Task<UserProfileResponse> GetProfile();
    }
}

