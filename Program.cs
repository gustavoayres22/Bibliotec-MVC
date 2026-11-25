using BibliotecMVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;   

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexaoPadrao")
    )
);

// Configura a autenticação utilizando coockies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityCookies(options =>
    {

        //Página para qual o usuario que será direcionado caso não esteja autenticado
        options.LoginPath = "/Login/Index";

        //Página para qualo usuário será direcionado caso nça esteja permissão
        options.AcessDeniedPath = "/Login/AcessoNegado";

        //Define o tempo de validde de auntenticação
        //Inatividade
        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        //Remova a validade da autenticação conforme a atividade do usuário
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

//A ordem é importate => primeiro identifica o usuário, depois ve se ele tem permissão

//Identifica o usuáio do cookie de autenticação
app.UseAuthentication();

//Verifica se o usuário possuipara acesar o recurso solicitado
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
