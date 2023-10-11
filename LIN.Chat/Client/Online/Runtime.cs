
using SILF.Script.Elements.Functions;
using SILF.Script.Interfaces;
using SILF.Script;
using LIN.Access.Auth.Controllers;

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

    }

}
