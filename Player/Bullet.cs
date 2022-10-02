using Godot;
using System;

public class Bullet : SpatialDespawnable
{

    [Export]
    protected float bulletSpeed = 2000;
    [Export]
    protected Color normalEnemyColor;
    [Export]
    protected Color specialEnemyColor; 
    protected Vector2 velocity; 
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Visible = false;
        SetProcess(false);
        Monitorable = false;
        Monitoring = false;
    }

    public void InitBullet(Vector2 position, Vector2 target)
    {
        spawned = true; 
        this.GlobalPosition = position;
        timer = 0; 
        this.LookAt(target); 
        velocity = (target - position).Normalized() * bulletSpeed;
        Monitorable = true;
        Monitoring = true;
        Visible = true;
        this.Connect("body_entered", this, "OnBodyEntered");
        this.Connect("area_entered", this, "OnAreaEntered");
        this.Modulate = new Color(1, 1, 1, 1); 
        SetProcess(true); 
    }

    public void InitBulletAsEnemy(bool special)
    {
        this.Modulate = special ? specialEnemyColor : normalEnemyColor; 
    }

    private void OnBodyEntered(Node node)
    {
       if (node is Player)
        {
            (node as Player).Stun(); 
        }
        this.CallDespawn(); 
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is EnemyTrigger)
        {
            (area as EnemyTrigger).Hit();
            this.CallDespawn(); 
        }
        else if (area is BulletThrower)
        {
            (area as BulletThrower).Hit();
            this.CallDespawn(); 
        }
        else if (area is Bullet)
        {
            (area as Bullet).CallDespawn();
            this.CallDespawn(); 
        }
        
    }

    public override void CallDespawn()
    {
        if (spawned)
        {
            base.CallDespawn();
            this.Disconnect("body_entered", this, "OnBodyEntered");
            this.Disconnect("area_entered", this, "OnAreaEntered");
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        this.Position += delta * velocity; 
    }
}
