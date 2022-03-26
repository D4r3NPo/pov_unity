using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridButton : MonoBehaviour
{
    public Selector Selector;
    public bool IsValid = false;
    public void Select()
    {
        GameManager.Instance.Choose(IsValid);
        Selector.Close();
    }
}
