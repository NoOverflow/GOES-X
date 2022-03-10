using Amazon.S3;
using GOES_I;
using GOES_I.Logging;
using GOES_X.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IQueryService, QueryService>((provider) =>
    new QueryService(
        new AmazonS3Client(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.USEast1)));
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<UserPreferencesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure logging
Logger.Init();

// Configure services
app.Services.GetService<IQueryService>().Start();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseEndpoints((endpoints) =>
{
    endpoints.MapControllers();
});

app.Run();
