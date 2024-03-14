using LIN.Types.Exp.Search.Models;
using SILF.Script.Elements.Functions;
using SILF.Script.Enums;
using SILF.Script.Interfaces;
using System.Reflection;
using static LIN.Allo.Client.Services.Scripts;

namespace LIN.Allo.Client.Elements.Drawers;



public partial class EmmaDrawer
{

    LIN.Emma.UI.Emma DocEmma { get; set; }



    /// <summary>
    /// Id único del elemento.
    /// </summary>
    private string UniqueId { get; init; } = Guid.NewGuid().ToString();






    /// <summary>
    /// Abre el elemento.
    /// </summary>
    public async void Show()
    {
        await js.InvokeAsync<object>("ShowDrawer", $"drawerEmma-{UniqueId}", $"close-drawerEmma-{UniqueId}");
        StateHasChanged();
    }





    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            LIN.Emma.UI.Functions.LoadActions(Scripts.Actions);
            DocEmma.OnPromptRequire += DocEmma_OnPromptRequire;
        }

    }


    private void DocEmma_OnPromptRequire(object? sender, string e)
    {

        if (DocEmma != null)
            DocEmma.ResponseIA = Access.Communication.Controllers.Messages.ToEmma(e, Access.Communication.Session.Instance.AccountToken);
    }


}

class A : IConsole
{
    public void InsertLine(string error, string result, SILF.Script.Enums.LogLevel logLevel)
    {
        var s = "";
    }
}