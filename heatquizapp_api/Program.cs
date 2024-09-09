using HeatQuizAPI.Extensions;
using HeatQuizAPI.Mapping;
using HeatQuizAPI.Models.BaseModels;
using HeatQuizAPI.Services;
using heatquizapp_api.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureDatabaseContext();
builder.Services.ConfigureAuthenticationAndAuthorization();

builder.Services.AddScoped<UserManager<User>>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files")));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.ConfigureStatisticsStartDateStorage();

//Seed database -- only once
//builder.Services.AddScoped<ISeedDatabase, SeedDatabase>();

builder.Services.AddControllers()
//Setup JSON serializer
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();

});

//------------------//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //HTTPS redirection in a production environment
    app.UseHttpsRedirection();

    app.UseHsts();
}

//Exception handling
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files")),
    RequestPath = "/Files"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files")),
    RequestPath = "/Files"
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

//Generate token @login 
app.UseMiddleware<TokenProviderMiddleware>();

app.MapControllers();

//Datapool accessibility placed after authorization and authentication
app.UseMiddleware<DatapoolAccessibilityMiddleware>();


//Seed database -- only once
/*var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
 
using (var scope = scopedFactory.CreateScope())
{
    var service = scope.ServiceProvider.GetService<ISeedDatabase>();
    
    service.SeedRolesAndFirstUsers();
    service.SeedDatapools();
    service.SeedLevelsOfDifficulty();

}*/

app.Run();
