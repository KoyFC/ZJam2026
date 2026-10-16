using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : Singleton<PlayerInputController>
{
    private bool m_InputActive = false;

    [SerializeField] private InputActionReference m_MoveActionReference;
    public Vector2 MoveInput { get; private set; }
    public static event Action<Vector2> OnMoveInputChanged;

    [SerializeField] private InputActionReference m_PrimaryFireActionReference;
    public bool IsPrimaryFireHeld { get; private set; }

    [SerializeField] private InputActionReference m_SecondaryFireActionReference;
    public static event Action OnSecondaryFire;

    private void OnEnable()
    {
        TryEnableInput();
    }

    private void OnDisable()
    {
        TryDisableInput();
    }

    private void TryEnableInput()
    {
        if (m_InputActive) return;

        SetActionEnabled(m_MoveActionReference, true, OnMovePerformed, OnMoveCanceled);
        SetActionEnabled(m_PrimaryFireActionReference, true, OnPrimaryFirePerformed, OnPrimaryFireCanceled);
        SetActionEnabled(m_SecondaryFireActionReference, true, OnSecondaryFirePerformed);

        m_InputActive = true;
    }

    private void TryDisableInput()
    {
        if (!m_InputActive) return;

        SetActionEnabled(m_MoveActionReference, false, OnMovePerformed, OnMoveCanceled);
        SetActionEnabled(m_PrimaryFireActionReference, false, OnPrimaryFirePerformed, OnPrimaryFireCanceled);
        SetActionEnabled(m_SecondaryFireActionReference, false, OnSecondaryFirePerformed);

        m_InputActive = false;

        MoveInput = Vector2.zero;
        IsPrimaryFireHeld = false;
    }

    public void SetInputActive(bool active)
    {
        if (active) TryEnableInput();
        else TryDisableInput();
    }

    #region Helpers
    private void SetActionEnabled(InputActionReference actionReference, bool enable, Action<InputAction.CallbackContext> performedCallback = null, Action<InputAction.CallbackContext> canceledCallback = null)
    {
        if (actionReference != null && actionReference.action != null)
        {
            if (enable)
            {
                if (performedCallback != null) actionReference.action.performed += performedCallback;
                if (canceledCallback != null) actionReference.action.canceled += canceledCallback;

                actionReference.action.Enable();
            }
            else
            {
                if (performedCallback != null) actionReference.action.performed -= performedCallback;
                if (canceledCallback != null) actionReference.action.canceled -= canceledCallback;

                actionReference.action.Disable();
            }
        }
    }
    #endregion

    #region Input Callbacks
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        OnMoveInputChanged?.Invoke(MoveInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
        OnMoveInputChanged?.Invoke(MoveInput);
    }

    private void OnPrimaryFirePerformed(InputAction.CallbackContext context)
    {
        IsPrimaryFireHeld = true;
    }

    private void OnPrimaryFireCanceled(InputAction.CallbackContext context)
    {
        IsPrimaryFireHeld = false;
    }

    private void OnSecondaryFirePerformed(InputAction.CallbackContext context)
    {
        OnSecondaryFire?.Invoke();
    }
    #endregion
}