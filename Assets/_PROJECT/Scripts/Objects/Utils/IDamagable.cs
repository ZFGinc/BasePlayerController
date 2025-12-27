namespace ZFGinc.Objects.Utils
{
    public interface IDamageable
    {
        public void TakeDamage(int damage);

        public void TakeHeal(int heal);
    }
}