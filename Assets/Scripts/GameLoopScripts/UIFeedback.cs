using UnityEngine;

public class UIFeedback : GameLoopStep
{
    public UIFeedback(int index, int nextStep) : base (index, nextStep)
    {

    }

    public override bool Execute()
    {
        GameManager.Instance.WaitForNextStep(true);
        base.Execute();
        Debug.Log("GameManager: Requesting end turn feedback.");
        EventBus.Publish(new EventFeedbackRequest());
        return true;
    }

    public override int NextStep()
    {
        if (GameManager.Instance._initialisationPhase || GameStateManager.Instance.GameHasEnded())
        {
            return _nextStep;
        }
        else
        {
            return 0;
        }
    }
}
