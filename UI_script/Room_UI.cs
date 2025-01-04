using System;
using System.Collections;
using System.Threading;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room_UI : MonoBehaviourPun
{
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private TextMeshProUGUI blue1NicknameText, blue2NicknameText, red1NicknameText, red2NicknameText;
    [SerializeField] private TextMeshProUGUI Room_name;
    private Tool tool_;
    private UI_parent parent;
    [SerializeField] private string _localNickname;
    private string _nowPlace;
    private string blue1Text = "", blue2Text = "", red1Text = "", red2Text = "";
    private Robot_color playerColor;
    private Robot_type playerType;
    private string refereeNickname = "";

    [PunRPC]
    void StartGame()
    {
        // 锁定房间，防止其他玩家加入
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        Robot_color playerColor;
        Robot_type playerType;
        string refereeNickname, preNickname;
        Transform generatePlace;
        ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        playerColor = (Robot_color)customProperties["Color"];
        playerType = (Robot_type)customProperties["Type"];
        refereeNickname = (string)customProperties["referee"];
        generatePlace = tool_.Get_generate_place(refereeNickname);
        preNickname = tool_.Get_robot_prename(playerType);
        Generate_robot(playerColor, playerType, refereeNickname, preNickname, generatePlace);
        parent.Set_Game_State(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    [PunRPC]
    void Clear_Last_player(string  refereeNickname)
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["referee"] == refereeNickname)
        {
            Set_player_Ready(false);
            Set_localplayer_parameter(Robot_color.Null, Robot_type.Null, "");
            refereeNickname = "";
            _nowPlace = "";
        }
    }

    [PunRPC]
    void Set_Nickname(string nickname, string setPlace, string lastPlace)
    {
        if (lastPlace == "RED1")
            red1Text = "";
        if (lastPlace == "RED2")
            red2Text = "";
        if (lastPlace == "BLUE1")
            blue1Text = "";
        if (lastPlace == "BLUE2")
            blue2Text = "";
        if (setPlace == "RED1")
            red1Text = nickname;
        if (setPlace == "RED2")
            red2Text = nickname;
        if (setPlace == "BLUE1")
            blue1Text = nickname;
        if (setPlace == "BLUE2")
            blue2Text = nickname;
    }

    [PunRPC]
    void Sync_Room_Data(string r1, string r2, string b1,string b2)
    {
        red1Text = r1;
        red2Text = r2;
        blue1Text = b1;
        blue2Text = b2;
    }
    IEnumerator ExecutePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Sync_Room_Data",RpcTarget.All,red1Text, red2Text, blue1Text, blue2Text);
            }
        }
    }
    public void OnLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (CheckPlayer())
            {
                photonView.RPC("StartGame", RpcTarget.All);
            }
            else
            {
                startText.text = "";
                foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    ExitGames.Client.Photon.Hashtable customProperties = player.CustomProperties;
                    bool ready = (bool)customProperties["IsReady"];
                    if (!ready)
                        startText.text += player.NickName + " ";
                }

                startText.text += "玩家没有选择机器";
            }
        }
    }

    private bool CheckPlayer()
    {
        // 遍历当前房间的所有玩家
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            ExitGames.Client.Photon.Hashtable customProperties = player.CustomProperties;
            bool ready = (bool)customProperties["IsReady"];
            if (ready == false) return false;
        }

        return true;
    }

    public void Red1_set()
    {
        if (red1NicknameText.text != "" && red1NicknameText.text != PhotonNetwork.LocalPlayer.NickName)
            photonView.RPC("Clear_Last_player", RpcTarget.All, "RED1");
        playerColor = Robot_color.RED;
        playerType = Robot_type.Hero;
        refereeNickname = "RED1";
        Set_localplayer_parameter(playerColor, playerType, refereeNickname);
        if (refereeNickname != _nowPlace)
            photonView.RPC("Set_Nickname", RpcTarget.All, _localNickname, refereeNickname, _nowPlace);
        _nowPlace = refereeNickname;
    }

    public void Red2_set()
    {
        if (red2NicknameText.text != "" && red2NicknameText.text != PhotonNetwork.LocalPlayer.NickName)
            photonView.RPC("Clear_Last_player", RpcTarget.All, "RED2");
        playerColor = Robot_color.RED;
        playerType = Robot_type.Infantry;
        refereeNickname = "RED2";
        Set_localplayer_parameter(playerColor, playerType, refereeNickname);
        if (refereeNickname != _nowPlace)
            photonView.RPC("Set_Nickname", RpcTarget.All, _localNickname, refereeNickname, _nowPlace);
        _nowPlace = refereeNickname;
    }

    public void Blue1_set()
    {
        if (blue1NicknameText.text != "" && blue1NicknameText.text != PhotonNetwork.LocalPlayer.NickName)
            photonView.RPC("Clear_Last_player", RpcTarget.All, "BLUE1");
        playerColor = Robot_color.BLUE;
        playerType = Robot_type.Hero;
        refereeNickname = "BLUE1";
        Set_localplayer_parameter(playerColor, playerType, refereeNickname);
        if (refereeNickname != _nowPlace)
            photonView.RPC("Set_Nickname", RpcTarget.All, _localNickname, refereeNickname, _nowPlace);
        _nowPlace = refereeNickname;
    }

    public void Blue2_set()
    {
        if (blue2NicknameText.text != "" && blue2NicknameText.text != PhotonNetwork.LocalPlayer.NickName)
            photonView.RPC("Clear_Last_player", RpcTarget.All, "BLUE2");
        playerColor = Robot_color.BLUE;
        playerType = Robot_type.Infantry;
        refereeNickname = "BLUE2";
        Set_localplayer_parameter(playerColor, playerType, refereeNickname);
        if (refereeNickname != _nowPlace)
            photonView.RPC("Set_Nickname", RpcTarget.All, _localNickname, refereeNickname, _nowPlace);
        _nowPlace = refereeNickname;
    }

    private void Start()
    {
        if (_localNickname == "")
            _localNickname = PhotonNetwork.LocalPlayer.NickName;
        if (Room_name.text == "")
            Room_name.text = "Room_name:  " + PhotonNetwork.CurrentRoom.Name;
        parent = gameObject.GetComponentInParent<UI_parent>();
        GameObject TOOL = GameObject.FindGameObjectWithTag("Tool");
        _nowPlace = "";
        tool_ = TOOL.GetComponent<Tool>();
        if (tool_ == null)
        {
            Debug.LogError("场景缺少关键实体Tool");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = true;
            startText.text = "Start Game";
        }
        else
        {
            startButton.interactable = false;
            startText.text = "等待房主开启";
        }
        StartCoroutine(ExecutePeriodically());
    }

    private void OnGUI()
    {
        if (_localNickname == "")
            _localNickname = PhotonNetwork.LocalPlayer.NickName;
        blue1NicknameText.text = blue1Text;
        blue2NicknameText.text = blue2Text;
        red1NicknameText.text = red1Text;
        red2NicknameText.text = red2Text;
    }

    public void Set_player_Ready(bool Isready)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", Isready } });
    }

    public void Set_localplayer_parameter(Robot_color robotColor, Robot_type robotType, string refereeNickname)
    {
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties["Color"] = robotColor;
        playerProperties["Type"] = robotType;
        playerProperties["referee"] = refereeNickname;

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        Set_player_Ready(true);
    }

    public void Generate_robot(Robot_color robotColor, Robot_type robotType, string nickname, string robotPre,
        Transform place)
    {
        GameObject robot = PhotonNetwork.Instantiate(robotPre, place.position, place.rotation, 0);
        GameObject rules;
        rules = GameObject.FindGameObjectWithTag("Rule");
        while (!rules)
        {
            rules = GameObject.FindGameObjectWithTag("Rule");
        }
        robot.GetComponentInChildren<Referee_control>().Init(robotType, robotColor, nickname,rules.GetComponent<Rule_RMUL2025>());
        parent.Set_Robot(robot);
    }
}