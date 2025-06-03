


public enum InteractionResult
{
    Success,
    Failed
}

public interface IInteractable
{
    /// <summary>
    /// Ejecuta la interacción. Devuelve un resultado indicando si fue exitosa o no.
    /// </summary>
    InteractionResult Interact(IInteractor interactor);

    /// <summary>
    /// Devuelve el texto que se mostrará como consejo de interacción al jugador.
    /// </summary>
    string GetInteractTip();

    /// <summary>
    /// Indica si actualmente se puede interactuar con este objeto.
    /// </summary>
    bool CanInteract();

    /// <summary>
    /// Indica si solo se puede interactuar con este objeto una vez.
    /// </summary>
    bool IsOneTimeUse();

    /// <summary>
    /// Indica si el objeto está activo y debe mostrarse como interactuable.
    /// </summary>
    bool IsActive();
}
