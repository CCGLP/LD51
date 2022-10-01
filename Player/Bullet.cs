using Godot;
using System;

public class Bullet : SpatialDespawnable
{

    [Export]
    protected float bulletSpeed = 2000;

    protected Vector2 velocity; 
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Visible = false;
        SetProcess(false); 
    }

    public void InitBullet(Vector2 position, Vector2 target)
    {
        this.GlobalPosition = position;

        this.LookAt(target); 
        velocity = (target - position).Normalized() * bulletSpeed;

        Visible = true;
        this.Connect("body_entered", this, "OnBodyEntered");
        this.Connect("area_entered", this, "OnAreaEntered"); 
        SetProcess(true); 
    }


    private void OnBodyEntered(Node node)
    {
       
        this.CallDespawn(); 
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is EnemyTrigger)
        {
            (area as EnemyTrigger).Hit();
            this.CallDespawn(); 
        }
    }

    public override void CallDespawn()
    {
        base.CallDespawn();
        this.Disconnect("body_entered", this, "OnBodyEntered");
        this.Disconnect("area_entered", this, "OnAreaEntered");
    }

    public override void _Process(float delta)
    {
        this.Position += delta * velocity; 
    }
}
