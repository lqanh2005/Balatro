using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerContain : MonoBehaviour
{
 
    public DeckController deckController;
    public HandManager handManager;
    public void Init()
    {
        handManager.Init();
        deckController.Init();
    }

  
}
