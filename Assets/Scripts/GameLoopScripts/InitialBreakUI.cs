using UnityEngine;

public class InitialBreakUI : GameLoopStep
{
    public InitialBreakUI(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting the UI part of the initial break.");
        EventBus.Publish(new EventInitialBreakUIRequest());
    }
}
