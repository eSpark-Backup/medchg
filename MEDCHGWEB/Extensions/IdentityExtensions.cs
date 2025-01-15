namespace MEDCHGWEB.Extensions;

using MEDCHGClassLibrary.Utilities.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class CurrentUserModel
{
    public string? Id { get; set; } 
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? ImgUrl { get; set; }
    public string? UserRole { get; set; }
    public string? UserId { get; set; }
    public string? ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public string? ProviderDisplayName { get; set; }
    public string? Designation { get; set; }
}
public static class HttpContextHelper
{
    public static HttpContext? Current =>
        // If you have access to the context in a static way, use it; otherwise, this will return null
        new HttpContextAccessor().HttpContext;
}
public static class IdentityExtensions
{
    public static CurrentUserModel GetUserInfo(this ClaimsPrincipal user)
    {
        Console.WriteLine("Claims Count On Identity Extension At:- " + " " + DateTime.Now + " " + user.Claims.Count());
        var httpContext = HttpContextHelper.Current;
        // Get all cookies as a list
        var cookies = httpContext.Request.Cookies.Where(x=>x.Key == "jwtToken").FirstOrDefault();
        // Return the value of the second cookie
        var token = cookies.Value; // 1 because it's zero-indexed
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var User = new CurrentUserModel();
        User.Id = jwtSecurityToken.Claims.First(claim => claim.Type == "Id").Value;
        User.Name = jwtSecurityToken.Claims.First(claim => claim.Type == "UserName").Value;
        User.Email = jwtSecurityToken.Claims.First(claim => claim.Type == "Email").Value;
        User.ImgUrl = jwtSecurityToken.Claims.First(claim => claim.Type == "ImgUrl").Value;
        User.UserRole = jwtSecurityToken.Claims.First(claim => claim.Type == "UserRole").Value;
        User.UserId = jwtSecurityToken.Claims.First(claim => claim.Type == "UserId").Value;
        if (User.UserRole is "Admin" || User.UserRole is "Doctor" || User.UserRole is "User")
        {
            User.ProviderId = jwtSecurityToken.Claims.First(claim => claim.Type == "ProviderId").Value;
            User.ProviderName = jwtSecurityToken.Claims.First(claim => claim.Type == "ProviderName").Value;
            User.Designation = jwtSecurityToken.Claims.First(claim => claim.Type == "Designation").Value;
            User.ProviderDisplayName = jwtSecurityToken.Claims.First(claim => claim.Type == "ProviderDisplayName").Value;
        }
        User.FullName = jwtSecurityToken.Claims.First(claim => claim.Type == "FullName").Value;
        var allClaims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();
        foreach (var claim in allClaims)
        {
            switch (claim.Type)
            {
                case "Id":
                    User.Id = claim.Value;
                    break;
                case "UserName":
                    User.Name = claim.Value;
                    break;
                case "Email":
                    User.Email = claim.Value;
                    break;
                case "ImgUrl":
                    User.ImgUrl = claim.Value;
                    break;
                case "UserRole":
                    User.UserRole = claim.Value;
                    break;
                case "UserId":
                    User.UserId = claim.Value;
                    break;
                case "ProviderId":
                    User.ProviderId = claim.Value;
                    break;
                case "ProviderName":
                    User.ProviderName = claim.Value;
                    break;
                case "Designation":
                    User.Designation = claim.Value;
                    break;
                case "FullName":
                    User.FullName = claim.Value;
                    break;
                    // Add additional cases for other claim types if needed
            }
        }
        return User;
    }

    public static async Task AddUpdateClaim(this ClaimsPrincipal user, IHttpContextAccessor httpContextAccessor, string key, string value)
    {
        var identity = user.Identity as ClaimsIdentity;
        if (identity == null)
            return;

        // Check for existing claim and remove it
        var existingClaim = identity.FindFirst(key);
        if (existingClaim != null)
            identity.RemoveClaim(existingClaim);

        // Add new claim
        identity.AddClaim(new Claim(key, value));

        var authenticationManager = httpContextAccessor.HttpContext?.SignInAsync(user);
        if (authenticationManager != null)
        {
            // This is just an example, adjust as per your authentication scheme
            await authenticationManager;
        }
    }
}

