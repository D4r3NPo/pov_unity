using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRatio : MonoBehaviour
{
    public float Ratio;
    void Awake() => SetRatio(Ratio);
    void OnValidate() => SetRatio(Ratio);
    void SetRatio(float ratio) => GetComponent<Camera>().aspect = Ratio;
}
