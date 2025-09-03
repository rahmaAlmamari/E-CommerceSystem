using AutoMapper;
using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceSystem.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepo _reviewRepo;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderProductsService _orderProductsService;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepo reviewRepo,
            IProductService productService,
            IOrderProductsService orderProductsService,
            IOrderService orderService,
            IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _productService = productService;
            _orderProductsService = orderProductsService;
            _orderService = orderService;
            _mapper = mapper;
        }

        public IEnumerable<Review> GetAllReviews(int pageNumber, int pageSize, int pid)
        {
            var query = _reviewRepo.GetReviewByProductId(pid);

            var pagedProducts = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return pagedProducts;
        }

        public Review GetReviewsByProductIdAndUserId(int pid, int uid)
        {
            return _reviewRepo.GetReviewsByProductIdAndUserId(pid, uid);
        }

        public Review GetReviewById(int rid)
        {
            var review = _reviewRepo.GetReviewById(rid);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {rid} not found.");
            return review;
        }

        public IEnumerable<Review> GetReviewByProductId(int pid)
        {
            return _reviewRepo.GetReviewByProductId(pid);
        }

        public void AddReview(int uid, int pid, ReviewDTO reviewDTO)
        {
            // Get all orders for the user ...
            var orders = _orderService.GetOrderByUserId(uid);

            foreach (var order in orders)
            {
                // Check if the product exists in any of the user's orders ...
                var products = _orderProductsService.GetOrdersByOrderId(order.OID);

                foreach (var product in products)
                {
                    if (product != null && product.PID == pid)
                    {
                        // Check if the user has already added a review for this product ...
                        var existingReview = GetReviewsByProductIdAndUserId(pid, uid);
                        if (existingReview != null)
                            throw new InvalidOperationException("You have already reviewed this product.");

                        // Map ReviewDTO -> Review (AutoMapper) ...
                        var review = _mapper.Map<Review>(reviewDTO);
                        review.PID = pid;
                        review.UID = uid;
                        review.ReviewDate = DateTime.Now;

                        _reviewRepo.AddReview(review);

                        // Recalculate and update the product's overall rating ...
                        RecalculateProductRating(pid);
                    }
                }
            }
        }

        public void UpdateReview(int rid, ReviewDTO reviewDTO)
        {
            var review = GetReviewById(rid);

            // Map incoming ReviewDTO onto the existing Review (AutoMapper) ...
            _mapper.Map(reviewDTO, review);
            review.ReviewDate = DateTime.Now;
            // Wrapped update operation in try/catch to handle concurrency conflicts
            try
            {
                _reviewRepo.UpdateReview(review);
            }
            catch (DbUpdateConcurrencyException)
            {
                // Throw custom exception if another user modified the same review
                throw new InvalidOperationException("The review was modified by another process. Please reload and try again.");
            }
           
            //_reviewRepo.UpdateReview(review);

            // Fix: pass PID, not rating ...
            RecalculateProductRating(review.PID);
        }

        public void DeleteReview(int rid)
        {
            var review = _reviewRepo.GetReviewById(rid);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {rid} not found.");

            _reviewRepo.DeleteReview(rid);
            RecalculateProductRating(review.PID);
        }

        private void RecalculateProductRating(int pid)
        {
            // Get all reviews for this product (fix: filter by pid) ...
            var reviews = _reviewRepo.GetReviewByProductId(pid).ToList();

            var product = _productService.GetProductById(pid);

            // Calculate the average rating (handle no reviews => 0) ...
            var averageRating = reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating);

            // Update the product's overall rating (fix: convert to decimal) ...
            product.OverallRating = Convert.ToDecimal(averageRating);

            // Save the updated product ...
            _productService.UpdateProduct(product);
        }
    }
}
