using System;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private float _nextShotForce;
    private Vector3 _nextShotVector;
    private bool _shotCalculated = false;


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
        _nextShotForce = 1.0f;
        _nextShotVector = Vector3.forward;
    }

    private void HandleInitialBreakRequest(EventInitialBreakRequest requestEvent)
    {
        Debug.Log("AIManager: Calculating Initial break.");
        _nextShotForce = 1.0f;
        _nextShotVector = Vector3.forward;
        _shotCalculated = true;
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    public Tuple<Vector3, float> NextShotInfo()
    {
        _shotCalculated = false;
        return new Tuple<Vector3, float>(_nextShotVector, _nextShotForce);
    }
}
