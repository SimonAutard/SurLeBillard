using UnityEngine;

public class BallsSetup : GameLoopStep
{
    public BallsSetup(int index, int nextStep) : base(index, nextStep)
    {
        
    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting the setup of a new game (scene/physics).");
        EventBus.Publish(new EventInitialBallsSetupRequest());
    }
}
