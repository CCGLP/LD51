using Godot;
using System.Collections.Generic;

public static class GodotFindUtils
{
    public static List<NODE> FindObjectsOfType<NODE>(this SceneTree tree) where NODE : Node
    {
        return tree.Root.GetNodesInChildren<NODE>(recursive: true);
    }
    public static NODE FindObjectOfType<NODE>(this SceneTree tree) where NODE : Node
    {
        return tree.Root.GetNodeInChildren<NODE>(recursive: true);
    }

    public static NODE GetNodeInChildren<NODE>(this Node parent, bool recursive = false) where NODE : Node
    {
        foreach (Node child in parent.GetChildren())
        {
            if (child is NODE castedChild) return castedChild;
            if (recursive) 
            { 
                NODE result = child.GetNodeInChildren<NODE>(recursive); 
                if (result != null) return result;
            }
        }

        return null;
    }
    public static List<NODE> GetNodesInChildren<NODE>(this Node parent, bool recursive = false) where NODE : Node
    {
        List<NODE> children = new List<NODE>();

        foreach (Node child in parent.GetChildren())
        {
            if (child is NODE castedChild) children.Add(castedChild);
            if (recursive) children.AddRange(child.GetNodesInChildren<NODE>(recursive));
        }

        return children;
    }

    public static Node GetSceneRoot(this Node node)
    {
        Node parent = node.GetParent();
        if (parent == null) return node;
        return parent.GetSceneRoot();
    }
}