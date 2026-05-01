using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

class Program
{
    static void Main()
    {
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwMTlkNzEzNS0xZjhjLTc4NmMtYjVmMC1kNDBlODZjZWU4MTciLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBnZWFyYm94LmNvbSIsInVzZXJuYW1lIjoiYWRtaW4iLCJyb2xlIjoiQWRtaW4iLCJleHAiOjE3Nzc2MjQzNDUsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI4OSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTI4OSJ9.w2Vh5ueH5BlZU1nYiq-39j4QnG9mCHRbYXV-fVVVXAM";
        var key = Encoding.ASCII.GetBytes("9IYwYEkdYdynYyTFqM3z0yWd7gEpNrpDVMSJVVpnI00");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = "http://localhost:5289",
                ValidAudience = "http://localhost:5289",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out var validatedToken);
            Console.WriteLine("Token is valid!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Validation failed: " + ex.Message);
        }
    }
}
