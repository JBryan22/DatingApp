using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        //the SPA will send a JSON serialized object to us, which means we wont have a string username and string password
        //we need to create a Data Transfer Object (DTO) so that this method can be passed an object containing everything in the JSON
        //we can't use User because it includes Id and passwordhash and salt.
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate the request
            //All validation happens in the UserDTO class where we added [Required] attributes
            //validation checking is done automatically because of the ApiController attribute

            //this code is only necessary if we aren't using the APIController attribute above.
            //ApiController attribute also makes it unnecessary to put a [FromBody] attribute in front of the parameter in Register
            //We may have to use code like this if we are in a different type of controller
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }
            
            //JWT tokens allow a user to not have to authenticate with the database (SQL) every time they login.
            //When they first login, the database will validate their credentials and then create and pass back a JWT
            //The JWT is then passed to the server every future time that same user wants to login. 
            //The server is able to authenticate the user based on the JWT rather than having to access the database

            //Building the JWT token
            //our token contains 2 claims at this point. ID and username of the user that was validated above
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //retrieving our security key from appsettings and encoding it into bytes. 
            //Normally our token key would be very long and random rather than what is is currently
            //the token string is used for all JWT token creations and is very important to keep secure otherwise people can pretend to be users that have been authenticated
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //now we encrypt the key using the Hmac hashing algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //creating the descriptor. expires in 1 day
            //we sign the JWT with the creds created and encrypted above
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //write token into response with the token created 2 lines above
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}