using Godot;
using System;
using System.Collections.Generic;

public class LineMapGenerator : Node2D
{

    [Signal]
    public delegate void MapGenerated(Vector2 firstPosition);
    public enum SquareValue
    {
        COLLISION, 
        FLOOR
    }

    protected SquareValue[,] map;

    protected List<Line> lines;

    [Export]
    public Vector2 worldMapSize;

    [Export]
    private int branchNumber;

    [Export]
    private int branchLongitude; 

    [Export]
    private float nodeSize;
    [Export]
    private int numberOfLines; 

    [Export]
    private PackedScene floorScene;

    [Export]
    private PackedScene collisionScene;

    [Export]
    private bool polish = false;

    protected List<Node2D> floors; 

    protected RandomNumberGenerator random;

    protected Node2D parent; 
    public Vector2 MapStartWorldPoint
    {
        get
        {
            return new Vector2(0 - (worldMapSize.x * nodeSize) * 0.5f + nodeSize * 0.5f, (worldMapSize.y * nodeSize) * 0.5f - nodeSize * 0.5f);
        }
    }

    public Vector2 MapEndWorldPoint
    {
        get
        {
            return new Vector2(MapStartWorldPoint.x + worldMapSize.x * nodeSize - nodeSize, MapStartWorldPoint.y - worldMapSize.y * nodeSize + nodeSize);
        }
    }

  
    protected int GetIndexFromWorldPos(Vector2 position)
    {
        for(int i = 0; i < floors.Count; i++)
        {
            if (floors[i].Position.DistanceTo(position) < 32)
            {
                return i; 
            }
        }
        return 0; 
    }
    public void InitializeAndGenerateMap()
    {

        random = new RandomNumberGenerator();
        floors?.Clear(); 
        floors = new List<Node2D>(); 
        random.Randomize();
        CleanPreviousMap(); 
        InitializeParent(); 
        InitializeMap();
        GenerateMap();
        DrawMap();
        EmitSignal("MapGenerated", GetInitialPosition(), GetLastPosition()); 
    }

    protected void CleanPreviousMap()
    {
        if (parent != null)
        {
            parent.QueueFree();
            parent = null; 
        }
    }

    protected void InitializeParent()
    {
        parent = new Node2D();
        parent.Position = Vector2.Zero;
        parent.Scale = Vector2.One;
        AddChild(parent); 
    }
    private void GenerateMap()
    {
        GenerateLines();
        GenerateBranches();
        GenerateLimits();
        if (polish)
        {
            PolishMapDeletingAloneSquares(); 
        }
    }

    private void PolishMapDeletingAloneSquares()
    {
        for (int i = 1; i < map.GetLength(0) -1; i++)
        {
            for (int j = 1; j< map.GetLength(1)-1; j++)
            {
                if (map[i,j] == SquareValue.COLLISION)
                {
                    if (map[i-1, j] == SquareValue.FLOOR && map[i,j-1] == SquareValue.FLOOR && map[i+1,j] == SquareValue.FLOOR && map[i, j+1] == SquareValue.FLOOR
                        && map[i-1, j-1] == SquareValue.FLOOR && map[i+1, j+1] == SquareValue.FLOOR && map[i-1, j+1] == SquareValue.FLOOR && map[i+1, j-1] == SquareValue.FLOOR)
                    {
                        map[i, j] = SquareValue.FLOOR; 
                    }
                }
            }
        }
    }



    #region Lines
    private void GenerateLines()
    {

        for (int i = 1; i < numberOfLines; i++)
        {
            GenerateRandomLine();
        }
        GenerateLastLine();
    }

    private void GenerateRandomLine()
    {
        if (random.Randf() < 0.5f)
        {
            GenerateLineVertical(); 
        }
        else
        {
            GenerateLineHorizontal(); 
        }
    }

    private void GenerateLineVertical()
    {
        uint xPoint = (uint)random.RandiRange(0, (int)worldMapSize.x-1);
        lines.Add(new Line(new Vector2(xPoint, 0), Line.LineType.VERTICAL, nodeSize, worldMapSize));
        for (int j = 0; j < worldMapSize.y; j++)
        {
            map[xPoint, j] = SquareValue.FLOOR;
        }
    }

    private void GenerateLineHorizontal()
    {
        uint yPoint = (uint)random.RandiRange(0, (int)worldMapSize.y-1);
        lines.Add(new Line(new Vector2(0, yPoint), Line.LineType.HORIZONTAL, nodeSize, worldMapSize));
        for (int i = 0; i < worldMapSize.x; i++)
        {
            map[i, yPoint] = SquareValue.FLOOR;
        }
    }

    private void GenerateLastLine()
    {
        int horizontalCount = 0, verticalCount = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            switch (lines[i].Type)
            {
                case Line.LineType.HORIZONTAL:
                    horizontalCount++;
                    break;
                case Line.LineType.VERTICAL:
                    verticalCount++;
                    break;
                default:
                    break;
            }
        }

        GenerateLineWhenCountNeeds(horizontalCount, verticalCount);
    }

    private void GenerateLineWhenCountNeeds(int horizontalCount, int verticalCount)
    {

        if ((horizontalCount == 0 && verticalCount == 0) )
        {
            GenerateRandomLine();
        }
        else if (horizontalCount == 0)
        {
            GenerateLineHorizontal();
        }
        else if (verticalCount == 0)
        {
            GenerateLineVertical();
        }
    }

    #endregion

    #region Branches
    private void GenerateBranches()
    {

        for (int i = 0; i < branchNumber; i++)
        {
            int aux = random.RandiRange(0, lines.Count-1);
            GenerateBranch(lines[aux]);
        }

    }


    private void GenerateBranch(in Line line)
    {

        if (random.RandiRange(0, 100) <= 50)
        {
            GenerateBranchUpRight(line);
        }
        else
        {
            GenerateBranchDownLeft(line);
        }
    }

    private void GenerateBranchUpRight(in Line line)
    {
        int branchIterations = 0;
        Vector2 pos = GetBranchStartPoint(line);
        while (branchIterations < branchLongitude && pos.y < worldMapSize.y - 1 && pos.x < worldMapSize.x - 1)
        {
            branchIterations++;
            if (random.RandiRange(0, 100) <= 50)
            {
                pos.y++;
            }
            else
            {
                pos.x++;
            }
            map[(int)pos.x, (int)pos.y] = SquareValue.FLOOR;

        }
    }

    private void GenerateBranchDownLeft(in Line line)
    {
        int branchIterations = 0;
        Vector2 pos = GetBranchStartPoint(line);

        while (branchIterations < branchLongitude && pos.y > 0 && pos.x > 0)
        {
            branchIterations++;
            if (random.RandiRange(0, 100) <= 50)
            {
                pos.y--;
            }
            else
            {
                pos.x--;
            }
            map[(int)pos.x, (int)pos.y] = SquareValue.FLOOR;

        }
    }

    private Vector2 GetBranchStartPoint(in Line line)
    {
        Vector2 result = new Vector2(0, 0);
        switch (line.Type)
        {
            case Line.LineType.HORIZONTAL:
                result.y = line.StartPoint.y;
                result.x = random.RandiRange(0, (int)worldMapSize.x-1);
                break;
            case Line.LineType.VERTICAL:
                result.y = random.RandiRange(0, (int)worldMapSize.y-1);
                result.x = line.StartPoint.x;
                break;
            default:
                break;
        }
        return result;
    }


    #endregion

    #region Limits
    private void GenerateLimits()
    {
        GenerateHorizontalLimits();
        GenerateVerticalLimits();
    }


    private void GenerateHorizontalLimits()
    {
        InstantiateGroupSquare(collisionScene, new Vector2(0, -worldMapSize.y * 0.5f * nodeSize - nodeSize * 0.5f), new Vector2(worldMapSize.x, 1));
        InstantiateGroupSquare(collisionScene, new Vector2(0, worldMapSize.y *0.5f * nodeSize + nodeSize *0.5f), new Vector2(worldMapSize.x , 1));
    }

    private void GenerateVerticalLimits()
    {
        InstantiateGroupSquare(collisionScene, new Vector2(-worldMapSize.x * 0.5f * nodeSize - nodeSize*0.5f, 0), new Vector2(1,worldMapSize.y+2));
        InstantiateGroupSquare(collisionScene, new Vector2(worldMapSize.x * 0.5f * nodeSize + nodeSize*0.5f, 0), new Vector2(1, worldMapSize.y+2));
    }

    private void InstantiateGroupSquare(PackedScene scene, Vector2 position, Vector2 scale)
    {
        var newScene = (Node2D) scene.Instance();
        newScene.Position = position; 
        newScene.Scale *= scale;
        parent.AddChild(newScene); 
    }

    #endregion



    void DrawMap()
    {
        Vector2 worldPosition = MapStartWorldPoint;
        int index = 0; 
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {

                InstantiateMapSquare(worldPosition, map[i, j]);
                worldPosition.y -= nodeSize;
            }
            worldPosition.y = MapStartWorldPoint.y;
            worldPosition.x += nodeSize;
        }

    }



    

    

    private void InstantiateMapSquare(Vector2 worldPosition, SquareValue mapValue)
    {
        PackedScene objectToInstantiate = mapValue == SquareValue.COLLISION ? collisionScene : floorScene;
        if (objectToInstantiate != null)
        {
            var newScene = (Node2D)objectToInstantiate.Instance();
            newScene.Position = worldPosition;
            parent.AddChild(newScene);

            if (mapValue == SquareValue.FLOOR)
                floors.Add(newScene); 
        }
           
    }


    private void InitializeMap()
    {
        map = new SquareValue[(int)worldMapSize.x, (int)worldMapSize.y];
        lines = new List<Line>();
    }



    public Vector2 GetInitialPosition()
    {
        Vector2 worldPosition = MapStartWorldPoint; 
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j< map.GetLength(1); j++)
            {
                if (map[i, j] == SquareValue.FLOOR)
                {
                    return worldPosition;
                }
                worldPosition.y -= nodeSize; 
            }

            worldPosition.y = MapStartWorldPoint.y;
            worldPosition.x += nodeSize; 
        }

        return worldPosition; 
    }

    public Vector2 GetLastPosition()
    {
        Vector2 worldPosition = MapEndWorldPoint;
        for (int i = map.GetLength(0)-1; i >= 0; i--)
        {
            for (int j = map.GetLength(1)-1; j>= 0;  j--)
            {
                if (map[i, j] == SquareValue.FLOOR)
                {
                    return worldPosition;
                }
                worldPosition.y += nodeSize;
            }

            worldPosition.y = MapEndWorldPoint.y;
            worldPosition.x -= nodeSize;
        }

        return worldPosition;
    }

    public Vector2 GetRandomFloorPoint()
    {
        return floors[random.RandiRange(0, floors.Count - 1)].GlobalPosition; 
    }




}
