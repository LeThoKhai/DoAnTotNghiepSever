using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebSiteHocTiengNhat.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {

        private readonly ICoursesRepository _repository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ICommentRepository _commentRepository;

        public CoursesController(ICoursesRepository repository, ILessonRepository lessonRepository, ICommentRepository commentRepository)
        {
            _repository = repository;
            _lessonRepository = lessonRepository;
            _commentRepository = commentRepository;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _repository.GetAllAsync();
            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }
        [HttpGet("details/{id}")]
        public async Task<ActionResult<CourseDetail>> GetCourseDetail(int id)
        {
            var course = await _repository.GetByIdAsync(id);
            //var ls = await _lessonRepository.GetByCourseId(id);
            var ls = await _lessonRepository.GetListLessonByCourseId(id);
            var cm = await _commentRepository.GetCommentsByCourseIdAsync(id);

            if (course == null)
            {
                return NotFound();
            }
            CourseDetail courseDetail = new CourseDetail()
            {
                Course = course,
                lessons = ls.ToList(),
                comments = cm.ToList(),
            };
            return Ok(courseDetail);
        }



        //// PUT: api/Courses/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCourse(int id, [FromForm] Course course, IFormFile? image)
        //{
        //    if (id != course.CourseId)
        //    {
        //        return BadRequest("Course ID mismatch.");
        //    }

        //    var existingCourse = await _repository.GetByIdAsync(id);
        //    if (existingCourse == null)
        //    {
        //        return NotFound();
        //    }

        //    // Nếu người dùng không nhập hình ảnh, giữ nguyên hình ảnh cũ
        //    if (image != null)
        //    {
        //        existingCourse.Image = await SaveImage(image); // Cập nhật ảnh mới
        //    }

        //    // Cập nhật các trường khác ngoại trừ CourseId
        //    existingCourse.CourseName = course.CourseName;
        //    existingCourse.Price = course.Price;
        //    existingCourse.Content = course.Content;
        //    existingCourse.Status = course.Status;

        //    try
        //    {
        //        await _repository.UpdateAsync(existingCourse); // Lưu thay đổi qua repository
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!await CourseExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Courses
        //[HttpPost]
        //public async Task<ActionResult<Course>> PostCourse([FromForm] Course course, IFormFile? image)
        //{
        //    if (image != null)
        //    {
        //        course.Image = await SaveImage(image);
        //    }

        //    await _repository.AddAsync(course);
        //    return CreatedAtAction("GetCourse", new { id = course.CourseId }, course);
        //}

        //// DELETE: api/Courses/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCourse(int id)
        //{
        //    var course = await _repository.GetByIdAsync(id);
        //    if (course == null)
        //    {
        //        return NotFound();
        //    }

        //    await _repository.DeleteAsync(id);
        //    return NoContent();
        //}

        //// Private method to save image
        //private async Task<string> SaveImage(IFormFile image)
        //{
        //    var savePath = Path.Combine("wwwroot/images", image.FileName);
        //    using (var fileStream = new FileStream(savePath, FileMode.Create))
        //    {
        //        await image.CopyToAsync(fileStream);
        //    }
        //    return "/images/" + image.FileName;
        //}

        //private async Task<bool> CourseExists(int id)
        //{
        //    return await _repository.GetByIdAsync(id) != null;
        //}
        public class CourseDetail
        {
            public Course Course { get; set; }
            public List<Lesson> lessons { get; set; }
            public List<Comment> comments { get; set; }
        }
    }
}
