using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyPeasyFirstPersonController
{
    public class InputManager : MonoBehaviour, IInputManager
    {
        private InputSystem_Actions actions;

        public Vector2 moveInput => actions.Player.Move.ReadValue<Vector2>();
        public Vector2 lookInput => actions.Player.Look.ReadValue<Vector2>();

        public bool jump => actions.Player.Jump.WasPressedThisFrame();
        public bool sprint => actions.Player.Sprint.IsPressed();
        public bool crouch => actions.Player.Crouch.IsPressed();
        public bool slide => actions.Player.Crouch.IsPressed();

        private void Awake()
        {
            actions = new InputSystem_Actions();
        }

        private void OnEnable() => actions.Enable();
        private void OnDisable() => actions.Disable();
    }
}