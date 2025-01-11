using UnityEngine;

public class NewGameSetup : GameLoopStep
{
    public NewGameSetup(int index, int nextStep) : base(index, nextStep)
    {

    }

    public override bool Execute()
    {
        base.Execute();
        Debug.Log("GameManager: Requesting the setup of a new game (rules).");
        EventBus.Publish(new EventNewGameSetupRequest());
        return false;
    }
}
