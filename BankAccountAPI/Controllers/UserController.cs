using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BankAccountAPI.DTO;
using BankAccountAPI.Helpers;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BankAccountAPI.Controllers
{
    //todo: break the logic to Repository-Service pattern
    //todo: add logger
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(BankContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromHeader(Name = "Username")] string username,
            [FromBody] UserDTO dto)
        {
            username.ValidateUsername();
            
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                //TODO: add custom NotFoundException
                //TODO: move error messages to a constant class or resx file
                throw new Exception("User not found.");

            if (user.Username != "Admin") 
                //TODO: add custom InvalidPermissionException
                throw new Exception("Only admin users can create new users.");
            
            if (dto.Username.Length > 10) 
                throw new Exception("Username's maximum length is 10.");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new Exception("Username already exists.");
            
            //TODO: pass API gateway endpoint via appsettings
            //TODO: Add a APIGatewayService to handle all api gateway calls
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.github.com/");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("PostCode", dto.PostCode.ToString());
            request.Headers.Add("State", dto.State.ToUpper());
            
            var result = await client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Invalid post code and state combination.");
            }

            //TODO: Add UserDTO properties validation and mapper
            var newUser = new User
            {
                Username = dto.Username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Address = dto.Address,
                PostCode = dto.PostCode,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Post),
                new {id = dto.Username},
                dto);
        }
    }
}