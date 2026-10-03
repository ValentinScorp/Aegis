using Aegis.Core;

namespace Aegis.View
{
    public class BodyDollView : HealthView
    {
        private BodyPartView[] _parts;

        private void Awake()
        {
            _parts = GetComponentsInChildren<BodyPartView>(true);
            foreach (var p in _parts) p.Apply(1f, 1f);
        }
        public override void Refresh(BodyHealth body)
        {
            foreach (BodyPartId part in System.Enum.GetValues(typeof(BodyPartId)))
                OnPartChanged(part, body.GetCurrent(part), body.GetMax(part));
        }
        public override void OnPartChanged(BodyPartId part, float current, float max)
        {
            foreach (var p in _parts)
                if (p.Part == part) { p.Apply(current, max); return; }
        }
    }
}