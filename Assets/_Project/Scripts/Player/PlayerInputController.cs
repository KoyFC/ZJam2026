using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private bool m_InputActive = false;

    [SerializeField] private InputActionReference m_MoveActionReference;
    public Vector2 MoveInput { get; private set; }
    public event Action<Vector2> OnMoveInputChanged;

    [SerializeField] private InputActionReference m_FireActionReference;
    public event Action OnFire;

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
        SetActionEnabled(m_FireActionReference, true, OnInteractPerformed);

        m_InputActive = true;
    }

    private void TryDisableInput()
    {
        if (!m_InputActive) return;

        SetActionEnabled(m_MoveActionReference, false, OnMovePerformed, OnMoveCanceled);
        SetActionEnabled(m_FireActionReference, false, OnInteractPerformed);

        m_InputActive = false;

        MoveInput = Vector2.zero;
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

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        OnFire?.Invoke();
    }
    #endregion
}