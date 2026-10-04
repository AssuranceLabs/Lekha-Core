using LekhaCore.Application.DTOs.Excel;
using LekhaCore.Domain.Common;

namespace LekhaCore.Application.Interfaces.IService;

public interface IExcelService
{
    Task<byte[]> ExportEmployeesAsync();
    Task<byte[]> ExportAssetsAsync();
    Task<byte[]> ExportWorkflowsAsync();
    Task<byte[]> ExportAuditLogsAsync();
    Task<Result<EmployeeImportResultDto>> ImportEmployeesAsync(Stream fileStream);
}
