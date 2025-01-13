using UnityEngine;

public class EndGame : GameLoopStep
{
    public EndGame(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override bool Execute()
    {
        base.Execute();
        Debug.Log("GameManager: Requesting the end game round-up.");
        EventBus.Publish(new EventProphecyGenerationRequest());
        return false;
    }
}
