using UnityEngine;

public class NextPlayerTurnUI : GameLoopStep
{
    public NextPlayerTurnUI(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting the start of the turn on UI side.");
        EventBus.Publish(new EventNextTurnUIDisplayRequest(GameStateManager.Instance.ActivePlayer()));
    }
}
