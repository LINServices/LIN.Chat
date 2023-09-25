namespace LIN.Chat.Client.Sections;


public partial class ChatSection
{


    /// <summary>
    /// Hub de conexión
    /// </summary>
    public static Access.Communication.Hubs.ChatHub? Hub { get; set; }



    /// <summary>
    /// Integrante del chat
    /// </summary>
    [Parameter]
    public MemberChatModel Iam { get; set; } = new();







}
