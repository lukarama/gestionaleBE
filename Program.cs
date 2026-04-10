using Gestionale.Api.Data;
using Microsoft.EntityFrameworkCore;
using Gestionale.Api.Services;
using Gestionale.Api.Services.Interfaces;
using Gestionale.Api.Common;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddScoped<IVisiteMedicheService, VisiteMedicheService>();
builder.Services.AddScoped<ITipiDocumentoService, TipiDocumentoService>();
builder.Services.AddScoped<IDocumentiDipendentiService, DocumentiDipendentiService>();
builder.Services.AddScoped<IDocumentiMezziService, DocumentiMezziService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();