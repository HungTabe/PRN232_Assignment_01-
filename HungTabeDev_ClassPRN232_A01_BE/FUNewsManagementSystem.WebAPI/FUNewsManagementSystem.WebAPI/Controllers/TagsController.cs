using FUNewsManagementSystem.WebAPI.DTOs;
using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/Tags")]
    [ApiController]
    //[Authorize(Roles = "Staff,Admin")]
    public class TagsController : ODataController
    {
        private readonly ITagRepository _repository;

        public TagsController(ITagRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Gets()
        {
            var tags = _repository.Query()
                .Select(t => new TagDTO
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                    Note = t.Note
                });
            return Ok(tags);
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(int id)
        {
            var tag = await _repository.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound($"Tag with ID {id} not found.");
            }
            var tagDto = new TagDTO
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };
            return Ok(tagDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TagDTO tagDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var tag = new Tag
                {
                    TagId = tagDto.TagId,
                    TagName = tagDto.TagName,
                    Note = tagDto.Note
                };
                await _repository.AddAsync(tag);
                tagDto.TagId = tag.TagId;
                return CreatedAtAction(nameof(Get), new { id = tagDto.TagId }, tagDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create tag: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TagDTO tagDto)
        {
            if (id != tagDto.TagId)
            {
                return BadRequest("Tag ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var tag = new Tag
                {
                    TagId = tagDto.TagId,
                    TagName = tagDto.TagName,
                    Note = tagDto.Note
                };
                await _repository.UpdateAsync(tag);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update tag: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete tag: {ex.Message}");
            }
        }
    }
}
