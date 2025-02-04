// Importing necessary namespace for ASP.NET Core Identity functionality
using Microsoft.AspNetCore.Identity;

namespace Api.Models
{
    // Defining the AppUser class which extends IdentityUser to add custom properties
    public class AppUser : IdentityUser
    {
        // Adding a custom property "FullName" to store the user's full name
        public string? FullName { get; set; }
    }
}