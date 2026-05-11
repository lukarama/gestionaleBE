using Gestionale.Api.Common;
using Gestionale.Api.Common.Auth;
using Gestionale.Api.DTOs;

namespace Gestionale.Api.Services.Interfaces;

public interface IIssueReportsService
{
    Task<ServiceResult<List<IssueReportDto>>> GetAllAsync(UserContext user);
    Task<ServiceResult<IssueReportDto>> GetByIdAsync(int id, UserContext user);
    Task<ServiceResult<IssueReportDto>> CreateAsync(CreateIssueReportDto dto, UserContext user);
    Task<ServiceResult<IssueReportDto>> UpdateStatusAsync(int id, UpdateRequestStatusDto dto, UserContext user);
}
