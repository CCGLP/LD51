using Godot;
using System;

public class EnemyTrigger : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    Enemy parent; 
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        parent = this.GetParent() as Enemy;
        this.Connect("body_entered", this, "OnBodyEntered");
    }

    public void OnBodyEntered(Node node)
    {
        if (node is Player)
        {
            (node as Player).Stun();
            parent.Hit(); 
        }
    }

    public void Hit()
    {
        parent.Hit(); 
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
