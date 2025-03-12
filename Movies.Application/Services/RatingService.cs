using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Models;
using Movies.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMovieRepository _movieRepository;

        public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
        {
            _ratingRepository = ratingRepository;
            _movieRepository = movieRepository;
        }
        public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token)
        {
            if (rating <= 0 || rating > 5)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure
                    {
                        PropertyName = "Rating",
                        ErrorMessage = "Rating must be between 1 and 5"
                    }
                });
            }

            var movieExists = await _movieRepository.ExistsByIdAsync(movieId, token);
            if (!movieExists)
            {
                return false;
            }
            return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);

        }

        public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token)
        {
            return await _ratingRepository.DeleteRatingAsync(movieId, userId, token);
        }

        public async Task<IEnumerable<MovieRating>> GetRatingsForUsersAsync(Guid userId, CancellationToken token)
        {
            return await _ratingRepository.GetRatingsForUsersAsync(userId, token);
        }
    }
}
