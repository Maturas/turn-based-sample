using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBasedSample.Input
{
    public class InputController : MonoBehaviour
    {
        public event Action<Vector2> OnClickEvent;

        private void OnClick(InputValue value)
        {
            OnClickEvent?.Invoke(Mouse.current.position.ReadValue());
        }
    }
}