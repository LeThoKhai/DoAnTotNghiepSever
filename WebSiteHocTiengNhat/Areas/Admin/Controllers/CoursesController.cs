using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly ICoursesRepository _coursesRepository;

        public CoursesController(ICoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _coursesRepository.GetAllAsync();
            return View(courses);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Course course, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    course.Image = await SaveImage(imageUrl);
                }
                await _coursesRepository.AddAsync(course);
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var course = await _coursesRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        public async Task<IActionResult> Update(int id)
        {
            var course = await _coursesRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Course course, IFormFile? imageUrl)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await _coursesRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
               // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    course.Image = existingProduct.Image;
                }
                else
                {
                    // Lưu hình ảnh mới
                    course.Image = await SaveImage(imageUrl);

                }
                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.CourseName = course.CourseName;
                existingProduct.Price = course.Price;
                existingProduct.Content = course.Content;
                existingProduct.Status = course.Status;
                existingProduct.Image = course.Image;
                await _coursesRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }
        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _coursesRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id, Course cs)
        {
            var course = await _coursesRepository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            await _coursesRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
