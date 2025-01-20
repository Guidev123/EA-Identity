using System.ComponentModel;

namespace IdentityService.API.Extensions;

public enum ResponseMessages
{
    [Description("Error")]
    ERROR,
    [Description("Success")]
    SUCCESS,
    [Description("Your credentials are wrong")]
    WRONG_CREDENTIALS,
    [Description("Your account is locked")]
    LOCKED_ACCOUNT,
    [Description("You can not delete this user now")]
    CANT_DELETE_USER,
    [Description("User not found")]
    USER_NOT_FOUND,
    [Description("You can not change your password")]
    CANT_CHANGE_PASSWORD,
    [Description("Invalid Refresh Token")]
    INVALID_REFRESH_TOKEN
}
