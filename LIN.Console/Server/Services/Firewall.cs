using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace LIN.Console.Server.Services;

public class Firewall
{

    public static string HttpIPv4(IHttpContextAccessor http)
    {
        // Obtener el contexto HTTP actual
        var httpContext = http.HttpContext;

        if (httpContext == null )
            return "";
        

        // Obtener la dirección IP del cliente
        var ipAddress = httpContext.Connection.RemoteIpAddress;

        // Verificar si la dirección IP es de IPv4 o IPv6
        if (ipAddress != null)
        {
            if (ipAddress.IsIPv4MappedToIPv6)
            {
                ipAddress = ipAddress.MapToIPv4();
            }

            // ipAddress ahora contiene la dirección IP del cliente
            var ipString = ipAddress.ToString();
            return ipString;
        }

        return "";
    }


}

