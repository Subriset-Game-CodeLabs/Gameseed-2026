using UnityEngine;

namespace Input
{
    public abstract class ActionMap : IState
    {
        protected InputActions InputActions;
        public abstract bool HasPollable { get; }

        public ActionMap(InputActions action)
        {
            InputActions = action;
        }

        public abstract void OnEnter();
        public abstract void OnExit();

        public virtual void OnUpdate()
        {
        }
    }

    public class UIActionMap : ActionMap
    {
        private InputButton _submit;
        private InputButton _cancel;

        public InputButton Submit => _submit;
        public InputButton Cancel => _cancel;

        public override bool HasPollable => false;

        public UIActionMap(InputActions action) : base(action)
        {
            _submit = new InputButton(action.UI.Submit);
            _cancel = new InputButton(action.UI.Cancel);
        }

        public override void OnEnter() => InputActions.UI.Enable();
        public override void OnExit() => InputActions.UI.Disable();
    }

    public class PlayerActionMap : ActionMap
    {
        private InputValue<Vector2> _movement;
        private InputButton _smash;
        private InputButton _attack;
        private InputButton _interact;

        public InputValue<Vector2> Movement => _movement;
        public InputButton Smash => _smash;
        public InputButton Attack => _attack;
        public InputButton Interact => _interact;

        public override bool HasPollable => true;

        public PlayerActionMap(InputActions action) : base(action)
        {
            _movement = new InputValue<Vector2>(action.Player.Move);
            _smash = new InputButton(action.Player.Smash);
            _attack = new InputButton(action.Player.Attack);
            _interact = new InputButton(action.Player.Interact);
        }

        public override void OnEnter() => InputActions.Player.Enable();
        public override void OnExit() => InputActions.Player.Disable();

        public override void OnUpdate()
        {
            _movement.ForcePoll();
        }
    }

}