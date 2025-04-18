using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockBase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private TMP_Text tvCount;

    private void Init(int param)
    {
        tvCount.text = param.ToString();
        spriteRenderer.sprite = grassSprite;
    }

    private void OnMouseEnter()
    {
        
    }

    private void OnMouseUp()
    {
    
    }

    private void OnMouseDown()
    {

    }
}
