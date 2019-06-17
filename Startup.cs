using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
                                                     .AddDefaultTokenProviders()
                                                     .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Livros}/{action=Index}/{id?}");
            });

            CreateRoles(serviceProvider).Wait();
            CreateAdmin(serviceProvider).Wait();
            SeedLivros(serviceProvider).Wait();
        }
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] rolesNames = { "Admin", "Client" };

            foreach (var namesRole in rolesNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(namesRole);
                if (!roleExist)
                    roleManager.CreateAsync(new IdentityRole(namesRole)).Wait();
            }
        }

        private async Task CreateAdmin(IServiceProvider serviceProvider)
        {
            string EmailAdmin = "admin@admin.com";
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminExist = await userManager.FindByEmailAsync(EmailAdmin);
            if (adminExist == null)
            {
                var user = new ApplicationUser { UserName = EmailAdmin, Email = EmailAdmin, Nome = "Administrador", Sobrenome = "" };
                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }
        private async Task SeedLivros(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (dbContext.Livro.Count() == 0)
            {
                IEnumerable<Livro> seedLivros = new List<Livro>() {
                    new Livro(){ Nome = "Romeu e Julieta", Autor = "Shakespeare",Ano = 1591},
                    new Livro(){ Nome = "Dom Quixote", Autor = "Miguel de Cervantes", Ano = 1605},
                    new Livro(){ Nome = "Guerra e Paz", Autor = "Liev Tolstói", Ano = 1869},
                    new Livro(){ Nome = "A Montanha Mágica", Autor = "Thomas Mann", Ano = 1924},
                    new Livro(){ Nome = "Cem Anos de Solidão", Autor = "Gabriel García Márquez", Ano = 1967},
                    new Livro(){ Nome = "Em Busca do Tempo Perdido", Autor = "Marcel Proust", Ano = 1913},
                    new Livro(){ Nome = "A Divina Comédia", Autor = "Dante Alighieri", Ano = 1321},
                    new Livro(){ Nome = "Orgulho e Preconceito", Autor = "Jane Austen", Ano = 1813},
                    new Livro(){ Nome = "Grande Sertão", Autor = "Veredas, Guimarães Rosa", Ano = 1956},
                    new Livro(){ Nome = "O Castelo", Autor = "Franz Kafka", Ano = 1926},
                };

                dbContext.Livro.AddRange(seedLivros);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
