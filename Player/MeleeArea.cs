using Godot;
using System;

public class MeleeArea : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    CPUParticles2D graphicEffect;

    protected float timer = -1f; 
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("area_entered", this, "OnAreaEntered"); 
        graphicEffect = this.GetNodeInChildren<CPUParticles2D>();

    }


    private void OnAreaEntered(Area2D other)
    {
        if (other is EnemyTrigger)
        {
            (other as EnemyTrigger).Hit(); 
        }
        else if (other is Bullet)
        {
            (other as Bullet).CallDespawn(); 
        }
        else if (other is BulletThrower)
        {
            (other as BulletThrower).Hit(); 
        }
    }
    public void ActiveMeleeArea()
    {
        this.Monitorable = true;
        this.Monitoring = true;
        
        this.graphicEffect.Emitting = true;
        this.timer = 0.4f; 
        
    }



    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (timer > 0)
        {
            this.timer -= delta;
            if (timer <= 0)
            {
                this.Monitorable = false;
                this.Monitoring = false; 
            }
        }
    }
}
