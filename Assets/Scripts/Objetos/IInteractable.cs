using UnityEngine;

public enum InteractionType
{
    INTERACT,
    GRAB,
    TOGGLE,
    HOLD,
    DRAG
}


public interface IInteractable
{
    string ItemID { get; }
    GameObject GameObject { get; }

    void Highlight();
    void Unhighlight();
    void Interact();
}

public interface IGrabbable : IInteractable
{
    bool IsGrabbable { get; }
    void Grab();
    void Drop(ISocket socket);
}

public interface IHablable : IInteractable
{
    void Hablar(IGrabbable hablarObject);
    int GetDialogoIndex();
    void AvanzarDialogoIndex(bool finalizar);
    void TerminarDialogo();
}

public interface ISocket : IInteractable
{
    bool PlaceInSocket(IGrabbable placedObject);
    void TakeFromSocket();
}