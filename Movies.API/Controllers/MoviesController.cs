﻿using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using System.Reflection;
using Movies.Application.Models;
using Movies.API.Mapping;
using Movies.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Movies.API.Auth;
using Movies.Contracts.Responses;
using Microsoft.AspNetCore.Routing;
using static Movies.API.ApiEndpoints;
using Movies.Contracts.Requests;
using Asp.Versioning;

namespace Movies.API.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
        {
            var movie = request.MapToMovie();
            await _movieService.CreateAsync(movie, token);

            var response = movie.MapToResponse();
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, response);
            //return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", response);
        }

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, [FromServices] LinkGenerator linkGenerator, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();

            var movie = Guid.TryParse(idOrSlug, out var id) ? await _movieService.GetByIdAsync(id, userId, token) : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
            if (movie is null)
            {
                return NotFound();
            }

            var response = movie.MapToResponse();

            var movieObj = new { id = movie.Id };
            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id }),
                Rel = "self",
                Type = "GET"
            });

            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: movieObj),
                Rel = "self",
                Type = "PUT"
            });

            response.Links.Add(new Link
            {
                Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: movieObj),
                Rel = "self",
                Type = "DELETE"
            });

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movies = await _movieService.GetAllAsync(userId, token);

            var movieResponse = movies.MapToResponse();
            return Ok(movieResponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            var movie = request.MapToMovie(id);
            var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
            if (updatedMovie is null)
            {
                return NotFound();
            }

            var response = movie.MapToResponse();

            return Ok(response);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, token);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }

    }
}
