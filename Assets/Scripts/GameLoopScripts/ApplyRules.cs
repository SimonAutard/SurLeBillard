using UnityEngine;

public class ApplyRules : GameLoopStep
{
    public ApplyRules(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting the application of rules.");
        EventBus.Publish(new EventApplyRulesRequest(PhysicsManager.Instance.TurnCollisions(), PhysicsManager.Instance.TurnPocketings()));
    }

}
