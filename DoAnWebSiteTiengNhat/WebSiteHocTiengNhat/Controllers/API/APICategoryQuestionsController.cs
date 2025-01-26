using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using WebSiteHocTiengNhat.Models;
using WebSiteHocTiengNhat.Repository;

namespace WebSiteHocTiengNhat.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APICategoryQuestionsController : Controller
    {
        private readonly ICategoryQuestionRepository _categoryQuestionRepository;
        private readonly IQuestionRepository _questionRepository;
        public APICategoryQuestionsController(ICategoryQuestionRepository categoryQuestionRepository) 
        {   
            _categoryQuestionRepository = categoryQuestionRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryQuestion>>> getCategories()
        {
            var categoryquestion= await _categoryQuestionRepository.GetAllAsync();
            return Ok(categoryquestion);
        }
        [HttpGet("getQuestionByCategoryQuestionId/{id}")]
        public async Task<ActionResult<Category>> getQuestionByQuestionCategoryId(int id)
        {
            var listquestion = await _categoryQuestionRepository.GetByCategoryQuestionId(id);

            if (listquestion == null)
            {
                return NotFound();
            }

            return Ok(listquestion);
        }
/*        [HttpGet()]
        //[HttpGet("getQuestionReading")]
        //public async Task<ActionResult<Category>> getQuestionReading()
        //{
        //    var categoryquestion = await _categoryQuestionRepository.GetAllAsync();
        //    categoryquestion=categoryquestion.Where(n=>n.IsReading==true);
            
        //    var listquestion = await _categoryQuestionRepository.GetListQuestions(categoryquestion.CategoryQuestionId);

        //    if (listquestion == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(listquestion);
        //}*/
    }
}
