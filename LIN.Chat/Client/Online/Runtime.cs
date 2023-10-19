
using LIN.Allo.Client.Shared;
using SILF.Script;
using SILF.Script.Elements.Functions;
using SILF.Script.Interfaces;

namespace LIN.Allo.Client.Online;


/// <summary>
/// Runtime de SILF.Script para LIN Cloud Console
/// </summary>
internal class Scripts
{
    public class SILFFunction : IFunction
    {
        public Tipo? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Parameter> Parameters { get; set; } = new();


        Action<List<SILF.Script.Elements.ParameterValue>> Action;

        public SILFFunction(Action<List<SILF.Script.Elements.ParameterValue>> action)
        {
            Action = action;
        }



        public FuncContext Run(Instance instance, List<SILF.Script.Elements.ParameterValue> values)
        {
            Action.Invoke(values);
            return new();
        }


    }


    /// <summary>
    /// Funciones
    /// </summary>
    public static List<IFunction> Actions { get; set; } = new();


    /// <summary>
    /// Construye las funciones
    /// </summary>
    public static void Build()
    {

        // Mensaje
        Actions.Add(new SILFFunction((values) =>
        {

        })
        // Propiedades
        {
            Name = "disconnect"
        });

        Actions.Add(new SILFFunction(async (values) =>
        {
            var s = values.Where(T => T.Name == "nombre").FirstOrDefault();
            var value = values.Where(T => T.Name == "contenido").FirstOrDefault();
            var conversacion = Pages.Chat.ComponentRefs.Where(T => T.Member.Conversation.Name.ToLower() == s.Value.ToString()?.ToLower()).FirstOrDefault();

            if (conversacion == null)
                return;


         await   ChatSection.Hub!.SendMessage(conversacion.Member.Conversation.ID, value.Value.ToString() ?? "");
           

        })
        // Propiedades
        {
            Name = "mensaje",
            Parameters = new()
            {
                new Parameter("nombre", new("string")),
                new Parameter("contenido", new("string"))
            }
        });
    }

}
