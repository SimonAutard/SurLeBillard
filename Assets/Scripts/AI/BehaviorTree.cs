using System.Collections.Generic;
using System.Linq;

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

class NTree
{
    private BallAsNode NodeData;
    private List<NTree> Children;
    private List<NTree> Ancestry;

    public NTree(NTree parent, BallAsNode nodeData, int remainingLevels)
    {
        NodeData = nodeData;
        Ancestry = RewindAncestry(parent);
        Children = new List<NTree>();
        if (remainingLevels > 0) { DesignateChildren(remainingLevels); }
    }

    public NTree GetParent()
    {
        return Ancestry.Last();
    }

    public List<NTree> GetAllPockets()
    {
        //TODO : ECRIRE FONCTION
        return null;
    }
    private List<NTree> RewindAncestry(NTree parent)
    {
        List<NTree> result = new List<NTree>();
        if (parent == null) result = null;
        else if (parent.Ancestry == null) result = new List<NTree> { parent };
        else
        {
            result = parent.Ancestry;
            result.Add(parent);
        }
        return result;
    }

    private void DesignateChildren(int remainingLevels)
    {
        List<BallAsNode> possibleChildren = new List<BallAsNode>(NodeData.connectedBalls);
        foreach (NTree ancestor in Ancestry)
        {
            possibleChildren.Remove(ancestor.NodeData);
        }
        foreach (BallAsNode child in possibleChildren)
        {
            Children.Add(new NTree(this, child, remainingLevels - 1));
        }
    }
}