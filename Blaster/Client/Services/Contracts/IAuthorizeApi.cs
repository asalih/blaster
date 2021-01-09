﻿using Blaster.Shared.User;
using System.Threading.Tasks;

namespace Blaster.Client.Services.Contracts
{
    public interface IAuthorizeApi
    {
        Task Login(LoginModel loginModel);

        Task Register(RegisterModel registerModel);

        Task Logout();

        Task<UserInfo> GetUserInfo();
    }
}
