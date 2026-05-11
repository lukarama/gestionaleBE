using System.Text;
using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.Logging;
using Gestionale.Api.Options;
using Gestionale.Api.Repositories;
using Gestionale.Api.Repositories.Interfaces;
using Gestionale.Api.Security;
using Gestionale.Api.Services;
using Gestionale.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendPolicy";

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddSingleton<ILoggerProvider, GdprSafeFileLoggerProvider>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<DocumentiDipendentiOptions>(builder.Configuration.GetSection(DocumentiDipendentiOptions.SectionName));
builder.Services.Configure<CorsOptions>(builder.Configuration.GetSection(CorsOptions.SectionName));
builder.Services.Configure<AppLoggingOptions>(builder.Configuration.GetSection(AppLoggingOptions.SectionName));

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
    string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
    string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException("Configurazione JWT incompleta. Valorizzare Jwt:Issuer, Jwt:Audience e Jwt:Key.");
}

if (!builder.Environment.IsDevelopment() &&
    jwtOptions.Key.StartsWith("CHANGE_THIS", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("Jwt:Key non puo usare il valore placeholder in ambienti non Development.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Security.Jwt");

                logger.LogWarning(
                    context.Exception,
                    "JWT authentication failed. TraceId={TraceId}; Path={Path}",
                    context.HttpContext.TraceIdentifier,
                    context.HttpContext.Request.Path.Value);

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Security.Jwt");

                logger.LogWarning(
                    "JWT challenge returned 401. TraceId={TraceId}; Path={Path}; Error={Error}",
                    context.HttpContext.TraceIdentifier,
                    context.HttpContext.Request.Path.Value,
                    context.Error ?? "none");

                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Security.Authorization");

                logger.LogWarning(
                    "Authorization returned 403. TraceId={TraceId}; Path={Path}",
                    context.HttpContext.TraceIdentifier,
                    context.HttpContext.Request.Path.Value);

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMansioniService, MansioniService>();
builder.Services.AddScoped<IDipendentiService, DipendentiService>();
builder.Services.AddScoped<IMezziService, MezziService>();
builder.Services.AddScoped<ITipologieMezzoService, TipologieMezzoService>();
builder.Services.AddScoped<IFornitoriService, FornitoriService>();
builder.Services.AddScoped<IMaterialiService, MaterialiService>();
builder.Services.AddScoped<ICategorieMaterialeService, CategorieMaterialeService>();
builder.Services.AddScoped<IDpiService, DpiService>();
builder.Services.AddScoped<ICategorieDpiService, CategorieDpiService>();
builder.Services.AddScoped<IAssegnazioniDpiService, AssegnazioniDpiService>();
builder.Services.AddScoped<ICantieriService, CantieriService>();
builder.Services.AddScoped<IAssegnazioniMezziService, AssegnazioniMezziService>();
builder.Services.AddScoped<IAssegnazioniMaterialiService, AssegnazioniMaterialiService>();
builder.Services.AddScoped<IMovimentiMaterialeService, MovimentiMaterialeService>();
builder.Services.AddScoped<ITipiMovimentoMaterialeService, TipiMovimentoMaterialeService>();
builder.Services.AddScoped<IMagazzinoService, MagazzinoService>();
builder.Services.AddScoped<IImportazioniMagazzinoService, ImportazioniMagazzinoService>();
builder.Services.AddScoped<IVisiteMedicheService, VisiteMedicheService>();
builder.Services.AddScoped<ITipiDocumentoService, TipiDocumentoService>();
builder.Services.AddScoped<IDocumentiCantieriService, DocumentiCantieriService>();
builder.Services.AddScoped<IDocumentiDipendentiService, DocumentiDipendentiService>();
builder.Services.AddScoped<IDocumentiDipendentiStorageService, DocumentiDipendentiStorageService>();
builder.Services.AddScoped<IDocumentiMezziService, DocumentiMezziService>();
builder.Services.AddScoped<IStatiAssegnazioneService, StatiAssegnazioneService>();
builder.Services.AddScoped<IAssenzeService, AssenzeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUtentiAdminService, UtentiAdminService>();
builder.Services.AddScoped<IExpenseRequestRepository, ExpenseRequestRepository>();
builder.Services.AddScoped<IMaterialRequestRepository, MaterialRequestRepository>();
builder.Services.AddScoped<IIssueReportRepository, IssueReportRepository>();
builder.Services.AddScoped<IExpenseRequestsService, ExpenseRequestsService>();
builder.Services.AddScoped<IMaterialRequestsService, MaterialRequestsService>();
builder.Services.AddScoped<IIssueReportsService, IssueReportsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        var corsOptions = builder.Configuration
            .GetSection(CorsOptions.SectionName)
            .Get<CorsOptions>() ?? new CorsOptions();

        var allowedOrigins = corsOptions.AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException("Configurazione CORS incompleta. Valorizzare Cors:AllowedOrigins.");
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
