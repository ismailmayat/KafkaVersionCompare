using KafkaVersionCompare.Services;
using Microsoft.Extensions.Caching.Memory;

//need this to execute page with js to get initial cp versions
Microsoft.Playwright.Program.Main(new[] { "install" });

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddLogging(fs => fs.AddConsole());
builder.Services.AddSingleton<ReleaseParser>();
builder.Services.AddScoped<IReleaseBuilder>(ctr=> new ReleasePageCrawlerBuilder("https://archive.apache.org/dist/kafka/", ctr.GetService<ReleaseParser>(), ctr.GetService<ILoggerFactory>().CreateLogger<ReleasePageCrawlerBuilder>(),ctr.GetService<IMemoryCache>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();