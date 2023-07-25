using Microsoft.Owin.Security.OAuth;
using SportCalendar.Model;
using SportCalendar.ServiceCommon;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;

namespace SportCalendar.WebApi.Controllers
{
    public class RegisterController : ApiController
    {
        public RegisterController(IUserService userService)
        {
            UserService = userService;
        }

        protected IUserService UserService { get; set; }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/signup")]
        public async Task<HttpResponseMessage> RegisterAsync([FromBody] User newUser)
        {
            try
            {
                User addedUser = await UserService.InsertUserAsync(newUser);
                if (addedUser != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, addedUser);
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, "User already in database");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
            }
        }

        [HttpGet]
        [Route("api/logout")]
        public HttpResponseMessage Logout()
        {
            HttpContext.Current.GetOwinContext().Authentication.SignOut();
            return Request.CreateResponse(HttpStatusCode.OK, "Logged out successfully.");
        }
    }
}
