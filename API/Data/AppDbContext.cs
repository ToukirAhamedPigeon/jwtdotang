// Importing the required namespaces
using Api.Models; // Importing the custom user model (AppUser)
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Importing IdentityDbContext for Identity management
using Microsoft.EntityFrameworkCore; // Importing Entity Framework Core for database interactions

// Defining the namespace for the data layer
namespace API.Data
{
    // Defining the database context class that inherits from IdentityDbContext
    // This provides Identity functionalities such as user authentication and authorization
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        // Constructor for the AppDbContext class
        // Accepts DbContextOptions and passes them to the base class (IdentityDbContext)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Constructor logic can be added here if necessary
        }
    }
}
