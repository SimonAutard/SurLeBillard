using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct BallAsNode
{
    public int ballID;
    public bool isCorrectSide;
    public List<BallAsNode> connectedBalls;
    public List<PocketAsNode> connectedPockets;
}

public struct PocketAsNode
{
    public int PocketID;
    public PocketAsNode(int pocketID) { PocketID = pocketID; }
}

public struct HitParameters
{
    public float Force;
    public Vector3 Direction;
    public HitParameters(float force, Vector3 direction) { Force = force; Direction = direction; }
}

public class NTree
{
    public BallAsNode NodeData { get; set; }
    public List<NTree> Children { get; set; }
    public List<NTree> Ancestry { get; set; }
    public int RemainingLevels { get; set; }

    public NTree(NTree parent, BallAsNode nodeData, int remainingLevels)
    {
        RemainingLevels = remainingLevels;
        NodeData = nodeData;
        Ancestry = RewindAncestry(parent);
        Children = new List<NTree>();
        if (remainingLevels > 0) { DesignateChildren(remainingLevels); }
    }

    public List<NTree> GetAncestry()
    {
        return Ancestry;
    }

    public List<NTree> GetTreeNodesConnectedToPockets()
    {
        List<NTree> result = new List<NTree>();
        if (NodeData.connectedPockets.Count >0)
        {
            result.Add(this);
        }
        if (Children.Count>0)
        {
            foreach (NTree child in Children)
            {
                List<NTree> childResult = child.GetTreeNodesConnectedToPockets();
                result = result.Concat(childResult).ToList();
            }
        }
        return result;
    }
    private List<NTree> RewindAncestry(NTree parent)
    {
        List<NTree> result = new List<NTree>();
        if (parent == null) result = null;
        else if (parent.Ancestry == null) result = new List<NTree> { parent };
        else
        {
            result = new List<NTree> (parent.Ancestry);
            result.Add(parent);
        }
        return result;
    }

    private void DesignateChildren(int remainingLevels)
    {
        List<BallAsNode> possibleChildren = new List<BallAsNode>(NodeData.connectedBalls);
        if (Ancestry != null)
        {
            foreach (NTree ancestor in Ancestry)
            {
                possibleChildren.Remove(ancestor.NodeData);
            }
        }

        foreach (BallAsNode child in possibleChildren)
        {
            Children.Add(new NTree(this, child, remainingLevels - 1));
        }
    }

}