using System;
using Medical.Core.Enum;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Dtos.User.OrderDtos;
using Medical.Service.Dtos.User.ReviewDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IReviewService
	{
        int CreateReview(MedicineReviewItemDto reviewDto);

        void DeleteReview(MedicineReviewDeleteDto deleteDto);

        void UpdateOrderStatus(int id, ReviewStatus newStatus);

        PaginatedList<ReviewPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        ReviewDetailDto GetById(int id);
        int GetPendingReviewCount();
    }
}

