using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerInputHandler : MonoBehaviour
{ 
       [SerializeField] private PlayerInput _playerInput;
       [SerializeField] private EventSystem _eventSystem;
       [SerializeField] private CursorController _cursorController;
        
    
        private bool _isControllerConnected;
        public static event Action<bool> OnInputDeviceChanged;
        
    
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _cursorController = GetComponent<CursorController>();
        }
    

        private void OnEnable()
        {
            _playerInput.currentActionMap.Enable();
            InputSystem.onDeviceChange += OnDeviceChange;
            
            _playerInput.actions["Move"].performed += OnMoving;
            _playerInput.actions["Move"].canceled += OnMoving;
            
            _playerInput.actions["Interact"].started += Interact;
            
            _playerInput.actions["CancelSpell"].started += CancelSpell;
            
            _playerInput.actions["Pause"].started += OnPauseGame;
        }
        
        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
            
            _playerInput.actions["Move"].performed -= OnMoving;
            _playerInput.actions["Move"].canceled -= OnMoving;

            _playerInput.actions["Interact"].started -= Interact;
            
            _playerInput.actions["CancelSpell"].started -= CancelSpell;

            _playerInput.actions["Pause"].started -= OnPauseGame;
        }
    
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
            { 
                DetectCurrentInputDevice();
            }
        }
        
        private void DetectCurrentInputDevice()
        {
            _isControllerConnected = Gamepad.all.Count > 0;
            OnInputDeviceChanged?.Invoke(_isControllerConnected);
            
            //Debug.Log(_isControllerConnected
            //? "Controller connected: Switching to Gamepad controls."
            //: "No controller connected: Switching to Keyboard/Mouse controls.");
        }

        private void OnMoving(InputAction.CallbackContext context)
        {
            _cursorController.Direction = context.ReadValue<Vector2>();
        }

        private void Interact(InputAction.CallbackContext context)
        {
            _cursorController.ClickWithSpell();
        }

        private void CancelSpell(InputAction.CallbackContext context)
        {
            _cursorController.Cancel();
        }

        private void OnPauseGame(InputAction.CallbackContext context)
        {
            if(!_cursorController.GameInPause) _cursorController.PauseGame();
        }
}

