using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public enum Game_State
{
    Running,
    Waiting,
    RedWin,
    BlueWin,
    Unprepared,
}

public struct Robot_Data
{
    public int MaxHp, Hp, WinPoint, level;
}

public class Rule_RMUL2025 : MonoBehaviourPun
{
    public float gameRunningTime, gameTime;
    public int redGold, blueGold;
    public int redWinPoints, blueWinPoints;
    public Game_State gameState;
    public Robot_Data red1, blue1, red3, blue3;
    private float lastTime;
    public Referee_control localRobot;
    [SerializeField] Center_buff center_buff;
    [SerializeField] Supply_buff supply_buff_red, Supply_buff_blue;

    [PunRPC]
    public void Kill_Get_Xp(string nickname, int level)
    {
        if (localRobot.Get_nickname() == nickname)
        {
            localRobot.Kill_Xp(level);
        }
    }

    [PunRPC]
    public void Add_WinPoint(string nickname, int num)
    {
        if (localRobot.Get_nickname() == nickname)
        {
            localRobot.selfWinpoint += num;
        }
    }

    [PunRPC]
    public void Sync_Other_Robot(string nickname, int maxHp, int hp, int winPoint, int level)
    {
        if (nickname == "RED1")
        {
            red1.Hp = hp;
            red1.MaxHp = maxHp;
            red1.WinPoint = winPoint;
            red1.level = level;
        }
        else if (nickname == "RED2")
        {
            red3.Hp = hp;
            red3.MaxHp = maxHp;
            red3.WinPoint = winPoint;
            red3.level = level;
        }
        else if (nickname == "BLUE1")
        {
            blue1.Hp = hp;
            blue1.MaxHp = maxHp;
            blue1.WinPoint = winPoint;
            blue1.level = level;
        }
        else if (nickname == "BLUE2")
        {
            blue3.Hp = hp;
            blue3.MaxHp = maxHp;
            blue3.WinPoint = winPoint;
            blue3.level = level;
        }
    }

    private void Winponit_Settle()
    {
        redWinPoints = red1.WinPoint + red3.WinPoint;
        blueWinPoints = blue1.WinPoint + blue3.WinPoint;
    }

    public void Set_Robot_Data(string nickname, int maxHp, int hp, int winPoint, int level)
    {
        if (nickname == "RED1")
        {
            red1.Hp = hp;
            red1.MaxHp = maxHp;
            red1.WinPoint = winPoint;
            red1.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, red1.MaxHp, red1.Hp, red1.WinPoint, red1.level);
        }
        else if (nickname == "RED2")
        {
            red3.Hp = hp;
            red3.MaxHp = maxHp;
            red3.WinPoint = winPoint;
            red3.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, red3.MaxHp, red3.Hp, red3.WinPoint, red3.level);
        }
        else if (nickname == "BLUE1")
        {
            blue1.Hp = hp;
            blue1.MaxHp = maxHp;
            blue1.WinPoint = winPoint;
            blue1.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, blue1.MaxHp, blue1.Hp, blue1.WinPoint,
                blue1.level);
        }
        else if (nickname == "BLUE2")
        {
            blue3.Hp = hp;
            blue3.MaxHp = maxHp;
            blue3.WinPoint = winPoint;
            blue3.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, blue3.MaxHp, blue3.Hp, blue3.WinPoint,
                blue3.level);
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
                // if(gameState == Game_State.Running)
                // {
                //     if(localRobot.Get_nickname() == "RED1")
                //         photonView.RPC("Sync_Other_Robot", RpcTarget.All, localRobot.Get_nickname(), red1);
                //     if(localRobot.Get_nickname() == "RED2")
                //         photonView.RPC("Sync_Other_Robot", RpcTarget.All, localRobot.Get_nickname(), red3);
                //     if(localRobot.Get_nickname() == "BLUE1")
                //         photonView.RPC("Sync_Other_Robot", RpcTarget.All, localRobot.Get_nickname(), blue1);
                //     if(localRobot.Get_nickname() == "BLUE2")
                //         photonView.RPC("Sync_Other_Robot", RpcTarget.All, localRobot.Get_nickname(), blue3);
                // }
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

    void Update()
    {
        if (gameState != Game_State.Running)
        {
            center_buff.Set_Work(false);
            supply_buff_red.Set_Work(false);
            Supply_buff_blue.Set_Work(false);
        }
        else
        {
            center_buff.Set_Work(true);
            supply_buff_red.Set_Work(true);
            Supply_buff_blue.Set_Work(true);
        }

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
                Winponit_Settle();
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
                if (gameRunningTime >= 300)
                {
                    if (redWinPoints > blueWinPoints)
                    {
                        gameState = Game_State.RedWin;
                    }
                    else if (blueWinPoints > redWinPoints)
                    {
                        gameState = Game_State.BlueWin;
                    }
                    else
                    {
                    }

                    photonView.RPC("Sync_Game_state", RpcTarget.Others, gameState);
                }
            }


            lastTime = gameRunningTime;
        }
    }
}