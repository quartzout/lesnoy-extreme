using System.Security.Principal;
using Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;

namespace Core;

public class WinRightsService(ILogger<WinRightsService> logger) : IWinRightsService
{
    public bool CheckAdmin()
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