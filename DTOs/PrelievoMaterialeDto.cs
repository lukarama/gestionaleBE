using System;

namespace Gestionale.Api.DTOs;

public class PrelievoMaterialeDto
{
    public int MaterialeId { get; set; }
    public decimal Quantita { get; set; }
    public int? DipendenteId { get; set; }
    public int? CantiereId { get; set; }
    public string? Note { get; set; }
}