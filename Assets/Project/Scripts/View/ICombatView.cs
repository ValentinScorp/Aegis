using Aegis.Core;

namespace Aegis.View
{
    public interface ICombatView
    {
        void Bind(Unit unit);
        void Unbind();
    }
}