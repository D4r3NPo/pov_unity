using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterIcon : MonoBehaviour
{
    public Character Character;
    public Image Image;
    public Button Button;
    void OnValidate()
    {
        if (Character) if (Character.CharacterIcon!=this) Character.CharacterIcon = this;
    }
    void Awake()
    {
        if (GameManager.Instance.Debug && Character.IsValid)
        {
            ColorBlock colorBlock = ColorBlock.defaultColorBlock;
            colorBlock.normalColor = colorBlock.selectedColor = colorBlock.pressedColor= colorBlock.highlightedColor = Color.red;
            Button.colors = colorBlock;
        }
    }
    public void Select()
    {
        GameManager.Instance.Choose(Character.IsValid);
    }
}
