using FUNewsManagementSystem.WebAPI.Data;
using FUNewsManagementSystem.WebAPI.DTOs;
using FUNewsManagementSystem.WebAPI.Models;
using FUNewsManagementSystem.WebAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/odata/SystemAccounts")]
    public class SystemAccountsController : ODataController
    {
        private readonly ISystemAccountRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly FUNewsManagementContext _context;

        public SystemAccountsController(
            ISystemAccountRepository repository,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            FUNewsManagementContext context
            )
        {
            _repository = repository;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _repository.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new IdentityUser
            {
                UserName = model.AccountEmail,
                Email = model.AccountEmail
            };

            var result = await _userManager.CreateAsync(user, model.AccountPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Gán vai trò
            var role = model.AccountRole == 1 ? "Staff" : model.AccountRole == 2 ? "Lecturer" : "User";
            await _userManager.AddToRoleAsync(user, role);

            // Lưu vào bảng SystemAccount (đồng bộ với Identity)
            var maxId = await _context.SystemAccounts.MaxAsync(sa => (short?)sa.AccountId) ?? 0;
            var systemAccount = new SystemAccount
            {
                AccountId = (short)(maxId + 1),
                AccountName = model.AccountName,
                AccountEmail = model.AccountEmail,
                AccountRole = model.AccountRole
            };
            await _repository.AddAsync(systemAccount);

            return CreatedAtAction(nameof(Get), new { id = systemAccount.AccountId }, systemAccount);
        }
    }
}


