using UnityEngine;

namespace Aegis.UI
{
    public class FaceCamera : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.forward = _mainCamera.transform.forward;
        }
    }
}