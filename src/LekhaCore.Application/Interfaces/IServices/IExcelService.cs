namespace LekhaCore.Application.Interfaces.IService;

public interface IExcelService
{
    Task<byte[]> ExportAssetsAsync();
    Task<byte[]> ExportWorkflowsAsync();
    Task<byte[]> ExportAuditLogsAsync();
}
