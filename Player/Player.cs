using Godot;
using System;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Signal]
    public delegate void HitPlayer(); 
    // Called when the node enters the scene tree for the first time.
    [Export]
    protected float speed = 100; 
    protected Vector2 actualVelocity;

    protected Node2D pivotWeaponPoint;
    protected Node2D bulletSpawnPoint;
    protected MeleeArea meleeArea; 
    protected Pool2D<Bullet> bulletPool;

    protected float stunTime = 3;
    protected float stunTimer = 0;
    protected float stunSpeedReduction = 200; 
    protected float stunModifier = 1;
    protected float stunAddModifier = 0.2f;
    public bool realStun = false;

    
    protected float meleeAreaTimer = 1.4f; 
    public override void _Ready()
    {
        bulletPool = new Pool2D<Bullet>();
        bulletPool.Init("res://Player/Bullet.tscn", 30, this.GetParent()); 
        actualVelocity = Vector2.Zero;
        pivotWeaponPoint = GetNode("WeaponPivot") as Node2D;
        bulletSpawnPoint = GetNode("WeaponPivot/SpawnBulletPoint") as Node2D; 
        meleeArea = this.GetNodeInChildren<MeleeArea>();
    }

    public void RealStun()
    {
        this.actualVelocity = Vector2.Zero;
        realStun = true; 
    }
    public void Stun()
    {
        EmitSignal("HitPlayer"); 
    }

    public void RecolocateWhenNewMap(Vector2 newPosition)
    {
        this.Position = newPosition; 
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var lookTransform = Vector2.Zero; 
        if (Input.GetConnectedJoypads().Count > 0)
        {
            lookTransform = new Vector2(Input.GetActionStrength("right_stick_right") - Input.GetActionStrength("right_stick_left"), -Input.GetActionStrength("right_stick_up") + Input.GetActionStrength("right_stick_down"));

            lookTransform = lookTransform * 100;

            lookTransform = this.GlobalPosition + lookTransform;
        }
        else
        {
            lookTransform = GetGlobalMousePosition(); 
        }
        pivotWeaponPoint.LookAt(lookTransform);


        var speed = this.speed; 
        if (stunTimer > 0)
        {
            stunTimer -= delta;
            speed = Mathf.Clamp(speed - stunSpeedReduction * stunModifier, 100, this.speed) ;
        }

        if (meleeAreaTimer > 0)
        {
            meleeAreaTimer -= delta; 
        }

        if (!realStun)
        {

            if (Input.IsActionPressed("ui_right"))
            {
                actualVelocity.x = speed;
            }
            else if (Input.IsActionPressed("ui_left"))
            {
                actualVelocity.x = -speed;
            }
            else
            {
                actualVelocity.x = 0;
            }

            if (Input.IsActionPressed("ui_up"))
            {
                actualVelocity.y = -speed;
            }
            else if (Input.IsActionPressed("ui_down"))
            {
                actualVelocity.y = speed;
            }
            else
            {
                actualVelocity.y = 0;
            }

            if (Input.IsActionJustPressed("shoot"))
            {
                var bullet = bulletPool.Instantiate();
                bullet.InitBullet(bulletSpawnPoint.GlobalPosition, lookTransform);
            }
            if (meleeAreaTimer <= 0f && Input.IsActionJustPressed("melee"))
            {
                meleeAreaTimer = 1.4f;
                meleeArea.ActiveMeleeArea(); 
            }
        }

    }


    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        MoveAndSlide(actualVelocity); 
    }
}
