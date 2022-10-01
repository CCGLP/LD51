using Godot;
using System;

public class Enemy : KinematicBody2D
{
    [Signal]
    public delegate void CallEnemyDown(Enemy enemy); 
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private NavigationAgent2D agent;
    protected Vector2 velocity; 
    protected int actualIndex = 0; 
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        agent = this.GetNodeInChildren<NavigationAgent2D>();
        agent.Connect("velocity_computed", this, "OnVelocityComputed");
        var random = new RandomNumberGenerator();
        random.Randomize();
        agent.MaxSpeed = random.RandfRange(agent.MaxSpeed / 2, agent.MaxSpeed); 

    }

    public void SetTarget(Vector2 target)
    {
        agent.SetTargetLocation(target); 
    }



    public void Hit()
    {
        EmitSignal("CallEnemyDown", this);
        QueueFree(); 
    }

    private int framesStuck = 0;
    private float mod = 1; 
    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (agent.IsNavigationFinished())
        {
            return; 
        }

        var targetPos = agent.GetNextLocation();
        var direction = GlobalPosition.DirectionTo(targetPos);
        if (direction.x < 0.01f && direction.x > -0.01f)
        {
            direction.x = 0.1f * mod;
            framesStuck++;
            if (framesStuck > 10)
            {
                GD.Print("Stuck");
                mod *= -1.2f;
                framesStuck = 0; 
            }
        }
        else
        {
            framesStuck = 0;
            mod = 1.0f;
        }

        if (direction.y < 0.01f && direction.y > -0.01f)
        {
            direction.y = 0.1f * mod;
            framesStuck++; 
            if (framesStuck > 10)
            {
                GD.Print("Stuck"); 
                mod *= -1.2f;
                framesStuck = 0; 
            }
            
        }
        else
        {
            framesStuck = 0;
            mod = 1.0f; 
        }
        var velocity = direction * agent.MaxSpeed;
        
        MoveAndSlide(velocity); 

    }

    protected void OnVelocityComputed(Vector2 velocity)
    {
        this.velocity = velocity; 
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
