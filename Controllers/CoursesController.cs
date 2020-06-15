using AutoMapper;

using CourseLibrary.API.Models;
using CourseLibrary.API.Services;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseLibraryRepository courseLibraryRepository;
        private readonly IMapper mapper;

        public CoursesController(
            ICourseLibraryRepository courseLibraryRepository,
            IMapper mapper)
        {
            this.courseLibraryRepository = courseLibraryRepository
                ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            if (!this.courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorCourses = this.courseLibraryRepository.GetCourses(authorId);
            return Ok(this.mapper.Map<IEnumerable<CourseDto>>(authorCourses));
        }

        [HttpGet("{courseId}", Name = nameof(GetCourseForAuthor))]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!this.courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var course = this.courseLibraryRepository.GetCourse(authorId, courseId);
            if (course == null)
            {
                return NotFound();
            }

            return this.mapper.Map<CourseDto>(course);
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, CourseForCreateDto course)
        {
            if (!this.courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = this.mapper.Map<Entities.Course>(course);
            this.courseLibraryRepository.AddCourse(authorId, courseEntity);
            this.courseLibraryRepository.Save();

            var courseToReturn = this.mapper.Map<CourseDto>(courseEntity);
            return CreatedAtRoute(
                nameof(GetCourseForAuthor),
                new { authorId, courseId = courseToReturn.Id },
                courseToReturn);
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(
            Guid authorId,
            Guid courseId,
            CourseForUpdateDto course)
        {
            if (!this.courseLibraryRepository.AuthorExists(authorId)) 
            {
                return NotFound();
            }

            var courseEntity = this.courseLibraryRepository.GetCourse(authorId, courseId);
            if(courseEntity == null)
            {
                var courseToAdd = this.mapper.Map<Entities.Course>(course);
                courseToAdd.Id = courseId;

                this.courseLibraryRepository.AddCourse(authorId, courseToAdd);
                this.courseLibraryRepository.Save();

                var courseToReturn = this.mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute(
                    nameof(GetCourseForAuthor),
                    new { authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }

            this.mapper.Map(course, courseEntity);
            this.courseLibraryRepository.UpdateCourse(courseEntity);
            this.courseLibraryRepository.Save();
            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(
            Guid authorId,
            Guid courseId,
            JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!this.courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = this.courseLibraryRepository.GetCourse(authorId, courseId);
            if (courseEntity == null) 
            {
                var courseDto = new CourseForUpdateDto();
                patchDocument.ApplyTo(courseDto, ModelState);
                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }
                var courseToAdd = this.mapper.Map<Entities.Course>(courseDto);
                courseToAdd.Id = courseId;

                this.courseLibraryRepository.AddCourse(authorId, courseToAdd);
                this.courseLibraryRepository.Save();

                var courseToReturn = this.mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute(
                    nameof(GetCourseForAuthor),
                    new { authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }

            var courseToPatch = this.mapper.Map<CourseForUpdateDto>(courseEntity);
            patchDocument.ApplyTo(courseToPatch, ModelState);
            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }
            this.mapper.Map(courseToPatch, courseEntity);
            this.courseLibraryRepository.UpdateCourse(courseEntity);
            this.courseLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!this.courseLibraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseEntity = this.courseLibraryRepository.GetCourse(authorId, courseId);
            if (courseEntity == null)
            {
                return NotFound();
            }

            this.courseLibraryRepository.DeleteCourse(courseEntity);
            this.courseLibraryRepository.Save();

            return NoContent();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }

    }
}
