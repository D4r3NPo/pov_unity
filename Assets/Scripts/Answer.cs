using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer : MonoBehaviour
{
    public GameObject AnswerObj;
    void Awake() => AnswerObj.SetActive(GameManager.Instance.Debug);
}
