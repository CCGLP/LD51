using Godot;
using System;
using System.Collections.Generic;

public class Pool2D <T> : Node where T : SpatialDespawnable
{
  
    protected PackedScene scene; 
    protected int initialQuantity = 10;
    protected List<T> availableNodeList;
    protected Node parent; 
    public void Init(string nodePath, int initialQuantity, Node parent)
    {
        
        scene = (PackedScene) GD.Load(nodePath); 
        this.initialQuantity = initialQuantity;
        this.parent = parent;
        InitialInstantiate();
    }
    public void InitialInstantiate()
    {
        availableNodeList = new List<T>();
        for (int i = 0; i< initialQuantity; i++)
        {
            var newNode = scene.Instance() as T;
            parent.CallDeferred("add_child",newNode); 
            availableNodeList.Add(newNode); 
            
        }
    }
   

    public T Instantiate()
    {
        if (availableNodeList.Count > 0)
        {
            T node = availableNodeList[0];
            availableNodeList.Remove(node);
            node.Connect("Despawn", this, "Despawn");
            node.Visible = true;
            node.SetProcess(true); 
            return node; 
        }
        else
        {
            T node = scene.Instance() as T;
            node.Visible = false;
            parent.AddChild(node); 
            availableNodeList.Add(node);
            return Instantiate(); 
        }
    }


    protected void Despawn(T node)
    {
        GD.Print("Despawning: " + node.Name); 
        node.Disconnect("Despawn", this, "Despawn");
        node.Visible = false;
        node.SetProcess(false); 
        availableNodeList.Add(node); 
    }
    public override void _Ready()
    {
        
    }

}
