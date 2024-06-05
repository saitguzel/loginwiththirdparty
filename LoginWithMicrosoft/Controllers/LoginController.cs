using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Mvc;

namespace LoginWithMicrosoft.Controllers;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task Login()
    {
        await HttpContext.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme,
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("MicrosoftResponse")
            });
    }

    public async Task<IActionResult> MicrosoftResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        {
            claim.Issuer,
            claim.OriginalIssuer,
            claim.Type,
            claim.Value
        });
        return RedirectToAction("index", "home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction("Index");
    }
}