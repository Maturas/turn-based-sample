using System;
using TurnBasedSample.Core;
using UnityEngine;

namespace TurnBasedSample.Input
{
    public enum SelectionMode
    {
        None,
        All,
        Tiles,
        UnitsTeamRed,
        UnitsTeamBlue
    }
    
    public class ClickSelector : MonoBehaviour
    {
        [SerializeField] private SelectionModeConfig[] selectionModeConfigs;
        
        private readonly RaycastHit[] _raycastCache = new RaycastHit[1];

        public event Action<IClickable> OnClickedEvent;
        
        public SelectionMode CurrentSelectionMode { get; set; }
        
        private SelectionModeConfig CurrentSelectionModeConfig => Array.Find(selectionModeConfigs, c => c.Mode == CurrentSelectionMode);
        
        private void Start()
        {
            GameManager.Instance.InputController.OnClickEvent += OnClick;
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance)
                GameManager.Instance.InputController.OnClickEvent -= OnClick;
        }

        private void OnClick(Vector2 pointerPosition)
        {
            IClickable clickable = null;
            var layerMask = CurrentSelectionModeConfig.LayerMask;
            var ray = GameManager.Instance.MainCamera.ScreenPointToRay(pointerPosition);
            if (Physics.RaycastNonAlloc(ray, _raycastCache, 100.0f, layerMask) > 0)
            {
                var hit = _raycastCache[0];
                var hitObject = hit.collider.gameObject;
                hitObject.TryGetComponent(out clickable);
            }
            
            OnClickedEvent?.Invoke(clickable);
        }

        [Serializable]
        public class SelectionModeConfig
        {
            public SelectionMode Mode;
            public LayerMask LayerMask;
        }
    }
}