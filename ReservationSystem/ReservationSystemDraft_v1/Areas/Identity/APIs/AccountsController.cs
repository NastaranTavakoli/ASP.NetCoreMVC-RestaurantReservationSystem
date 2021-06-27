using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReservationSystemDraft_v1.Areas.Identity.Models;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Helpers;
using Profile = ReservationSystemDraft_v1.Areas.Identity.Models.Profile;

namespace ReservationSystemDraft_v1.Areas.Identity.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountsController> _logger;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings, SignInManager<IdentityUser> signInManager, ILogger<AccountsController> logger, IMapper mapper, ApplicationDbContext context)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                    lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    var user = await _userManager.FindByNameAsync(model.Email);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new(ClaimTypes.Name, user.UserName),
                        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));

                    var token = new JwtSecurityToken(
                        issuer: _appSettings.Domain,
                        audience: _appSettings.Domain,
                        expires: DateTime.Now.AddHours(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "User account locked out.");
                    return BadRequest(ModelState);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }

            return BadRequest(ModelState);
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    try
                    {
                        await _userManager.AddToRoleAsync(user, "Member");
                        _logger.LogInformation("User created a new account with password.");

                        var existingPerson = await _context.People.FirstOrDefaultAsync(p => p.Email == model.Email);
                        if (existingPerson == null)
                        {
                            var person = new Person()
                            {
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Email = model.Email,
                                Phone = model.Phone,
                                UserId = user.Id
                            };
                            await _context.People.AddAsync(person);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            existingPerson.FirstName = model.FirstName;
                            existingPerson.LastName = model.LastName;
                            existingPerson.Phone = model.Phone;
                            existingPerson.UserId = user.Id;
                            await _context.SaveChangesAsync();
                        }
                        var authClaims = new List<Claim> {
                    new(ClaimTypes.Name, user.UserName),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new (ClaimTypes.Role, "Member")};
                        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));

                        var token = new JwtSecurityToken(
                            issuer: _appSettings.Domain,
                            audience: _appSettings.Domain,
                            expires: DateTime.Now.AddDays(7),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                    catch (Exception)
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError("Exception", "Something went wrong!!");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return BadRequest(ModelState);
        }



        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = User.Identity?.Name;
            if (user == null)
            {
                return NotFound();
            }
            var member = await _context.People.SingleOrDefaultAsync(p => p.Email == user);
            var userProfile = _mapper.Map<Profile>(member);
            userProfile.UserName = user;
            return Ok(userProfile);
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Update model)
        {

            var email = User.Identity?.Name;
            if (email == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, model.Password);
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        var person = await _context.People.FirstOrDefaultAsync(p => p.Email == email);
                        person.FirstName = model.FirstName;
                        person.LastName = model.LastName;
                        person.Phone = model.Phone;
                        person.Email = model.Email;
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    throw new Exception( "Something went wrong!!");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }
            return BadRequest(ModelState);
        }



    }

}
