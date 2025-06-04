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
            FUNewsManagementContext context)
        {
            _repository = repository;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Get()
        {
            var accounts = _repository.Query()
                .Select(sa => new SystemAccountDTO
                {
                    AccountId = sa.AccountId,
                    AccountName = sa.AccountName,
                    AccountEmail = sa.AccountEmail,
                    AccountRole = sa.AccountRole
                });
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        [EnableQuery]
        public async Task<IActionResult> Get(short id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound($"System account with ID {id} not found.");
            }
            var accountDto = new SystemAccountDTO
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole
            };
            return Ok(accountDto);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] SystemAccountDTO accountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var account = new SystemAccount
                {
                    AccountId = accountDto.AccountId,
                    AccountName = accountDto.AccountName,
                    AccountEmail = accountDto.AccountEmail,
                    AccountRole = accountDto.AccountRole
                };
                await _repository.AddAsync(account);
                return CreatedAtAction(nameof(Get), new { id = accountDto.AccountId }, accountDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create system account: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(short id, [FromBody] SystemAccountDTO accountDto)
        {
            if (id != accountDto.AccountId)
            {
                return BadRequest("System account ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var account = new SystemAccount
                {
                    AccountId = accountDto.AccountId,
                    AccountName = accountDto.AccountName,
                    AccountEmail = accountDto.AccountEmail,
                    AccountRole = accountDto.AccountRole
                };
                await _repository.UpdateAsync(account);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update system account: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(short id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete system account: {ex.Message}");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid email or password.");
            }

            var account = await _context.SystemAccounts.FirstOrDefaultAsync(sa => sa.AccountEmail == model.Email);
            if (account == null)
            {
                return Unauthorized("System account not found.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, account.AccountRole == 0 ? "Admin" : account.AccountRole == 1 ? "Staff" : "Lecturer")
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new IdentityUser
                {
                    UserName = model.AccountEmail,
                    Email = model.AccountEmail
                };

                var result = await _userManager.CreateAsync(user, model.AccountPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.Select(e => e.Description));
                }

                var role = model.AccountRole == 0 ? "Admin" : model.AccountRole == 1 ? "Staff" : "Lecturer";
                await _userManager.AddToRoleAsync(user, role);

                var accountDto = new SystemAccountDTO
                {
                    AccountName = model.AccountName,
                    AccountEmail = model.AccountEmail,
                    AccountRole = model.AccountRole
                };
                var account = new SystemAccount
                {
                    AccountName = accountDto.AccountName,
                    AccountEmail = accountDto.AccountEmail,
                    AccountRole = accountDto.AccountRole
                };
                await _repository.AddAsync(account);
                accountDto.AccountId = account.AccountId;

                await transaction.CommitAsync();
                return CreatedAtAction(nameof(Get), new { id = accountDto.AccountId }, accountDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Xóa IdentityUser nếu SystemAccount thất bại
                var user = await _userManager.FindByEmailAsync(model.AccountEmail);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
                return BadRequest($"Failed to register account: {ex.Message}");
            }
        }
    }
}


