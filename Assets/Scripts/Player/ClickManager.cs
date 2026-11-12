using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    public class ClickManager : MonoBehaviourSingleton<ClickManager>
    {
        private bool _inpause;
        [SerializeField] private bool CanClick;
        
        [SerializeField] private Camera _camera;
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private LayerMask _layerMask;
    
        public Vector3 LastPosition;
        private InputAction tab;

        private void OnEnable()
        {
            _playerInput.actions["PcClick"].started += GetPCClick;
            _playerInput.actions["FirstTouch"].started += GetFirstTouch;
            
            EventBus.OnGamePaused += OnPause;
            EventBus.OnGameResume += OnResume;

            EventBus.OnLevelFinished += DisableClick;
            EventBus.OnPlayerTakedCard += EnableClick;
        }
    
        private void OnDisable()
        {
            _playerInput.actions["PcClick"].started -= GetPCClick;
            _playerInput.actions["FirstTouch"].started -= GetFirstTouch;
            
            EventBus.OnGamePaused -= OnPause;
            EventBus.OnGameResume -= OnResume;

            EventBus.OnLevelFinished -= DisableClick;
            EventBus.OnPlayerTakedCard -= EnableClick;
        }

        private void GetPCClick(InputAction.CallbackContext obj)
        {
            OnClicked(Mouse.current.position.ReadValue());
        }

        private void GetFirstTouch(InputAction.CallbackContext obj) 
        {
            OnClicked(Input.mousePosition);
        }
    
        private void OnClicked(Vector2 clickPos)
        {
            if (_inpause || !CanClick) return;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 5f);
            if (Physics.Raycast(ray, out hit,Mathf.Infinity, _layerMask )) 
            {
                LastPosition = hit.point;
                GameObject hitedGm = hit.collider.gameObject;
                IClickable iClickable = hitedGm.GetComponent<IClickable>();
                if (iClickable != null)
                {
                    iClickable.OnClick();
                }
                EventBus.OnPlayerClicked?.Invoke();
            }
            
        }
        
        private void EnableClick()
        {
            CanClick = true;
        }
        private void DisableClick()
        {
            CanClick = false;
        }
        
        private void OnPause()
        {
            _inpause = true;
        }

        private void OnResume()
        {
            _inpause = false;
        }
    }
}
