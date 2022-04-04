
public class EnemyHealthSystem 
{
    private int health;

    public EnemyHealthSystem(int health)
    {
        this.health = health;
    }
    
    public void Damage(int damage)
    {
        health -= damage;
    }


}
