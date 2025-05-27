using FUNewsManagementSystem.WebAPI.DTOs;
using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/SystemAccounts")]
    public class SystemAccountsController : ODataController
    {
        private readonly ISystemAccountRepository _repository;

        public SystemAccountsController(ISystemAccountRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _repository.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("GetById/{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(short id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Post([FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _repository.AddAsync(account);
            return Created(account);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Put(short id, [FromBody] SystemAccount account)
        {
            if (id != account.AccountId) return BadRequest();
            await _repository.UpdateAsync(account);
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

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
        {
            var account = await _repository.AuthenticateAsync(model.Email, model.Password);
            if (account == null) return Unauthorized();
            return Ok(new { account.AccountId, account.AccountName, account.AccountEmail, account.AccountRole });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var account = new SystemAccount
                {
                    AccountName = model.AccountName,
                    AccountEmail = model.AccountEmail,
                    AccountPassword = model.AccountPassword,
                    AccountRole = model.AccountRole
                };

                var newAccount = await _repository.RegisterAsync(account);
                return CreatedAtAction(nameof(Get), new { id = newAccount.AccountId }, newAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

