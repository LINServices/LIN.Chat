namespace LIN.Allo.Client.Services;


/// <summary>
/// Observador.
/// </summary>
public static class ConversationsObserver
{


    /// <summary>
    /// Data de las conversaciones.
    /// </summary>
    public readonly static Dictionary<int, ConversationLocal> Data = [];


    /// <summary>
    /// Elementos a observar.
    /// </summary>
    private readonly static Dictionary<int, List<IMessageChanger>> Trackers = [];




    /// <summary>
    /// Agregar elemento al observador.
    /// </summary>
    /// <param name="conversation">Id de la conversación.</param>
    /// <param name="messageChanger">Objecto a observar.</param>
    public static void Suscribe(int conversation, IMessageChanger messageChanger)
    {

        // Obtener los observables.
        Trackers.TryGetValue(conversation, out var trackers);

        // Si no existe.
        if (trackers == null)
        {
            // Crear la lista de observables.
            Trackers.Add(conversation,
            [
                messageChanger
            ]);
            return;
        }

        // Agregar el objeto a la lista.
        trackers.Add(messageChanger);

    }



    /// <summary>
    /// Eliminar objeto de la lista de observables.
    /// </summary>
    /// <param name="messageChanger">Objeto,</param>
    public static void UnSuscribe(IMessageChanger messageChanger)
    {
        // Eliminar objetos.
        foreach (var e in Trackers.Values)
            e.RemoveAll(t => t == messageChanger);

    }



    /// <summary>
    /// Notificar cambios a los observables.
    /// </summary>
    /// <param name="conversation">Id de la conversación.</param>
    public static void Notification(int conversation)
    {

        // Obtener.
        Trackers.TryGetValue(conversation, out var trackers);

        // No hay.
        if (trackers == null)
            return;

        // Notificar cambios.
        foreach (var item in trackers)
            item.Change();

    }



    /// <summary>
    /// Crear conversación.
    /// </summary>
    /// <param name="conversation">Modelo.</param>
    public static void Create(ConversationModel conversation)
    {

        Data.TryGetValue(conversation.ID, out var local);

        if (local == null)
        {
            Data.Add(conversation.ID, new()
            {

                Conversation = conversation,
                Messages = conversation.Mensajes ?? []
            }); ;
            return;
        }


    }



    /// <summary>
    /// Agregar mensaje.
    /// </summary>
    /// <param name="conversation">Id de la conversación.</param>
    /// <param name="message">Modelo del mensaje.</param>
    public static void PushMessage(int conversation, MessageModel message)
    {

        Data.TryGetValue(conversation, out var local);

        Trackers.TryGetValue(conversation, out var trackers);

        if (local == null || trackers == null)
            return;


        var exist = local.Messages.Where(t => t.Guid == message.Guid);

        if (exist.Any())
        {
            foreach (var item in exist)
                item.IsLocal = false;

            foreach (var tracker in trackers)
                tracker.Change();

            return;
        }


        local.Messages.Add(message);

        foreach (var tracker in trackers)
            tracker.Change();

    }



    /// <summary>
    /// Obtener la conversación.
    /// </summary>
    /// <param name="id">Id de la conversación.</param>
    public static ConversationLocal? Get(int id)
    {
        Data.TryGetValue(id, out var local);
        return local;
    }


}


public class ConversationLocal
{

    public ConversationModel Conversation { get; set; } = null!;

    public List<MessageModel> Messages { get; set; } = [];

}


public interface IMessageChanger
{

    void Change();

}