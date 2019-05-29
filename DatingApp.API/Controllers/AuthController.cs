using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        //the SPA will send a JSON serialized object to us, which means we wont have a string username and string password
        //we need to create a Data Transfer Object (DTO) so that this method can be passed an object containing everything in the JSON
        //we can't use User because it includes Id and passwordhash and salt.
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate the request

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if(await _repo.UserExists(userForRegisterDto.Username))
            {
                return BadRequest("Username already exists");
            }
            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            //need to return a route that is the location of the newly created user
            return StatusCode(201);
        }
    }
}