using UnityEngine;

public class ProphecyGeneration : GameLoopStep
{
    public ProphecyGeneration(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting the generation of prophecies.");
        EventBus.Publish(new EventProphecyGenerationRequest());
    }
}
