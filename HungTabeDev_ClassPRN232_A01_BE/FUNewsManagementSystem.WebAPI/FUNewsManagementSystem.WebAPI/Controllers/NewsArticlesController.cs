using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/NewsArticles")]
    public class NewsArticlesController : ODataController
    {
        private readonly INewsArticleRepository _repository;

        public NewsArticlesController(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _repository.GetAllAsync();
            return Ok(articles);
        }

        [HttpGet("GetById/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(string id)
        {
            var article = await _repository.GetByIdAsync(id);
            if (article == null) return NotFound();
            return Ok(article);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Post([FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _repository.AddAsync(article);
            return Created(article);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] NewsArticle article)
        {
            if (id != article.NewsArticleId) return BadRequest();
            await _repository.UpdateAsync(article);
            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("GetByUser/{userId}")]
        [EnableQuery]
        public async Task<IActionResult> GetByUserId(short userId)
        {
            var articles = await _repository.GetByUserIdAsync(userId);
            return Ok(articles);
        }

        [HttpGet("GetReport")]
        [EnableQuery]
        public async Task<IActionResult> GetReport(DateTime startDate, DateTime endDate)
        {
            var report = await _repository.GetReportAsync(startDate, endDate);
            return Ok(report);
        }
    }
}
