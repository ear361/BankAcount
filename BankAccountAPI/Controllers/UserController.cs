using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BankAccountAPI.DTO;
using BankAccountAPI.Exceptions;
using BankAccountAPI.Helpers;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BankAccountAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BankContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserController> _logger;

        public UserController(BankContext context, IHttpClientFactory httpClientFactory,
            ILogger<UserController> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromHeader(Name = "Username")] string username,
            [FromBody] UserDTO dto)
        {
            username.ValidateUsername();

            if (dto == null)
                throw new InvalidCastException("Invalid User model");
            if (string.IsNullOrWhiteSpace(dto.Username)) 
                throw new InvalidInputException($"New user's username cannot be empty. Username: {dto.Username}");
            if (dto.Username.Length > 10) 
                throw new InvalidInputException($"New user username's maximum length is 10. Username: {dto.Username}");

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null)
                throw new ItemNotFoundException("User not found.");

            if (user.Username != "Admin") 
                throw new InvalidPermissionException("Only admin users can create new users.");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                throw new InvalidInputException("Username already exists.");
            
            var client = _httpClientFactory.CreateClient("APIGateWayClient");
            var content = new StringContent(JsonConvert.SerializeObject(dto)
                , Encoding.UTF8, "application/json");
            
            var result = await client.PostAsync("postcodecheck", content);
            if (!result.IsSuccessStatusCode)
            {
                throw new InvalidInputException("Invalid post code and state combination.");
            }

            //TODO: add mappers
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
            _logger.LogInformation($"User created. Username: {user.Username}");

            return Created(string.Empty, dto);
        }
    }
}