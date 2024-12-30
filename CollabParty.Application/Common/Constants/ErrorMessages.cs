namespace CollabParty.Application.Common.Constants;

public static class ErrorMessages
{
    public const string DefaultErrorMessage = "An error has occured.";
    public const string EmailInUse = "Email is already in use.";
    public const string UsernameInUse = "Username is already in use.";
    public const string RegistrationFailed = "Registration failed.";
    public const string UserNotFound = "User not found.";
    public const string TokenNotFound = "Token not found.";
    public const string TokenExpired = "Token has expired.";
    public const string TokenInvalid = "Token is invalid.";
    public const string SessionNotFound = "Session not found.";

    public const string OldPassword = "Cannot use a previous password.";
    public const string PasswordResetFailed = "Failed to reset password.";
    public const string CurrentPasswordMismatch = "Password does not match the current password.";
    public const string ChangePasswordFailed = "Password change request has failed.";
    public const string AlreadyExists = "Resource already exsists.";
    public const string ValidationFailed = "One or more validation errors occurred.";
    public const string InvalidCredentials = "Email or password is incorrect.";
}