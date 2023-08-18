namespace LIN.Console.Client.Online;

public class StaticHub
{

    /// <summary>
    /// Llave del dispositivo
    /// </summary>
    public static string Key;



    /// <summary>
    /// Hub de conexion
    /// </summary>
    public static LIN.Access.Auth.Hubs.AccountHub? Hub = null;



    /// <summary>
    /// Obtiene el ID del HUB
    /// </summary>
    public static string? HubID => Hub?.ID;



    /// <summary>
    /// Crea el HUB
    /// </summary>
    public static void LoadHub()
    {

        if (Hub == null)
        {

            // Arma el modelo
            var model = new DeviceModel()
            {
                Name = "Web",
                Cuenta = Access.Developer.Session.Instance.Account.ID,
                Modelo = "Dispositivo web",
                BateryConected = MainLayout.IsChargin,
                BateryLevel = MainLayout.LevelBattery,
                Manufacter = "WEB",
                OsVersion = "Navegador",
                Platform = Platforms.Web,
                App = Applications.CloudConsole,
                DeviceKey = Key,
                Token = Access.Developer.Session.Instance.AccountToken
            };

            Hub = new(model);

        }

        Hub.OnReceivingCommand += Hub_OnReceiveCommand;
    }



    /// <summary>
    /// Recibe un comando desde el hub de conexion
    /// </summary>
    private static void Hub_OnReceiveCommand(object? sender, string e)
    {
        try
        {
            var builder = new SILF.Script.Builder(e);
            builder.Replace(Scripts.Actions);
            builder.Build();
            var app = builder.CreateApp();
            app.Run();
        }
        catch
        {
        }
    }



    /// <summary>
    /// Cierra la conexion con el hub
    /// </summary>
    public static async void Leave()
    {
        if (Hub != null)
        {
            await Hub.CloseSesion();
            Hub = null;
        }
    }



    /// <summary>
    /// Envia un comando
    /// </summary>
    /// <param name="comando"></param>
    public static async void NotificateAccount(string comando)
    {

        if (Hub != null)
        {
            Hub.SendCommand(LIN.Access.Developer.Session.Instance.Informacion.ID, comando);
        }

    }


}