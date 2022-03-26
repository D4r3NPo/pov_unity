using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;
using Renci.SshNet;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public enum POV { Subjective, Semi_Subjective, Objective }
[Serializable]
public class Task
{
    [TextArea(3, 3)] public string Info;
    public int Exemple;
    public int Exercice;
    public int[] Trials;
    public int Current;
    public int GetRandom()
    {
        int index;
        do index = Random.Range(0, Trials.Length);
        while (done.Contains(index));
        done.Add(index);
        return Trials[index];
    }
    List<int> done;
    public int Progress;
    public Task(int exemple, int exercice, int trialsStart, int trialsEnd, string info)
    {
        Current = -1;
        done = new List<int>();
        Info = info;
        Exemple = exemple;
        Exercice = exercice;
        Trials = new int[trialsEnd - trialsStart];
        for (int i = 0; i < Trials.Length; i++) Trials[i] = i + trialsStart;
        Progress = 0;
    }
}
public class GameManager : MonoBehaviour
{
    const string Task1InfoOLD = "Tâche \n Tu recherches une personne. Pour t’aider, tu peux changer ce que tu vois en appuyant sur l'oeil en bas de l'écran. Une fois que tu as trouvé, clique sur la personne qui te semble la bonne. ";
    const string Task2InfoOLD = "Tâche \n Tu recherches le point de vue correspondant à ce que voit la personne dans le village. Pour t’aider, tu peux changer ce que tu vois en appuyant sur l'oeil en bas de l'écran. Une fois que tu as trouvé, clique sur la fenêtre en haut de l’écran qui te semble être le bon.";
    const string Task3InfoOLD = "Tâche \n Tu recherches une personne invisible dans le village, pour la retrouver, tu as son point de vue en haut de l’écran. Pour t’aider, tu peux changer ce que tu vois en appuyant sur l'oeil en bas de l'écran. Une fois que tu penses avoir trouvé où se trouve la personne, clique sur son emplacement.";
    const string Task1Info = "Tâche \nTu joues à cache-cache avec deux personnes. \n Cherche quel personne voit l’image en haut à droite. \n Quand tu la trouves, clique sur elle. \n\nTu peux te déplacer et tourner sur toi même et changer de point de vu avec les commandes en bas de l'écran.";
    const string Task2Info = "Tâche \nCherche la personne qui se cache dans le village.\n Clique sur l'image qui montre ce qu'elle voit. \n\nTu peux te déplacer et tourner sur toi même et changer de point de vu avec les commandes en bas de l'écran.";
    const string Task3Info = "Tâche \nTu joues à cache-cache avec une personne invisible.\n Retrouve-la grâce à l’image en haut de ton écran. \n Et montre sur la Carte où tu penses qu'elle se trouve.\n La Carte s'ouvre avec le boutton à droite de l'écran \n\nTu peux te déplacer et tourner sur toi même et changer de point de vu avec les commandes en bas de l'écran.";

    Task[] Tasks = new Task[]
    {
        new Task(1,2,3,11,Task1Info),
        new Task(12,13,14,22,Task2Info),
        new Task(23,24,25,33,Task3Info)
    };

    public static GameManager Instance;
    public EventSystem EventSystem;
    public GraphicRaycaster GraphicRaycaster;
    public bool Debug => Username == "Admin" && Age == 0;

    public Text Info;
#if UNITY_EDITOR
    [MenuItem("Tools/QuickStart %p", priority = 0)]
    static void QuickStart()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        if (EditorSceneManager.GetActiveScene().name != "Map" || EditorSceneManager.sceneCount > 1) EditorSceneManager.OpenScene("Assets/Scenes/Map.unity", OpenSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }
#endif

    DateTime IDTime;
    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Instance = this;
        MainMenu.SetActive(true);
        ConfirmMenu.SetActive(false);
        IntroDisplayer.SetActive(false);
        EventSystem = GetComponent<EventSystem>();
        CurrentLoadLevel = 0;
    }

    [Header("Userdata")]
    public Text Console;
    public string Username = string.Empty;
    public int Age = -1;
    public void SetUsername(string username) => Username = username;
    public void SetAge(string age) => Age = int.TryParse(age, out int result) ? Mathf.Abs(result) : -1;
    public List<string> Logs = new List<string>();

    [Header("Main Menu")]
    public GameObject MainMenu;
    public GameObject HUD;

    public void Play()
    {
        if (string.IsNullOrWhiteSpace(Username)) { Console.text = "Vous devez rentrer un prénom valide"; return; }
        if (Age == -1) { Console.text = "Vous devez rentrer un âge valide"; return; }

        IDTime = DateTime.Now;

        Logs.Add("POV Export Data File | Version 1.1 | Enzo Suares | 2021");
        Logs.Add("Prénom: " + Username);
        Logs.Add("Age: " + Age + " ans");
        Logs.Add("Heure de démarrage: " + IDTime.ToShortTimeString());

        MainMenu.SetActive(false);
        HUD.SetActive(true);

        //CurrentLevel++;
        NextLevel();
    }

    //Next, Check answer, scripatble object for config map

    bool MoveForward, TurnLeft, TurnRight;

    private Vector2 previousTouchPosition;

    //Main Menu Anim
    public Transform CityCenter;
    public float MainMenuAnimSpeed = 7f;
    public float ViewSpeed = 21f;
    void Update()
    {
        //Main Menu Animation
        if (MainMenu.activeSelf)
        {
            CityCenter.Rotate(Vector3.up, Time.deltaTime * MainMenuAnimSpeed, Space.World);
            View.transform.SetPositionAndRotation(CityCenter.GetChild(0).position, CityCenter.GetChild(0).rotation);
        }
        else if (CurrentLevelData)
        {
            transform.position =
                CurrentPOV switch
                {
                    POV.Subjective => CurrentLevelData.PlayerStart.Subjective.position,
                    POV.Semi_Subjective => CurrentLevelData.PlayerStart.Semi_Subjective.position,
                    POV.Objective => CurrentLevelData.PlayerStart.Objective.position,
                    _ => CurrentLevelData.PlayerStart.Objective.position,
                };
            transform.rotation =
                CurrentPOV switch
                {
                    POV.Subjective => CurrentLevelData.PlayerStart.Subjective.rotation,
                    POV.Semi_Subjective => CurrentLevelData.PlayerStart.Semi_Subjective.rotation,
                    POV.Objective => CurrentLevelData.PlayerStart.Objective.rotation,
                    _ => CurrentLevelData.PlayerStart.Objective.rotation,
                };
            CurrentLevelData.PlayerStart.Player.SetActive(CurrentPOV != POV.Subjective);

            //Game Controls
            //Rotate
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q) || TurnLeft) viewPivot.Rotate(Vector3.up, Time.deltaTime * -ViewSpeed, Space.World);
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || TurnRight) viewPivot.Rotate(Vector3.up, Time.deltaTime * ViewSpeed, Space.World);
            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow) || MoveForward) viewPivot.Translate(Vector3.forward * Time.deltaTime * 5f, Space.Self);
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) viewPivot.Translate(-Vector3.forward * Time.deltaTime * 5f, Space.Self);

            if (Input.GetKeyDown(KeyCode.Space)) NextPOV();

            if (Input.GetMouseButtonDown(0)) PressButton();
            if (Input.GetMouseButtonUp(0)) ReleaseButton();

            //Touch
            if (Input.touchCount == 1)
            {
                Touch currentTouch = Input.GetTouch(0);
                if (currentTouch.phase == TouchPhase.Began) PressButton();
                if (currentTouch.phase == TouchPhase.Ended) TurnLeft = TurnRight = MoveForward = false;
                if (currentTouch.phase == TouchPhase.Moved) viewPivot.Rotate(Vector3.up, (MoveForward ? -1f : 1f) * Time.deltaTime * (previousTouchPosition - currentTouch.position).x, Space.World);
                previousTouchPosition = currentTouch.position;
            }
        }
    }
    void ReleaseButton() => TurnLeft = TurnRight = MoveForward = false;
    void PressButton()
    {
        PointerEventData pointerEventData;
        pointerEventData = new PointerEventData(EventSystem);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster.Raycast(pointerEventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name == "TurnLeft")
            {
                TurnLeft = true;
                break;
            }
            else if (result.gameObject.name == "TurnRight")
            {
                TurnRight = true;
                break;
            }
            else if (result.gameObject.name == "MoveForward")
            {
                MoveForward = true;
                break;
            }
        }
    }
    public POV CurrentPOV;
    public void NextPOV() => CurrentPOV = (CurrentPOV == POV.Semi_Subjective) ? 0 : CurrentPOV + 1;


    enum Type { Exemple, Exercice, Aléatoire }
    [Header("Levels")]
    int currentTaskIndex = 0;
    Task CurrentTask => Tasks[currentTaskIndex];
    int CurrentLevel => CurrentTask.Current;
    int CurrentLoadLevel = 0;
    Type type => CurrentTask.Progress switch { 1 => Type.Exemple, 2 => Type.Exercice, _ => Type.Aléatoire };
    public void NextLevel()
    {
        //if (currentTaskIndex == -1) currentTaskIndex = 0;
        if (Tasks[currentTaskIndex].Progress == 5)
        {
            if (currentTaskIndex < 2) currentTaskIndex++;
            else StartCoroutine(Load(34));
        }
        if (Tasks[currentTaskIndex].Progress == 0)
        {
            StartCoroutine(Load(Tasks[currentTaskIndex].Exemple));
            Tasks[currentTaskIndex].Progress++;
        }
        else if (Tasks[currentTaskIndex].Progress == 1)
        {
            StartCoroutine(Load(Tasks[currentTaskIndex].Exercice));
            Tasks[currentTaskIndex].Progress++;
        }
        else if (Tasks[currentTaskIndex].Progress == 2)
        {
            StartCoroutine(Load(Tasks[currentTaskIndex].GetRandom()));
            Tasks[currentTaskIndex].Progress++;
        }
        else if (Tasks[currentTaskIndex].Progress == 3)
        {
            StartCoroutine(Load(Tasks[currentTaskIndex].GetRandom()));
            Tasks[currentTaskIndex].Progress++;
        }
        else if (Tasks[currentTaskIndex].Progress == 4)
        {
            StartCoroutine(Load(Tasks[currentTaskIndex].GetRandom()));
            Tasks[currentTaskIndex].Progress++;
        }
    }


    float StartLevelTime = 0;
    IEnumerator Load(int levelToLoad)
    {
        AsyncOperation async;
        // UnityEngine.Debug.Log(CurrentLoadLevel);
        if (CurrentLoadLevel != 0)
        {
            async = SceneManager.UnloadSceneAsync(CurrentLoadLevel);
            while (!async.isDone) yield return null;
        }
        CurrentTask.Current = levelToLoad;
        CurrentLoadLevel = levelToLoad;
        async = SceneManager.LoadSceneAsync(CurrentLoadLevel, LoadSceneMode.Additive);
        while (!async.isDone) yield return null;

        CurrentLevelData = FindObjectOfType<LevelData>();
        Info.text = " Tâche " + (currentTaskIndex + 1).ToString() + " " + (type == Type.Exemple ? "Exemple" : type == Type.Exercice ? "Exercice" : "Niveau " + CurrentLevel);

        DisplayTasklIntro();

        foreach (Camera cam in FindObjectsOfType<Camera>()) cam.aspect = 4f / 3f;
    }

    [Header("LevelData")]
    public LevelData CurrentLevelData;

    [Header("Intro")]
    public GameObject IntroDisplayer;
    public Image IntroBackground;
    public Text IntroText;
    bool TaskIntro = false;

    public void TaskInfo() => DisplayTasklIntro(true);
    //        void DisplayTasklIntro(bool forced = false)
    void DisplayTasklIntro(bool forced = true)
    {
        HUD.SetActive(false);
        if (forced)
        {
            TaskIntro = true;
            IntroDisplayer.SetActive(true);
            CurrentLevelData.Hide();
            HUD.SetActive(false);
            IntroText.text = CurrentTask.Info;
        }
        else DisplayLevelIntro();
    }

    void DisplayLevelIntro()
    {
        //ToDo : anim text appearing, Fade Background, Voice
        if (string.IsNullOrWhiteSpace(CurrentLevelData.Intro)) return;
        IntroDisplayer.SetActive(true);
        IntroText.text = CurrentLevelData.Intro;
        TaskIntro = false;
    }
    public void HideIntro()
    {
        // if (TaskIntro) DisplayLevelIntro();
        // else
        // {
        HUD.SetActive(true);
        StartLevelTime = Time.time;
        IntroDisplayer.SetActive(false);
        CurrentLevelData.Show();
        HUD.SetActive(true);
        //}
    }

    [Header("Camera")]
    Camera view;
    internal Camera View => view ? view : view = GetComponent<Camera>();
    Transform viewPivot => CurrentLevelData.PlayerStart.transform;

    public GameObject ConfirmMenu;
    public void Choose(bool valid)
    {
        WaitingForConfirm = valid;

        ConfirmMenu.SetActive(true);
        CurrentLevelData.Hide();
        HUD.SetActive(false);
    }
    public bool? WaitingForConfirm = null;
    public void Confirm()
    {
        float time = Mathf.Abs(StartLevelTime - Time.time);
        Logs.Add(
            ((bool)WaitingForConfirm ? "Réussite" : "Echec   ") +
            " du Niveau " + ((CurrentLevel > 9) ? CurrentLevel.ToString() : (" " + CurrentLevel.ToString())) +
            " : Tâche " + currentTaskIndex /*+ " Difficulté " + CurrentDifficulty */+
            " en " + time.ToString() + "secondes");
        Save();
        ConfirmMenu.SetActive(false);
        WaitingForConfirm = null;

        NextLevel();
    }
    public void Cancel()
    {
        EventSystem.SetSelectedGameObject(null);
        WaitingForConfirm = null;
        ConfirmMenu.SetActive(false);
        CurrentLevelData.Show();
        HUD.SetActive(true);
    }
    void Save()
    {
        //ToDo : Cloud Export
        string rootpath = !(Application.platform == RuntimePlatform.IPhonePlayer) ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) : Application.persistentDataPath;
        Directory.CreateDirectory(rootpath + "/" + "POV_Export" + "/");
        string path = rootpath + "/" + "POV_Export" + "/" + Username.ToString() + IDTime.ToShortTimeString().Replace(':', 'h') + ".txt";
        File.WriteAllLines(path, Logs, System.Text.Encoding.UTF8);
    }
}

