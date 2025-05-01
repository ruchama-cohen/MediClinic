using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using BLL.Models;
namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

            [HttpPost("login")]
            //id pass
            public async IActionResult Login(LogInRequest request)
            {


                    }

            [HttpPost("signup")]
            public  async IActionResult SignIn(SignInRequest request)
            {
                // הרשמת משתמש חדש
            }

            [HttpPost("refresh-token")]
            public IActionResult RefreshToken(TokenRequest request)
            {
                // רענון טוקן - אופציונלי
            }
        

    }
}
