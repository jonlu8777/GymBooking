using GymBooking.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace GymBooking.Web.Data
{
    public class UserRoleInit
    {

       public static async Task InitAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Member" };

            IdentityResult identityResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {

                    identityResult = await roleManager.CreateAsync(role: new IdentityRole(roleName));
                } 
            }
            var email = "admin@Gymbokning.se";
            var password = "Qwerty123!";

            if(userManager.FindByEmailAsync(email).Result == null) {

                ApplicationUser user = new()
                {
                    Email = email,
                    UserName = email
                };

                IdentityResult result = userManager.CreateAsync(user, password).Result;

                if(result.Succeeded) 
                {
                userManager.AddToRoleAsync(user, role:"Admin").Wait(); 
                
                }


            }


        }

    }
}
