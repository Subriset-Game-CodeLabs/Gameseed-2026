using UnityEngine;

namespace Input
{
    public class InputManager : PersistentSingleton<InputManager>
    {
        private InputActions _inputActions;
        private FiniteStateMachine _actionMapStates;
        private PlayerActionMap _player;
        private UIActionMap _ui;

        public PlayerActionMap PlayerInput => _player;
        public UIActionMap UIInput => _ui;

        protected override void Awake()
        {
            base.Awake();
            InitializeManager();
        }

        private void Update()
        {
            _actionMapStates.Update();
        }

        private void InitializeManager()
        {
            _inputActions = new InputActions();
            _player = new PlayerActionMap(_inputActions);
            _ui = new UIActionMap(_inputActions);
            _actionMapStates = new FiniteStateMachine();
            _actionMapStates.ChangeState(_player);
        }

        public void PlayerMode() => _actionMapStates.ChangeState(_player);
        public void UIMode() => _actionMapStates.ChangeState(_ui);
    }
}