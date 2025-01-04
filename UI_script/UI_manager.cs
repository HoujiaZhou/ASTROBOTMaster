using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UI_manager : MonoBehaviour
{
    Dictionary<string, int> uiDictionary = new Dictionary<string, int>();
    private bool Connect_state, Room_state, game_state;
    private GameObject[] UI_Instance = new GameObject[10];
    private GameObject[] UI_ = new GameObject[10];
    private bool[] UI_Network_ = new bool[10];
    private int UI_num = 0;
    private GameObject netWork;
    private int sortNum = 0;
    private GameObject Robot;

    private void Start()
    {
        netWork = GameObject.FindGameObjectWithTag("Network");
        string[] ui_name =
        {
            "Base",
            "Start",
            "Room",
            "Referee",
            "Robot",
            "ExitGame"
        };
        string[] ui_path =
        {
            "UI_prefabs/Base_image",
            "UI_prefabs/Start_UI",
            "UI_prefabs/Choose_UI",
            "UI_prefabs/Referee_UI",
            "UI_prefabs/Robot_UI",
            "UI_prefabs/ExitInGame_UI"
        };
        string[] ui_path_asset =
        {
            "Assets/Resources/UI_prefabs/Base_image.prefab",
            "Assets/Resources/UI_prefabs/Start_UI.prefab",
            "Assets/Resources/UI_prefabs/Choose_UI.prefab",
            "Assets/Resources/UI_prefabs/Referee_UI.prefab",
            "Assets/Resources/UI_prefabs/Robot_UI.prefab",
            "Assets/Resources/UI_prefabs/ExitInGame_UI.prefab"
        };
        for (int i = 0; i < ui_name.Length; i++)
        {
#if UNITY_EDITOR
            Creat_UI_instance(ui_name[i], ui_path_asset[i]);
#else
            Creat_UI_instance(ui_name[i],ui_path[i]);
#endif
        }
    }


    public void Creat_UI_instance(string nickname, string path)
    {
        uiDictionary.Add(nickname, UI_num);
#if UNITY_EDITOR
        UI_Instance[UI_num] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
#else
        UI_Instance[UI_num] = Resources.Load<GameObject>(path);
#endif
        if (UI_Instance[UI_num] == null)
        {
            Debug.LogError(nickname + "UI路径错误 " + path);
        }

        UI_num++;
    }

    public void Show_UI(string nickname, Vector3 place, bool isnetwork)
    {
        if (UI_[uiDictionary[nickname]]) return;
        if (!isnetwork)
            UI_[uiDictionary[nickname]] = Instantiate(UI_Instance[uiDictionary[nickname]], place, Quaternion.identity);
        else
            UI_[uiDictionary[nickname]] = PhotonNetwork.Instantiate(nickname, place, Quaternion.identity);
        UI_[uiDictionary[nickname]].GetComponent<UI_parent>()
            .Init(nickname, gameObject.GetComponent<UI_manager>(), sortNum++, isnetwork);
        UI_Network_[uiDictionary[nickname]] = isnetwork;
    }

    public void destroy_UI(string nickname)
    {
        if (UI_[uiDictionary[nickname]] == null)
        {
            Debug.LogError(nickname + "UI未被创建 无法销毁");
        }

        sortNum--;
        Destroy(UI_[uiDictionary[nickname]]);
    }

    public void destroy_AllUI()
    {
        for (int i = 0; i < UI_num; i++)
        {
            if (UI_Network_[i] == false)
            {
                if (UI_[i] != null)
                    Destroy(UI_[i]);
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (UI_[i] != null)
                        PhotonNetwork.Destroy(UI_[i]);
                }
            }
        }
    }

    public bool CheckNetworkState()
    {
        return Connect_state;
    }

    public bool CheckRoomState()
    {
        return Room_state;
    }

    public bool CheckGameState()
    {
        return game_state;
    }

    public void Set_Game_State(bool state)
    {
        if (state!=game_state)
        {
            destroy_AllUI();
        }
        game_state = state;
    }

    public void Set_Robot(GameObject robot)
    {
        Robot = robot;
    }

    public GameObject Get_Robot()
    {
        return Robot;
    }

    private void Update()
    {
        Connect_state = netWork.GetComponent<NetWork>().Connect_state();
        if (Room_state == false)
        {
            Room_state = netWork.GetComponent<NetWork>().Room_state();
            if (Room_state == true)
            {
                destroy_UI("Start");
            }
        }
        else
        {
            Room_state = netWork.GetComponent<NetWork>().Room_state();
        }
    }

    void OnGUI()
    {
        if (game_state == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            if (Room_state == false)
            {
                Show_UI("Base", new Vector3(0, 0, 0), false);
                Show_UI("Start", new Vector3(0, 0, 0), false);
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                    Show_UI("Room", new Vector3(0, 0, 0), true);
            }
        }

        if (game_state == true)
        {
            //绘制机器人UI
            
            Show_UI("Referee", new Vector3(0, 0, 0), false);
            Show_UI("Robot", new Vector3(0, 0, 0), false);
            if (Input.GetKeyDown(KeyCode.Escape)== true)
            {
                Show_UI("ExitGame", new Vector3(0, 0, 0), false);
            }
        }
    }
}