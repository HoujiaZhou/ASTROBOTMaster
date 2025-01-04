using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class mainMenu_UI : MonoBehaviourPun
{
    private bool Connect_state,Room_state;
    [SerializeField] private Image Connect_night;
    [SerializeField] private TextMeshProUGUI player_nameholder, join_roomholder;
    [SerializeField] private TMP_InputField player_name;
    [SerializeField] private TMP_InputField Room_name;
    [SerializeField] private Button join_room;
    private UI_parent parent;

    public void OnQuitgame_()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    private void Start()
    {
        parent = gameObject.GetComponent<UI_parent>();
    }

    public void Onconnect_()
    {
        if (!string.IsNullOrWhiteSpace(player_name.text) && !Connect_state)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "cn";
            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = true;
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "40874b42-0b4e-4222-bbf5-68ecd9511fe3";
            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "ns.photonengine.cn";
            PhotonNetwork.NickName = player_name.text;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            player_nameholder.text = "请输入非空姓名";
        }
    }

    public void Onjoin_room()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        if (Room_name.text == "")
        {
            join_roomholder.text = "请输入非空房间名";
        }
        else
        {
            PhotonNetwork.JoinOrCreateRoom(Room_name.text, roomOptions, default);
        }
    }

    private void Update()
    {
        Connect_state = parent.Connect_state();
        Room_state = parent.Room_state();
    }

    private void OnGUI()
    {
        if (Connect_state == true)
        {
            player_name.interactable = false;
            Room_name.interactable = true;
            join_room.interactable = true;
            if (Connect_night.color != Color.green)
                Connect_night.color = Color.green;
        }
        else if (Connect_state == false)
        {
            player_name.interactable = true;
            Room_name.interactable = false;
            join_room.interactable = false;
            if (Connect_night.color != Color.red)
                Connect_night.color = Color.red;
        }
    }
}