using System;
using _2048.Scripts.Values;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _2048.Scripts.Input
{
    public class InputHandler : MonoBehaviour
    {
        private InputSystem_Actions _actions;

        public event Action<Direction> OnMove;

        private void Awake()
        {
            _actions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _actions.Player.Enable();
            _actions.Player.Move.performed += OnPlayerMoved;
        }

        private void OnDisable()
        {
            _actions.Player.Move.performed -= OnPlayerMoved;
            _actions.Player.Disable();
        }

        private void OnDestroy()
        {
            _actions.Dispose();
        }

        private void OnPlayerMoved(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (input == Vector2.zero)
                return;

            Direction direction;

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                direction = input.x > 0
                    ? Direction.Right
                    : Direction.Left;
            }
            else
            {
                direction = input.y > 0
                    ? Direction.Up
                    : Direction.Down;
            }

            
            Debug.Log(direction);
            OnMove?.Invoke(direction);
        }
    }
}