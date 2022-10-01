using Godot;
using System;

public class SpatialDespawnable : Area2D
{
   
    [Signal]
    public delegate void Despawn(Spatial node);

    [Export]
    protected float timeToDie = 5; 

    private float timer = 0; 
 

    public override void _Process(float delta)
    {
        base._Process(delta);
        timer += delta;
        if (timer > timeToDie)
        {
            CallDespawn();
        }
    }

    public virtual void CallDespawn()
    {
        timer = 0;
        Translate(new Vector2(3000, 3000));
        EmitSignal("Despawn", this);
    }
}
