using AutoMapper;

using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public AuthorsController(
            ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository
                ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [HttpHead]
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
            [FromQuery] AuthorsResourseParameters authorsResourseParameters)
        {
            var authors = this.courseLibraryRepository.GetAuthors(authorsResourseParameters);

            return Ok(this.mapper.Map<IEnumerable<AuthorDto>>(authors));
        }

        [HttpGet("{authorId}", Name = nameof(GetAuthors))]
        public ActionResult<AuthorDto> GetAuthors(Guid authorId)
        {
            var author = this.courseLibraryRepository.GetAuthor(authorId);

            if (author == null)
            {
                return NotFound();
            }

            return Ok(this.mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorForCreateDto author) 
        {
            var authorEntity = this.mapper.Map<Entities.Author>(author);
            this.courseLibraryRepository.AddAuthor(authorEntity);
            this.courseLibraryRepository.Save();

            var authorToReturn = this.mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute(
                nameof(GetAuthors),
                new { authorId = authorToReturn.Id },
                authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorsOptions() 
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");

            return Ok();
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorEntity = this.courseLibraryRepository.GetAuthor(authorId);

            if (authorEntity == null) 
            {
                return NotFound();
            }

            this.courseLibraryRepository.DeleteAuthor(authorEntity);
            this.courseLibraryRepository.Save();

            return NoContent();
        }
    }
}
