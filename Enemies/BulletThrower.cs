using Godot;
using System;

public class BulletThrower : Area2D
{
    [Signal]
    public delegate void BulletThrowerDown(BulletThrower enemy); 
    public  static Pool2D<Bullet> bulletPool; 
    AnimatedSprite sprite;
    protected Player target;

    [Export]
    protected Color specialColor;
    [Export]
    private float specialChance = 0.2f;
    private RandomNumberGenerator random;
    private bool special = false; 
    public override void _Ready()
    {
        sprite = this.GetNodeInChildren<AnimatedSprite>();
        random = new RandomNumberGenerator();
        random.Randomize(); 

        if (random.Randf() < specialChance)
        {
            special = true;
            this.Modulate = specialColor;
            sprite.SpeedScale = 1.2f; 
        }
        if (bulletPool == null)
        {
            bulletPool = new Pool2D<Bullet>();
            bulletPool.Init("res://Enemies/BulletEnemie.tscn", 30, GetParent()); 
        }
         
        sprite.Connect("frame_changed", this, "OnFrameChanged"); 
    }

    public void SetTarget(Player player)
    {
        this.target = player; 
    }

    public void OnFrameChanged()
    {
        if (target!= null && !target.realStun)
        {
            if (!special)
            {
                var spaceState = GetWorld2d().DirectSpaceState;
                var result = spaceState.IntersectRay(Position, target.Position);
                if (result["collider"] is Player)
                {

                    var newBullet = bulletPool.Instantiate();
                    newBullet.InitBullet(Position, target.GlobalPosition);
                    newBullet.InitBulletAsEnemy(special); 
                }
            }

            if (special)
            {
                var newBullet = bulletPool.Instantiate();
                newBullet.InitBullet(Position, target.GlobalPosition);
                newBullet.InitBulletAsEnemy(special);
            }
        }
    }

    public void Hit()
    {
        EmitSignal("BulletThrowerDown", this);
        Destroy(); 
    }

    public void Destroy()
    {
        QueueFree(); 
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
