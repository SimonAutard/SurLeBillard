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

    public override int NextStep()
    {
        // NOTE: This step doesn't actually need to check for this anymore since the game ending detection has been moved.
        // It's still useful for the the snippet that allows to abruptly stop the game avec n turns.
        if (GameStateManager.Instance.GameHasEnded())
        {
            // this high value obviously doesn't exists as a step id. This will make the phase end.
            // yes I know it's not the cleanest. I might make this better at some point... Not really... Maybe... It's classified.
            return 99999;
        }
        else
        {
            return _nextStep;
        }
    }

}
