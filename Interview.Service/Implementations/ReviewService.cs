using Interview.Domain.Common.IUnitOfWork;
using Interview.Domain.Entities;
using Interview.Domain.Global;
using Interview.Service.Interfaces;

namespace Interview.Service.Implementations
{
    public class ReviewService : BaseService, IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private ILog _logger;
        public ReviewService(IUnitOfWork unitOfWork, ILog logger) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }


        public async Task<Result> AddAsync(Review review)
        {
            if (review is null) return Result.Failure("review cannot be null");

            if (string.IsNullOrWhiteSpace(review.Comment))
                return Result.Failure("Comment is required");

            if (review.Rating > 5) review.Rating = 5;

            if (review.Rating < 1) review.Rating = 1;
            
            if (review.Comment.Length > 500)
                return Result.Failure("Comment cannot exceed 500 characters");

            // validate user id


            try
            {
                await _unitOfWork.Repository<Review>().AddAsync(review);
                return await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to add review asynchronously: {ex.Message}");
            }
        }
        public async Task<Result> UpdateAsync(Review review)
        {
            
            if (review is null) return Result.Failure("Review cannot be null");
            if (!IsReviewExists(review.Id).IsSuccess) return Result.Failure("Review not found");
            if (review.Comment.Length > 500) return Result.Failure("Comment cannot exceed 500 characters");
          
            if (review.Rating > 5) review.Rating = 5;

            if (review.Rating < 1) review.Rating = 1;

            try
            {
                _unitOfWork.Repository<Review>().Update(review);
                return await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to update review asynchronously: {ex.Message}");
            }
        }
        private Result IsReviewExists(int reviewId)
        {
            var isExists = _unitOfWork.Repository<Review>().Table.Any(x => x.Id == reviewId);
            return isExists ? Result.Success() : Result.Failure("Review not found");
        }
        public async Task<Result> DeleteAsync(int id)
        {
            if (!IsReviewExists(id).IsSuccess) return Result.Failure("Review not found");

            var entity = await _unitOfWork.Repository<Review>().GetAsync(d => d.Id == id);

            try
            {
                _unitOfWork.Repository<Review>().Delete(entity);
                return await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex, System.Diagnostics.EventLogEntryType.Error);
                return Result.Failure($"Failed to delete review asynchronously: {ex.Message}");
            }
        }
    }
}
