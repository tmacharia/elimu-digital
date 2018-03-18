using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.AccountViewModels;
using Common.Models;
using DAL.Contexts;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;

namespace web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IRepositoryFactory _repos;
        private readonly IDataManager _dataManager;
        private readonly AppDbContext _appDbContext;
        private readonly LePadContext _lepadContext;
        private readonly IEmailSender _emailSender;
        private readonly IUploader _uploader;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly string _externalCookieScheme;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IRepositoryFactory factory,
            IDataManager dataManager,
            AppDbContext appDbContext,
            LePadContext lePadContext,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            IUploader uploader,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _repos = factory;
            _dataManager = dataManager;
            _appDbContext = appDbContext;
            _lepadContext = lePadContext;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _uploader = uploader;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        [HttpGet]
        public ActionResult List()
        {
            var users = _userManager.Users.ToList();
            
            return Ok(users);
        }
        public ActionResult Roles()
        {
            var roles = _appDbContext.Roles.ToList();

            return Ok(roles);
        }
        public ActionResult SeedRoles()
        {
            List<AppRole> list = new List<AppRole>()
            {
                new AppRole()
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new AppRole()
                {
                    Name = "Lecturer",
                    NormalizedName = "Lecturer"
                },
                new AppRole()
                {
                    Name = "Student",
                    NormalizedName = "Student"
                }
            };

            _appDbContext.Roles.AddRange(list);
            _appDbContext.SaveChanges();

            return RedirectToAction("Roles");
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        [Route("account/profile/{names}")]
        public async Task<IActionResult> Profile(string names)
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = GetProfile(user);

            ViewBag.likes = _repos.Likes.ListWith("By").Where(x => x.By.Id == profile.Id).ToList();
            ViewBag.comments = _repos.Comments.ListWith("By").Where(x => x.Id == profile.Id).ToList();

            if (user.AccountType == AccountType.Student)
            {
                var student = _repos.Students
                                        .ListWith("StudentUnits","StudentUnits.Unit.Class","StudentUnits.Unit.Course", "Course", "Scores", "Profile")
                                        .Where(x => x.Profile.Id == profile.Id)
                                        .FirstOrDefault();

                ViewBag.student = student;

                ViewBag.units = _dataManager.MyUnits<Student>(user.AccountId).ToList();

                ViewBag.mates = _dataManager.MyClassMates(user.AccountId).ToList();
            }
            else if(user.AccountType == AccountType.Lecturer)
            {
                var lecturer = _repos.Lecturers
                                         .ListWith("Units", "Units.Class", "Units.Course", "UploadedContent", "Likes", "Profile")
                                         .Where(x => x.Profile.Id == profile.Id)
                                         .FirstOrDefault();

                ViewBag.lecturer = lecturer;
                ViewBag.units = _dataManager.MyUnits<Lecturer>(user.AccountId).ToList();

                ViewBag.colleagues = _dataManager.MyColleagues(user.AccountId).Take(10).ToList();
            }

            return View(profile);
        }

        [HttpGet]
        [Route("account/edit/{names}")]
        public async Task<IActionResult> Edit(string names)
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = GetProfile(user);

            return View(profile);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Profile()
        {
            try
            {
                int type = int.Parse(Request.Form["Type"]);

                var user = await _userManager.GetUserAsync(User);

                var profile = new Profile
                {
                    FullNames = Request.Form["FullNames"],
                    NationalID = int.Parse(Request.Form["ID"])
                };
                if (Request.Form.Files.Count > 0)
                {
                    IFile file = new FormFile(Request.Form.Files[0]);
                    profile.PhotoUrl = await _uploader.Upload(file);
                }
                if (type == 1)
                {
                    //lec
                    Lecturer lecturer = new Lecturer(Guid.Parse(user.Id))
                    {
                        Profile = profile
                    };
                    _lepadContext.Lecturers.Add(lecturer);
                    _lepadContext.SaveChanges();

                    user.AccountId = lecturer.Id;
                    user.AccountType = AccountType.Lecturer;
                }
                else if (type == 2)
                {
                    //student
                    Student student = new Student(Guid.Parse(user.Id))
                    {
                        YearOfStudy = int.Parse(Request.Form["YrOfStudy"]),
                        AcademicYear = Request.Form["AcademicYr"],
                        RegNo = Request.Form["RegNo"],
                        Profile = profile,
                    };
                    _lepadContext.Students.Add(student);
                    _lepadContext.SaveChanges();

                    user.AccountId = student.Id;
                    user.AccountType = AccountType.Student;
                }
                else if(type == 0)
                {
                    // admin 
                    Admin admin = new Admin(Guid.Parse(user.Id))
                    {
                        Profile = profile
                    };

                    _lepadContext.Administrators.Add(admin);
                    _lepadContext.SaveChanges();

                    user.AccountId = admin.Id;
                    user.AccountType = AccountType.Administrator;
                }

                // add claims we need in the app (userId, accountType)

                var claims = new List<Claim>{
                    new Claim("UserId", user.Id),
                    new Claim("ProfileId", profile.Id.ToString())
                };

                if(!string.IsNullOrWhiteSpace(profile.FullNames))
                {
                    claims.Add(new Claim("FullNames", profile.FullNames));
                }

                if (!string.IsNullOrWhiteSpace(profile.PhotoUrl))
                {
                    claims.Add(new Claim("PhotoUrl", profile.PhotoUrl));
                }

                await _userManager.AddClaimsAsync(user, claims);

                //update user identity account
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectPermanent("/");
                }
                else
                {
                    return Content(result.Errors.First().Description);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Require the user to have a confirmed email before they can log on.
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ViewBag.error = "You must have a confirmed email to log in.";

                        return View(model);
                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");

                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ViewBag.error = "Incorrect email or password";
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private Profile GetProfile(AppUser user)
        {
            Profile profile = null;

            switch (user.AccountType)
            {
                case AccountType.Administrator:
                    profile = _lepadContext.Administrators
                                        .Include(x => x.Profile)
                                        .FirstOrDefault(x => x.AccountId.ToString() == user.Id)
                                        ?.Profile;
                    break;
                case AccountType.Lecturer:
                    profile = _lepadContext.Lecturers
                                        .Include(x => x.Profile)
                                        .FirstOrDefault(x => x.AccountId.ToString() == user.Id)
                                        ?.Profile;
                    break;
                case AccountType.Student:
                    profile = _lepadContext.Students
                                        .Include(x => x.Profile)
                                        .FirstOrDefault(x => x.AccountId.ToString() == user.Id)
                                        ?.Profile;
                    break;
                case AccountType.None:
                    break;
                default:
                    break;
            }

            if (profile == null)
                return new Profile()
                {
                    FullNames = "",
                    PhotoUrl = ""
                };
            else
                return profile;
        }

        private string CheckRole(AppUser user)
        {
            switch (user.AccountType)
            {
                case AccountType.Administrator:
                    return "Administrator";
                case AccountType.Lecturer:
                    return "Lecturer";
                case AccountType.Student:
                    return "Student";
                case AccountType.None:
                    return "Unregistered";
                default:
                    return "Unregistered";
            }
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email, AccountType = model.AccountType };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // add user to role
                    switch (user.AccountType)
                    {
                        case AccountType.Lecturer:
                            await _userManager.AddToRoleAsync(user, "Lecturer");
                            break;
                        case AccountType.Student:
                            await _userManager.AddToRoleAsync(user, "Student");
                            break;
                        default:
                            break;
                    }

                    await _userManager.AddClaimAsync(user, new Claim("Role", user.AccountType.ToString()));
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        $"<h3 style='color:#53a6fa;'>Welcome to Gobel Digital University,</h3>\n\n" +
                        $"Please confirm your account by clicking this <a href='{callbackUrl}'>link</a>");
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }

                ViewBag.error = result.Errors.First().Description;
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");

            return RedirectPermanent("/");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"<h3 style='color:#53a6fa;'>You requested a password reset,</h3>/n/n" +
                   $"Please reset your password by clicking this <a href='{callbackUrl}'>link</a>\n\n" +
                   $"If this wasn't you, please reset your password to secure your account.");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if(code == null)
            {
                ViewBag.error = "Error";
                return View();
            }

            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            ViewBag.error = result.Errors.First().Description;
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

            throw new NotImplementedException();
            //return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> SendCode()
        {
            throw new NotImplementedException();
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        //
        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
