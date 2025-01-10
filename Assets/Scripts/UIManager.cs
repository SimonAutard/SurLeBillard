using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager
{
    private GameManager _gameManager;


    // used for debug/testing
    private TextMeshProUGUI _displayText;
    private List<string> _log = new List<string>();

    

    public UIManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        Initialize();
    }

    private void Initialize()
    {
        // subscribe to all events that this component needs to listen to at all time
    }

    ~UIManager()
    {
        // Unsubscribe from all events before getting destroyed to avoid memory leaks
    }
}
