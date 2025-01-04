using System;
using Photon.Pun;
using UnityEngine;

public class ExitInGame_UI : MonoBehaviourPun
{
    private bool cursorVisible;
    private CursorLockMode cursorLock;
    private UI_parent parent;

    void Start()
    {
        parent = gameObject.GetComponent<UI_parent>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnExitGame()
    {
        parent.Set_Game_State(false);
        cursorVisible = true;
        cursorLock = CursorLockMode.Confined;
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.farClipPlane = 2000f;
        // 设置标签为 Main Camera
        cameraObject.tag = "MainCamera";
        PhotonNetwork.LeaveRoom();

        parent.Destroyself();
    }

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnStayGame()
    {
        cursorVisible = false;
        cursorLock = CursorLockMode.Locked;
        parent.Destroyself();
    }

    private void OnDestroy()
    {
        Cursor.visible = cursorVisible;
        Cursor.lockState = cursorLock;
    }
}