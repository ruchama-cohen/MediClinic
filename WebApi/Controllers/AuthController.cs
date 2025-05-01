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
            public 
            public async IActionResult Login(LogInRequest request)
            {

             
                    }

            [HttpPost("Patient Verification Request")]
            public  async IActionResult PatientVerificationRequest(PatientVerificationRequest request)
            {
            // בקשה לאימות משתמש
        }
        [HttpPost("SiteRegistrationRequest")]

        public async IActionResult SiteRegistrationRequest(SiteRegistrationRequest request)
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
