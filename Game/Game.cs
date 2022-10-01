using Godot;
using System;
using System.Collections.Generic;

public class Game : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    public enum GenerationType
    {
        HARDER,
        SOFTER,
        INITIAL,
        SPECIAL
    }

    private LineMapGenerator generator;
    private Player player;
    private Portal portal;
    private Label timeLabel;

    private float gameTimer = 10;
    private int indexTimer = 10;
    [Export]
    protected PackedScene enemyScene; 

    [Export]
    private int initialWorldSizeX = 20;
    [Export]
    private int initialWorldSizeY = 20;

    [Export]
    private int addWorldSizeX = 4;
    [Export]
    private int addWorldSizeY = 2;
    [Export]
    protected float enemiesByRound = 0.6f; 

    protected CanvasItem heartsLayout;
    protected CanvasItem[] heartsUI; 

    private const int maxWorldSizeX = 73;
    private const int maxWorldSizeY = 40;

    private int life = 3;
    private int numberOfGenerationsPassed = 0; 

    protected SceneTreeTween heartsTween;

    protected List<Enemy> activeEnemies;
    protected float enemies = 0; 
    
    public override void _Ready()
    {
        heartsLayout = GetNode("CanvasLayer/Control/HeartsLayout") as CanvasItem;
        heartsUI = new CanvasItem[heartsLayout.GetChildCount()]; 
        for (int i = 0; i< heartsLayout.GetChildCount(); i++)
        {
            heartsUI[i] = heartsLayout.GetChild(i) as CanvasItem;
        }

        activeEnemies = new List<Enemy>();
        
        
        generator = this.GetNodeInChildren<LineMapGenerator>();
        generator.worldMapSize = new Vector2(initialWorldSizeX, initialWorldSizeY); 
        player = this.GetNodeInChildren<Player>();
        portal = this.GetNodeInChildren<Portal>();
        timeLabel = this.GetNodeInChildren<Label>(true); 
        generator.Connect("MapGenerated", this, "RecolocateWhenNewMap");
        portal.Connect("PortalTouched", this, "GenerateNewMap"); 
        GenerateNewMap(GenerationType.INITIAL);
    }

    protected void GenerateNewMap()
    {
        numberOfGenerationsPassed++; 
        GenerateNewMap(GenerationType.HARDER); 
    }

    protected void GenerateBadNewMap()
    {
        heartsTween?.Kill();

        for(int i = 0; i< activeEnemies.Count; i++)
        {
            activeEnemies[i].QueueFree(); 
        }

        activeEnemies.Clear(); 
        heartsTween = CreateTween();


        heartsTween.TweenProperty(heartsLayout, "modulate", new Color(1, 1, 1, 1), 0.3f);

        float alpha = 1; 
        for (int i = 0; i< 10; i++)
        {
            var newAlpha = alpha;
            heartsTween.TweenProperty(heartsUI[life - 1], "modulate", new Color(1, 1, 1, newAlpha), 0.1f); 
            alpha = alpha > 0.5f ? 0 : 1; 
        }
       
        heartsTween.TweenInterval(0.2f);
        heartsTween.TweenProperty(heartsLayout, "modulate", new Color(1, 1, 1, 0), 0.3f);

        life--;
        if (life > 0)
        {

            GenerateNewMap(GenerationType.SOFTER);

        }
        else
        {
            GameOver();
        }
    }

    protected void GameOver()
    {
        if (Menu.maxScore < numberOfGenerationsPassed)
        {
            Menu.maxScore = numberOfGenerationsPassed;
        }

        GetTree().ChangeScene("res://Menu/Menu.tscn"); 
    }
    protected void GenerateNewMap(GenerationType generation)
    {
      
        if (generation == GenerationType.HARDER)
        {
            generator.worldMapSize.x = Mathf.Clamp(generator.worldMapSize.x + addWorldSizeX, 0, maxWorldSizeX);
            generator.worldMapSize.y = Mathf.Clamp(generator.worldMapSize.y + addWorldSizeY, 0, maxWorldSizeY);
        }
        else if (generation == GenerationType.SOFTER)
        {
            generator.worldMapSize.x = Mathf.Clamp(generator.worldMapSize.x - addWorldSizeX, 0, maxWorldSizeX);
            generator.worldMapSize.y = Mathf.Clamp(generator.worldMapSize.y - addWorldSizeY, 0, maxWorldSizeY);
        }

        generator.InitializeAndGenerateMap();
        GenerateEnemies();
        if (generation == GenerationType.INITIAL)
        {
            OnAllEnemiesCleaned();
        }
    }

    protected void GenerateEnemies()
    {

        for (int i = 0; i< enemies; i++)
        {
            var newEnemy = enemyScene.Instance() as Enemy;
            newEnemy.GlobalPosition = generator.GetRandomFloorPoint();
            AddChild(newEnemy);
            newEnemy.Connect("CallEnemyDown", this, "OnEnemyDown"); 
            activeEnemies.Add(newEnemy); 
        }
                enemies += enemiesByRound; 

    }

    protected void OnEnemyDown(Enemy enemy)
    {
        activeEnemies.Remove(enemy); 
        if (activeEnemies.Count == 0)
        {
            OnAllEnemiesCleaned(); 
        }
    }
    protected void OnAllEnemiesCleaned()
    {
        portal.SetActive(); 
    }
    protected void RecolocateWhenNewMap(Vector2 startPosition, Vector2 endPosition)
    {
        gameTimer = 10;
        indexTimer = 10;
        timeLabel.Text = "10";
        timeLabel.Modulate = new Color(1, 0.5f, 0.5f, 0.01f); 
        player.RecolocateWhenNewMap(startPosition);
        portal.RecolocateWhenNewMap(endPosition); 
    }

   
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        if (Input.IsActionJustPressed("test"))
        {
            generator.InitializeAndGenerateMap(); 
        }
        gameTimer -= delta; 

        if (gameTimer < indexTimer - 1)
        {
            indexTimer--;
            timeLabel.Text = indexTimer.ToString();
            timeLabel.Modulate = new Color(1, 0.5f, 0.5f, 1 - (indexTimer * 0.1f)); 
            if (indexTimer <= 0)
            {
                GenerateBadNewMap(); 
            }
        }
     
        for (int i = 0; i< activeEnemies.Count; i++)
        {
            activeEnemies[i].SetTarget(player.Position); 
        }


    }
}
