
using System.Timers;

public class Enemy
{
    public string Name { get; }
    public int Health { get; private set; }
    public int AttackPower { get; }

    private System.Timers.Timer attackTimer;
    private Player targetPlayer;

    public Enemy(string name, int health, int attackPower)
    {
        Name = name;
        Health = health;
        AttackPower = attackPower;

        attackTimer = new System.Timers.Timer(3000); 
        attackTimer.Elapsed += AttackTimerElapsed;
        attackTimer.AutoReset = true;
        attackTimer.Start();
    }

    public void Attack(Player player)
    {
        targetPlayer = player;
        DamagePlayer();
    }

    private void AttackTimerElapsed(object sender, ElapsedEventArgs e)
    {
        DamagePlayer();
    }

    private void DamagePlayer()
    {
        if (targetPlayer is not null)
        {
            targetPlayer.Damage(AttackPower);
            Console.WriteLine($"{Name} attacked you for {AttackPower} damage!");
        }
    }
}