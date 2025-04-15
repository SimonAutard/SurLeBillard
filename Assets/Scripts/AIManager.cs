using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private float _nextShotForce;
    private Vector3 _nextShotVector;
    private bool _shotCalculated = false;

    // Configuration de l'arbre de décision
    [SerializeField] int treeMaxLevels;

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

    /// <summary>
    /// Cree un nodeData pour chaque bille en jeu
    /// </summary>
    /// <returns></returns>
    private List<BallAsNode> CreateNodeData()
    {
        //Cretion de la représentation des poches dans les nodes
        GameObject[] pocketsGO = PhysicsManager.Instance.GetPockets();
        List<PocketAsNode> allPocketsAsNodes = new List<PocketAsNode>();
        for (int p = 0; p < pocketsGO.Length; p++) allPocketsAsNodes.Add(new PocketAsNode(p));

        List<BallRoll> remainingBalls = PhysicsManager.Instance.GetRemainingBalls();

        List<BallAsNode> result = new List<BallAsNode>();
        foreach (BallRoll ball in remainingBalls)
        {
            BallAsNode newBallAsNode;

            newBallAsNode.ballID = ball._ballId;

            //Verification du camp de la bille
            if (newBallAsNode.ballID > GameStateManager.Instance.blackBallID) newBallAsNode.isCorrectSide = true;
            else newBallAsNode.isCorrectSide = false;

            newBallAsNode.connectedBalls = new List<BallAsNode>(); //Initialisation à vide des connectedBalls

            newBallAsNode.connectedPockets = FindConnectedPockets(newBallAsNode.isCorrectSide,newBallAsNode.ballID, allPocketsAsNodes);

            result.Add(newBallAsNode);
        }
        result = FindConnectedBalls(result); //remplissage des connected Balls
        return result;
    }

    ///Demande au physicsManager de trouver les billes connectees puis les transforme en ballAsNode
    private List<BallAsNode> FindConnectedBalls(List<BallAsNode> allBallsAsNodes)
    {
        int ballCount = allBallsAsNodes.Count;
        for (int i = 0; i < ballCount; i++)
        {
            //Comme allBallsAsNodes es tune liste, on ne peut pas la modifier directement. Il faut extrauire lelement, le modifier seul, puis le remettre dans la liste
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

    /// <summary>
    /// Demande au physicsManager de trouver les poches connectees puis les transforme en pocketAsNode
    /// </summary>
    /// <param name="ballID"></param>
    /// <param name="allPockets"></param>
    /// <returns></returns>
    private List<PocketAsNode> FindConnectedPockets(bool ballIsCorrectSide, int ballID, List<PocketAsNode> allPockets)
    {
        List<PocketAsNode> result = new List<PocketAsNode>();
        //Si on ne veut pas empocher cette bille, on ne lui met aucune connectedPocket
        if (ballIsCorrectSide)
        {
            List<int> pocketsID = PhysicsManager.Instance.GetAllConnectedPocketsID(ballID);
            foreach (int i in pocketsID) result.Add(allPockets.Find(node => node.PocketID == i));
        }
        return result;
    }

    private HitParameters FindOneCorrectPath(List<BallAsNode> allBallsAsNodes)
    {
        NTree nTree = new NTree(null, allBallsAsNodes.Find(node => node.ballID == 0), treeMaxLevels);
        List<NTree> allFinalNodes = nTree.GetAllFinalNodes();

        HitParameters result = new HitParameters(0,Vector3.zero);
        bool viablePath = false;

        foreach (NTree finalNode in allFinalNodes)
        {
            List<NTree> finalNodeAncestry = finalNode.Ancestry;
            foreach (NTree ancestor in finalNodeAncestry)
            {
                List<PocketAsNode> possiblePockets = ancestor.NodeData.connectedPockets;
                if (possiblePockets != null)
                {
                    foreach(PocketAsNode pocket in possiblePockets)
                    {
                        viablePath = PhysicsManager.Instance.CalculateHitParametersForPath(pocket.PocketID, ancestor, out HitParameters hitParameters);
                        if (viablePath) { result = hitParameters; break; }

                    }
                }
                if (viablePath) { break; }
            }
            if (viablePath) { break; }
        }
        return result;
    }
}
