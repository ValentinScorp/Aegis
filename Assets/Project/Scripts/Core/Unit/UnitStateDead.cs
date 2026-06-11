using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateDead : IUnitState
    {
        private Unit _self;

        public UnitStateDead(Unit unit)
        {
            _self = unit;
        }
        public void Enter()
        {
            // Debug.Log("Enter Dead");            
        }
        public void Exit()
        {
            // Debug.Log("Exit Dead");            
        }
        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {
            
        }
        public void OnActionsUpdate(float deltaTime)
        {
        }

    }
}