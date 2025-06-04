using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/NewsArticles")]
    //[Authorize(Roles = "Staff,Admin")] // Yêu cầu vai trò Staff hoặc Admin cho CRUD
    public class NewsArticlesController : ODataController
    {
        private readonly INewsArticleRepository _repository;

        public NewsArticlesController(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var articles = _repository.Query();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(string id)
        {
            var article = await _repository.GetByIdAsync(id);
            if (article == null)
            {
                return NotFound($"News article with ID {id} not found.");
            }
            return Ok(article);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewsArticle article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repository.AddAsync(article);
                return CreatedAtAction(nameof(Get), new { id = article.NewsArticleId }, article);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create news article: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] NewsArticle article)
        {
            if (id != article.NewsArticleId)
            {
                return BadRequest("News article ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repository.UpdateAsync(article);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update news article: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete news article: {ex.Message}");
            }
        }

        [HttpGet("ByUser/{userId}")]
        [EnableQuery]
        public IActionResult GetByUserId(short userId)
        {
            var articles = _repository.QueryByUserId(userId);
            return Ok(articles);
        }

        [HttpGet("Report")]
        //[Authorize(Roles = "Admin")] // Only Admin
        public async Task<IActionResult> GetReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date must be before end date.");
            }
            try
            {
                var report = await _repository.GetReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to generate report: {ex.Message}");
            }
        }
    }
}
