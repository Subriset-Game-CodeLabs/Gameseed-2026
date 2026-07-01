using System;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputButton
    {
        private InputAction _action;
        public InputAction Action => _action;

        private event Action _onDown = delegate { };
        public event Action OnDown
        {
            add { _onDown += value; }
            remove { _onDown -= value; }
        }
        public bool Down => _action.WasPressedThisFrame();
        public bool Pressed => _action.IsPressed();

        public InputButton(InputAction action)
        {
            _action = action;
            _action.performed += OnPerformed;
        }

        private void OnPerformed(InputAction.CallbackContext context)
        {
            _onDown.Invoke();
        }
    }
}