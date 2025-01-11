using UnityEngine;

public class InitialBreak : GameLoopStep
{
    public InitialBreak(int index, int nextStep) : base (index, nextStep)
    {

    }

    public override bool Execute()
    {
        base.Execute();
        Debug.Log("GameManager: Requesting the initial break.");
        EventBus.Publish(new EventInitialBreakRequest());
        return true;
    }
}
