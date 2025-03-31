using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        _nextShotForce = 0.1f;
        _nextShotVector = Vector3.forward;
        _shotCalculated = true;
        // No direct publish of this shot data because AIManager doesn't know if it's needed right now. In practice, the event would be caught by UIManager and the data stored until needed
        // which comes down to the same thing as storing it here and letting UIManager access it when it needs to
    }

    private void HandleInitialBreakRequest(EventInitialBreakRequest requestEvent)
    {
        Debug.Log("AIManager: Calculating Initial break.");
        _nextShotForce = 0.1f;
        _nextShotVector = Vector3.forward;
        _shotCalculated = true;
        // No direct publish of this shot data because AIManager doesn't know if it's needed right now. In practice, the event would be caught by UIManager and the data stored until needed
        // which comes down to the same thing as storing it here and letting UIManager access it when it needs to
        Debug.Log("AIManager: calling NextStep");
        EventBus.Publish(new EventGameloopNextStepRequest());
    }

    public Tuple<Vector3, float> NextShotInfo()
    {
        _shotCalculated = false;
        return new Tuple<Vector3, float>(_nextShotVector, _nextShotForce);
    }

    private List<BallAsNode> CreateNodeData()
    {
        
        GameObject[] pocketsGO = PhysicsManager.Instance.GetPockets();
        List<PocketAsNode> allPocketsAsNodes = new List<PocketAsNode>();
        for (int p = 0; p < pocketsGO.Length; p++) allPocketsAsNodes.Add(new PocketAsNode(p));

        List<BallRoll> remainingBalls = PhysicsManager.Instance.GetRemainingBalls();

        List<BallAsNode> result = new List<BallAsNode>();
        foreach (BallRoll ball in remainingBalls)
        {
            BallAsNode newBallAsNode;

            newBallAsNode.ballID = ball._ballId;

            if (newBallAsNode.ballID > GameStateManager.Instance.blackBallID) newBallAsNode.isCorrectSide = true;
            else newBallAsNode.isCorrectSide = false;

            newBallAsNode.connectedBalls = new List<BallAsNode>();

            newBallAsNode.connectedPockets = FindConnectedPockets(newBallAsNode.ballID, allPocketsAsNodes);

            result.Add(newBallAsNode);
        }
        result = FindConnectedBalls(result);
        return result;
    }

    private List<BallAsNode> FindConnectedBalls(List<BallAsNode> allBallsAsNodes)
    {
        int ballCount = allBallsAsNodes.Count;
        for (int i = 0; i < ballCount; i++)
        {
            BallAsNode currentNode = allBallsAsNodes[i];
            List<int> connectedBallsID = PhysicsManager.Instance.GetAllConnectedBallsID(currentNode.ballID);
            foreach(int  ballID in connectedBallsID)
            {
                currentNode.connectedBalls.Add(allBallsAsNodes.Find(node => node.ballID == ballID));
            }
            allBallsAsNodes[i] = currentNode;
        }
        return allBallsAsNodes;
    }

    private List<PocketAsNode> FindConnectedPockets(int ballID, List<PocketAsNode> allPockets)
    {
        //TODO : Retirer tous els trous si ballID = 0 ou ballID = enemyBALL
        List<int> pocketsID = PhysicsManager.Instance.GetAllConnectedPocketsID(ballID);
        List<PocketAsNode> result = new List<PocketAsNode>();
        foreach (int i in pocketsID) result.Add(allPockets.Find(node => node.PocketID == i));
        return result;
    }
}
