using Personal_Sitios.Data;
using Personal_Sitios.Filters;
using Personal_Sitios.Helpers;
using Personal_Sitios.Repositories;
using Personal_Sitios.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<BitacoraExceptionFilter>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<BitacoraExceptionFilter>();
});

builder.Services.AddSingleton<DbContext>();

builder.Services.AddScoped<LoginRepository>();
builder.Services.AddScoped<MenuRepository>();
builder.Services.AddScoped<BitacoraRepository>();
builder.Services.AddScoped<RolesRepository>();
builder.Services.AddScoped<PantallasRepository>();
builder.Services.AddScoped<UsuariosRepository>();
builder.Services.AddScoped<PermisosRepository>();

builder.Services.AddScoped<BitacoraService>();
builder.Services.AddScoped<EncryptionHelper>();
builder.Services.AddScoped<PermisoAuthorizeAttribute>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();