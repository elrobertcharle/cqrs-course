var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";

        // This is a crucial security check.
        options.Audience = "post_query_api"; // <-- IMPORTANT roberto: do the same for all audience
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Only if needed
    });
});

var app = builder.Build();

app.UseCors("AllowNextApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.Run();
