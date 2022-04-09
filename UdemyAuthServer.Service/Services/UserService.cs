using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.DTOs;
using UdemyAuthServer.Core.Model;
using UdemyAuthServer.Core.Services;

namespace UdemyAuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp{ Email = createUserDto.Email, UserName = createUserDto.UserName}; 
            var result = await _userManager.CreateAsync(user,createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<UserAppDto>.Failed(new ErrorDto(errors,true),400);
            }
            return Response<UserAppDto>.Succes(ObjectMapper.mapper.Map<UserAppDto>(user), 200);

        }

        public async Task<Response<UserAppDto>> GetUserByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Response<UserAppDto>.Failed("username not found",404,true);
            }
            return Response<UserAppDto>.Succes(ObjectMapper.mapper.Map<UserAppDto>(user), 200);
        }
    }
}
