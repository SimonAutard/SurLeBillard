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
        if (GameStateManager.Instance.GameHasEnded())
        {
            return 0;
        }
        else
        {
            return _nextStep;
        }
    }
}
