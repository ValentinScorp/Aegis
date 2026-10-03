using UnityEngine;
using UnityEngine.UI;
using Aegis.Core;

namespace Aegis.View
{
    public class CrosshairView : MonoBehaviour
    {
        [SerializeField] private float _armLength = 10f;
        [SerializeField] private float _gap = 4f;
        [SerializeField] private float _thickness = 2f;
        [SerializeField] private Color _color = new Color(1f, 1f, 1f, 0.9f);

        private Unit _unit;
        private GameObject _root;

        private void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            _root = new GameObject("Crosshair", typeof(RectTransform));
            _root.transform.SetParent(transform, false);

            float offset = _gap + _armLength * 0.5f;
            MakeBar("Left",  new Vector2(-offset, 0f), new Vector2(_armLength, _thickness));
            MakeBar("Right", new Vector2( offset, 0f), new Vector2(_armLength, _thickness));
            MakeBar("Up",    new Vector2(0f,  offset), new Vector2(_thickness, _armLength));
            MakeBar("Down",  new Vector2(0f, -offset), new Vector2(_thickness, _armLength));
            MakeBar("Dot",   Vector2.zero,             new Vector2(_thickness, _thickness));

            _root.SetActive(false);
        }

        private void MakeBar(string barName, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(barName, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(_root.transform, false);

            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            var img = go.GetComponent<Image>();
            img.color = _color;
            img.raycastTarget = false;
        }

        public void Bind(Unit unit)
        {
            Unbind();
            _unit = unit;
            if (_unit == null) return;
            _unit.AimStarted += Show;
            _unit.AimEnded += Hide;
            _root.SetActive(_unit.IsAiming);
        }

        public void Unbind()
        {
            if (_unit == null) return;
            _unit.AimStarted -= Show;
            _unit.AimEnded -= Hide;
            _unit = null;
            _root.SetActive(false);
        }

        private void Show() => _root.SetActive(true);
        private void Hide() => _root.SetActive(false);
        private void OnDestroy() => Unbind();
    }
}