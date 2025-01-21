using UnityEngine;

public class UIFeedback : GameLoopStep
{
    public UIFeedback(int index, int nextStep) : base (index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting end turn feedback.");
        EventBus.Publish(new EventFeedbackRequest());
    }

    public override int NextStep()
    {
        if (GameManager.Instance._initialisationPhase || GameStateManager.Instance.GameHasEnded())
        {
            return _nextStep;
        }
        // only loops during the main phase, as long as the game doesn't end
        else
        {
            return 0;
        }
    }
}
