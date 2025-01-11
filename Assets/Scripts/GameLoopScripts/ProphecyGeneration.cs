using UnityEngine;

public class ProphecyGeneration : GameLoopStep
{
    public ProphecyGeneration(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override bool Execute()
    {
        base.Execute();
        Debug.Log("GameManager: Requesting the generation of prophecies.");
        EventBus.Publish(new EventProphecyGenerationRequest());
        return true;
    }
}
