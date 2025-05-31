using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollWheelHandle : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 20f;

    void Update()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            scrollRect.verticalNormalizedPosition += scrollDelta * scrollSpeed * Time.deltaTime;
        }
    }
}
