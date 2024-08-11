using System;
using Medical.Service.Dtos.Admin.MedicineDtos;
using Medical.Service.Dtos.User.MedicineDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IMedicineService
	{
        int Create(MedicineCreateDto createDto);
        PaginatedList<MedicinePaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<MedicineGetDto> GetAll(string? search = null);
        MedicineDetailsDto GetById(int id);
        void Update(int id, MedicineUpdateDto updateDto);
        void Delete(int id);

        int BasketItem(MedicineBasketItemDto createDto,string userId);

        void RemoveItemFromBasket(MedicineBasketDeleteDto removeDto,string userId);

        void UpdateItemCountInBasket(MedicineBasketItemDto updateDto,string userId);


        PaginatedList<MedicinePaginatedGetDtoForUser> GetAllByPageForUser(string? search = null, int page = 1, int size = 9, int? categoryId = null);

        List<MedicineGetDtoLatest> GetAllLatest(string? search = null);

        MedicineGetDtoForUser GetByIdForUser(int id);



    }
}

