using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//TODO Instantiate on Awake Icon
public class Character : MonoBehaviour
{
    public bool IsValid;
    public CharacterIcon CharacterIcon;
    void OnValidate()
    {
        if (CharacterIcon) if (CharacterIcon.Character != this) CharacterIcon.Character = this;
    }
    void Update()
    {
        if (CharacterIcon)
        {
            CharacterIcon.transform.position = GameManager.Instance.View.WorldToScreenPoint(transform.position + Vector3.up);
            CharacterIcon.Button.enabled = GameManager.Instance.WaitingForConfirm == null;
            CharacterIcon.Image.enabled = CharacterIcon.Button.enabled ? Vector3.Dot(GameManager.Instance.View.transform.forward, transform.position - GameManager.Instance.View.transform.position) > 0f : false;
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.color = IsValid ? Color.red : Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);
        for (int i = 0; i < 5; i++)
        {
            Gizmos.DrawSphere(transform.position + transform.forward * i, 0.25f);
        }
    }

}
