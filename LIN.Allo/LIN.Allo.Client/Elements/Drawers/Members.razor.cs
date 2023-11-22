namespace LIN.Allo.Client.Elements.Drawers;


public partial class Members
{


    /// <summary>
    /// Nombre del grupo.
    /// </summary>
    [Parameter]
    public string Name { get; set; } = string.Empty;



    /// <summary>
    /// Id del grupo.
    /// </summary>
    [Parameter]
    public int ConversationId { get; set; }



    /// <summary>
    /// Id único del elemento.
    /// </summary>
    private string UniqueId { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Lista de modelos de miembros.
    /// </summary>
    private List<Types.Auth.Abstracts.SessionModel<MemberChatModel>> MemberModels { get; set; } = new();



    /// <summary>
    /// Lista de controles de miembros.
    /// </summary>
    private List<Member> MemberControls { get; set; } = new();



    /// <summary>
    /// Control de un integrante.
    /// </summary>
    private Member MemberControl
    {
        set => MemberControls.Add(value);
    }



    /// <summary>
    /// Esta cargado.
    /// </summary>
    private bool IsLoad { get; set; }


    /// <summary>
    /// Abrir el elemento.
    /// </summary>
    public async void Show()
    {
        await jsRuntime.InvokeVoidAsync("ShowDrawer", $"drawer-{UniqueId}", $"close-drawer-{UniqueId}");
        StateHasChanged();
    }



    /// <summary>
    /// Evento después de renderizar.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {

        // Si no esta cargado.
        if (!IsLoad)
        {
            // Respuesta de la API.
            var result = await Access.Communication.Controllers.Conversations.MembersInfo(ConversationId, Access.Communication.Session.Instance.AccountToken);

            // Modelos a la UI.
            MemberModels = result.Models.OrderByDescending(t => t.Profile.Rol).ToList();
            IsLoad = true;
            StateHasChanged();
        }

        // Evento base.
        await base.OnAfterRenderAsync(firstRender);
    }


}



