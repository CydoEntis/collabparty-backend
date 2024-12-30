namespace CollabParty.Application.Common.Errors;

public static class ErrorMessageConstants
{
    public const string DefaultErrorMessage = "An error has occured.";
    public const string EmailInUse = "Email is already in use.";
    public const string UsernameInUse = "Username is already in use.";
    public const string RegistrationFailed = "Registration failed.";
    public const string UserNotFound = "User not found.";
    public const string TokenNotFound = "Token not found.";
    public const string TokenExpired = "Token has expired.";
    public const string SessionTokenNotFound = "Session token not found.";

    public const string OldPassword = "Cannot use a previous password.";
    public const string PasswordResetFailed = "Failed to reset password.";
    public const string CurrentPasswordMismatch = "Password does not match the current password.";
    public const string ChangePasswordFailed = "Password change request has failed.";
    public const string AlreadyExists = "Resource already exsists.";
}

public static class SuccessMessageConstants
{
    public const string LoginSuccessful = "Log in succesful.";
    public const string RegistrationSuccessful = "Registration successful";
    public const string TokenRefreshSuccessful = "Tokens refreshed.";
    public const string LogoutSuccessful = "Logged out successfully.";
    public const string PasswordResetSuccess = "Password has been reset.";

    public const string PasswordResetEmailSent =
        "If an account with that email exists, a password reset link will be sent.";
}

public static class ErrorTypeConstants
{
    public const string ChangePassword = "Change Password Exception";
    public const string AlreadyExists = "Resource Already Exsists";
    public const string FetchException = "Resource Fetch Exception";
    public const string InvalidToken = "Invalid Token";
    public const string NotFoundException = "Not Found Exception";
    public const string PermissionException = "Permission Denied Exception";
    public const string ResourceCreationException = "Resource Creation Exception";
    public const string ResourceModificationException = "Resource Modification Exception";
    public const string ValidationException = "Validation Exception";
}