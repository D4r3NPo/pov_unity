using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewIcon : MonoBehaviour
{
    public bool IsValid = false;
    public void Select(bool isValid) => GameManager.Instance.Choose(isValid);
    void Awake()
    {
        Button = GetComponent<Button>();
        RawImage = GetComponent<RawImage>();
        if (GameManager.Instance.Debug && IsValid) RawImage.color = Color.red;
    }
    Button Button;
    RawImage RawImage;
    void Update() => Button.enabled = RawImage.enabled = GameManager.Instance.WaitingForConfirm == null;
}
