using System.Security.Claims;
using Contracts.UserDtos;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;

        public UsersController(IUserService userService, ICountryService countryService, ICityService cityService)
        {
            _userService = userService;
            _countryService = countryService;
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var identityUserId = GetCurrentIdentityUserId();
            if (identityUserId == null)
                return Unauthorized();

            var user = await _userService.GetUserByIdentityUserIdAsync(identityUserId);
            if (user == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(new[] { user });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var identityUserId = GetCurrentIdentityUserId();
            if (identityUserId == null)
                return Unauthorized();

            var user = await _userService.GetUserByIdentityUserIdAsync(identityUserId);
            if (user == null)
                return NotFound("A profile has not been created for the current identity user.");

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"The user with ID {id} was not found.");

            if (!IsCurrentUser(user))
                return Forbid();

            return Ok(user);
        }

        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound($"The user with Username:'{username}' was not found.");

            if (!IsCurrentUser(user))
                return Forbid();

            return Ok(user);
        }

        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound($"The user with Email:'{email}' was not found.");

            if (!IsCurrentUser(user))
                return Forbid();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserPostDto userPostDto)
        {
            var identityUserId = GetCurrentIdentityUserId();
            if (identityUserId == null)
                return Unauthorized();

            var validationResult = await ValidateLocationAsync(userPostDto.CountryId, userPostDto.CityId);
            if (validationResult != null)
                return validationResult;

            var existingProfile = await _userService.GetUserByIdentityUserIdAsync(identityUserId);
            if (existingProfile != null)
                return Conflict("A profile already exists for the current identity user.");

            var userForUsernameControl = await _userService.GetUserByUsernameAsync(userPostDto.Username);
            if (userForUsernameControl != null)
                return Conflict($"A user with the same Username:'{userPostDto.Username}' already exists.");

            var userForEmailControl = await _userService.GetUserByEmailAsync(userPostDto.Email);
            if (userForEmailControl != null)
                return Conflict($"A user with the same Email:'{userPostDto.Email}' already exists.");

            var user = new User
            {
                IdentityUserId = identityUserId,
                Username = userPostDto.Username,
                Email = userPostDto.Email,
                PhoneNumber = userPostDto.PhoneNumber,
                Age = userPostDto.Age,
                CountryId = userPostDto.CountryId,
                CityId = userPostDto.CityId,
                Address = userPostDto.Address,
                Budget = userPostDto.Budget,
                IsPremium = userPostDto.IsPremium
            };

            await _userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.UserID }, await _userService.GetUserByIdAsync(user.UserID));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserPutDto userPutDto)
        {
            var userForControl = await _userService.GetUserByIdAsync(id);
            if (userForControl == null)
                return NotFound($"The user with ID {id} was not found.");

            if (!IsCurrentUser(userForControl))
                return Forbid();

            var validationResult = await ValidateLocationAsync(userPutDto.CountryId, userPutDto.CityId);
            if (validationResult != null)
                return validationResult;

            var userForUsernameControl = await _userService.GetUserByUsernameAsync(userPutDto.Username);
            if (userForUsernameControl != null && userForUsernameControl.Id != id)
                return Conflict($"A user with the same Username:'{userPutDto.Username}' already exists.");

            var userForEmailControl = await _userService.GetUserByEmailAsync(userPutDto.Email);
            if (userForEmailControl != null && userForEmailControl.Id != id)
                return Conflict($"A user with the same Email:'{userPutDto.Email}' already exists.");

            var user = new User
            {
                UserID = id,
                IdentityUserId = userForControl.IdentityUserId,
                Username = userPutDto.Username,
                Email = userPutDto.Email,
                PhoneNumber = userPutDto.PhoneNumber,
                Age = userPutDto.Age,
                CountryId = userPutDto.CountryId,
                CityId = userPutDto.CityId,
                Address = userPutDto.Address,
                Budget = userPutDto.Budget,
                IsPremium = userPutDto.IsPremium
            };

            var updatedUser = await _userService.UpdateUserAsync(user);
            return Ok(await _userService.GetUserByIdAsync(updatedUser.UserID));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound($"The user with ID {id} was not found.");

            if (!IsCurrentUser(user))
                return Forbid();

            await _userService.DeleteUserAsync(id);
            return Ok($"The user {user.Username} (ID:{id}) has been deleted.");
        }

        private async Task<IActionResult?> ValidateLocationAsync(int countryId, int cityId)
        {
            var country = await _countryService.GetCountryByIdAsync(countryId);
            if (country == null)
                return BadRequest($"The country with ID {countryId} was not found.");

            var city = await _cityService.GetCityByIdAsync(cityId);
            if (city == null)
                return BadRequest($"The city with ID {cityId} was not found.");

            if (city.CountryId != countryId)
                return BadRequest($"The city with ID {cityId} does not belong to the country with ID {countryId}.");

            return null;
        }

        private bool IsCurrentUser(UserDto user)
        {
            var identityUserId = GetCurrentIdentityUserId();
            return !string.IsNullOrWhiteSpace(identityUserId) && user.IdentityUserId == identityUserId;
        }

        private string? GetCurrentIdentityUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
