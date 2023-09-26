using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Hosting.LocalApiAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Play.Identity.Service.Dtos;
using Play.Identity.Service.Entities;
using Play.Identity.Service.Extensions;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Play.Identity.Service.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Policy = LocalApi.PolicyName)]
    public class UsersControllers : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UsersControllers(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }


        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> Get()
        {
            var users = userManager.Users
                                        .ToList()
                                        .Select(user => user.AsDto());

            return Ok(users);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<UserDto>> GetByIdAsync(Guid Id)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            return user.AsDto();
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult> PutAsync(Guid Id, UpdateUserDto updateUserDto)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.Email;
            user.Gil = updateUserDto.Gil;

            await userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteAsync(Guid Id)
        {
            var user = await userManager.FindByIdAsync(Id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            await userManager.DeleteAsync(user);

            return NoContent();
        }
    }
}