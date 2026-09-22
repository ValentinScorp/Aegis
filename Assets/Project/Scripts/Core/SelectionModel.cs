using System;
using UnityEngine;

namespace Aegis.Core
{
    public class SelectionModel
    {
        private Unit _selectedUnit;
        public Unit Selected {
            get => _selectedUnit;
            private set => _selectedUnit = value;
        }
        public event Action<Unit> Changed;

        public void Select(Unit unit)
        {
            if (unit == _selectedUnit) return;
            _selectedUnit?.Select(false);
            _selectedUnit = unit;
            _selectedUnit?.Select(true);
            Changed?.Invoke(_selectedUnit);
            Debug.Log("Invoke Select!");
        }

        public void Clear() => Select(null);
    }
}