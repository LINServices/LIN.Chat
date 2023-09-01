namespace LIN.Chat.Client.Pages;


public partial class Chat
{


    /// <summary>
    /// Lista de conversaciones
    /// </summary>
    private readonly List<MemberChatModel> ConversaciónModels = new();



    /// <summary>
    /// Hub de conexión Realtime
    /// </summary>
    private LIN.Access.Communication.Hubs.ChatHub? ActualHub { get; set; }



    /// <summary>
    /// Mi perfil
    /// </summary>
    private MemberChatModel? Member { get; set; }



    /// <summary>
    /// Lista de Chats abiertos
    /// </summary>
    private readonly Dictionary<int, (Access.Communication.Hubs.ChatHub, MemberChatModel, A)> Chats = new();



    /// <summary>
    /// Pagina actual de Chat
    /// </summary>
    private Shared.ChatSection? ChatPage { get; set; }



    /// <summary>
    /// Cuando la pagina se inicia
    /// </summary>
    protected override void OnInitialized()
    {

        // Valida el login
        if (!LIN.Access.Communication.Session.IsOpen)
        {
            nav.NavigateTo("/login");
            return;
        }

        base.OnInitialized();
    }




}
