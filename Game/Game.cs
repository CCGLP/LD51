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
    protected PackedScene bulletTrowerScene;
    [Export]
    protected PackedScene textUp;
    [Export]
    protected PackedScene hopeScene;
    [Export]
    protected PackedScene heartScene; 

    [Export]
    private int initialWorldSizeX = 20;
    [Export]
    private int initialWorldSizeY = 20;

    [Export]
    private int addWorldSizeX = 4;
    [Export]
    private int addWorldSizeY = 2;

    [Export]
    protected List<int> numberOfEnemiesPerRound;
    [Export]
    protected List<int> numberOfBulletThrowersPerRound;

    protected AudioStreamPlayer interludeAudio, clockAudio;
    protected Control InterludePanel; 

    protected CanvasItem heartsLayout, hopeLayout, mouseTutorialLayout;
    protected CanvasItem[] heartsUI;
    protected Label[] hopesUI; 
   

    private const int maxWorldSizeX = 73;
    private const int maxWorldSizeY = 40;

    private int life = 3;
    private int numberOfGenerationsPassed = 0; 

    protected SceneTreeTween heartsTween;

    protected List<Enemy> activeEnemies;
    protected List<BulletThrower> activeBulletThrowers; 
    protected float enemies = 0;
    protected float bulletThrowers = 0;
    protected HopeRecolectable actualHope;
    protected HeartRecolectable actualHeart; 
    protected int hopeIndex = 0; 

    protected RandomNumberGenerator random;
    public override void _Ready()
    {
        random = new RandomNumberGenerator();
        random.Randomize(); 
        heartsLayout = GetNode("CanvasLayer/Control/HeartsLayout") as CanvasItem;
        heartsUI = new CanvasItem[heartsLayout.GetChildCount()]; 
        for (int i = 0; i< heartsLayout.GetChildCount(); i++)
        {
            heartsUI[i] = heartsLayout.GetChild(i) as CanvasItem;
        }

        hopeLayout = GetNode("CanvasLayer/Control/HopeLayout") as CanvasItem;
        hopesUI = new Label[hopeLayout.GetChildCount()]; 
        for (int i = 0; i< hopesUI.Length; i++)
        {
            hopesUI[i] = hopeLayout.GetChild(i) as Label;
        }

        mouseTutorialLayout = GetNode("CanvasLayer/Control/MousePanel") as CanvasItem; 

        activeEnemies = new List<Enemy>();
        activeBulletThrowers = new List<BulletThrower>();

        interludeAudio = GetNode("InterludePlayer") as AudioStreamPlayer;
        clockAudio = GetNode("ClockPlayer") as AudioStreamPlayer; 
        InterludePanel = GetNode("CanvasLayer/Control/InterludePanel") as Control; 
        generator = this.GetNodeInChildren<LineMapGenerator>();
        generator.worldMapSize = new Vector2(initialWorldSizeX, initialWorldSizeY); 
        player = this.GetNodeInChildren<Player>();
        player.Connect("HitPlayer", this, "OnHitPlayer"); 
        portal = this.GetNodeInChildren<Portal>();
        timeLabel = this.GetNodeInChildren<Label>(true); 
        generator.Connect("MapGenerated", this, "RecolocateWhenNewMap");
        portal.Connect("PortalTouched", this, "GenerateNewMap"); 
        GenerateNewMap(GenerationType.INITIAL);
    }

    public void OnHitPlayer()
    {
        gameTimer -= 0.3f;

        var newText = textUp.Instance() as TextUp;
        newText.Text = "-0.3s"; 
        newText.RectGlobalPosition = player.GlobalPosition;
        AddChild(newText); 
        //DOstuff
    }

    protected void GenerateNewMap()
    {
        numberOfGenerationsPassed++;
        if (hopeIndex < 4)
            GenerateNewMap(GenerationType.HARDER);
        else
            GenerateNewMap(GenerationType.SPECIAL);
    }

    protected void GenerateBadNewMap()
    {
        heartsTween?.Kill();
        gameTimer = 11;

        player.RealStun(); 

        for(int i = 0; i< activeEnemies.Count; i++)
        {
            activeEnemies[i].QueueFree(); 
        }

        for (int i = 0; i< activeBulletThrowers.Count; i++)
        {
            activeBulletThrowers[i].QueueFree(); 
        }

        activeBulletThrowers.Clear(); 
        activeEnemies.Clear(); 
        heartsTween = CreateTween();

        heartsTween.TweenProperty(InterludePanel, "modulate", new Color(1, 1, 1, 1), 0.6f);
        
        heartsTween.TweenCallback(interludeAudio, "play"); 
        heartsTween.TweenInterval(2.0f);
        heartsTween.TweenCallback(this, "OnEndBadTransition");
        heartsTween.TweenProperty(InterludePanel, "modulate", new Color(1, 1, 1, 0), 0.3f);
        heartsTween.TweenInterval(0.2f);
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
        
    }

    protected void OnEndBadTransition()
    {
        life--;
        player.realStun = false; 
        if (life > 0)
        {

            GenerateNewMap(GenerationType.SOFTER);

            GenerateHope(); 
        }
        else
        {
            GameOver();
        }
    }


    protected void GenerateHope()
    {
        if (hopeIndex < 4)
        {
            var hope = hopeScene.Instance() as HopeRecolectable;
            AddChild(hope);
            hope.Connect("RecolectedHope", this, "OnRecolectedHope");
            hope.Init(hopesUI[hopeIndex].Text, generator.GetRandomFloorPoint());
            actualHope = hope;
        }
        
    }

    private void OnRecolectedHope()
    {
        actualHope = null; 
        heartsTween = CreateTween(); 
        heartsTween.TweenProperty(hopeLayout, "modulate", new Color(1, 1, 1, 1), 0.3f);

        float alpha = 1;
        for (int i = 0; i < 11; i++)
        {
            var newAlpha = alpha;
            heartsTween.TweenProperty(hopesUI[hopeIndex], "modulate", new Color(1, 1, 1, newAlpha), 0.1f);
            alpha = alpha > 0.5f ? 0 : 1;
        }

        heartsTween.TweenInterval(1.2f);
        heartsTween.TweenProperty(hopeLayout, "modulate", new Color(1, 1, 1, 0), 0.3f);
        hopeIndex++;

    }
    protected void GameOver()
    {
        if (Menu.maxScore < numberOfGenerationsPassed)
        {
            Menu.maxScore = numberOfGenerationsPassed;
        }
        BulletThrower.bulletPool = null; 

        GetTree().ChangeScene("res://Menu/Menu.tscn"); 
    }
    protected void GenerateNewMap(GenerationType generation)
    {
        actualHope?.QueueFree();
        actualHeart?.QueueFree();
        actualHope = null;
        actualHeart = null; 
        if (generation != GenerationType.INITIAL)
        {
            mouseTutorialLayout.Visible = false; 
        }
        var bullets = this.GetNodesInChildren<Bullet>(); 

        if (generation == GenerationType.SPECIAL)
        {
            GetTree().ChangeScene("res://Menu/GameEnd.tscn"); 

            return; 
        }
        for (int i = 0; i< bullets.Count; i++)
        {
            bullets[i].CallDespawn(); 
        }
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
        if (generation == GenerationType.HARDER)
        {
            GenerateHeart();
        }
        if (generation == GenerationType.INITIAL)
        {
            OnAllEnemiesCleaned();
        }
    }

    protected void GenerateHeart()
    {
        if (life < 3 && random.Randf() < 0.5f)
        {
            actualHeart = heartScene.Instance() as HeartRecolectable;
            actualHeart.GlobalPosition = generator.GetRandomFloorPoint();
            actualHeart.Connect("OnHeartRecolected", this, "OnHeartRecolected");
            AddChild(actualHeart); 
        }
    }

    protected void OnHeartRecolected()
    {
        actualHeart = null;
        life++;
        heartsTween = CreateTween(); 
        heartsTween.TweenProperty(heartsLayout, "modulate", new Color(1, 1, 1, 1), 0.3f);

        float alpha = 1;
        for (int i = 0; i < 11; i++)
        {
            var newAlpha = alpha;
            heartsTween.TweenProperty(heartsUI[life - 1], "modulate", new Color(1, 1, 1, newAlpha), 0.1f);
            alpha = alpha > 0.5f ? 0 : 1;
        }

        heartsTween.TweenInterval(0.2f);
        heartsTween.TweenProperty(heartsLayout, "modulate", new Color(1, 1, 1, 0), 0.3f);
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

        for (int i = 0; i< bulletThrowers; i++)
        {
            var newBulletThrower = bulletTrowerScene.Instance() as BulletThrower; 
            newBulletThrower.GlobalPosition = generator.GetRandomFloorPoint();
            AddChild(newBulletThrower);
            newBulletThrower.Connect("BulletThrowerDown", this, "OnBulletThrowerDown");
            newBulletThrower.SetTarget(player); 
            activeBulletThrowers.Add(newBulletThrower); 
        }
        var normalizedNumberOfGenerations = Mathf.Clamp(numberOfGenerationsPassed+1, 0, numberOfEnemiesPerRound.Count-1);
 
        enemies = numberOfEnemiesPerRound[normalizedNumberOfGenerations];
        if (normalizedNumberOfGenerations == numberOfGenerationsPassed+1)
            bulletThrowers = numberOfBulletThrowersPerRound[normalizedNumberOfGenerations];
        else
            bulletThrowers += numberOfBulletThrowersPerRound[normalizedNumberOfGenerations]; 
    }
    
    protected void OnBulletThrowerDown(BulletThrower bulletThrower)
    {
        activeBulletThrowers.Remove(bulletThrower); 
        if (activeEnemies.Count == 0 && activeBulletThrowers.Count == 0)
        {
            OnAllEnemiesCleaned(); 
        }
    }
    protected void OnEnemyDown(Enemy enemy)
    {
        activeEnemies.Remove(enemy); 
        if (activeEnemies.Count == 0 && activeBulletThrowers.Count == 0)
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
        if (heartsTween == null || !heartsTween.IsRunning() )
        {
            if (numberOfGenerationsPassed > 0)
            {
                gameTimer -= delta;

                if (gameTimer < indexTimer - 1)
                {
                    clockAudio.PitchScale = random.RandfRange(0.9f, 1.1f);
                    clockAudio.Play();
                    indexTimer--;
                    timeLabel.Text = indexTimer.ToString();
                    timeLabel.Modulate = new Color(1, 0.5f, 0.5f, 1 - (indexTimer * 0.1f));
                    if (indexTimer <= 0)
                    {
                        GenerateBadNewMap();
                    }
                }
            }
        }
     
        for (int i = 0; i< activeEnemies.Count; i++)
        {
            activeEnemies[i].SetTarget(player.Position); 
        }


    }
}
