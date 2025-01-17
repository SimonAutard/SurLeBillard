using UnityEngine;

public class NewGameSetup : GameLoopStep
{
    public NewGameSetup(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override void Execute()
    {
        GameManager.Instance.WaitForNextStep(false);
        base.Execute();
        Debug.Log("GameManager: Requesting the setup of a new game (rules).");
        EventBus.Publish(new EventNewGameSetupRequest());
    }
}
