using UnityEngine;

namespace Aegis.Core
{
    public class WorldEntity
    {
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }
        public bool isEnemy { get; internal set; }

        public void SetPosition(Vector3 position)
        {
            if (this.position != position) {
                this.position = position;
            }
        }
    }
}
