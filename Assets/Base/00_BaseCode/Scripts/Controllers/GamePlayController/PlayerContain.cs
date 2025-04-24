using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class PlayerContain : MonoBehaviour
{
 
    public DeckController deckController;
    public HandManager handManager;
    public void Init()
    { 
        deckController.Init();

    }

  
}
