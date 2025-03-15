using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Muqabalati.Domain.Common.IRepository;
using Muqabalati.Domain.Common.IUnitOfWork;
using Muqabalati.Repository.EntityFrameworkCore.Context;
using Muqabalati.Repository.Repository;
using Muqabalati.Repository.UnitOfWork;
using Muqabalati.Service.Interfaces;
using Muqabalati.Service.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Muqabalati.Domain.Identity;
using Muqabalati.Repository.DbInitializer;
using Muqabalati.Utilities.Identity;
using Muqabalati.Service.Implementations;
using Muqabalati.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection3"));
});


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
{
    option.Password.RequiredLength = 4;
    option.Password.RequireDigit = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


#region AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
#endregion

#region Custom Services

builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();
//builder.Services.Configure<EmailSettings>(builder.Configuration.Get("EmailConfiguration"));
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILog, LogService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<IGenAIApiService, GenAIApiService>();
#endregion

#region Session Management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion

var app = builder.Build();

SeedDatabase();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{Area=Customer}/{controller=Interview}/{action=StartInterview}/{id?}");
    //pattern: "{Area=Customer}/{controller=Interview}/{action=InterviewResult}/{id?}");

app.MapRazorPages();

app.Run();


void SeedDatabase()
{
    using (IServiceScope Scope = app.Services.CreateScope())
    {
        IDbInitializer initializer = Scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        initializer.Initialize();
    }
}
