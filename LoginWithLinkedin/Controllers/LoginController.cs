using LoginWithLinkedin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace LoginWithLinkedin.Controllers;

public class LoginController : Controller
{
    IConfiguration _configuration;
    string _clientID = string.Empty;
    string _clientSecret = string.Empty;
    string _redirectUrl = string.Empty;
    public LoginController(IConfiguration configuration)
    {
        this._configuration = configuration;
        _clientID = _configuration["LinkedinAuth:ClientId"];
        _clientSecret = _configuration["LinkedinAuth:ClientSecret"];
        _redirectUrl = _configuration["LinkedinAuth:RedirectUrl"];
    }
    public IActionResult Index()
    {
        ViewBag.ClientID = _clientID;
        ViewBag.RedirectUrl = _redirectUrl;
        return View();
    }

    [HttpGet("/signin-linkedin")]
    public ActionResult SigninLinkedin(string code, string state)
    {
        //https://developer.linkedin.com/
        //https://www.linkedin.com/developers/apps/220219408/auth
        try
        {
            var client = new RestClient("https://www.linkedin.com/oauth/v2/accessToken");
            var request = new RestRequest();
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("redirect_uri", _redirectUrl);
            request.AddParameter("client_id", _clientID);
            request.AddParameter("client_secret", _clientSecret);

            var response = client.Execute(request, Method.Post);
            var content = response.Content;
            var res = JsonConvert.DeserializeObject<LinkedinToken>(content);

            var client2 = new RestClient("https://api.linkedin.com/v2/userinfo");
            request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {res.access_token}");
            var request2 = new RestRequest();
            var response2 = client2.Execute(request, Method.Get);
            var content2 = response2.Content;
            var user = JsonConvert.DeserializeObject<LinkedinUserInfo>(content2);

            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}


