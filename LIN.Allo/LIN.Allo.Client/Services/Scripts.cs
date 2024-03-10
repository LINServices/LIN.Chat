using LIN.Allo.Client.Pages.Sections;
using SILF.Script;
using SILF.Script.Elements;
using SILF.Script.Elements.Functions;
using SILF.Script.Interfaces;
using SILF.Script.Runtime;

namespace LIN.Allo.Client.Services;


internal class Scripts
{


    public class SILFFunction : IFunction
    {
        public Tipo? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Parameter> Parameters { get; set; } = new();
        public Context Context { get ; set; }

        Action<List<SILF.Script.Elements.ParameterValue>> Action;

        public SILFFunction(Action<List<SILF.Script.Elements.ParameterValue>> action)
        {
            Action = action;
        }



        public FuncContext Run(Instance instance, List<SILF.Script.Elements.ParameterValue> values, ObjectContext objectContext)
        {
            Action.Invoke(values);
            return new();
        }


    }


    /// <summary>
    /// Funciones.
    /// </summary>
    public static List<IFunction> Actions { get; set; } = new();



    /// <summary>
    /// Construye las funciones.
    /// </summary>
    public static void Build()
    {

        // Acción.
        SILFFunction actionMessage =
        new(async (param) =>
        {

            // Propiedades.
            var id = param.Where(T => T.Name == "id").FirstOrDefault();
            var content = param.Where(T => T.Name == "contenido").FirstOrDefault();

            // Obtener la conversación.
            _ = int.TryParse(id.Objeto.Value.ToString(), out int idInt);

            // Obtener el observador.

            /* Cambio no fusionado mediante combinación del proyecto 'LIN.Allo.App (net8.0-windows10.0.19041.0)'
            Antes:
                        var conversation = Components.ConversationsObserver.Get(idInt);
            Después:
                        var conversation = ConversationsObserver.Get(idInt);
            */
            var conversation = ConversationsObserver.Get(idInt);

            // No existe.
            if (conversation == null)
                return;

            // Generar guid.
            string guid = Guid.NewGuid().ToString();

            // Publicar el mensaje en local.

            /* Cambio no fusionado mediante combinación del proyecto 'LIN.Allo.App (net8.0-windows10.0.19041.0)'
            Antes:
                        Components.ConversationsObserver.PushMessage(conversation.Conversation.ID, new()
            Después:
                        ConversationsObserver.PushMessage(conversation.Conversation.ID, new()
            */
            ConversationsObserver.PushMessage(conversation.Conversation.ID, new()
            {
                Contenido = content?.Objeto.Value.ToString(),
                Time = DateTime.Now,
                Guid = guid,
                IsLocal = true,
                Conversacion = conversation.Conversation,
                Remitente = Access.Communication.Session.Instance.Profile
            });

            // Enviar el mensaje al servicio.
            await ChatSection.Hub!.SendMessage(conversation.Conversation.ID, content?.Objeto.Value.ToString() ?? "", guid);

        })
        {
            Name = "mensaje",
            Parameters =
            [
                new Parameter("id", new("number")),
                new Parameter("contenido", new("string"))
            ]
        };




        // Acción.
        SILFFunction search =
        new(async (param) =>
        {

            var content = param.Where(T => T.Name == "contenido").FirstOrDefault();

            string uri = LIN.Modules.Web.AddParameters("https://www.google.com/search", new()
            {
                {"q",content?.Objeto?.Value?.ToString() ?? "" }
            });

            await Launcher.Default.OpenAsync(uri);

        })
        {
            Name = "search",
            Parameters =
            [
                new Parameter("contenido", new("string"))
            ]
        };


        // Acción.
        SILFFunction actionSelect =
        new(async (param) =>
        {

            // Propiedades.
            var id = param.Where(T => T.Name == "id").FirstOrDefault();

            // Obtener la conversación.
            _ = int.TryParse(id?.Objeto.Value.ToString(), out int idInt);


            Chat.Instance?.Select(idInt);

        })
        {
            Name = "select",
            Parameters =
            [
                new Parameter("id", new("number"))
            ]
        };

        // Agregar.
        Actions.AddRange([actionMessage, actionSelect, search]);

    }


}