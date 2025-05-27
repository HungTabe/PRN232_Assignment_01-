using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/Categories")]
    public class CategoriesController : ODataController
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("GetById/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(short id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Post([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _repository.AddAsync(category);
            return Created(category);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Put(short id, [FromBody] Category category)
        {
            if (id != category.CategoryId) return BadRequest();
            await _repository.UpdateAsync(category);
            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
