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
        private readonly ApplicationDbContext _dbContext;
        public ExamController(HttpClient httpClient,ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
            _httpClient = httpClient;
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



        public async Task<IActionResult> Update(int id)
        {
            var item = _dbContext.Exams.FirstOrDefault(n => n.ExamId == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Exam exam)
        {
            if (id != exam.ExamId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct =  _dbContext.Exams.FirstOrDefault(n => n.ExamId == id);
                if (existingProduct != null)
                {
                    existingProduct.ExamName = exam.ExamName;
                    existingProduct.Content = exam.Content;
                    _dbContext.Exams.Update(existingProduct);
                    _dbContext.SaveChanges();
                    return RedirectToAction(nameof(Detail), new { id = existingProduct.ExamId });
                }
            }
            return RedirectToAction(nameof(Detail), new { id = id });
        }



        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var item = _dbContext.Exams.FirstOrDefault(n => n.ExamId == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Delete(int id, Exam exam)
        {
            var item = _dbContext.Exams.FirstOrDefault(n => n.ExamId == id);
            if (item == null)
            {
                return NotFound();
            }
            _dbContext.Remove(item);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
