using Godot;
using System;

public class Line
{

    public enum LineType
    {
        NULL,
        HORIZONTAL,
        VERTICAL
    }

    private readonly LineType type;
    private readonly float nodeSize;
    private readonly Vector2 worldSize, startPoint;

    public Line(Vector2 startPoint, LineType type, float nodeSize, Vector2 worldSize)
    {
        this.startPoint = startPoint;
        this.type = type;
        this.nodeSize = nodeSize;
        this.worldSize = worldSize;
    }

    public Vector2 GetWorldStartPoint()
        => new Vector2
        {
            x = (startPoint.x - worldSize.x * 0.5f) * nodeSize,
            y = (startPoint.y < worldSize.y * 0.5f)
                ? +(startPoint.y - worldSize.y * 0.5f) * -nodeSize
                : -(startPoint.y - worldSize.y * 0.5f) * +nodeSize,
        };

    public LineType Type
        => type;

    public Vector2 StartPoint
        => startPoint;

}