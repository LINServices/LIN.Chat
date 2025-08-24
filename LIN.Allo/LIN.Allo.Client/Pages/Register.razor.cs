using LIN.Types.Cloud.Identity.Models.Identities;

namespace LIN.Allo.Client.Pages;


public partial class Register
{

    /// <summary>
    /// Usuario.
    /// </summary>
    private string User { get; set; } = string.Empty;


    /// <summary>
    /// Nombre.
    /// </summary>
    private string Name { get; set; } = string.Empty;


    /// <summary>
    /// Contraseña.
    /// </summary>
    private string Password { get; set; } = string.Empty;


    /// <summary>
    /// Error es visible.
    /// </summary>
    bool ErrorVisible { get; set; }


    /// <summary>
    /// Mensaje de error.
    /// </summary>
    string ErrorMessage { get; set; } = string.Empty;


    /// <summary>
    /// Sección actual.
    /// </summary>
    private int Section { get; set; } = 0;



    /// <summary>
    /// Oculta los errores
    /// </summary>
    void HideError()
    {
        ErrorVisible = false;
        StateHasChanged();
    }


    /// <summary>
    /// Muestra un mensaje
    /// </summary>
    async Task ShowError(string message)
    {
        ErrorVisible = true;
        ErrorMessage = message;
        Section = 2;
        StateHasChanged();
        await Task.Delay(3000);
        Section = 0;
        StateHasChanged();
    }





    /// <summary>
    /// Crear cuenta.
    /// </summary>
    async void Start()
    {

        Section = 3;
        StateHasChanged();

        if (User.Length <= 0 || Password.Length <= 0 || Name.Length <= 0)
        {
            await ShowError("Completa todos los campos");
            return;
        }

        if (Password.Length < 4)
        {
            await ShowError("La contraseña debe tener mas de 4 dígitos");
            return;
        }

        // Model
        AccountModel modelo = new()
        {
            Name = Name,
            Identity = new()
            {
                Unique = User
            },
            Password = Password
        };

        // Creacion
        var res = await LIN.Access.Auth.Controllers.Account.Create(modelo);


        // Respuesta
        switch (res.Response)
        {

            case Responses.Success:
                // Guardar.
                Section = 1;
                StateHasChanged();
                break;

            case Responses.NotConnection:
                await ShowError("Error de conexión");
                return;

            case Responses.ExistAccount:
                await ShowError($"Ya existe un usuario con el nombre '{User}'");
                return;

            default:
                await ShowError($"Hubo un error grave");
                return;
        }


        var x = Task.Delay(3000);

        var (_, Response) = await Access.Communication.Session.LoginWith(res.Token);

        await x;

        if (Response == Responses.Success)
        {
            nav.NavigateTo("/");
        }
        else
        {
            await ShowError($"Su cuenta fue creada, pero hubo un error al iniciar sesión");
            nav.NavigateTo("/");
            return;
        }




    }

}