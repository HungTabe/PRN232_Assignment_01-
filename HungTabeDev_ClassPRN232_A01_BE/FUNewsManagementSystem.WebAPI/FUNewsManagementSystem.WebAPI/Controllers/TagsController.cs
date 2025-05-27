using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/Tags")]
    public class TagsController : ODataController
    {
        private readonly ITagRepository _repository;

        public TagsController(ITagRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _repository.GetAllAsync();
            return Ok(tags);
        }

        [HttpGet("GetById/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(int id)
        {
            var tag = await _repository.GetByIdAsync(id);
            if (tag == null) return NotFound();
            return Ok(tag);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Post([FromBody] Tag tag)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _repository.AddAsync(tag);
            return Created(tag);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Tag tag)
        {
            if (id != tag.TagId) return BadRequest();
            await _repository.UpdateAsync(tag);
            return NoContent();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
