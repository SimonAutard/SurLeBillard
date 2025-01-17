using UnityEngine;

public class NextPlayerTurn : GameLoopStep
{
    public NextPlayerTurn(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting the start of the next player's turn.");
        EventBus.Publish(new EventNextPlayerTurnStartRequest());
    }

}
