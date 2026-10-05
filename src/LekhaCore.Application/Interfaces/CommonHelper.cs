namespace LekhaCore.Application.Common;

public static class CommonHelper
{
    public static string GenerateApimsTransactionId(string apiCode) => $"{apiCode}-{DateTime.Now:yyyyMMddHHmmssfff}";
}
