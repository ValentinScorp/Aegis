namespace Aegis.Core
{
    public enum CameraMode
    {
        Free,        // вільна стратегічна камера (поточна поведінка)
        Follow,      // Діабло-стиль: слідкує за вибраним юнітом, фіксований ізометричний кут
        ThirdPerson  // Odyssey-стиль: за спиною юніта, керування мишею
    }
}
