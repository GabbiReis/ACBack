using AppMobile.AppMobile.Dados;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppMobileContext>(options =>
    options.UseSqlServer(@"Data Source=201.62.57.93,1434;
                            User ID=RA046051;
                            Password=046051;
                            TrustServerCertificate=true"));

//builder.Services.AddDbContext<AppMobileContext>(options =>
//    options.UseSqlServer(@"Data Source=10.107.176.41,1434;
//                            User ID=RA046051;
//                            Password=046051;
//                            TrustServerCertificate=true"));





// Adicione o serviço do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AppMobile", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Habilitar middleware para servir o Swagger gerado como um endpoint JSON
    app.UseSwagger();
    // Habilitar middleware para servir o swagger-ui (HTML, JS, CSS, etc.),
    // especificando o endpoint Swagger JSON
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppMobile V1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
