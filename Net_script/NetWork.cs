using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections;

public class NetWork : MonoBehaviourPunCallbacks
{
    public bool IsConnected = false, Isjoined = false, Iscreat = false;
    private Referee_control referee_;
    private Robot_control robot_control;
    private Tool tool_;

    private void Start()
    {
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties["Color"] = 0;
        playerProperties["Type"] = 0;
        playerProperties["referee"] = "";
        playerProperties["IsReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }


    public override void OnLeftRoom()
    {
        
        base.OnLeftRoom();
        Isjoined = false;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        IsConnected = true;
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        base.OnConnectedToMaster();
        IsConnected = false;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Isjoined = true;
    }


    public bool Connect_state()
    {
        return IsConnected;
    }

    public bool Room_state()
    {
        return Isjoined;
    }
}