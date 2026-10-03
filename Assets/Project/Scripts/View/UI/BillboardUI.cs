using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start() {
        _mainCamera = Camera.main;
    }

    private void LateUpdate() {
        transform.forward = _mainCamera.transform.forward;
    }
}
