using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aegis.Core
{
    public class UnitStateIdle : IUnitState
    {
        public UnitState State => UnitState.Idle;
        private Unit _self;

        public UnitStateIdle(Unit unit)
        {
            _self = unit;
        }
        public void Enter()
        {           
        }
        public void Exit()
        {            
        }
        public void OnInteractionsUpdate(WorldEntity closestTarget)
        {}
        public void OnActionsUpdate(float deltaTime)
        {}
    }
}