using System;
using Medical.Service.Dtos.Admin.MedicineDtos;

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
    }
}

