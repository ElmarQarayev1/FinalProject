using System;
using AutoMapper;
using Medical.Core.Entities;
using Medical.Core.Enum;
using Medical.Data;
using Medical.Data.Repositories.Implementations;
using Medical.Data.Repositories.Interfaces;
using Medical.Service.Dtos.User.DoctorDtos;
using Medical.Service.Dtos.User.MedicineDtos;
using Medical.Service.Dtos.User.OrderDtos;
using Medical.Service.Dtos.User.ReviewDtos;
using Medical.Service.Exceptions;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medical.Service.Implementations.Admin
{
    public class ReviewService : IReviewService
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly Data.Repositories.Interfaces.IBasketRepository _basketRepository;

        private readonly IReviewRepository _reviewRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        private readonly AppDbContext _context;


        public ReviewService(IMedicineRepository medicineRepository, IHttpContextAccessor httpContextAccessor, Data.Repositories.Interfaces.IBasketRepository basketRepository, IMapper mapper, IWebHostEnvironment env, IReviewRepository reviewRepository, AppDbContext appDbContext)
        {
            _medicineRepository = medicineRepository;
            _mapper = mapper;
            _env = env;
            _basketRepository = basketRepository;
            _httpContextAccessor = httpContextAccessor;
            _reviewRepository = reviewRepository;
            _context = appDbContext;
        }
        public int GetPendingReviewCount()
        {
            return _reviewRepository.GetAll(x => x.Status == ReviewStatus.Pending).Count();
        }

        public async Task<int> CreateReviewAsync(MedicineReviewItemDto reviewDto, string userId)
        {
            var medicine =  _medicineRepository.Get(x => x.Id == reviewDto.MedicineId);

            if (medicine == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Medicine not found");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "User not found");
            }

            var review = new MedicineReview
            {
                AppUserId = userId,
                MedicineId = reviewDto.MedicineId,
                CreatedAt = DateTime.Now,
                Status = ReviewStatus.Pending,
                Text = reviewDto.Text,
                Rate = reviewDto.Rate
            };

            _reviewRepository.Add(review);
             _reviewRepository.Save();

            return review.Id;
        }

        public void DeleteReview(MedicineReviewDeleteDto deleteDto,string userId)
        {

            var medicine = _medicineRepository.Get(x => x.Id == deleteDto.MedicineId);
            if (medicine == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "MedicineId", "Medicine not found");
            }


            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "AppUserId", "User not found or not authenticated");
            }

            var review = _reviewRepository.Get(x =>
                x.Id == deleteDto.MedicineReviewId &&
                x.AppUserId == userId &&
                x.MedicineId == deleteDto.MedicineId);

            if (review == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "MedicineReviewId", "Review not found");
            }

            _reviewRepository.Delete(review);
            _reviewRepository.Save();
        }

        public void UpdateReviewStatus(int id, ReviewStatus newStatus)
        {
            var review = _reviewRepository.Get(o => o.Id == id);

            if (review == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Review not found");
            }


            if (review.Status == newStatus)
            {
                throw new RestException(StatusCodes.Status400BadRequest, $"Review is already {newStatus}");
            }


            review.Status = newStatus;

            _context.MedicineReviews.Update(review);
            _context.SaveChanges();
        }

        public PaginatedList<ReviewPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _reviewRepository.GetAll(o =>
                (search == null || o.Medicine.Name.Contains(search) || o.Text.Contains(search))
                ).Include(r => r.Medicine)
        .Include(r => r.AppUser)
                .Select(review => new ReviewPaginatedGetDto
                {
                    Id = review.Id,
                    FullName = review.AppUser.FullName,        
                    CreatedAt = review.CreatedAt,
                    Text = review.Text,
                    Status = review.Status.ToString()
                });

            return PaginatedList<ReviewPaginatedGetDto>.Create(query, page, size);
        }


        public ReviewDetailDto GetById(int id)
        {

            var review = _reviewRepository.Get(x => x.Id == id, "Medicine", "AppUser");
              
            if (review == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "Review not found");
            }

            return new ReviewDetailDto
            {
                Id = review.Id,
                FullName = review.AppUser.FullName,
                CreatedAt = review.CreatedAt,
                MedicineName = review.Medicine.Name,
                Text = review.Text,
                Status = review.Status.ToString(),
                Rate = review.Rate
            };
        }

      


    }
}

