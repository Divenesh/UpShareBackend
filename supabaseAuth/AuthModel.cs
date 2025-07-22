using System;
using Supabase;
using Supabase.Gotrue;

namespace UpShareBackend.supabaseAuth;

public class EnterNewPasswordViewModel
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class AuthModel
{
    private static readonly string SupabaseUrl = "https://oymbbgtbqpywfngyivur.supabase.co";
    private static readonly string SupabaseKey =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im95bWJiZ3RicXB5d2ZuZ3lpdnVyIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkyMjY2OTksImV4cCI6MjA2NDgwMjY5OX0.RrXHBHwX_4Je1GFTWUhppBt0EZK-n4T4RZ0DsgJc1cI";
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    private readonly Supabase.Client _supabaseClient;

    public AuthModel(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public static Supabase.Client CreateClient()
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
        };

        return new Supabase.Client(SupabaseUrl, SupabaseKey, options);
    }

    public AuthModel()
    {
        _supabaseClient = CreateClient();
    }

    public async Task<Session> SignInAsync(string email, string password)
    {
        try
        {
            var session = await _supabaseClient.Auth.SignIn(email, password);
            Console.WriteLine($"User signed in: {session?.User?.Email ?? "Unknown"}");
            Console.WriteLine(
                $"Session: {_supabaseClient.Auth.CurrentSession?.AccessToken ?? "No token"}"
            );
            return session ?? throw new Exception("Failed to sign in - null session returned");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignIn error: {ex.Message}");
            throw;
        }
    }

    public async Task<Session> SignUpAsync(string email, string password)
    {
        try
        {
            var response = await _supabaseClient.Auth.SignUp(email, password);
            return response ?? throw new Exception("Failed to sign up - null response returned");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignUp error: {ex.Message}");
            throw;
        }
    }

    // Handle sign out
    public async Task SignOutAsync()
    {
        try
        {
            await _supabaseClient.Auth.SignOut();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignOut error: {ex.Message}");
            throw;
        }
    }

    public async Task ResendConfirmationEmail(string email)
    {
        try
        {
            await _supabaseClient.Auth.SendMagicLink(email);
            Console.WriteLine($"Confirmation email resent to: {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resending confirmation email: {ex.Message}");
            throw;
        }
    }

    public async Task SendForgetPassword(string email)
    {
        try
        {
            await _supabaseClient.Auth.ResetPasswordForEmail(email);
            Console.WriteLine($"Forget Password sent to : {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending forget password link: {ex.Message}");
            throw;
        }
    }

    public async Task<Session> UpdateUserPassword(EnterNewPasswordViewModel model)
    {
        try
        {
            await _supabaseClient.Auth.SetSession(model.Token, model.Token);

            await _supabaseClient.Auth.Update(
                new Supabase.Gotrue.UserAttributes { Password = model.NewPassword }
            );
            Console.WriteLine("Password updated successfully.");
            return _supabaseClient.Auth.CurrentSession
                ?? throw new Exception("No session available after password update");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating password: {ex.Message}");
            throw;
        }
    }

    public async Task<string> UpdateUserProfile(IFormFile file, string userId)
    {
        try
        {
            var _supabaseClient = CreateClient();
            await _supabaseClient.InitializeAsync();

            var bucketName = "upshare-user-items";
            var publicFolder = "public";

            using var stream = file.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            string uniqueFileName =
                $"{publicFolder}/{userId}/profilePictures/{Guid.NewGuid()}_{file.FileName}";

            string fileUrl;

            if (_supabaseClient.Storage.From(bucketName) != null)
            {
                var uploadResponse = await _supabaseClient
                    .Storage.From(bucketName)
                    .Upload(
                        fileBytes,
                        uniqueFileName,
                        new Supabase.Storage.FileOptions
                        {
                            ContentType = file.ContentType,
                            Upsert = true,
                        }
                    );
                fileUrl = _supabaseClient.Storage.From(bucketName).GetPublicUrl(uniqueFileName);
                return fileUrl;
            }
            else
            {
                Console.WriteLine("No authenticated session found, using unauthenticated upload.");
                throw new UnauthorizedAccessException(
                    "User must be authenticated to upload files."
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating user profile: {ex.Message}");
            throw;
        }
    }
}
