using UnityEngine;
using UnityEngine.UI;
using Aegis.Core;

namespace Aegis.View
{
    public class CrosshairView : MonoBehaviour
    {
        [SerializeField] private float _armLength = 8f;
        [SerializeField] private float _gap = 8f;
        [SerializeField] private float _thickness = 2f;   // в екранних пікселях; 2 дає чіткіші лінії
        [SerializeField] private Color _color = new Color(1f, 1f, 1f, 0.9f);

        private Unit _unit;
        private GameObject _root;
        private Canvas _canvas;
        private float _lastScale = -1f;
        private RectTransform _left, _right, _up, _down, _dot;

        private void Awake()
        {
            var self = GetComponent<RectTransform>();
            if (self == null) self = gameObject.AddComponent<RectTransform>();

            var parentCanvas = transform.parent != null
                ? transform.parent.GetComponentInParent<Canvas>()
                : null;

            if (parentCanvas != null) {
                // всередині існуючого Canvas: свій Canvas не потрібен, розтягуємось на батька
                _canvas = parentCanvas.rootCanvas;
                self.anchorMin = Vector2.zero;
                self.anchorMax = Vector2.one;
                self.offsetMin = Vector2.zero;
                self.offsetMax = Vector2.zero;
                self.localScale = Vector3.one;
                self.localRotation = Quaternion.identity;
            } else {
                // самостійний корінь (як було раніше)
                _canvas = gameObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = 100;
            }

            _root = new GameObject("Crosshair", typeof(RectTransform));
            var rootRt = (RectTransform)_root.transform;
            rootRt.SetParent(transform, false);
            rootRt.anchorMin = rootRt.anchorMax = rootRt.pivot = new Vector2(0.5f, 0.5f);
            rootRt.anchoredPosition = Vector2.zero;
            rootRt.sizeDelta = Vector2.zero;

            _left = MakeBar("Left");
            _right = MakeBar("Right");
            _up = MakeBar("Up");
            _down = MakeBar("Down");
            _dot = MakeBar("Dot");

            Layout();
            _root.SetActive(false);
        }

        private RectTransform MakeBar(string barName)
        {
            var go = new GameObject(barName, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(_root.transform, false);

            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);

            var img = go.GetComponent<Image>();
            img.color = _color;
            img.raycastTarget = false;
            return rt;
        }

        // Розміри рахуємо щоразу, коли змінюється масштаб Canvas (зміна роздільності вікна)
        private void Layout()
        {
            float s = Mathf.Max(0.01f, _canvas.scaleFactor);
            _lastScale = _canvas.scaleFactor;

            // товщина кратна цілому числу екранних пікселів, інакше тонкі лінії розмиваються
            float t = Mathf.Max(1f, Mathf.Round(_thickness * s)) / s;
            float offset = _gap + _armLength * 0.5f;

            Set(_left, new Vector2(-offset, 0f), new Vector2(_armLength, t));
            Set(_right, new Vector2(offset, 0f), new Vector2(_armLength, t));
            Set(_up, new Vector2(0f, offset), new Vector2(t, _armLength));
            Set(_down, new Vector2(0f, -offset), new Vector2(t, _armLength));
            Set(_dot, Vector2.zero, new Vector2(t, t));
        }

        private static void Set(RectTransform rt, Vector2 pos, Vector2 size)
        {
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        private void Update()
        {
            if (_canvas != null && !Mathf.Approximately(_lastScale, _canvas.scaleFactor))
                Layout();
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
