using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using FaceRecognitionWebAPI.Repository;
using FaceRecognitionWebAPI.Respository;
using FaceRecognitionWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IFaceExpressionRepository, FaceExpressionRepository>();
builder.Services.AddScoped<IFaceToTrainRepository, FaceToTrainRepository>();
builder.Services.AddScoped<IAugmentedFaceRepository, AugmentedFaceRepository>();
builder.Services.AddScoped<IFaceToRecognizeRepository, FaceToRecognizeRepository>();
builder.Services.AddScoped<IFaceRecognitionStatusRepository, FaceRecognitionStatusRepository>();
builder.Services.AddScoped<IFaceRecognitionService, FaceRecognitionService>();
builder.Services.AddScoped<IImageAugmentationService, ImageAugmentationService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Faces To Recognize")),
    RequestPath = "/FacesToRecognize"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Face Dataset")),
    RequestPath = "/FaceDataset"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Face Expression Samples")),
    RequestPath = "/FaceExpression"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Augmented Faces")),
    RequestPath = "/AugmentedFaces"
});

app.UseCors(options => options.WithOrigins("*").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
