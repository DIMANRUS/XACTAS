namespace XACTAS.WEB.Controllers;

[Route("[controller]/[action]")]
public class CultureController : ControllerBase
{
    public IActionResult SetCulture([FromQuery] string culture)
    {
        if (culture != null)
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));
        }

        return Redirect("/");
    }
}