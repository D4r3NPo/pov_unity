using UnityEngine;

public class LevelData : MonoBehaviour
{
    [TextArea] public string Intro;
    public PlayerStart PlayerStart;
    GameObject[] ToActiate;

    void Awake()
    {
        ToActiate = GameObject.FindGameObjectsWithTag("ToActivate");
        foreach (GameObject obj in ToActiate) obj.SetActive(false);
    }

    public void Show()
    {
        foreach (GameObject obj in ToActiate) obj.SetActive(true);
    }
    public void Hide()
    {
        foreach (GameObject obj in ToActiate) obj.SetActive(false);
    }

    void OnDrawGizmos()
    {
        foreach (var item in FindObjectsOfType<Camera>())
        {
            if (item.GetComponent<GameManager>()) continue;
            Gizmos.color = item.GetComponentInParent<Character>() ? Color.red : Color.blue;
            Gizmos.DrawSphere(item.transform.position, 1f);
            for (int i = 0; i < 5; i++)
            {
                Gizmos.DrawSphere(item.transform.position + item.transform.forward * i, 0.25f);
            }

        }
    }
}
