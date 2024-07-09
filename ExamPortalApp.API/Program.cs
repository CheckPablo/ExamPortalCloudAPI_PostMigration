using ExamPortalApp.Contracts.Data.Dtos.Custom;
using ExamPortalApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//string localIP = LocalIPAddress();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddOptions<ExamPortalSettings>().BindConfiguration("ExamPortalSettings");
builder.Services.AddDbContext<ExamPortalDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ExamPortalDatabaseContext>();
builder.Services.AddCustomServices();
builder.Services.AddMapper();
builder.Services.AddCors();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var key = builder.Configuration["ExamPortalSettings:Key"];
    var issuer = builder.Configuration["ExamPortalSettings:Issuer"];
    var audience = builder.Configuration["ExamPortalSettings:Issuer"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{

    options.AddPolicy(name:"Policy1",
       builder =>
        {
           /*policy.WithOrigins("http://127.0.0.1:5066/",
                              "http://127.0.0.1:5066/")
                                .AllowAnyHeader()
                               .AllowAnyMethod();*/
            builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                .AllowAnyHeader().AllowAnyMethod();

        });

    options.AddPolicy("MyCorsPolicy",
       builder => builder
          .SetIsOriginAllowedToAllowWildcardSubdomains()
          .WithOrigins("https://*.", "http://*.")
          .AllowAnyMethod()
          .AllowCredentials()
          .AllowAnyHeader()
          .Build()
       );
    options.AddPolicy(name: MyAllowSpecificOrigins,
      builder => builder
         .SetIsOriginAllowedToAllowWildcardSubdomains()
         .WithOrigins("http://192.168.151.215:4200/", "http://*.")
         .AllowAnyMethod()
         .AllowCredentials()
         .AllowAnyHeader()
         .Build()
      );

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache(); 

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
//app.UseCors();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
#region Use Static Files
try
{

    if (!app.Environment.IsDevelopment())
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
         /* ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                "Origin, X-Requested-With, Content-Type, Accept"); */
                  ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
                "*, X-Requested-With, Content-Type, Accept");
        },


    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
        RequestPath = "/Uploads"
    });
/*     
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "wwwroot";

        if (env.IsDevelopment())
        {
            spa.UseAngularCliServer(npmScript: "start");
        }
    }); */
    }


catch (Exception ex)
{
    Console.WriteLine(ex.Message + ex.StackTrace);
}


#endregion

app.UseAuthentication();
app.UseAuthorization(); 
app.MapControllers();
app.MapControllerRoute(
               name: "default",
               pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");
//app.UseSession();
app.Run();

/*static string LocalIPAddress()
{
    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
    {
        socket.Connect("192.168.0.160",7066);
        IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
        if (endPoint != null)
        {
            return endPoint.Address.ToString();
        }
        else
        {
            return "127.0.0.1";
        }
    }
}*/
