using System;
using Photon.Pun;
using UnityEngine;

public class UI_parent : MonoBehaviourPun
{
    public string UI_name = "";
    private UI_manager manager;
    public bool Isnetwork;
    [PunRPC]
    void Set_sort(int sort)
    {
        gameObject.GetComponent<Canvas>().sortingOrder = sort;
    }

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("UI_manager").GetComponent<UI_manager>();
    }

    public void Set_name(string name)
    {
        if (UI_name != "") return;
        UI_name = name;
    }

    public void Init(string name, UI_manager manager, int sort, bool isnetwork)
    {
        UI_name = name;
        this.manager = manager;
        Isnetwork = isnetwork;
        if (isnetwork)
        {
            photonView.RPC("Set_sort",RpcTarget.All,sort);
        }
        else
        {
            gameObject.GetComponent<Canvas>().sortingOrder = sort;
        }
    }
    public void Set_manager(UI_manager manager)
    {
        if (manager == null)
        {
            Debug.LogError(UI_name + "绑定管理器为空");
        }
        this.manager = manager;
    }
    public bool Connect_state()
    {
        return manager.CheckNetworkState();
    }
    public bool Room_state()
    {
        return manager.CheckRoomState();;
    }

    public void Set_Game_State(bool state)
    {
        if(manager == null)
            manager = GameObject.FindGameObjectWithTag("UI_manager").GetComponent<UI_manager>();
        manager.Set_Game_State(state);
    }
    public void Set_Robot(GameObject robot)
    {
        manager.Set_Robot(robot);
    }
    public void Destroyself()
    {
        manager.destroy_UI(UI_name);
    }

    public GameObject Get_Robot()
    {
        return manager.Get_Robot();
    }
}