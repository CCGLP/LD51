using Godot;
using System;

public class HeartRecolectable : Area2D
{
    [Signal]
    public delegate void OnHeartRecolected(); 
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("body_entered", this, "OnBodyEntered");

    }

    private void OnBodyEntered(Node node)
    {
        if (node is Player)
        {
            EmitSignal("OnHeartRecolected");
            QueueFree(); 
        }
    }
 
}
