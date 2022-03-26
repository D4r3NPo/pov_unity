using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public GameObject _Selector;
    public GameObject OpenBt;
    public GameObject CloseBt;

    public void Open()
    {
        OpenBt.SetActive(false);
        _Selector.SetActive(true);
        CloseBt.SetActive(true);
    }
    public void Close()
    {
        OpenBt.SetActive(true);
        _Selector.SetActive(false);
        CloseBt.SetActive(false);
    }
}
