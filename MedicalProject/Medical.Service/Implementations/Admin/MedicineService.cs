using System;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Core.Enum;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.Admin.MedicineDtos;
using Medical.Service.Dtos.Admin.ServiceDtos;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Exceptions;
using Medical.Service.Helpers;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace Medical.Service.Implementations.Admin
{
	public class MedicineService:IMedicineService
	{
        private readonly IMedicineRepository _medicineRepository;
        private readonly Data.Repositories.Interfaces.IBasketRepository _basketRepository;

        private readonly IReviewRepository _reviewRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ICategoryRepository _categoryRepository;

        public MedicineService(IMedicineRepository medicineRepository, IHttpContextAccessor httpContextAccessor, Data.Repositories.Interfaces.IBasketRepository basketRepository, IMapper mapper, ICategoryRepository categoryRepository, IWebHostEnvironment env,IReviewRepository reviewRepository)
        {
            _medicineRepository = medicineRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _env = env;
            _basketRepository = basketRepository;
            _httpContextAccessor = httpContextAccessor;
            _reviewRepository = reviewRepository;
        }


      



        public int Create(MedicineCreateDto createDto)
        {

            Category category = _categoryRepository.Get(x => x.Id == createDto.CategoryId);

            if (category == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Category not found");
            }

            if (_medicineRepository.Exists(x => x.Name == createDto.Name))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "MedicineName already exists");
            }

            Medicine medicine = new Medicine
            {
                Name = createDto.Name,
                Desc = createDto.Desc,
                Price = createDto.Price,
                CategoryId = createDto.CategoryId,
                MedicineImages = new List<MedicineImage>()
            };

            foreach (var file in createDto.Files)
            {
                var filePath = FileManager.Save(file, _env.WebRootPath, "uploads/medicines");
                medicine.MedicineImages.Add(new MedicineImage { ImageName = filePath });
            }

            _medicineRepository.Add(medicine);
            _medicineRepository.Save();

            return medicine.Id;
        }





        public void Delete(int id)
        {
           
            Medicine entity = _medicineRepository.Get(x => x.Id == id, "MedicineImages");

            if (entity == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Medicine not found");
            }

           
            foreach (var image in entity.MedicineImages)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/medicines", image.ImageName);
            }

            
            _medicineRepository.Delete(entity);

           
            _medicineRepository.Save();
        }





        public List<MedicineGetDto> GetAll(string? search = null)
        {
            var medicines = _medicineRepository.GetAll(x => search == null || x.Name.Contains(search)).ToList();
            return _mapper.Map<List<MedicineGetDto>>(medicines);
        }


        public List<MedicineGetDtoLatest> GetAllLatest(string? search = null)
        {
           
            var medicines = _medicineRepository
                .GetAll(x => search == null || x.Name.Contains(search),"MedicineReviews","MedicineImages")
                .OrderByDescending(x => x.CreateAt)
                .Take(4) 
                .ToList();

            return _mapper.Map<List<MedicineGetDtoLatest>>(medicines);
        }




        public PaginatedList<MedicinePaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {

            var query = _medicineRepository.GetAll(x => x.Name.Contains(search) || search == null,"Category");


            var paginated = PaginatedList<Medicine>.Create(query, page, size);

            var medicineDtos = _mapper.Map<List<MedicinePaginatedGetDto>>(paginated.Items);

            return new PaginatedList<MedicinePaginatedGetDto>(medicineDtos, paginated.TotalPages, page, size);
        }

        public PaginatedList<MedicinePaginatedGetDtoForUser> GetAllByPageForUser(string? search = null, int page = 1, int size = 9, int? categoryId = null)
        {

            if (categoryId != null)
            {
                Category category = _categoryRepository.Get(x => x.Id == categoryId);

                if (category == null)
                {
                    throw new RestException(StatusCodes.Status404NotFound, "CategoryId", "Category not found");
                }
            }

            var query = _medicineRepository.GetAll(
                x =>
                    (string.IsNullOrEmpty(search) || x.Name.Contains(search)) &&
                    (categoryId == null || x.CategoryId == categoryId),
                "MedicineImages",
                "MedicineReviews"
            );

           
            var paginated = PaginatedList<Medicine>.Create(query, page, size);

          
            var medicineDtos = _mapper.Map<List<MedicinePaginatedGetDtoForUser>>(paginated.Items);

          
            return new PaginatedList<MedicinePaginatedGetDtoForUser>(medicineDtos, paginated.TotalPages, page, size);
        }





        public MedicineDetailsDto GetById(int id)
        {
            Medicine medicine = _medicineRepository.Get(x => x.Id == id, "MedicineImages");

            if (medicine == null)
                throw new RestException(StatusCodes.Status404NotFound, "Medicine not found");

            return _mapper.Map<MedicineDetailsDto>(medicine);
        }

        public MedicineGetDtoForUser GetByIdForUser(int id)
        {
            Medicine medicine = _medicineRepository.Get(x => x.Id == id, "MedicineImages","MedicineReviews", "MedicineReviews.AppUser");

            if (medicine == null)
                throw new RestException(StatusCodes.Status404NotFound, "Medicine not found");

            return _mapper.Map<MedicineGetDtoForUser>(medicine);
        }



        public void Update(int id, MedicineUpdateDto updateDto)
        {
            Category category = _categoryRepository.Get(x => x.Id == updateDto.CategoryId);

            if (category == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Category not found");
            }

            Medicine medicine = _medicineRepository.Get(x => x.Id == id, "MedicineImages");

            if (medicine == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Id", "Medicine not found by given Id");
            }

            if (!string.Equals(medicine.Name, updateDto.Name, StringComparison.OrdinalIgnoreCase) &&
                _medicineRepository.Exists(x => x.Name.ToUpper() == updateDto.Name.ToUpper() && x.Id != id))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Name", "MedicineName already exists");
            }

            medicine.Name = updateDto.Name;
            medicine.Price = updateDto.Price;
            medicine.Desc = updateDto.Desc;
            medicine.CategoryId = updateDto.CategoryId;


            List<MedicineImage> pictures = medicine.MedicineImages.Where(x => updateDto.ExistPictureIds.Contains(x.Id)).ToList();
            List<MedicineImage> removedPictures = medicine.MedicineImages.Where(x => !updateDto.ExistPictureIds.Contains(x.Id)).ToList();

            medicine.MedicineImages = pictures;

            foreach (var imgFile in updateDto.Files)
            {
                MedicineImage Img = new MedicineImage
                {
                    ImageName = FileManager.Save(imgFile, _env.WebRootPath, "uploads/medicines"),
                };
                medicine.MedicineImages.Add(Img);
            }

            medicine.ModifiedAt = DateTime.Now;

            _medicineRepository.Save();

            foreach (var item in removedPictures)
            {
                FileManager.Delete(_env.WebRootPath, "uploads/medicines", item.ImageName);
            }

        }
        public int BasketItem(MedicineBasketItemDto createDto, string userId)
        {

            var medicine = _medicineRepository.Get(x => x.Id == createDto.MedicineId);

            if (medicine == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Medicine not found");
            }

            if (!string.IsNullOrEmpty(userId))
            {

                return AddBasketItemToDatabase(createDto, userId);
            }
            else
            {
                throw new RestException(StatusCodes.Status400BadRequest, "User not found");
            }

        }


        private int AddBasketItemToDatabase(MedicineBasketItemDto createDto, string userId)
        {

            var existingBasketItem = _basketRepository.Get(x =>
                x.AppUserId == userId && x.MedicineId == createDto.MedicineId);

            if (existingBasketItem != null)
            {

                existingBasketItem.Count += createDto.Count;
                _basketRepository.Save();
                return existingBasketItem.Id;
            }


            var basketItem = new BasketItem
            {
                AppUserId = userId,
                MedicineId = createDto.MedicineId,
                Count = createDto.Count,
            };

            _basketRepository.Add(basketItem);
            _basketRepository.Save();

            return basketItem.Id;
        }


        public void RemoveItemFromBasket(MedicineBasketDeleteDto removeDto, string userId)
        {

            var medicine = _medicineRepository.Get(x => x.Id == removeDto.MedicineId);

            if (medicine == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Medicine not found");
            }

            if (!string.IsNullOrEmpty(userId))
            {

                RemoveBasketItemFromDatabase(removeDto, userId);
            }
            else
            {
                throw new RestException(StatusCodes.Status400BadRequest, "User not found");
            }

        }


        private void RemoveBasketItemFromDatabase(MedicineBasketDeleteDto removeDto, string userId)
        {

            var basketItem = _basketRepository.Get(x =>
                x.AppUserId == userId && x.MedicineId == removeDto.MedicineId);

            if (basketItem != null)
            {

                _basketRepository.Delete(basketItem);
                _basketRepository.Save();
            }
        }




        public void UpdateItemCountInBasket(MedicineBasketItemDto updateDto, string userId)
        {
            var medicine = _medicineRepository.Get(x => x.Id == updateDto.MedicineId);

            if (medicine == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Medicine not found");
            }
            if (!string.IsNullOrEmpty(userId))
            {

                UpdateBasketItemCountInDatabase(updateDto, userId);
            }
            else
            {
                throw new RestException(StatusCodes.Status400BadRequest, "User not found");
            }


        }


        private void UpdateBasketItemCountInDatabase(MedicineBasketItemDto updateDto, string userId)
        {

            var basketItem = _basketRepository.Get(x =>
                x.AppUserId == userId && x.MedicineId == updateDto.MedicineId);

            if (basketItem != null)
            {

                basketItem.Count = updateDto.Count;
                _basketRepository.Save();
            }
        }




    }
}

