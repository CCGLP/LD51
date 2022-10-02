using Godot;
using System;

public class TextUp : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var tween = CreateTween();
        tween.SetParallel(true);

        
        tween.TweenProperty(this, "rect_global_position", RectGlobalPosition + Vector2.Up * 100, 0.5f);
        tween.TweenProperty(this, "modulate", new Color(1, 0, 0, 0), 0.6f);

        tween.Connect("finished", this, "Destroy"); 

    }

    private void Destroy()
    {
        QueueFree(); 
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
