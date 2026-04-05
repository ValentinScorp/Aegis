namespace Aegis.Core
{
    public interface IWorldUpdatable
    {
        void OnInteractionsUpdate();
        void OnActionsUpdate(float deltaTime);
    }
}