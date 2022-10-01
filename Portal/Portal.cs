using Godot;
using System;

public class Portal : Area2D
{

    [Signal]
    public delegate void PortalTouched();

    protected bool isActive = false;
    [Export]
    protected float rotationSpeed = 10; 
    public override void _Ready()
    {
        this.Connect("body_entered", this, "OnBodyEntered");
    }

    public void SetActive()
    {
        isActive = true;
        this.Modulate = new Color(1, 0, 0, 1); 
    }
    public void OnBodyEntered(Node body) {

        if (body is Player && isActive)
        {
            CallDeferred("EmitPortalTouched");
            isActive = false; 
        }
    }

    public void EmitPortalTouched()
    {
        EmitSignal("PortalTouched");
    }
    public void RecolocateWhenNewMap(Vector2 newPosition)
    {
        isActive = false;
        this.Modulate = new Color(1, 1, 1, 1); 
        this.Position = newPosition;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (isActive)
        {
            this.Rotate(delta * rotationSpeed);
        }
    }
}
