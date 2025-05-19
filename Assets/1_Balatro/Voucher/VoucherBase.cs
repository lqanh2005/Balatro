using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoucherBase : MonoBehaviour
{
    public int id;

    public Image avt;

    public void OnAnim()
    {

    }
    public  void OnActive()
    {
        Debug.LogError("OnActive: " + id);  
    }
}
