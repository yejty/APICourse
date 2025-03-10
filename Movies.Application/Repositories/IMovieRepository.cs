﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public interface IMovieRepository
    {
        Task<bool> CreateAsync(Movie movie);

        Task<Movie?> GetByIdAsync(Guid id);

        Task<Movie?> GetBySlugAsync(string slug);

        Task<IEnumerable<Movie>> GetAllAsync();

        Task<bool> UpdateAsync(Movie movie);

        Task<bool> DeleteByIdAsync(Guid id);
    }
}
