using Godot;
using System;

public class Menu : Control
{

    public static int maxScore = 0;

    protected Button startButton;
    protected Label scoreLabel;

    public override void _Ready()
    {
        scoreLabel = GetNode("Score") as Label;
        scoreLabel.Text = "Max Score: " + maxScore.ToString(); 
        startButton = this.GetNodeInChildren<Button>();
        startButton.Connect("button_down", this, "OnStartClicked"); 
    }


    protected void OnStartClicked()
    {
        GetTree().ChangeScene("res://Game/Game.tscn"); 
    }
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
