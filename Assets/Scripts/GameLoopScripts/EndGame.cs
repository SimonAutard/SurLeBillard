using UnityEngine;

public class EndGame : GameLoopStep
{
    public EndGame(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(false);
        base.Execute();
        Debug.Log("GameManager: Requesting the end game round-up.");
        EventBus.Publish(new EventEndGameRoundupRequest());
    }
}
