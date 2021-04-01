using ApiSecurityInAction.Auth;
using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
	{
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[BasicAuth] // Anybody can register an account, and they won't be authenticated first
        public async Task<ActionResult<IdentityResult>> RegisterUser([FromBody] UserDto user)
		{
            var userResult = await userManager.CreateAsync(new IdentityUser(user.UserName), user.Password);
            return CreatedAtAction("RegisterUser", userResult);
		}

        // Added just for testing purposes. Not found in the book.
        /// <summary>
        /// Returns a user
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns></returns>
        [HttpGet]
        [BasicAuth]
        public async Task<ActionResult<UserDto>> GetUser(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null) return Ok(new UserDto() { UserName = user.UserName, Password = user.PasswordHash });
            return NotFound();
        }
    }
}
