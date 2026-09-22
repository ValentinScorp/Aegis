using UnityEngine;
using TMPro;
using Aegis.Core;
using System.Collections.Generic;
using Aegis.Services;

namespace Aegis.View
{
    public class BodyHealthView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _headText;
        [SerializeField] private TMP_Text _torsoText;
        [SerializeField] private TMP_Text _armLeftText;
        [SerializeField] private TMP_Text _armRightText;
        [SerializeField] private TMP_Text _leftLegText;
        [SerializeField] private TMP_Text _legRightText;
        [SerializeField] private SelectionController _selectionController;
        private Dictionary<BodyPartId, TMP_Text> _labels;
        private SelectionModel _selectionModel;
        private Unit _boundUnit;

        private void Awake()
        {
            if (_selectionController != null) {
                _selectionModel = _selectionController.Model;
            }
            
            _labels = new Dictionary<BodyPartId, TMP_Text>
            {
                { BodyPartId.Head,     _headText },
                { BodyPartId.Torso,    _torsoText },
                { BodyPartId.ArmLeft,  _armLeftText },
                { BodyPartId.ArmRight, _armRightText },
                { BodyPartId.LegLeft,  _leftLegText },
                { BodyPartId.LegRight, _legRightText }
            };
        }
        private void OnEnable()
        {
            Debug.Log("OnEnable");
            if (_selectionModel != null) {
                Debug.Log("Binding");
                _selectionModel.Changed += Bind;
            }
        }

        private void OnDisable()
        {
            if (_selectionModel != null)
                _selectionModel.Changed -= Bind;
        }
        public void Bind(Unit unit)
        {
            Unbind();
            if (unit == null) return;

            _boundUnit = unit;
            unit.BodyPartHealthChanged += OnPartChanged;
            unit.Died += OnDepleted;
            RefreshAll(unit.BodyHealth);
            gameObject.SetActive(true);
        }
        public void Unbind()
        {
            if (_boundUnit == null) return;

            _boundUnit.BodyPartHealthChanged -= OnPartChanged;
            _boundUnit.Died -= OnDepleted;
            _boundUnit = null;
        }

        public void OnPartChanged(BodyPartId part, float current, float max)
        {
            Debug.Log("Part changed");
            if (_labels.TryGetValue(part, out var text))
                text.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
        }

        public void RefreshAll(BodyHealth body)
        {
            foreach (BodyPartId part in System.Enum.GetValues(typeof(BodyPartId))) {
                OnPartChanged(part, body.GetCurrent(part), body.GetMax(part));
            }
        }

        public void OnDepleted()
        {
            // gameObject.SetActive(false);
        }
    }
}