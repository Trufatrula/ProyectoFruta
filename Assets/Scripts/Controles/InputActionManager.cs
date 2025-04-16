using UnityEngine;
using UnityEngine.InputSystem;
using System;

public static class ActionMaps
{
    public const string PlayerMovement = "PlayerMovement";
    public const string UI = "UI";
}

public class InputActionManager : MonoBehaviour
{
    private PlayerControls inputActions;

    private string currentActionMap = "PlayerMovement";

    public event Action<Vector2> OnMove;
    public event Action OnJump;
    public event Action OnDash;
    public event Action<bool> OnCrouch;
    public event Action OnPrimaryShot;
    public event Action OnSecondaryShot;

    public event Action OnSubmit;
    public event Action OnCancel;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        EnableCurrentActionMap();
    }

    private void OnDisable()
    {
        DisableCurrentActionMap();
    }
    
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    /// <summary>
    /// Desactiva el mapa de acciones activo y activa uno nuevo.
    /// </summary>
    /// <param name="newActionMapName">El mapa a activar</param>
    public void SwitchActionMap(string newActionMapName)
    {
        if (currentActionMap == newActionMapName)
            return;

        DisableCurrentActionMap();
        currentActionMap = newActionMapName;
        EnableCurrentActionMap();
    }

    /// <summary>
    /// Activa el ultimo mapa de acciones y se suscribe a sus eventos.
    /// </summary>
    private void EnableCurrentActionMap()
    {
        switch (currentActionMap)
        {
            case ActionMaps.PlayerMovement:
                inputActions.MovementMap.Enable();
                SubscribePlayerMovementEvents();
                break;
            case ActionMaps.UI:
                inputActions.UIMap.Enable();
                SubscribeUIEvents();
                break;
            default:
                Debug.LogWarning($"El action map {currentActionMap} no existe.");
                break;
        }
    }

    /// <summary>
    /// Se desuscribe de los eventos segun el mapa activado.
    /// </summary>
    private void DisableCurrentActionMap()
    {
        switch (currentActionMap)
        {
            case ActionMaps.PlayerMovement:
                UnsubscribePlayerMovementEvents();
                inputActions.MovementMap.Disable();
                break;
            case ActionMaps.UI:
                UnsubscribeUIEvents();
                inputActions.UIMap.Disable();
                break;
            default:
                Debug.LogWarning($"El action map {currentActionMap} no existe.");
                break;
        }
    }

    #region PlayerMovement Subscriptions

    private void SubscribePlayerMovementEvents()
    {
        inputActions.MovementMap.Move.performed += HandleMove;
        inputActions.MovementMap.Move.canceled += HandleMove;
        inputActions.MovementMap.Jump.performed += HandleJump;
        inputActions.MovementMap.Dash.performed += HandleDash;
        inputActions.MovementMap.Crouch.performed += HandleCrouchPerformed;
        inputActions.MovementMap.Crouch.canceled += HandleCrouchCanceled;
        inputActions.MovementMap.PrimaryShot.performed += HandlePrimaryShot;
        inputActions.MovementMap.SecondaryShot.performed += HandleSecondaryShot;
    }

    private void UnsubscribePlayerMovementEvents()
    {
        inputActions.MovementMap.Move.performed -= HandleMove;
        inputActions.MovementMap.Move.canceled -= HandleMove;
        inputActions.MovementMap.Jump.performed -= HandleJump;
        inputActions.MovementMap.Dash.performed -= HandleDash;
        inputActions.MovementMap.Crouch.performed -= HandleCrouchPerformed;
        inputActions.MovementMap.Crouch.canceled -= HandleCrouchCanceled;
        inputActions.MovementMap.PrimaryShot.performed -= HandlePrimaryShot;
        inputActions.MovementMap.SecondaryShot.performed -= HandleSecondaryShot;
    }

    private void HandleMove(InputAction.CallbackContext context)
    {
        Vector2 moveValue = context.ReadValue<Vector2>();
        OnMove?.Invoke(moveValue);
    }

    private void HandleJump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void HandleDash(InputAction.CallbackContext context)
    {
        OnDash?.Invoke();
    }

    private void HandleCrouchPerformed(InputAction.CallbackContext context)
    {
        OnCrouch?.Invoke(true);
    }

    private void HandleCrouchCanceled(InputAction.CallbackContext context)
    {
        OnCrouch?.Invoke(false);
    }

    private void HandlePrimaryShot(InputAction.CallbackContext context)
    {
        OnPrimaryShot?.Invoke();
    }

    private void HandleSecondaryShot(InputAction.CallbackContext context)
    {
        OnSecondaryShot?.Invoke();
    }

    #endregion

    #region UI Subscriptions

    private void SubscribeUIEvents()
    {
        //inputActions.UIMap.Submit.performed += HandleSubmit;
        //inputActions.UIMap.Cancel.performed += HandleCancel;
    }

    private void UnsubscribeUIEvents()
    {
        //inputActions.UIMap.Submit.performed -= HandleSubmit;
        //inputActions.UIMap.Cancel.performed -= HandleCancel;
    }

    //private void HandleSubmit(InputAction.CallbackContext context)
    //{
    //    OnSubmit?.Invoke();
    //}

    //private void HandleCancel(InputAction.CallbackContext context)
    //{
    //    OnCancel?.Invoke();
    //}

    #endregion

    /// <summary>
    /// Para cambiar el binding de alguna tecla en especifico.
    /// </summary>
    /// <param name="actionName">Nombre de la accion a rebindear.</param>
    /// <param name="onComplete">Callback cuando la accion esta terminada.</param>
    public void RebindKey(string actionName, Action onComplete)
    {
        InputAction action = inputActions.FindAction(actionName, true);
        if (action == null)
        {
            Debug.LogError("Action not found: " + actionName);
            return;
        }

        action.Disable();
        action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnComplete(operation =>
            {
                action.Enable();
                operation.Dispose();
                onComplete?.Invoke();
            })
            .Start();
    }

    public string GetCurrentActionMap()
    {
        return currentActionMap;
    }
}