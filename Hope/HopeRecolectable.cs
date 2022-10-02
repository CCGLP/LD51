using Godot;
using System;

public class HopeRecolectable : Area2D
{
    [Signal]
    public delegate void RecolectedHope();

    public void Init(string letter, Vector2 position)
    {
        var label = this.GetNodeInChildren<Label>();
        this.GlobalPosition = position;
        label.Text = letter;
        this.Connect("body_entered", this, "OnBodyEntered");
    }

    protected void OnBodyEntered(Node body)
    {
        if (body is Player)
        {
            EmitSignal("RecolectedHope");
            QueueFree(); 
        }
    }



}
