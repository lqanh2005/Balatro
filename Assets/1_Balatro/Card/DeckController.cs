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
    public List<CardDeck> deck;

    [SerializeField] private Transform deckPos;
    [SerializeField] private int maxHandsize = 8;
    public GameObject deckVisual;

    public void Init()
    {
        this.RegisterListener(EventID.START_GAME, delegate { StartGame(); });
        this.RegisterListener(EventID.END_GAME, delegate { EndGame(); });
    }


    public void StartGame()
    {
        CreateDeck();
        ShuffleDeck();
        GamePlayController.Instance.playerContain.handManager.isFirstDraw = true;
        DrawCards(8);
    }
    private void EndGame()
    {
        drawCards.Clear();
        discardCards.Clear();
        handCards.Clear();
    }

    public void CreateDeck()
    {
        drawCards.Clear();
        discardCards.Clear();
        foreach (CardDeck card in deck)
        {
            for (int i = 0; i < card.amout; i++)
            {
                PlayingCard newCard = SimplePool2.Spawn(card.cardPrefab.gameObject, deckPos.position, Quaternion.identity).GetComponent<PlayingCard>();
                newCard.id = card.id;
                drawCards.Add(newCard);
            }
        }
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
    private void AnimateDeckVisual()
    {
        if(deckPos != null)
        {
            deckPos.DOLocalMoveY(0.1f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            UpdateDeckVisualHeight();
        }
    }

    private void UpdateDeckVisualHeight()
    {
        if (deckPos != null)
        {
            float height = drawCards.Count * 0.01f;
            deckPos.localPosition += new Vector3(0, 0, 0.01f);
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
    }
    public void OnDestroy()
    {
        this.RemoveListener(EventID.START_GAME, delegate { StartGame(); });
    }
}
[System.Serializable]
public class CardDeck
{
    public int id;
    public PlayingCard cardPrefab;
    public int amout;
}
