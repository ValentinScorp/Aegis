namespace Aegis.Core.AI
{
    public static class SoldierTreeFactory
    {
        private const float ReacquireMargin = 3f;

        public static BTNode Build() =>
            new Selector(
                // 1. Наказ гравця має вищий пріоритет за бій (як було у Walk)
                new Sequence(
                    new Condition(c => c.Unit.PlayerOrder.HasValue),
                    new WalkOrderAction()),

                // 2. Є ціль у периметрі
                new Sequence(
                    new Condition(c => c.Unit.CurrentTarget != null),
                    new Selector(
                        new Sequence(
                            new Condition(c => c.Unit.CanAttack(c.Unit.CurrentTarget)),
                            new AttackAction()),
                        new Sequence(
                            new Condition(CanChase),
                            new ChaseAction()))),

                // 3. Далеко від дому: повертаємось
                new Sequence(
                    new Condition(c => !c.Unit.IsInPerimeter(c.Unit.Position, 1.5f)),
                    new ReturnHomeAction()),

                // 4. Інакше стоїмо
                new IdleAction());

        // Гістерезис: переслідуємо до 20 м, нове переслідування починаємо лише ближче ніж 17 м
        private static bool CanChase(BTContext c)
        {
            var u = c.Unit;
            float limit = u.IsChasing ? u.LeashRadius : u.LeashRadius - ReacquireMargin;
            return u.IsInPerimeter(u.Position, limit);
        }
    }
}