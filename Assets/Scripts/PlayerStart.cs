using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    public Transform Subjective;
    public Transform Semi_Subjective;
    public Transform Objective;
    public GameObject Player;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(1f, 2f, 1f));

        Gizmos.color = Color.red;
    }
    void Awake() => Player = Instantiate(Resources.Load<GameObject>("Player"), transform);
}
