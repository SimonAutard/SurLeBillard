using System;
using System.Collections;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private float _nextShotForce;
    private Vector3 _nextShotVector;
    private bool _shotCalculated = false;
    [SerializeField] Vector3 _breakTrajectory = new Vector3(0.25f, 0.0f, 1.0f);
    [SerializeField] float _breakforce;


    // Design pattern du singleton
    private static AIManager _instance; // instance statique du ai manager

    public static AIManager Instance
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

    private void OnEnable()
    {
        EventBus.Subscribe<EventAIShotRequest>(HandleAIShotRequest);
        EventBus.Subscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EventAIShotRequest>(HandleAIShotRequest);
        EventBus.Unsubscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<EventAIShotRequest>(HandleAIShotRequest);
        EventBus.Unsubscribe<EventInitialBreakRequest>(HandleInitialBreakRequest);
    }

    private void HandleAIShotRequest(EventAIShotRequest requestEvent)
    {
        Debug.Log("AIManager: Calculating Atropos shot.");
        _nextShotForce = 0.1f;
        _nextShotVector = Vector3.forward;
        _shotCalculated = true;
        // No direct publish of this shot data because AIManager doesn't know if it's needed right now. In practice, the event would be caught by UIManager and the data stored until needed
        // which comes down to the same thing as storing it here and letting UIManager access it when it needs to
    }

    private void HandleInitialBreakRequest(EventInitialBreakRequest requestEvent)
    {
        Debug.Log("AIManager: Calculating Initial break.");
        _nextShotForce = _breakforce;
        _nextShotVector = _breakTrajectory;
        _shotCalculated = true;
        // No direct publish of this shot data because AIManager doesn't know if it's needed right now. In practice, the event would be caught by UIManager and the data stored until needed
        // which comes down to the same thing as storing it here and letting UIManager access it when it needs to
        Debug.Log("AIManager: calling NextStep");
        StartCoroutine(BreakShotDelay());
    }

    private IEnumerator BreakShotDelay()
    {
        yield return new WaitForSeconds(1.0f);
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    public Tuple<Vector3, float> NextShotInfo()
    {
        _shotCalculated = false;
        return new Tuple<Vector3, float>(_nextShotVector, _nextShotForce);
    }
}
