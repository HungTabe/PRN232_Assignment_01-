using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/Categories")]
    //[Authorize(Roles = "Staff,Admin")] // Yêu cầu vai trò Staff hoặc Admin
    public class CategoriesController : ODataController
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(short id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repository.AddAsync(category);
                return CreatedAtAction(nameof(Get), new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create category: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(short id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("Category ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _repository.UpdateAsync(category);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update category: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete category: {ex.Message}");
            }
        }
    }
}
