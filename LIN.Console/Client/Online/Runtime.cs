using LIN.Console.Client.Modales;

namespace LIN.Console.Client.Online;


/// <summary>
/// Runtime de SILF.Script para LIN Cloud Console
/// </summary>
internal class Scripts
{

    /// <summary>
    /// Lista de elementos
    /// </summary>
    public static Dictionary<string, Action<string>> Actions { get; set; } = new Dictionary<string, Action<string>>();



    /// <summary>
    /// Construye las funciones
    /// </summary>
    public static void Build()
    {

        // Actualiza los proyectos
        Actions.Add("cl.UpdateProjects", async (param) =>
        {
            Pages.Index.AreProjectLoaded = false;

            if (Pages.Index.Me != null)
                await Pages.Index.Me.LoadProjects();

        });


        // Abre modal de contacto
        Actions.Add("openCt", async (param) =>
        {
            var id = int.Parse(param);
            var modelo =  await LIN.Access.Controllers.Contact.Read(id);
            ContactModal.Modelo = modelo.Model;
            await App.JS.InvokeVoidAsync("ShowModal", $"contact-modal-A12", "contact-close-btn-A12");
            ContactModal.Context.Render();
        });


        // Cierra la sesión
        Actions.Add("disconnect", (param) =>
        {
            Pages.Index.Me.GoTo("/logout");
        });


    }

}
