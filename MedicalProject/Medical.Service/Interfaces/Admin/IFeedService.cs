using System;
using Medical.Service.Dtos.Admin.FeatureDtos;
using Medical.Service.Dtos.Admin.FeedDtos;
using Medical.Service.Dtos.User.FeatureDtos;
using Medical.Service.Dtos.User.FeedDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IFeedService
	{
        int Create(FeedCreateDto createDto);
        PaginatedList<FeedPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<FeedGetDto> GetAll(string? search = null);
        FeedGetDto GetById(int id);
        void Update(int id, FeedUpdateDto updateDto);
        List<FeedGetDtoForUser> GetAllUser(string? search = null);
        void Delete(int id);
    }
}

