namespace Game.Core.Interfaces
{
    public interface IDamageable
    {
        /// <summary>
        /// Получение урона
        /// </summary>
        /// <param name="damage">Урон</param>
        void TakeDamage(float damage);
    }
}
