using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public enum Game_State
{
    Running,
    Waiting,
    RedWin,
    BlueWin,
    Unprepared,
}

public class Rule_RMUL2025 : MonoBehaviourPun
{
    public float gameRunningTime, gameTime;
    public int redGold, blueGold;
    public int redWinPoints, blueWinPoints;
    public Game_State gameState;
    private float lastTime;
    private Referee_control localRobot;
    [PunRPC]
    public void Kill_Get_Xp(string nickname, int level)
    {
        if (localRobot.Get_nickname() == nickname)
        {
            localRobot.Kill_Xp(level);
        }
    }

    [PunRPC]
    public void Add_WinPoint(string nickname,int num)
    {
        if (localRobot.Get_nickname() == nickname)
        {
            localRobot.selfWinpoint += num;
            Add_Win_Points(num,localRobot.Get_Robot_color());
        }
    }

    public void Set_LocalRobot(Referee_control robot)
    {
        localRobot = robot;
    }
    private void Start()
    {
        gameState = Game_State.Unprepared;
        redWinPoints = 0;
        blueWinPoints = 0;
        redGold = 0;
        blueGold = 0;
        gameRunningTime = 0;
        gameTime = 60 * 5;

        StartCoroutine(ExecutePeriodically());
    }

    IEnumerator ExecutePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Sync_Time", RpcTarget.Others, gameRunningTime);
                photonView.RPC("Sync_Gold", RpcTarget.Others, redGold, Robot_color.RED);
                photonView.RPC("Sync_Gold", RpcTarget.Others, blueGold, Robot_color.BLUE);
                photonView.RPC("Sync_WinPoint", RpcTarget.Others, blueWinPoints, Robot_color.BLUE);
                photonView.RPC("Sync_WinPoint", RpcTarget.Others, redWinPoints, Robot_color.RED);
            }
        }
    }

    public void Start_Game()
    {
        gameState = Game_State.Waiting;
        redWinPoints = 0;
        blueWinPoints = 0;
        redGold = 0;
        blueGold = 0;
        gameRunningTime = 0;
        gameTime = 60 * 5;
        photonView.RPC("Sync_Game_state", RpcTarget.Others, gameState);
    }

    [PunRPC]
    void Sync_Gold(int gold, Robot_color robotColor)
    {
        if (robotColor == Robot_color.RED)
        {
            redGold = gold;
        }
        else if (robotColor == Robot_color.BLUE)
        {
            blueGold = gold;
        }
    }

    [PunRPC]
    void Sync_Game_state(Game_State state)
    {
        gameState = state;
    }

    [PunRPC]
    void Sync_WinPoint(int winPoint, Robot_color robotColor)
    {
        if (robotColor == Robot_color.RED)
        {
            redWinPoints = winPoint;
        }
        else if (robotColor == Robot_color.BLUE)
        {
            blueWinPoints = winPoint;
        }
    }

    public float Get_Alive_Time(int deadNum)
    {
        return 10 + deadNum * 10;
    }

    [PunRPC]
    void Sync_Time(float time)
    {
        gameRunningTime = time;
    }

    public int Get_Gold_Num(Robot_color robotColor)
    {
        if (robotColor == Robot_color.RED)
        {
            return redGold;
        }
        else if (robotColor == Robot_color.BLUE)
        {
            return blueGold;
        }
        else
        {
            Debug.LogWarning("机器颜色不合法");
            return 0;
        }
    }

    public Game_State Get_State()
    {
        return gameState;
    }

    public int Get_Win_Points(Robot_color robotColor)
    {
        if (robotColor == Robot_color.RED)
        {
            return redWinPoints;
        }
        else if (robotColor == Robot_color.BLUE)
        {
            return blueWinPoints;
        }
        else
        {
            Debug.LogWarning("机器颜色不合法");
            return 0;
        }
    }

    public int Get_time()
    {
        return (int)(gameTime - gameRunningTime);
    }

    public void Add_Gold(int amount, Robot_color robotColor)
    {
        if (robotColor == Robot_color.RED)
        {
            redGold += amount;
            photonView.RPC("Sync_Gold", RpcTarget.Others, redGold, robotColor);
        }
        else if (robotColor == Robot_color.BLUE)
        {
            blueGold += amount;
            photonView.RPC("Sync_Gold", RpcTarget.Others, blueGold, robotColor);
        }

        if (robotColor == Robot_color.All)
        {
            redGold += amount;
            blueGold += amount;
        }
    }

    public void Add_Win_Points(int amount, Robot_color robotColor)
    {
        if (robotColor == Robot_color.RED)
        {
            redWinPoints += amount;
            photonView.RPC("Sync_WinPoint", RpcTarget.Others, redWinPoints, robotColor);
        }
        else if (robotColor == Robot_color.BLUE)
        {
            blueWinPoints += amount;
            photonView.RPC("Sync_WinPoint", RpcTarget.Others, blueWinPoints, robotColor);
        }
    }

    void Update()
    {
        if (gameState == Game_State.Unprepared) return;
        if (!PhotonNetwork.IsMasterClient)
        {
            gameRunningTime += Time.deltaTime;
            lastTime = gameRunningTime;
        }
        else
        {
            if (gameState == Game_State.Waiting)
            {
                gameRunningTime += Time.deltaTime;
                if (gameRunningTime >= 10f)
                {
                    Add_Gold(200, Robot_color.All);
                    gameState = Game_State.Running;
                    gameRunningTime = 0;
                    photonView.RPC("Sync_Game_state", RpcTarget.Others, gameState);
                    photonView.RPC("Sync_Time", RpcTarget.Others, gameRunningTime);
                }
            }
            else if (gameState == Game_State.Running)
            {
                gameRunningTime += Time.deltaTime;

                {
                    if (lastTime < 60 && gameRunningTime >= 60f)
                    {
                        Add_Gold(200, Robot_color.All);
                    }

                    if (lastTime < 120 && gameRunningTime >= 120f)
                    {
                        Add_Gold(200, Robot_color.All);
                    }

                    if (lastTime < 180 && gameRunningTime >= 180f)
                    {
                        Add_Gold(300, Robot_color.All);
                    }

                    if (lastTime < 240 && gameRunningTime >= 240f)
                    {
                        Add_Gold(300, Robot_color.All);
                    }
                }
            }


            lastTime = gameRunningTime;
        }
    }
}