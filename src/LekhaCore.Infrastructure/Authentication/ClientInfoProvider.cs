using System.Net;
using LekhaCore.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LekhaCore.Infrastructure.Authentication;

public sealed class ClientInfoProvider(IHttpContextAccessor httpContextAccessor) : IClientInfoProvider
{
    public string BrowserInfo => Header("User-Agent");

    public string ClientIpAddress =>
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

    public string ComputerName =>
        httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

    public bool IsLocal
    {
        get
        {
            var address = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress;
            return address is not null && IPAddress.IsLoopback(address);
        }
    }

    public string Uri => httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty;

    public string BaseUri
    {
        get
        {
            var request = httpContextAccessor.HttpContext?.Request;
            if (request is null)
                return string.Empty;

            return $"{request.Scheme}://{request.Host.Value}";
        }
    }

    private string Header(string name) =>
        httpContextAccessor.HttpContext?.Request.Headers[name].ToString() ?? string.Empty;
}
