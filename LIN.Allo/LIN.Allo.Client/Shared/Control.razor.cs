namespace LIN.Allo.Client.Shared;


public partial class Control
{

    /// <summary>
    /// Evento al darle click.
    /// </summary>
    [Parameter]
    public Action<MemberChatModel> OnClick { get; set; } = (e) => { };



    /// <summary>
    /// Modelo.
    /// </summary>
    [Parameter]
    public MemberChatModel Member { get; set; }



    /// <summary>
    /// El control esta seleccionado.
    /// </summary>
    public bool IsSelect { get; set; } = false;



    /// <summary>
    /// Tiene nuevos mensajes.
    /// </summary>
    public bool IsNew = false;

  


    /// <summary>
    /// Renderizar.
    /// </summary>
    public void Render()
    {
        StateHasChanged();
    }



    /// <summary>
    /// Seleccionar.
    /// </summary>
    public void Select()
    {
        IsSelect = true;
        Render();
    }



    /// <summary>
    /// Deseleccionar.
    /// </summary>
    public void Unselect()
    {
        IsSelect = false;
        Render();
    }



}