using Microsoft.AspNetCore.Mvc;
using QuizBuilder.Models;
using System.Security.Claims;

namespace QuizBuilder.Controllers
{
    public abstract class QuizBaseController : ControllerBase
    {

        protected User GetUser() 
        {
            User currentUser = null;
            if (HttpContext == null) return null;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var id = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var email = identity.FindFirst("preferred_username").Value;
                currentUser = new User()
                {
                    Id = id,
                    Email = email
                };
            }
            return currentUser;
        }

    }
}
