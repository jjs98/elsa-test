using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

var services = builder.Services;
var configuration = builder.Configuration;

var connectionString =
    configuration.GetConnectionString("Elsa")
    ?? throw new InvalidOperationException("No connection string named 'Elsa' found.");

services.AddElsa(elsa =>
    elsa.UseIdentity(identity =>
        {
            identity.TokenOptions = options =>
                options.SigningKey = "large-signing-key-for-signing-JWT-tokens";
            identity.UseAdminUserProvider();
        })
        .UseDefaultAuthentication()
        .UseWorkflowManagement(management =>
            management.UseEntityFrameworkCore(ef =>
            {
                ef.UsePostgreSql(connectionString);
                ef.RunMigrations = true;
            })
        )
        .UseWorkflowRuntime(runtime =>
            runtime.UseEntityFrameworkCore(ef =>
            {
                ef.UsePostgreSql(connectionString);
                ef.RunMigrations = true;
            })
        )
        .UseScheduling()
        .UseJavaScript()
        .UseLiquid()
        .UseCSharp()
        .UseHttp(http =>
            http.ConfigureHttpOptions = options => configuration.GetSection("Http").Bind(options)
        )
        .UseWorkflowsApi()
        .AddActivitiesFrom<Program>()
        .AddWorkflowsFrom<Program>()
);

services.AddCors(cors =>
    cors.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*")
    )
);
services.AddRazorPages(options =>
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute())
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi();
app.UseWorkflows();
app.MapFallbackToPage("/_Host");
app.Run();
