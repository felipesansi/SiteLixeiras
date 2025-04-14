using Microsoft.AspNetCore.Identity;

namespace SiteLixeiras.Sevices
{
    public class SeedUserRolesInitial : ISeedUserRolesInitial
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRolesInitial(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                };

                await _roleManager.CreateAsync(role);
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var role = new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                };

                await _roleManager.CreateAsync(role);
            }
        }


        public async Task SeedUsersAsync()
        {
            if (await _userManager.FindByEmailAsync("usuario@localhost") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "usuario@localhost",
                    Email = "usuario@localhost",
                    NormalizedUserName = "USUARIO@LOCALHOST",
                    NormalizedEmail = "USUARIO@LOCALHOST",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(user, "123456");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }


            if (await _userManager.FindByEmailAsync("icenalixeiras@gmail.com") == null)
            {
                var admin = new IdentityUser
                {
                    UserName = "icena",
                    Email = "icenalixeiras@gmail.com",
                    NormalizedEmail = "ICENALIXEIRAS@GMAIL.COM",
                    NormalizedUserName = "ICENALIXEIRAS@GMAIL.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                var result = await _userManager.CreateAsync(admin, "Nusey#2025");
                if (result.Succeeded)
                {
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
        }
    }
}

