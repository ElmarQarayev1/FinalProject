using System;
using Medical.Core.Enum;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Dtos.User.OrderDtos;
using Medical.Service.Dtos.User.ReviewDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IReviewService
	{
        Task<int> CreateReviewAsync(MedicineReviewItemDto reviewDto, string userId);

        void DeleteReview(MedicineReviewDeleteDto deleteDto,string userId);

        void UpdateReviewStatus(int id, ReviewStatus newStatus);

        PaginatedList<ReviewPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        ReviewDetailDto GetById(int id);
        int GetPendingReviewCount();
    }
}

