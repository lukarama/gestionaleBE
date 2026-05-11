using Gestionale.Api.Common;
using Gestionale.Api.Data;
using Gestionale.Api.DTOs;
using Gestionale.Api.Models;
using Gestionale.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestionale.Api.Services
{
    public class MezziService : IMezziService
    {
        private const string TipoPossessoNoleggio = "noleggio";
        private const string TipoPossessoProprieta = "proprieta";

        private readonly AppDbContext _context;

        public MezziService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MezzoListDto>> GetAllAsync()
        {
            return await _context.Mezzis
                .Include(m => m.TipologiaMezzo)
                .Include(m => m.Fornitore)
                .OrderBy(m => m.Targa)
                .ThenBy(m => m.Marca)
                .ThenBy(m => m.Modello)
                .Select(m => new MezzoListDto
                {
                    Id = m.Id,
                    Targa = m.Targa,
                    CodiceInterno = m.CodiceInterno,
                    Marca = m.Marca,
                    Modello = m.Modello,
                    AnnoImmatricolazione = m.AnnoImmatricolazione,
                    DataRevisione = m.DataRevisione,
                    DataScadenzaBollo = m.DataScadenzaBollo,
                    DataScadenzaAssicurazione = m.DataScadenzaAssicurazione,
                    DataTagliando = m.DataTagliando,
                    TipologiaMezzoId = m.TipologiaMezzoId,
                    TipologiaMezzo = m.TipologiaMezzo != null ? m.TipologiaMezzo.Nome : null,
                    FornitoreId = m.FornitoreId,
                    Fornitore = m.Fornitore != null ? m.Fornitore.RagioneSociale : null,
                    TipoPossesso = m.TipoPossesso,
                    Attivo = m.Attivo
                })
                .ToListAsync();
        }


public async Task<List<ScadenzaMezzoDashboardDto>> GetDashboardScadenzeNativeAsync(int giorni = 30)
    {
        if (giorni <= 0)
            giorni = 30;

        var oggi = DateOnly.FromDateTime(DateTime.Today);
        var limite = oggi.AddDays(giorni);

        var mezzi = await _context.Mezzis
            .AsNoTracking()
            .Where(m => m.Attivo)
            .OrderBy(m => m.Targa)
            .Select(m => new
            {
                m.Id,
                m.Targa,
                m.Marca,
                m.Modello,
                m.DataRevisione,
                m.DataScadenzaBollo,
                m.DataScadenzaAssicurazione,
                m.DataTagliando
            })
            .ToListAsync();

        var result = new List<ScadenzaMezzoDashboardDto>();

        foreach (var mezzo in mezzi)
        {
            var labelMezzo =
                (string.IsNullOrWhiteSpace(mezzo.Targa) ? "Senza targa" : mezzo.Targa) +
                (!string.IsNullOrWhiteSpace(mezzo.Marca) || !string.IsNullOrWhiteSpace(mezzo.Modello)
                    ? " - " + ((mezzo.Marca ?? "") + " " + (mezzo.Modello ?? "")).Trim()
                    : "");

            AggiungiScadenzaSeValida(result, mezzo.Id, labelMezzo, "Revisione", mezzo.DataRevisione, oggi, limite);
            AggiungiScadenzaSeValida(result, mezzo.Id, labelMezzo, "Bollo", mezzo.DataScadenzaBollo, oggi, limite);
            AggiungiScadenzaSeValida(result, mezzo.Id, labelMezzo, "Assicurazione", mezzo.DataScadenzaAssicurazione, oggi, limite);
            AggiungiScadenzaSeValida(result, mezzo.Id, labelMezzo, "Tagliando", mezzo.DataTagliando, oggi, limite);
        }

        return result
            .OrderBy(x => x.DataScadenza)
            .ThenBy(x => x.Mezzo)
            .ThenBy(x => x.TipoScadenza)
            .ToList();
    }
    public async Task<MezzoDetailDto?> GetByIdAsync(int id)
        {
            return await _context.Mezzis
                .Where(m => m.Id == id)
                .Select(m => new MezzoDetailDto
                {
                    Id = m.Id,
                    Targa = m.Targa,
                    NumeroTelaio = m.NumeroTelaio,
                    CodiceInterno = m.CodiceInterno,
                    TipologiaMezzoId = m.TipologiaMezzoId,
                    Marca = m.Marca,
                    Modello = m.Modello,
                    AnnoImmatricolazione = m.AnnoImmatricolazione,
                    DataImmatricolazione = m.DataImmatricolazione,
                    DataRevisione = m.DataRevisione,
                    DataScadenzaBollo = m.DataScadenzaBollo,
                    DataScadenzaAssicurazione = m.DataScadenzaAssicurazione,
                    DataTagliando = m.DataTagliando,
                    FornitoreId = m.FornitoreId,
                    TipoPossesso = m.TipoPossesso,
                    Attivo = m.Attivo,
                    Note = m.Note
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<MezzoDetailDto>> CreateAsync(CreateMezzoDto dto)
        {
            var validazioneRelazioni = await ValidaRelazioniAsync(dto.TipologiaMezzoId, dto.FornitoreId);
            if (!validazioneRelazioni.Success)
                return ServiceResult<MezzoDetailDto>.Fail(validazioneRelazioni.Message!, validazioneRelazioni.StatusCode ?? 400);

            var validazioneDuplicati = await ValidaDuplicatiAsync(dto.Targa, dto.NumeroTelaio, dto.CodiceInterno);
            if (!validazioneDuplicati.Success)
                return ServiceResult<MezzoDetailDto>.Fail(validazioneDuplicati.Message!, validazioneDuplicati.StatusCode ?? 400);

            var tipoPossessoNormalizzato = NormalizzaTipoPossesso(dto.TipoPossesso);
            if (dto.TipoPossesso is not null && tipoPossessoNormalizzato is null)
                return ServiceResult<MezzoDetailDto>.Fail("Il tipo di possesso deve essere 'noleggio' o 'proprieta'.", 400);

            var mezzo = new Mezzi
            {
                Targa = Pulisci(dto.Targa),
                NumeroTelaio = Pulisci(dto.NumeroTelaio),
                CodiceInterno = Pulisci(dto.CodiceInterno),
                TipologiaMezzoId = dto.TipologiaMezzoId,
                Marca = Pulisci(dto.Marca),
                Modello = Pulisci(dto.Modello),
                AnnoImmatricolazione = dto.AnnoImmatricolazione,
                DataImmatricolazione = dto.DataImmatricolazione,
                DataRevisione = dto.DataRevisione,
                DataScadenzaBollo = dto.DataScadenzaBollo,
                DataScadenzaAssicurazione = dto.DataScadenzaAssicurazione,
                DataTagliando = dto.DataTagliando,
                FornitoreId = dto.FornitoreId,
                TipoPossesso = tipoPossessoNormalizzato,
                Attivo = dto.Attivo,
                Note = Pulisci(dto.Note),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            _context.Mezzis.Add(mezzo);
            await _context.SaveChangesAsync();

            var result = await GetByIdAsync(mezzo.Id);
            return ServiceResult<MezzoDetailDto>.Created(result!);
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateMezzoDto dto)
        {
            var mezzo = await _context.Mezzis.FirstOrDefaultAsync(m => m.Id == id);

            if (mezzo == null)
                return ServiceResult<bool>.Fail("Mezzo non trovato.", 404);

            var validazioneRelazioni = await ValidaRelazioniAsync(dto.TipologiaMezzoId, dto.FornitoreId);
            if (!validazioneRelazioni.Success)
                return ServiceResult<bool>.Fail(validazioneRelazioni.Message!, validazioneRelazioni.StatusCode ?? 400);

            var validazioneDuplicati = await ValidaDuplicatiAsync(dto.Targa, dto.NumeroTelaio, dto.CodiceInterno, id);
            if (!validazioneDuplicati.Success)
                return ServiceResult<bool>.Fail(validazioneDuplicati.Message!, validazioneDuplicati.StatusCode ?? 400);

            var tipoPossessoNormalizzato = NormalizzaTipoPossesso(dto.TipoPossesso);
            if (dto.TipoPossesso is not null && tipoPossessoNormalizzato is null)
                return ServiceResult<bool>.Fail("Il tipo di possesso deve essere 'noleggio' o 'proprieta'.", 400);

            mezzo.Targa = Pulisci(dto.Targa);
            mezzo.NumeroTelaio = Pulisci(dto.NumeroTelaio);
            mezzo.CodiceInterno = Pulisci(dto.CodiceInterno);
            mezzo.TipologiaMezzoId = dto.TipologiaMezzoId;
            mezzo.Marca = Pulisci(dto.Marca);
            mezzo.Modello = Pulisci(dto.Modello);
            mezzo.AnnoImmatricolazione = dto.AnnoImmatricolazione;
            mezzo.DataImmatricolazione = dto.DataImmatricolazione;
            mezzo.DataRevisione = dto.DataRevisione;
            mezzo.DataScadenzaBollo = dto.DataScadenzaBollo;
            mezzo.DataScadenzaAssicurazione = dto.DataScadenzaAssicurazione;
            mezzo.DataTagliando = dto.DataTagliando;
            mezzo.FornitoreId = dto.FornitoreId;
            mezzo.TipoPossesso = tipoPossessoNormalizzato;
            mezzo.Attivo = dto.Attivo;
            mezzo.Note = Pulisci(dto.Note);
            mezzo.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var mezzo = await _context.Mezzis.FirstOrDefaultAsync(m => m.Id == id);

            if (mezzo == null)
                return ServiceResult<bool>.Fail("Mezzo non trovato.", 404);

            _context.Mezzis.Remove(mezzo);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ServiceResult<bool>.Fail("Non puoi eliminare il mezzo perché ha record collegati.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaRelazioniAsync(int? tipologiaMezzoId, int? fornitoreId)
        {
            if (tipologiaMezzoId.HasValue)
            {
                var tipologiaEsiste = await _context.TipologieMezzos
                    .AnyAsync(t => t.Id == tipologiaMezzoId.Value);

                if (!tipologiaEsiste)
                    return ServiceResult<bool>.Fail("La tipologia mezzo selezionata non esiste.", 400);
            }

            if (fornitoreId.HasValue)
            {
                var fornitoreEsiste = await _context.Fornitoris
                    .AnyAsync(f => f.Id == fornitoreId.Value);

                if (!fornitoreEsiste)
                    return ServiceResult<bool>.Fail("Il fornitore selezionato non esiste.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidaDuplicatiAsync(
            string? targa,
            string? numeroTelaio,
            string? codiceInterno,
            int? excludeId = null)
        {
            var targaPulita = Pulisci(targa);
            var numeroTelaioPulito = Pulisci(numeroTelaio);
            var codiceInternoPulito = Pulisci(codiceInterno);

            if (!string.IsNullOrWhiteSpace(targaPulita))
            {
                var targaEsiste = await _context.Mezzis.AnyAsync(m =>
                    m.Targa == targaPulita &&
                    (!excludeId.HasValue || m.Id != excludeId.Value));

                if (targaEsiste)
                    return ServiceResult<bool>.Fail("Esiste già un mezzo con questa targa.", 400);
            }

            if (!string.IsNullOrWhiteSpace(numeroTelaioPulito))
            {
                var telaioEsiste = await _context.Mezzis.AnyAsync(m =>
                    m.NumeroTelaio == numeroTelaioPulito &&
                    (!excludeId.HasValue || m.Id != excludeId.Value));

                if (telaioEsiste)
                    return ServiceResult<bool>.Fail("Esiste già un mezzo con questo numero telaio.", 400);
            }

            if (!string.IsNullOrWhiteSpace(codiceInternoPulito))
            {
                var codiceEsiste = await _context.Mezzis.AnyAsync(m =>
                    m.CodiceInterno == codiceInternoPulito &&
                    (!excludeId.HasValue || m.Id != excludeId.Value));

                if (codiceEsiste)
                    return ServiceResult<bool>.Fail("Esiste già un mezzo con questo codice interno.", 400);
            }

            return ServiceResult<bool>.Ok(true);
        }

        private static string? Pulisci(string? valore)
        {
            return string.IsNullOrWhiteSpace(valore) ? null : valore.Trim();
        }

        private static string? NormalizzaTipoPossesso(string? valore)
        {
            var valorePulito = Pulisci(valore);
            if (valorePulito is null)
                return null;

            return valorePulito.ToLowerInvariant() switch
            {
                TipoPossessoNoleggio => TipoPossessoNoleggio,
                "proprieta" => TipoPossessoProprieta,
                "proprietà" => TipoPossessoProprieta,
                _ => null
            };
        }

        private static void AggiungiScadenzaSeValida(
    List<ScadenzaMezzoDashboardDto> lista,
    int mezzoId,
    string mezzo,
    string tipoScadenza,
    DateOnly? dataScadenza,
    DateOnly oggi,
    DateOnly limite)
        {
            if (!dataScadenza.HasValue)
                return;

            if (dataScadenza.Value > limite)
                return;

            lista.Add(new ScadenzaMezzoDashboardDto
            {
                MezzoId = mezzoId,
                Mezzo = mezzo,
                TipoScadenza = tipoScadenza,
                DataScadenza = dataScadenza.Value,
                GiorniAllaScadenza = dataScadenza.Value.DayNumber - oggi.DayNumber,
                Scaduta = dataScadenza.Value < oggi,
                InScadenza = dataScadenza.Value >= oggi && dataScadenza.Value <= limite
            });
        }
    }
}
