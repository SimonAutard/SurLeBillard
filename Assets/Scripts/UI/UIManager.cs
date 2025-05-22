 using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Unity.VisualScripting.Member;


public class UIManager : MonoBehaviour
{
    public bool _isClothoTurn { get; set; }
    public bool popupEnabled { get; set; }
    [SerializeField] private DialogManagement _dialogManagement;
    public List<UIProphecy> prophecies { get; set; }
    public bool storyDisplayed { get; set; }

    public bool VictoryPanelDisplayed { get; set; }


// Design pattern du singleton
private static UIManager _instance; // instance statique du ui manager

    //[SerializeField] GameObject cue;
    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // used for debug/testing
    private TextMeshProUGUI _displayText;
    private List<string> _log = new List<string>();

    private void OnEnable()
    {
        // subscribe to all events that this component needs to listen to at all time
        EventBus.Subscribe<EventInitialBreakUIRequest>(HandleInitialBreakRequest);
        EventBus.Subscribe<EventFeedbackRequest>(HandleFeedbackRequest);
        EventBus.Subscribe<EventNextTurnUIDisplayRequest>(HandleNextTurnUIDisplayRequest);
        EventBus.Subscribe<EventEndGameRoundupRequest>(HandleEndGameRoundupRequest);
        EventBus.Subscribe<EventBallWasCreated>(HandleNewWhiteBall);
    }

    private void OnDisable()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBreakUIRequest>(HandleInitialBreakRequest);
        EventBus.Unsubscribe<EventFeedbackRequest>(HandleFeedbackRequest);
        EventBus.Unsubscribe<EventNextTurnUIDisplayRequest>(HandleNextTurnUIDisplayRequest);
        EventBus.Unsubscribe<EventEndGameRoundupRequest>(HandleEndGameRoundupRequest);
        EventBus.Unsubscribe<EventBallWasCreated>(HandleNewWhiteBall);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
        EventBus.Unsubscribe<EventInitialBreakUIRequest>(HandleInitialBreakRequest);
        EventBus.Unsubscribe<EventFeedbackRequest>(HandleFeedbackRequest);
        EventBus.Unsubscribe<EventNextTurnUIDisplayRequest>(HandleNextTurnUIDisplayRequest);
        EventBus.Unsubscribe<EventEndGameRoundupRequest>(HandleEndGameRoundupRequest);
        EventBus.Unsubscribe<EventBallWasCreated>(HandleNewWhiteBall);
    }

    /// <summary>
    /// Called by UI when the player actually starts the game (basically: the button "new game" has been clicked)
    /// </summary>
    public void StartNewGame()
    {
        // TODO : UI related stuff if needed
        Debug.Log("UIManager : Requesting the start of a new game");
        EventBus.Publish(new EventNewGameRequest());
        EventBus.Publish(new EventMenuClickSignal());
       
    }

    public void AttachDialogManagement(DialogManagement dm)
    {
        _dialogManagement = dm;
    }

    /// <summary>
    /// Displays any needed dialogue between characters during the initial break and play the animation for it
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleInitialBreakRequest(EventInitialBreakUIRequest requestEvent)
    {
        Debug.Log("UIManager: Displaying everything needed for the initial break.");
        // TODO:
        // - Dialogues and other needed UI elements
        // - Fetch (directly, no event) AI behaviour to know where to place the cue
        // - Automated play animation (non player)
        // - Store the shot angle and force in two floats (can easily be changed as needed, I just went with 2 floats for now)

        // placeholder values
        //Vector3 BallCuePos = UISingleton.Instance.BallCuePos;
        //float force = UISingleton.Instance.force;
        Tuple<Vector3, float> placeholderBreak = AIManager.Instance.NextShotInfo();
        Vector3 BallCuePos = placeholderBreak.Item1;
        //float force = placeholderBreak.Item2;
        float force = 0.0f;
        // TODO: Handle the cue animation for the break and any dialogue or other UI event needed during that phase
        Debug.Log("UIManager: Requesting force application.");
        EventBus.Publish(new EventApplyForceToWhiteRequest(BallCuePos, force));

    }

    /// <summary>
    /// Display dialogs and updates the UI based on what happenned during the turn
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleFeedbackRequest(EventFeedbackRequest requestEvent)
    {
        Debug.Log("UIManager: Displaying info about what happened during the turn.");
        Debug.Log("UIManager: Displaying dialogs.");
        // TODO
        // - Fetch (directly, no event) needed data from GameStateManager (collisions, pocketings, etc)
        // - Update UI (opacity on pocketed balls, etc)
        // - Fetch (directly, no event) generated prophecies from NarrationManager
        // - Display prophecies recap
        // - Dialogs

        //Vérifer que la liste des collisions est vide ou pas
        //prophecies = NarrationManager.Instance.LastTurnProphecies();
        prophecies = new List<UIProphecy>(NarrationManager.Instance.LastTurnProphecies());
        
        if (prophecies.Count != 0)
           {
                Debug.Log("prophecies : " + prophecies);
                UISingleton.Instance.isCollided = true;
  
           }

        StartCoroutine(WaitPopup());
        //EventBus.Publish(new EventGameloopNextStepRequest());


    }

    /// <summary>
    /// Does everything needed on UI side to handle the start of the turn, depending on the active player
    /// </summary>
    /// <param name="requestEvent">contains _activePlayer</param>
    private void HandleNextTurnUIDisplayRequest(EventNextTurnUIDisplayRequest requestEvent)
    {
        if (requestEvent._activePlayer == ActivePlayerName.Clotho)
        {
            Debug.Log("UIManager: Displaying everything needed for player input");
            // TODO:
            // - Dialogues and other needed UI elements
            // - Cue input handling
            // - Store the shot angle and force in two floats (can easily be changed as needed, I just went with 2 floats for now)

            _isClothoTurn = true;
            _dialogManagement.ShowFirstDialogOnClothoTurn();

        }
        else
        {
            Debug.Log("UIManager: Displaying everything needed for AI auto play");
            // TODO:
            // - Dialogues and other needed UI elements
            // - Fetch (directly, no event) AI behaviour to know where to place the cue
            // - Automated play animation (non player)
            // - Store the shot angle and force in two floats (can easily be changed as needed, I just went with 2 floats for now)

            _isClothoTurn = false;
            _dialogManagement.ShowFirstDialogOnAtroposTurn();
        }
        // placeholder values
        //Vector3 vector = Vector3.forward;
        //float force = 1.0f;
        //Debug.Log("UIManager: Requesting force application.");
        //EventBus.Publish(new EventApplyForceToWhiteRequest(vector, force));
    }

    /// <summary>
    /// Does everything needed on UI side to handle the end of the game
    /// </summary>
    /// <param name="requestEvent"></param>
    private void HandleEndGameRoundupRequest(EventEndGameRoundupRequest requestEvent)
    {
        Debug.Log("UIManager: Displaying end game recap and dialogs.");
        // TODO:
        // - Display winner (fetch info from GameStateManager)
        // - Display dialogs depending on the winner
        // - Display gameplay recap? (fetch data from GameStateManager)
        // - Display story recap? (fetch data from NarrationManager)

        VictoryPanelDisplayed = true;


    }

    IEnumerator WaitPopup()
    {
       
        while (UISingleton.Instance.isCollided == true)
        {
            yield return null;
            //Debug.Log("*******************Popup ouverte*****************");
        }
        Debug.Log("UIManager: Requesting next step.");
        prophecies.Clear();
        EventBus.Publish(new EventGameloopNextStepRequest());

    }

    private void HandleNewWhiteBall(EventBallWasCreated requestEvent)
    {
        _dialogManagement.RefreshCue(requestEvent._ball);
    }

}
