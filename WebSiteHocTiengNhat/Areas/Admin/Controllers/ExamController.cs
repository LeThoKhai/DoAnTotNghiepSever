using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2019.Excel.ThreadedComments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;
using WebSiteHocTiengNhat.ViewModels;
using WebSiteHocTiengNhat.Data;
using Microsoft.EntityFrameworkCore;

namespace WebSiteHocTiengNhat.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
        private readonly string _apiKey = "AIzaSyDkh-E-q9D9RVA0mzxtPc35WTY9JjxyeoI";
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICoursesRepository _coursesRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ICategoryQuestionRepository _categoryQuestionRepository;
        private readonly ApplicationDbContext _dbContext;
        public ExamController(
            HttpClient httpClient,
            IQuestionRepository questionRepository,
            ICategoryRepository categoryRepository,
            ICoursesRepository coursesRepository,
            ICategoryQuestionRepository categoryQuestionRepository,
            ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
            _httpClient = httpClient;
            _categoryRepository = categoryRepository;
            _coursesRepository = coursesRepository;
            _questionRepository = questionRepository;
            _categoryQuestionRepository = categoryQuestionRepository;
        }



        public async Task<IActionResult> Index()
        {
            var list= await _dbContext.Exams.ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (ModelState.IsValid)
            {
                await _dbContext.Exams.AddAsync(exam);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int Id)
        {
            var item = _dbContext.Exams.FirstOrDefault(n => n.ExamId == Id);
            var listquestion = _dbContext.ExamQuestions.Where(n => n.ExamId == Id).ToList();
            if (item != null)
            {
                var detail = new ExamAndQuestionExamViewModel
                {
                    ExamId = item.ExamId,
                    Exam = item,
                    Examquestions = listquestion
                };
                return View(detail); 
            }
            return View();
        }

        //public async Task<IActionResult> Update(int id)
        //{
        //    var categories = await _categoryRepository.GetAllAsync();
        //    ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
        //    var exercise = await _ExamRepository.GetByIdAsync(id);
        //    if (exercise == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(exercise);
        //}
        //[HttpPost]
        //public async Task<IActionResult> Update(int id, Exercise exercise)
        //{
        //    if (id != exercise.ExerciseId)
        //    {
        //        return NotFound();
        //    }
        //    var exs = await _ExamRepository.GetByIdAsync(id);
        //    if (ModelState.IsValid)
        //    {
        //        var existingProduct = await _ExamRepository.GetByIdAsync(id);
        //        existingProduct.ExerciseName = exercise.ExerciseName;
        //        existingProduct.CategoryId = exercise.CategoryId;
        //        existingProduct.Content = exercise.Content;
        //        await _ExamRepository.UpdateAsync(existingProduct);
        //        await GenerateExam(exercise);
        //        return RedirectToAction(nameof(ExerciseList), new { courseId = existingProduct.CourseId });
        //    }
        //    return RedirectToAction(nameof(ExerciseList), new { courseId = exs.CourseId });
        //}
        //// Hiển thị form xác nhận xóa sản phẩm
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var exercise = await _ExamRepository.GetByIdAsync(id);
        //    if (exercise == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(exercise);
        //}
        //// Xử lý xóa sản phẩm
        //[HttpPost, ActionName("Delete")]
        //public async Task<IActionResult> Delete(int id, Exercise exercise)
        //{
        //    var lesson = await _ExamRepository.GetByIdAsync(id);
        //    if (lesson == null)
        //    {
        //        return NotFound();
        //    }
        //    int idcourse = lesson.CourseId;
        //    await _ExamRepository.DeleteAsync(id);
        //    return RedirectToAction(nameof(ExerciseList), new { courseId = idcourse });
        //}
    }
}
