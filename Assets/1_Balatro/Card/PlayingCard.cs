using BestHTTP.Extensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventDispatcher;

public class PlayingCard : CardBase
{
    
    public int level;
    public int chip;
    public IngredientType ingredientType;

    [HideInInspector] public bool isSelected;
    [HideInInspector] public bool isDrag;
    [HideInInspector] public bool isMouseDown;
    [HideInInspector] public bool isDraw;
    [HideInInspector] public Vector3 initialMousePos;
    

    

    public override void Init()
    {
        
        this.chip = ConfigData.Instance.GetChip(this.ingredientType, this.level);
        isSelected = false;
        isMouseDown = false;
        isDrag = false;
        isDraw = true;
        cardAnim.Init();
    }
    public PlayingCard GetCardBase()
    {
        return this;
    }
    public void FixedUpdate()
    {
        if (!isDraw || !isMouseDown) return;
        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMousePos.z = 0;

        float dragThreshold = 0.01f;

        if (!isDrag && Vector3.Distance(initialMousePos, currentMousePos) > dragThreshold)
        {
            isDrag = true;
        }

        if (isDrag)
        {
            this.transform.position = currentMousePos;
        }
    }

    public void OnMouseDown()
    {
        if (!isDraw) return;
        isMouseDown = true;
        isDrag = false;
        initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialMousePos.z = 0;

    }
    public void OnMouseEnter()
    {
        if (isDrag) return;
        if (!isDraw) return;
        cardAnim.PlayHoverAniamtion(true, isSelected);
        //showText.gameObject.SetActive(true);
    }
    public void OnMouseExit()
    {
        if (!isDraw || isDrag) return;
        cardAnim.PlayHoverAniamtion(false, isSelected);
        //showText.gameObject.SetActive(false);
    }
    public void OnMouseUp()
    {
        if (!isDraw) return;
        isMouseDown = false;

        if (!isDrag)
        {
            isSelected = !isSelected;
            if (isSelected)
            {
                if (GamePlayController.Instance.playerContain.handManager.seletedCards.Count < GamePlayController.Instance.playerContain.handManager.maxSelectedCard)
                {
                    GamePlayController.Instance.playerContain.handManager.seletedCards.Add(this);
                    RecipeChecker.GetMatchedRecipe(GamePlayController.Instance.playerContain.handManager.seletedCards);
                    cardAnim.PlaySelectedAniamtion(isSelected);
                }
                else
                {
                    cardAnim.PlaySelectedAniamtion1();
                }
            }
            else
            {
                GamePlayController.Instance.playerContain.handManager.seletedCards.Remove(this);
                RecipeChecker.GetMatchedRecipe(GamePlayController.Instance.playerContain.handManager.seletedCards);
                cardAnim.PlaySelectedAniamtion(isSelected);
            }
        }
        else
        {
            this.transform.DOLocalMove(new Vector3(cardAnim.originalPosition.x, cardAnim.originalPosition.y + (isSelected ? 0.2f : 0f)), 0.3f).SetEase(Ease.OutBack);
        }

        isDrag = false;
    }
    public void OnDisable()
    {
        DOTween.Kill(transform);
        cardAnim?.sequence?.Kill();
    }
    public void OnDestroy()
    {
        
        this.RemoveListener(EventID.END_GAME, delegate { SimplePool2.Despawn(this.gameObject); });
    }

    public override void OnActive()
    {
        var coinText = GamePlayController.Instance.uICtrl.coin;
        int currentCoin = coinText.text.ToInt32();

        currentCoin += chip;

        EffectHelper.PlayScoreBounce(coinText, currentCoin);
    }
}
