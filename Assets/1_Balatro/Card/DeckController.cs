using DG.Tweening;
using EventDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeckController : MonoBehaviour
{
    [Header("---------------------------------Data------------------------")]
    public List<PlayingCard> drawCards = new List<PlayingCard>(); // la hien tai
    public List<PlayingCard> discardCards = new List<PlayingCard>(); // la da bo
    public List<PlayingCard> handCards = new List<PlayingCard>(); // la tren tay
    public List<CardDeck> defaultDeck;
    public Dictionary<int, CardDeck> deckDict = new();

    [SerializeField] private Transform deckPos;
    [SerializeField] private int maxHandsize = 8;
    public GameObject deckVisual;
    [HideInInspector] public int deckSize;

    public void Init()
    {
        this.RegisterListener(EventID.START_GAME, delegate { StartGame(); });
        this.RegisterListener(EventID.END_GAME, delegate { EndGame(); });
        if (UseProfile.IsNewGame)
        {

            ResetToDefaultDeck();
            UseProfile.IsNewGame = false;
        }
        else
        {
            LoadDeck();
        }
        UseProfile.CurrentCard = deckSize;
        UseProfile.DrawCard = UseProfile.CurrentCard;
        GamePlayController.Instance.uICtrl.HandleUIDeck();
    }
    public void StartGame()
    {
        CreateDeck();
        ShuffleDeck();
        UseProfile.DrawCard = UseProfile.CurrentCard;
        GamePlayController.Instance.uICtrl.isWin = false;
        GamePlayController.Instance.playerContain.handManager.isFirstDraw = true;
        DrawCards(8);
    }
    private void EndGame()
    {
        GamePlayController.Instance.playerContain.handManager.cardViews.Clear();
        
        drawCards.Clear();
        discardCards.Clear();
        handCards.Clear();
    }

    public void CreateDeck()
    {
        drawCards.Clear();
        discardCards.Clear();
        deckSize = 0;

        foreach (var entry in deckDict.Values)
        {
            GameObject prefab = GetPrefabById(entry.id);
            for (int i = 0; i < entry.amout; i++)
            {
                deckSize++;
                PlayingCard newCard = SimplePool2.Spawn(prefab, deckPos.position, Quaternion.identity)
                    .GetComponent<PlayingCard>();

                newCard.id = entry.id;
                newCard.Resign();
                drawCards.Add(newCard);
            }
        }
        if (UseProfile.CurrentCard < deckSize) UseProfile.CurrentCard = deckSize;
    }

    public void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = drawCards.Count;
        while(n > 1)
        {
            int k = rng.Next(n--);
            PlayingCard value = drawCards[k];
            drawCards[k] = drawCards[n];
            drawCards[n] = value;
        }
    }
    public void SaveDeck()
    {
        DeckSaveData saveData = new DeckSaveData();

        foreach (var pair in deckDict)
        {
            saveData.decks.Add(new CardDeck
            {
                id = pair.Key,
                amout = pair.Value.amout
            });
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("DECK_DATA", json);
        PlayerPrefs.Save();
    }
    public void LoadDeck()
    {
        if (!PlayerPrefs.HasKey("DECK_DATA")) return;

        string json = PlayerPrefs.GetString("DECK_DATA");
        DeckSaveData saveData = JsonUtility.FromJson<DeckSaveData>(json);

        deckDict.Clear();

        foreach (var deck in saveData.decks)
        {
            PlayingCard prefab = GetPrefabById(deck.id).GetComponent<PlayingCard>();

            deckDict[deck.id] = new CardDeck
            {
                id = deck.id,
                amout = deck.amout,
            };
        }
    }
    public void ResetToDefaultDeck()
    {
        deckDict.Clear();
        foreach (var card in defaultDeck)
        {

            deckDict[card.id] = new CardDeck
            {
                id = card.id,
                amout = card.amout,
            };
        }
        SaveDeck();
    }
    public static GameObject GetPrefabById(int id)
    {
        GameObject prefab = Resources.Load<GameObject>("Models/Card_" + id);
        return prefab;
    }
    public void AddCardToDeck(PlayingCard cardPrefab, int id)
    {
        if (deckDict.ContainsKey(id))
        {
            deckDict[id].amout++;
        }
        else
        {
            deckDict[id] = new CardDeck
            {
                id = id,
                amout = 1
            };
        }
        SaveDeck();
    }
    public void ReCard(int id)
    {
        if (deckDict.ContainsKey(id))
        {
            deckDict[id].amout--;
        }
    }

    public PlayingCard DrawCard()
    {
        if(drawCards.Count == 0)
        {
            if(discardCards.Count == 0)
            {
                return null;
            }
            RecycleDiscard();
        }
        if (GamePlayController.Instance.playerContain.handManager.cardViews.Count >= maxHandsize) return null;
        PlayingCard _card = drawCards[0];
        drawCards.RemoveAt(0);
        UseProfile.DrawCard--;
        bool isInserted = false;
        for (int i = GamePlayController.Instance.playerContain.handManager.cardViews.Count - 1; i >= 0; i--)
        {
            if (GamePlayController.Instance.playerContain.handManager.cardViews[i].id == _card.id)
            {
                GamePlayController.Instance.playerContain.handManager.cardViews.Insert(i, _card);
                isInserted = true;
                break;
            }
        }
        if(!isInserted)
        {
            GamePlayController.Instance.playerContain.handManager.cardViews.Add(_card);
        }
        return _card;
    }

    public List<PlayingCard> DrawCards(int amount)
    {
        if (GamePlayController.Instance.uICtrl.isWin) return null; 
        List<PlayingCard> cards = new List<PlayingCard>();
        for (int i = 0; i < amount; i++)
        {
            PlayingCard card = DrawCard();
            if (card != null)
            {
                cards.Add(card);
            }
            else
            {
                break;
            }
        }
        for (int i = 0; i < cards.Count; i++)
        {
            int handIndex = GamePlayController.Instance.playerContain.handManager.cardViews.IndexOf(cards[i]);
            GamePlayController.Instance.playerContain.handManager.OnCardDrawn(cards[i], handIndex);
        }
        return cards;
    }

    public void DiscardCard(PlayingCard card)
    {
        if (GamePlayController.Instance.playerContain.handManager.cardViews.Contains(card))
        {
            GamePlayController.Instance.playerContain.handManager.cardViews.Remove(card);
            discardCards.Add(card);
        }
    }
    private void RecycleDiscard()
    {
        drawCards.AddRange(discardCards);
        discardCards.Clear();
        ShuffleDeck();
    }
    
    public void ClearHand()
    {
        foreach (PlayingCard card in GamePlayController.Instance.playerContain.handManager.cardViews)
        {
            discardCards.Add(card);
        }
        GamePlayController.Instance.playerContain.handManager.cardViews.Clear();
    }
    public void OnDisable()
    {
        this.RemoveListener(EventID.START_GAME, delegate { StartGame(); });
        this.RemoveListener(EventID.END_GAME, delegate { EndGame(); });
    }
    public void OnDestroy()
    {
        this.RemoveListener(EventID.START_GAME, delegate { StartGame(); });
        this.RemoveListener(EventID.END_GAME, delegate { EndGame(); });
    }
}

[System.Serializable]
public class DeckSaveData
{
    public List<CardDeck> decks = new();
}
[System.Serializable]
public class CardDeck
{
    public int id;
    public int amout;
}
