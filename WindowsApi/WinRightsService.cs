using System.Security.Principal;
using Microsoft.Extensions.Logging;
using WindowsApi.Abstractions;

namespace WindowsApi;

public class WinRightsService(ILogger<WinRightsService> logger) : IWinRightsService
{
    public bool IsProcessAdmin()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            var isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            return isAdmin;
        }
        catch (Exception exception)
        {
            logger.LogError("Error trying to determine process rights. Exception: {message}, InnerException: {innerMessage}", 
                exception.Message, exception.InnerException?.Message);
            return false;
        }
    }
}