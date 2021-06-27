using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Helpers;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterRestaurantModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly PhotoService _photoService;
        private AzureStorageConfig _storageConfig;

        public RegisterRestaurantModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger, ApplicationDbContext context, PhotoService photoService, IOptions<AzureStorageConfig> config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _photoService = photoService;
            _storageConfig = config.Value;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Your Email")]
            public string AdminEmail { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [MaxLength(255), MinLength(3)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [MaxLength(255), Display(Name = "Last Name"), MinLength(3)]
            public string LastName { get; set; }

            [Required, Display(Name = "Your Phone Number")]
            public string AdminPhone { get; set; }
            public string UserId { get; set; }

            [Required]
            [MaxLength(255), MinLength(3)]
            [Display(Name = "Business Name")]
            public string RestaurantName { get; set; }

            [Required, Display(Name = "Business phone number")]
            public string RestaurantPhone { get; set; }


            [Required, Display(Name = "Business Address")]
            public string RestaurantAddress { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Business Email")]
            public string RestaurantEmail { get; set; }

            [Display(Name = "Select Images of your business. You can select multiple files")]
            public List<IFormFile> Images { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Admin");
            string[] acceptedFileTypes = { ".jpg", ".jpeg", ".png" };
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.AdminEmail, Email = Input.AdminEmail, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {

                    try
                    {
                        var restaurant = new Restaurant();
                        await _userManager.AddToRolesAsync(user, new string[] { "Staff", "Manager" });
                        //await _userManager.AddToRoleAsync(user, "Manager");
                        _logger.LogInformation("User created a new account with password.");

                        if (Input.Images != null)
                        {

                            if (Input.Images.Count > 5)
                            {
                                ModelState.AddModelError("Exception", "You can only upload a maximum of 5 files");
                            }
                            if (Input.Images.Any(f => f.Length == 0))
                            {
                                ModelState.AddModelError("Exception", "Empty File");
                            }
                            if (Input.Images.Any(f => f.Length > 10485760))
                            {
                                ModelState.AddModelError("Exception", "Max file size exceeded");
                            }
                            if (!Input.Images.All(f => acceptedFileTypes.Contains(Path.GetExtension(f.FileName).ToLower())))
                            {
                                ModelState.AddModelError("Exception", "Invalid file type");
                            }
                            if (!ModelState.IsValid)
                            {
                                throw new Exception();
                            }
                            await _photoService.UploadPhotos(Input.Images, restaurant, _storageConfig);
                        }

                        if (ModelState.IsValid)
                        {
                            restaurant.Name = Input.RestaurantName;
                            restaurant.Phone = Input.RestaurantPhone;
                            restaurant.Address = Input.RestaurantAddress;
                            restaurant.Email = Input.RestaurantEmail;
                            await _context.Restaurants.AddAsync(restaurant);
                            await _context.SaveChangesAsync();
                            var manager = new Person()
                            {
                                FirstName = Input.FirstName,
                                LastName = Input.LastName,
                                Email = Input.AdminEmail,
                                Phone = Input.AdminPhone,
                                UserId = user.Id,
                                RestaurantId = restaurant.Id,
                            };
                            await _context.People.AddAsync(manager);
                            await _context.SaveChangesAsync();
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
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

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
