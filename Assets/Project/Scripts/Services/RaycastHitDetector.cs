using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aegis.Services
{
    public class RaycastHitDetector : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private PlayerInputListener _playerInputListener;
        public event Action<List<RaycastHit>> HitsDetected;
        private void Awake()
        {
            _playerInputListener = Utilities.ComponentResolver.ResolveOrFind(this, _playerInputListener);
        }
        private void Start()
        {
            if (_playerInputListener == null) return;
            _playerInputListener.TapPerformed += OnPlayerTap;
        }
        private void OnDestroy()
        {
            if (_playerInputListener == null) return;
            _playerInputListener.TapPerformed -= OnPlayerTap;
        }

        private void OnPlayerTap(Vector2 screenPosition)
        {
            if (IsMouseOverCanvas(screenPosition)) return;

            var ray = _camera.ScreenPointToRay((Vector3)screenPosition);

            HitObjects(ray);
        }
        private void HitObjects(Ray ray)
        {
            var hits = Physics.RaycastAll(ray, _camera.farClipPlane);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // Initialize the list directly from the array to avoid manual copying
            List<RaycastHit> hitTargets = new List<RaycastHit>(hits);

            if (hitTargets.Count > 0) {
                HitsDetected?.Invoke(hitTargets);
            }
        }

        private bool IsMouseOverCanvas(Vector2 screenPosition)
        {
            if (_canvas == null || EventSystem.current == null) 
                return false;

            GraphicRaycaster gr = _canvas.GetComponent<GraphicRaycaster>();
            
            if (gr == null) return false;

            var ped = new PointerEventData(EventSystem.current) {
                position = screenPosition
            };
            var results = new List<RaycastResult>();
            gr.Raycast(ped, results);

            // foreach (var result in results)
            //     Debug.Log($"MouseOverCanvas hit: {result.gameObject.name}");

            return results.Count > 0;
        }
    }
}

