using Godot;
using System;

public class GameEnd : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    [Export]
    protected string nh; 
    AudioStreamPlayer player, key; 
    Label inspireLabel,title;
    Control parentHide; 
    public override void _Ready()
    {
        player = this.GetNode("BreathPlayer")as AudioStreamPlayer;
        key = this.GetNode("KeyPlayer") as AudioStreamPlayer;
        parentHide = this.GetNode("Control") as Control; 
        inspireLabel = this.GetNode("Control/Button/Label") as Label;
        title = this.GetNode("Label") as Label; 
        var tween = CreateTween();
        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { "A"}));
        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { "V" }));

        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { "E" }));

        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { "L" }));

        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { "A" }));

        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { "I" }));
        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { nh }));

        tween.TweenInterval(0.6f);
        tween.TweenCallback(this, "AddLetter", new Godot.Collections.Array(new string[] { "O" }));

        tween.TweenInterval(0.6f);
        tween.TweenProperty(parentHide, "modulate", new Color(1, 1, 1, 1), 0.5f);
        tween.TweenCallback(this, "StartEnd");

    }

    private void AddLetter(string letter)
    {
        title.Text += letter;
        key.Play(); 
    }


    private void StartEnd()
    {
       player.Play();
    }


    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (player.GetPlaybackPosition() < 2f)
        {
            inspireLabel.Text = "Inhale.";
        }
        else if (player.GetPlaybackPosition() < 4f)
        {
            inspireLabel.Text = "Inhale.."; 
        }

        else if (player.GetPlaybackPosition() < 6f)
        {
            inspireLabel.Text = "Inhale..."; 
        }
        else if (player.GetPlaybackPosition() < 7f)
        {
            inspireLabel.Text = "Exhale.";
        }
        else if (player.GetPlaybackPosition() < 8.5f)
        {
            inspireLabel.Text = "Exhale.."; 
        }
        else if (player.GetPlaybackPosition() < 10f)
        {
            inspireLabel.Text = "Exhale..."; 
        }
    }
}
