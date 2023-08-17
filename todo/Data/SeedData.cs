using Microsoft.AspNetCore.Identity;

using todo.Enums;
using todo.Models;

namespace todo.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Tạo các vai trò nếu chưa tồn tại
            await CreateRolesAndAdminAsync(roleManager, userManager);
        }

        private static async Task CreateRolesAndAdminAsync(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.admin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.admin.ToString()));
            }

            if (!await roleManager.RoleExistsAsync(Roles.user.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.user.ToString()));
            }
            var admin = await userManager.FindByEmailAsync("admin@gmail.com");
            if (admin == null)
            {
                var identityUser = new User()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Fullname = "admin"
                };
                await userManager.CreateAsync(identityUser, "12345678");
            }
        }
    }
}
