using Blaster.Shared.User;
using Microsoft.AspNetCore.Identity;

namespace Blaster.Server.Models
{
    public static class CommonExtensions
    {
        const string _invalid = "Invalid email address or password!";

        public static LoginResultModel Invalid(this LoginResultModel loginResultModel)
        {
            loginResultModel.ErrorMessage = _invalid;

            return loginResultModel;
        }

        public static LoginResultModel MapFromSignInResult(this LoginResultModel loginResultModel, SignInResult signInResult)
        {
            //TODO:Move to resources
            
            const string _locked = "Your account has been locked out!";

            if (loginResultModel == null)
            {
                loginResultModel.ErrorMessage = _invalid;
                return loginResultModel;
            }

            loginResultModel.IsLockedOut = signInResult.IsLockedOut;
            loginResultModel.IsNotAllowed = signInResult.IsNotAllowed;
            loginResultModel.RequiresTwoFactor = signInResult.RequiresTwoFactor;
            loginResultModel.Succeeded = signInResult.Succeeded;

            if (loginResultModel.IsLockedOut)
            {
                loginResultModel.ErrorMessage = _locked;
                
                return loginResultModel;
            }

            if(loginResultModel.IsNotAllowed || loginResultModel.RequiresTwoFactor)
            {
                return loginResultModel;
            }

            if (!loginResultModel.Succeeded)
            {
                loginResultModel.ErrorMessage = _invalid;
            }

            return loginResultModel;
        }

        public static LoginResultModel NeedsEmailConfirmation(this LoginResultModel loginResultModel)
        {
            loginResultModel.IsNotAllowed = true;
            loginResultModel.Succeeded = false;

            return loginResultModel;
        }
    }
}
