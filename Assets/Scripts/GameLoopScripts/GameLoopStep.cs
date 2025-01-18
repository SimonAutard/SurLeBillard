using UnityEngine;

public abstract class GameLoopStep
{
    public int _index { get; set; }
    protected int _nextStep { get; set; }

    public GameLoopStep(int index, int nextStep)
    {
        _index = index;
        _nextStep = nextStep;
    }

    public virtual void Execute()
    {
        Debug.Log($"Step {_index}: Starting step execution");
    }

    public virtual int NextStep()
    {
        return _nextStep;
    }
}
