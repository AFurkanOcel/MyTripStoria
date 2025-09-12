using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.CityDtos;
using Contracts.UserDtos;
using Entities;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.UserID,
                    Username = user.Username,
                    Password = user.Password,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Age = user.Age,
                    CountryId = user.CountryId,
                    CityId = user.CityId,
                    Address = user.Address,
                    Budget = user.Budget,
                    IsPremium = user.IsPremium
                });
            }

            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Age = user.Age,
                CountryId = user.CountryId,
                CityId = user.CityId,
                Address = user.Address,
                Budget = user.Budget,
                IsPremium = user.IsPremium
            };
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Age = user.Age,
                CountryId = user.CountryId,
                CityId = user.CityId,
                Address = user.Address,
                Budget = user.Budget,
                IsPremium = user.IsPremium
            };
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.UserID,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Age = user.Age,
                CountryId = user.CountryId,
                CityId = user.CityId,
                Address = user.Address,
                Budget = user.Budget,
                IsPremium = user.IsPremium
            };
        }

        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddAsync(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteAsync(userId);
        }
    }
}

