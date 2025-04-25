using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeckController : MonoBehaviour
{
    [Header("---------------------------------Data------------------------")]
    public List<CardBase> drawCards = new List<CardBase>(); // la hien tai
    public List<CardBase> discardCards = new List<CardBase>(); // la da bo
    public List<CardBase> handCards = new List<CardBase>(); // la tren tay
    public List<Card> deck;

    [SerializeField] private Transform deckVisual;
    [SerializeField] private int maxHandsize = 8;
    [SerializeField] private SpriteRenderer cardBackSite;
    public void Init()
    {
        CreateDeck();
        ShuffleDeck();
        DrawCards(8);
        
    }

    public void CreateDeck()
    {
        drawCards.Clear();
        discardCards.Clear();
        foreach (Card card in deck)
        {
            for (int i = 0; i < card.amout; i++)
            {
                CardBase newCard = SimplePool2.Spawn(card.cardPrefab, deckVisual.position, Quaternion.identity).GetComponent<CardBase>();
                //newCard.Init();
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
            CardBase value = drawCards[k];
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
            float height = drawCards.Count * 0.01f; // Tăng tỉ lệ nếu muốn dày hơn
            deckVisual.localPosition += new Vector3(0, 0, 0.01f);
        }
    }

    public CardBase DrawCard()
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
        CardBase _card = drawCards[0];
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
        //GamePlayController.Instance.playerContain.handManager.cardViews.Add(_card);
        return _card;
    }

    public List<CardBase> DrawCards(int amount)
    {
        List<CardBase> cards = new List<CardBase>();
        for (int i = 0; i < amount; i++)
        {
            CardBase card = DrawCard();
            if (card != null)
            {
                cards.Add(card);
                GamePlayController.Instance.playerContain.handManager.OnCardDrawn(card, GamePlayController.Instance.playerContain.handManager.cardViews.IndexOf(card));
            }
            else
            {
                break;
            }
        }
        
        return cards;
    }

    public void DiscardCard(CardBase card)
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
        foreach (CardBase card in GamePlayController.Instance.playerContain.handManager.cardViews)
        {
            discardCards.Add(card);
        }
        GamePlayController.Instance.playerContain.handManager.cardViews.Clear();
    }
}
[System.Serializable]
public class Card
{
    public int id;
    public GameObject cardPrefab;
    public int amout;
}
