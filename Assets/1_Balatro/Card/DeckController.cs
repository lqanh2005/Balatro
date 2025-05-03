using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeckController : MonoBehaviour
{
    [Header("---------------------------------Data------------------------")]
    public List<CardView> drawCards = new List<CardView>(); // la hien tai
    public List<CardView> discardCards = new List<CardView>(); // la da bo
    public List<CardView> handCards = new List<CardView>(); // la tren tay
    public List<CardDeck> deck;

    [SerializeField] private Transform deckVisual;
    [SerializeField] private int maxHandsize = 8;
    [SerializeField] private SpriteRenderer cardBackSite;
    public void Init()
    {
        CreateDeck();
        ShuffleDeck();
        GamePlayController.Instance.playerContain.handManager.isFirstDraw = true;
        DrawCards(8);
        
    }

    public void CreateDeck()
    {
        drawCards.Clear();
        discardCards.Clear();
        foreach (CardDeck card in deck)
        {
            for (int i = 0; i < card.amout; i++)
            {
                CardView newCard = SimplePool2.Spawn(card.cardPrefab, deckVisual.position, Quaternion.identity).GetComponent<CardView>();
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
            CardView value = drawCards[k];
            drawCards[k] = drawCards[n];
            drawCards[n] = value;
        }
    }
    private void AnimateDeckVisual()
    {
        if(deckVisual != null)
        {
            deckVisual.DOLocalMoveY(0.1f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            UpdateDeckVisualHeight();
        }
    }

    private void UpdateDeckVisualHeight()
    {
        if (deckVisual != null)
        {
            float height = drawCards.Count * 0.01f;
            deckVisual.localPosition += new Vector3(0, 0, 0.01f);
        }
    }

    public CardView DrawCard()
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
        if (deckVisual != null)
        {
            deckVisual.DOPunchScale(new Vector3(-0.1f, -0.1f, -0.1f), 0.3f, 5, 0.5f);
            UpdateDeckVisualHeight();
        }
        CardView _card = drawCards[0];
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

    public List<CardView> DrawCards(int amount)
    {
        List<CardView> cards = new List<CardView>();
        for (int i = 0; i < amount; i++)
        {
            CardView card = DrawCard();
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

    public void DiscardCard(CardView card)
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
        foreach (CardView card in GamePlayController.Instance.playerContain.handManager.cardViews)
        {
            discardCards.Add(card);
        }
        GamePlayController.Instance.playerContain.handManager.cardViews.Clear();
    }
}
[System.Serializable]
public class CardDeck
{
    public int id;
    public GameObject cardPrefab;
    public int amout;
}
