//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
using Octokit;
using Service;
using Microsoft.Extensions.Configuration;
//using Github.API;
using Github.API.CachedServices;
using System.Net.Http.Headers;
using Github.API.CachedServices;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<GitHubIntegrationOptions>(builder.Configuration.GetSection(nameof(GitHubIntegrationOptions)));
builder.Services.AddGitHubIntegration(option => builder.Configuration.GetSection(nameof(GitHubIntegrationOptions)).Bind(option));
// ����� GitHubClient \����� ������
builder.Services.AddScoped<GitHubClient>(provider =>
    new GitHubClient(new Octokit.ProductHeaderValue("my-github-app")));
builder.Services.AddScoped<IGitHubServices, GithubService>();
builder.Services.Decorate<IGitHubServices, CachedServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
