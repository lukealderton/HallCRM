using CRM.Contracts.Results;
using CRM.Core.Users.Abstraction.Repositories;
using CRM.Core.Users.Domain;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CRM.Infrastructure.Users.Services
{
    public sealed class UserIdentityWriter : IUserIdentityWriter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserIdentityWriter(
            UserManager<ApplicationUser> objUserManager)
        {
            _userManager = objUserManager;
        }

        public async Task<BasicResult> UpdateUserAsync(
            UpdateUserRequest objRequest,
            CancellationToken objToken = default)
        {
            BasicResult objResult = new();

            if (objRequest == null)
            {
                objResult.Message = "User details are required.";
                return objResult;
            }

            if (objRequest.Id == Guid.Empty)
            {
                objResult.Message = "A valid user id is required.";
                return objResult;
            }

            ApplicationUser? objUser =
                await _userManager.FindByIdAsync(
                    objRequest.Id.ToString());

            if (objUser == null)
            {
                objResult.Message = "User not found.";
                return objResult;
            }

            objUser.Forename = CleanString(objRequest.Forename);
            objUser.Surname = CleanString(objRequest.Surname);
            //objUser.AccessLevel = objRequest.AccessLevel;
            objUser.Enabled = objRequest.Enabled;
            objUser.UpdatedUtc = DateTime.UtcNow;

            if (!String.IsNullOrWhiteSpace(objRequest.Email) &&
                !String.Equals(
                    objUser.Email,
                    objRequest.Email,
                    StringComparison.OrdinalIgnoreCase))
            {
                String strEmail = objRequest.Email.Trim();

                IdentityResult objEmailResult =
                    await _userManager.SetEmailAsync(
                        objUser,
                        strEmail);

                if (!objEmailResult.Succeeded)
                {
                    return FromIdentityResult(
                        objEmailResult,
                        "Failed to update the email address.");
                }

                IdentityResult objUsernameResult =
                    await _userManager.SetUserNameAsync(
                        objUser,
                        strEmail);

                if (!objUsernameResult.Succeeded)
                {
                    return FromIdentityResult(
                        objUsernameResult,
                        "Failed to update the username.");
                }
            }

            IdentityResult objUpdateResult =
                await _userManager.UpdateAsync(objUser);

            if (!objUpdateResult.Succeeded)
            {
                return FromIdentityResult(
                    objUpdateResult,
                    "Failed to update the user.");
            }

            if (!String.IsNullOrWhiteSpace(
                    objRequest.NewPassword))
            {
                String strToken =
                    await _userManager
                        .GeneratePasswordResetTokenAsync(objUser);

                IdentityResult objPasswordResult =
                    await _userManager.ResetPasswordAsync(
                        objUser,
                        strToken,
                        objRequest.NewPassword);

                if (!objPasswordResult.Succeeded)
                {
                    return FromIdentityResult(
                        objPasswordResult,
                        "The user was updated, but the password could not be changed.");
                }
            }

            objResult.Success = true;
            objResult.Message = "User updated successfully.";

            return objResult;
        }

        //public async Task<BasicResult> SetAccessLevelAsync(
        //    Guid objUserId,
        //    UserAccessLevel enmAccessLevel,
        //    CancellationToken objToken = default)
        //{
        //    BasicResult objResult = new();

        //    if (objUserId == Guid.Empty)
        //    {
        //        objResult.Message = "A valid user id is required.";
        //        return objResult;
        //    }

        //    ApplicationUser? objUser =
        //        await _userManager.FindByIdAsync(
        //            objUserId.ToString());

        //    if (objUser == null)
        //    {
        //        objResult.Message = "User not found.";
        //        return objResult;
        //    }

        //    objUser.AccessLevel = enmAccessLevel;
        //    objUser.UpdateDate = DateTime.UtcNow;

        //    IdentityResult objUpdateResult =
        //        await _userManager.UpdateAsync(objUser);

        //    return FromIdentityResult(
        //        objUpdateResult,
        //        objUpdateResult.Succeeded
        //            ? "User access level updated successfully."
        //            : "Failed to update the user access level.");
        //}

        public async Task<BasicResult> AddOrUpdateClaimAsync(
            Guid objUserId,
            String strKey,
            String strValue,
            CancellationToken objToken = default)
        {
            BasicResult objResult = new();

            if (objUserId == Guid.Empty)
            {
                objResult.Message = "A valid user id is required.";
                return objResult;
            }

            if (String.IsNullOrWhiteSpace(strKey))
            {
                objResult.Message = "A claim key is required.";
                return objResult;
            }

            ApplicationUser? objUser =
                await _userManager.FindByIdAsync(
                    objUserId.ToString());

            if (objUser == null)
            {
                objResult.Message = "User not found.";
                return objResult;
            }

            String strClaimKey = strKey.Trim();
            String strClaimValue =
                strValue?.Trim() ?? String.Empty;

            IList<Claim> colClaims =
                await _userManager.GetClaimsAsync(objUser);

            Claim? objExistingClaim =
                colClaims.FirstOrDefault(
                    x => x.Type == strClaimKey);

            IdentityResult objIdentityResult;

            if (objExistingClaim == null)
            {
                objIdentityResult =
                    await _userManager.AddClaimAsync(
                        objUser,
                        new Claim(
                            strClaimKey,
                            strClaimValue));
            }
            else
            {
                objIdentityResult =
                    await _userManager.ReplaceClaimAsync(
                        objUser,
                        objExistingClaim,
                        new Claim(
                            strClaimKey,
                            strClaimValue));
            }

            return FromIdentityResult(
                objIdentityResult,
                objIdentityResult.Succeeded
                    ? "Claim updated successfully."
                    : "Failed to update the claim.");
        }

        public async Task<BasicResult<String>>
            GenerateEmailConfirmationTokenAsync(
                Guid objUserId,
                CancellationToken objToken = default)
        {
            BasicResult<String> objResult = new();

            if (objUserId == Guid.Empty)
            {
                objResult.Message = "A valid user id is required.";
                return objResult;
            }

            ApplicationUser? objUser =
                await _userManager.FindByIdAsync(
                    objUserId.ToString());

            if (objUser == null)
            {
                objResult.Message = "User not found.";
                return objResult;
            }

            String strToken =
                await _userManager
                    .GenerateEmailConfirmationTokenAsync(
                        objUser);

            objResult.Success = true;
            objResult.Message =
                "Email confirmation token generated.";

            objResult.Result = strToken;

            return objResult;
        }

        private static BasicResult FromIdentityResult(
            IdentityResult objIdentityResult,
            String strMessage)
        {
            BasicResult objResult = new()
            {
                Success = objIdentityResult.Succeeded,
                Message = strMessage
            };

            if (!objIdentityResult.Succeeded)
            {
                objResult.AltMessage = String.Join(
                    Environment.NewLine,
                    objIdentityResult.Errors.Select(
                        x => x.Description));
            }

            return objResult;
        }

        private static String? CleanString(
            String? strValue)
        {
            return String.IsNullOrWhiteSpace(strValue)
                ? null
                : strValue.Trim();
        }
    }
}
