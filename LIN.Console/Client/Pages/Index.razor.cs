namespace LIN.Console.Client.Pages;


public partial class Index
{

    bool IsDevServerRunnig = false;

    /// <summary>
    /// Informacion de desarrollador
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {

        if (firstRender)
        {
            await Access.Developer.Session.LoginWith(LIN.Access.Sesion.Instance.Informacion.ID);
            base.StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);

    }




    public void GoTo(string url)
    {
        nav.NavigateTo(url);
    }


    private void GoToDevices()
    {
        nav.NavigateTo("/devices");
    }


    private void GoToTrans()
    {
        nav.NavigateTo("/transacciones");
    }

    private void GoToIA()
    {
        nav.NavigateTo("/ia/playground");
    }







    private async Task A()
    {

        await JSRuntime.InvokeAsync<object>("ShowDrawer", "drawerProject", "btnClose", "btnClose1");

        base.StateHasChanged();

    }


    private async void LoadStatus()
    {
        var res = await LIN.Access.Developer.Controllers.Server.IsRunning();
        IsDevServerRunnig = res;
        base.StateHasChanged();
    }

    /// <summary>
    /// Metodo de inicio
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        LoadStatus();

        // Obtiene los proyectos
        if (!AreProjectLoaded)
            await LoadProjects();

        // base
        await base.OnInitializedAsync();

    }


    /// <summary>
    /// Carga la lista de proyectos
    /// </summary>
    public async Task LoadProjects()
    {

        AreProjectLoaded = false;

        // Obtiene los dispositivos
        var result = await LIN.Access.Developer.Controllers.Project.ReadAllAsync(LIN.Access.Developer.Session.Instance.Token);


        // Evalua el resultado
        if (result.Response ==Responses.Success)
        {
            AreProjectLoaded = true;
            Proyectos = result.Models;
            base.StateHasChanged();
        }

    }


    public void GoToSupport()
    {
        nav.NavigateTo("/contactos");
    }


    string Saludo(string name)
    {

        DateTime horaActual = DateTime.Now;

        int hora = horaActual.Hour;

        if (hora >= 6 && hora < 12)
            return $"Buenos dias {name}";

        else if (hora >= 12 && hora < 18)
            return $"Buenos tardes {name}";

        else if (hora >= 18 && hora < 24)
            return $"Buenos noches {name}";

        else
            return $"Primero que el sol, Hola {name}";

    }




    async void OnSucces()
    {

        // Vuelve a cargar las llaves
        await LoadProjects();

        // Regresca la UI
        base.StateHasChanged();

    }



    private async Task OpenDrawer()
    {

        await JSRuntime.InvokeAsync<object>("ShowDrawer", "drawerProject", "btnClose", "btnClose1");

        base.StateHasChanged();

    }




    /// <summary>
    /// Genera el perfil de desarrollador
    /// </summary>
    async void CrearProfileSuccess()
    {
        await LoadProjects();
        nav.NavigateTo("/");

    }



    /// <summary>
    /// Genera el perfil de desarrollador
    /// </summary>
    void PerfilDev()
    {
        nav.NavigateTo("/newprofile");
    }









}
