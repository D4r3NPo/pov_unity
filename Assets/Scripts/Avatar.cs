using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ViewMode {FPS,TPS,GPS,SKY }
public class Avatar : MonoBehaviour
{
    [Header("------- Camera -------")]
    public ViewMode Mode;
    [Space]
    public Transform FPS;
    public Transform TPS;
    public Transform GPS;
    public Camera Camera;
    public float ViewSpeed= 2f;

    [Header("------- Movement -------")]
    //public bool Jump = false;
    public float Speed = 5f;
    //public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public CharacterController controller;
    Vector3 velocity;


    // Header("------- Gameplay -------")]
    public Carrousel<RawImage> Images;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if (controller.isGrounded && velocity.y < 0f) velocity.y = 0f;

        Vector3 move;
        if(Mode == ViewMode.SKY)
        {
            move = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * (Input.GetKey(KeyCode.Z) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f);
        }
        else
        {
            move = Vector3.ProjectOnPlane(Camera.transform.forward, Vector3.up).normalized * (Input.GetKey(KeyCode.Z) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f) + Vector3.ProjectOnPlane(Camera.transform.right, Vector3.up).normalized * (Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.Q) ? -1f : 0f);
        }
        

        controller.Move(move * Time.deltaTime * Speed);
        if ((Mode == ViewMode.FPS || Mode == ViewMode.TPS))
        {
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * ViewSpeed);
        }
        else if(Mode == ViewMode.GPS)
        {
            if (move != Vector3.zero) transform.forward = move;
        }
        else
        {
            transform.Rotate(Vector3.up,( Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.Q) ? -1f : 0f) *ViewSpeed) ;
        }

        //if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded) velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);


        velocity.y += Gravity * Time.deltaTime;
        velocity /= Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Mode == ViewMode.FPS && Camera.transform.parent != FPS)
        {
            Camera.transform.SetParent(FPS, true);
            Camera.transform.localPosition = Vector3.zero;
            Camera.transform.localRotation = Quaternion.identity;
        }
        if (Mode == ViewMode.TPS && Camera.transform.parent != TPS)
        {
            Camera.transform.SetParent(TPS, true); Camera.transform.localPosition = Vector3.zero;
            Camera.transform.localRotation = Quaternion.identity;
        }
        if ((Mode == ViewMode.GPS || Mode == ViewMode.SKY) && Camera.transform.parent != GPS)
        {
            Camera.transform.SetParent(GPS, true); Camera.transform.localPosition = Vector3.zero;
            Camera.transform.localRotation = Quaternion.identity;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Images.Current.enabled = false;
            Images.Decrement();
            Images.Current.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Images.Current.enabled = false;
            Images.Increment();
            Images.Current.enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Mode = (ViewMode)Mathf.Clamp((int)Mode - 1, 0, 3);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Mode = (ViewMode)Mathf.Clamp((int)Mode + 1, 0, 3);
        }


    }

   

}
