namespace Aegis.Core
{
    /// <summary>
    /// Хто "власник" руху юніта в конкретний момент.
    /// Indirect — клік по землі (NavMeshAgent будує шлях, Діабло-стиль).
    /// Direct — прямий ввід гравця кожен кадр (CharacterController, Odyssey-стиль).
    /// </summary>
    public enum UnitControlMode
    {
        Indirect,
        Direct
    }
}
