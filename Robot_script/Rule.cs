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

public struct Kill_Memsage
{
    public string killer_nickname, killed_nickname;
}

public class Rule : MonoBehaviourPun
{
    int[] chassis_HPlevel_Powerfirst_Infantry = { 150, 175, 200, 225, 250, 275, 300, 325, 350, 400 };
    int[] chassis_HPlevel_HPfirst_Infantry = { 200, 225, 250, 275, 300, 325, 350, 375, 400, 400 };
    int[] chassis_HPlevel_Powerfirst_Hero = { 200, 225, 250, 275, 300, 325, 350, 375, 400, 500 };
    int[] chassis_HPlevel_HPfirst_Hero = { 250, 275, 300, 325, 350, 375, 400, 425, 450, 500 };
    int[] chassis_Powerlevel_HPfirst_Infantry = { 85, 95, 105, 115, 125, 135, 145, 155, 165, 200 };
    int[] chassis_Powerlevel_Powerfirst_Infantry = { 110, 130, 140, 150, 160, 170, 180, 190, 195, 200 };
    int[] chassis_Powerlevel_HPfirst_Hero = { 85, 95, 105, 115, 125, 135, 145, 155, 165, 200 };
    int[] chassis_Powerlevel_Powerfirst_Hero = { 110, 130, 140, 150, 160, 170, 180, 190, 195, 200 };
    int[] shoot_Coolrate_Coolfirst_Infantry = { 40, 45, 50, 55, 60, 65, 70, 75, 80, 80 };
    int[] shoot_Heat_Coolfirst_Infantry = { 50, 85, 120, 155, 190, 225, 260, 295, 330, 400 };
    int[] shoot_Coolrate_Heatfirst_Infantry = { 10, 15, 20, 25, 30, 35, 40, 45, 50, 60 };
    int[] shoot_Heat_Heatfirst_Infantry = { 200, 250, 300, 350, 400, 450, 500, 550, 600, 650 };
    int[] shoot_Coolrate_Hero = { 40, 48, 56, 64, 72, 80, 88, 96, 104, 120 };
    int[] shoot_Heat_Hero = { 200, 230, 260, 290, 320, 350, 380, 410, 440, 500 };
    public float gameRunningTime, gameTime;
    public int redGold, blueGold;
    public int redWinPoints, blueWinPoints;
    public int OutposeMaxHp = 1500, BaseMaxHp = 5000;
    public Game_State gameState;
    public Robot_Data red1, blue1, red3, blue3, red4, blue4, redBase, blueBase, redOutpost, blueOutpost;
    private float lastTime;
    public Referee_control localRobot, localRedOutpost, localBlueOutpost, localRedBase, localBlueBase;
    public Kill_Memsage[] KillMemsages = new Kill_Memsage[100];
    public int killNum;
    public bool killMemsageUpdate;
    public string memsage;
    public float memsageTime;
    public bool normalMemsageUpdate;
    [SerializeField] Center_buff center_buff;
    [SerializeField] Supply_buff supply_buff_red, Supply_buff_blue;
    [SerializeField] CenterBuff_control centerBuff;

    public int Get_MaxHp(Chassis_Referee_type chassis, Robot_type type, int level)
    {
        if (type == Robot_type.Infantry)
        {
            if (chassis == Chassis_Referee_type.PowerFirst)
            {
                return chassis_HPlevel_Powerfirst_Infantry[level];
            }
            else
            {
                return chassis_HPlevel_HPfirst_Infantry[level];
            }
        }
        else if (type == Robot_type.Hero)
        {
            if (chassis == Chassis_Referee_type.PowerFirst)
            {
                return chassis_HPlevel_Powerfirst_Hero[level];
            }
            else
            {
                return chassis_HPlevel_HPfirst_Hero[level];
            }
        }
        else if (type == Robot_type.Outpost)
        {
            return OutposeMaxHp;
        }
        else if (type == Robot_type.Base)
        {
            return BaseMaxHp;
        }
        else return 0;
    }

    public int Get_MaxPower(Chassis_Referee_type chassis, Robot_type type, int level)
    {
        if (type == Robot_type.Infantry)
        {
            if (chassis == Chassis_Referee_type.PowerFirst)
            {
                return chassis_Powerlevel_Powerfirst_Infantry[level];
            }
            else
            {
                return chassis_Powerlevel_HPfirst_Infantry[level];
            }
        }
        else if (type == Robot_type.Hero)
        {
            if (chassis == Chassis_Referee_type.PowerFirst)
            {
                return chassis_Powerlevel_Powerfirst_Hero[level];
            }
            else
            {
                return chassis_Powerlevel_HPfirst_Hero[level];
            }
        }
        else return 0;
    }

    public int Get_CoolRate(Shoot_Referee_type chassis, Robot_type type, int level)
    {
        if (type == Robot_type.Infantry)
        {
            if (chassis == Shoot_Referee_type.CoolFirst)
            {
                return shoot_Coolrate_Coolfirst_Infantry[level];
            }
            else
            {
                return shoot_Coolrate_Heatfirst_Infantry[level];
            }
        }
        else if (type == Robot_type.Hero)
        {
            return shoot_Coolrate_Hero[level];
        }
        else return 0;
    }

    public int Get_MaxHeat(Shoot_Referee_type chassis, Robot_type type, int level)
    {
        if (type == Robot_type.Infantry)
        {
            if (chassis == Shoot_Referee_type.CoolFirst)
            {
                return shoot_Heat_Coolfirst_Infantry[level];
            }
            else
            {
                return shoot_Heat_Heatfirst_Infantry[level];
            }
        }
        else if (type == Robot_type.Hero)
        {
            return shoot_Heat_Hero[level];
        }
        else return 0;
    }

    [PunRPC]
    public void Sync_Kill_memsage(string killer_nickname, string killed_nickname, int killNum)
    {
        KillMemsages[killNum].killer_nickname = killer_nickname;
        KillMemsages[killNum].killed_nickname = killed_nickname;
        this.killNum = killNum;
        killMemsageUpdate = true;
    }

    [PunRPC]
    public void Sync_Normal_memsage(string memsage, float time)
    {
        this.memsage = memsage;
        memsageTime = time;
        normalMemsageUpdate = true;
    }

    [PunRPC]
    public void Kill_Get_Xp(string nickname, int level)
    {
        if (localRobot.Get_nickname() == nickname)
        {
            localRobot.Kill_Xp(level);
        }
    }

    [PunRPC]
    public void Give_Xp(string nickname, int xp)
    {
        if (localRobot.Get_nickname() == nickname)
        {
            localRobot.Add_Exp(xp);
        }

        if (nickname == "")
        {
            localRobot.Add_Exp(xp / 3);
        }

        //前哨站被击毁给予经验
    }

    [PunRPC]
    void Give_Defense(Robot_color color, int defense, float time)
    {
        if (localRobot.Get_Robot_color() == color)
        {
            localRobot.Add_Defense(defense, time);
        }

        if (color == Robot_color.RED)
        {
            if (localRedBase)
            {
                localRedBase.Add_Defense(defense, time);
            }

            if (localRedOutpost)
            {
                localRedOutpost.Add_Defense(defense, time);
            }
        }

        if (color == Robot_color.BLUE)
        {
            if (localBlueBase)
            {
                localBlueBase.Add_Defense(defense, time);
            }

            if (localBlueOutpost)
            {
                localBlueOutpost.Add_Defense(defense, time);
            }
        }
    }

    [PunRPC]
    void Give_DamageRate(Robot_color color, int rate, float time)
    {
        if (localRobot.Get_Robot_color() == color)
        {
            localRobot.Set_DamageRate(rate, time);
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
        else if (nickname == "RedBase")
        {
            redBase.Hp = hp;
            redBase.MaxHp = maxHp;
            redBase.WinPoint = winPoint;
            redBase.level = level;
        }
        else if (nickname == "BlueBase")
        {
            blueBase.Hp = hp;
            blueBase.MaxHp = maxHp;
            blueBase.WinPoint = winPoint;
            blueBase.level = level;
        }
        else if (nickname == "RedOutpost")
        {
            redOutpost.Hp = hp;
            redOutpost.MaxHp = maxHp;
            redOutpost.WinPoint = winPoint;
            redOutpost.level = level;
        }
        else if (nickname == "BlueOutpost")
        {
            blueOutpost.Hp = hp;
            blueOutpost.MaxHp = maxHp;
            blueOutpost.WinPoint = winPoint;
            blueOutpost.level = level;
        }
    }

    public void Kill_Memsage_Updata(string killer_nickname, string killed_nickname)
    {
        KillMemsages[++killNum].killer_nickname = killer_nickname;
        KillMemsages[killNum].killed_nickname = killed_nickname;
        photonView.RPC("Sync_Kill_memsage", RpcTarget.All, killer_nickname, killed_nickname, killNum);
    }

    public void Send_Memsage(string memsage, float time)
    {
        memsageTime = time;
        this.memsage = memsage;
        photonView.RPC("Sync_Normal_memsage", RpcTarget.All, memsage, time);
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
        else if (nickname == "RedBase")
        {
            redBase.Hp = hp;
            redBase.MaxHp = maxHp;
            redBase.WinPoint = winPoint;
            redBase.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, redBase.MaxHp, redBase.Hp, redBase.WinPoint,
                redBase.level);
        }
        else if (nickname == "BlueBase")
        {
            blueBase.Hp = hp;
            blueBase.MaxHp = maxHp;
            blueBase.WinPoint = winPoint;
            blueBase.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, blueBase.MaxHp, blueBase.Hp, blueBase.WinPoint,
                blueBase.level);
        }
        else if (nickname == "RedOutpost")
        {
            redOutpost.Hp = hp;
            redOutpost.MaxHp = maxHp;
            redOutpost.WinPoint = winPoint;
            redOutpost.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, redOutpost.MaxHp, redOutpost.Hp,
                redOutpost.WinPoint,
                redOutpost.level);
        }
        else if (nickname == "BlueOutpost")
        {
            blueOutpost.Hp = hp;
            blueOutpost.MaxHp = maxHp;
            blueOutpost.WinPoint = winPoint;
            blueOutpost.level = level;
            photonView.RPC("Sync_Other_Robot", RpcTarget.All, nickname, blueOutpost.MaxHp, blueOutpost.Hp,
                blueOutpost.WinPoint,
                blueOutpost.level);
        }
    }

    public void Set_LocalRobot(Referee_control robot)
    {
        localRobot = robot;
    }

    public void Set_LocalBuliding(Referee_control robot, string nickname)
    {
        if (nickname == "RedBase")
        {
            localRedBase = robot;
        }
        else if (nickname == "BlueBase")
        {
            localBlueBase = robot;
        }
        else if (nickname == "RedOutpost")
        {
            localRedOutpost = robot;
        }
        else if (nickname == "BlueOutpost")
        {
            localBlueOutpost = robot;
        }
    }

    private void Start()
    {
        gameState = Game_State.Unprepared;
        redWinPoints = 0;
        blueWinPoints = 0;
        redGold = 0;
        blueGold = 0;
        gameRunningTime = 0;
        gameTime = 60 * 7;
        killNum = 0;
        killMemsageUpdate = false;
        StartCoroutine(ExecutePeriodically());
    }

    IEnumerator ExecutePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Sync_Time", RpcTarget.Others, gameRunningTime);
                photonView.RPC("Sync_Gold", RpcTarget.Others, redGold, Robot_color.RED);
                photonView.RPC("Sync_Gold", RpcTarget.Others, blueGold, Robot_color.BLUE);
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
        gameTime = 60 * 7;
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
            if (center_buff) center_buff.Set_Work(false);
            if (supply_buff_red) supply_buff_red.Set_Work(false);
            if (Supply_buff_blue) Supply_buff_blue.Set_Work(false);
        }
        else
        {
            if (center_buff) center_buff.Set_Work(true);
            if (supply_buff_red) supply_buff_red.Set_Work(true);
            if (Supply_buff_blue) Supply_buff_blue.Set_Work(true);
        }

        if (gameState == Game_State.Unprepared) return;
        if (!PhotonNetwork.IsMasterClient)
        {
            gameRunningTime += Time.deltaTime;
            Winponit_Settle();
            lastTime = gameRunningTime;
        }
        else
        {
            if (gameState == Game_State.Waiting)
            {
                gameRunningTime += Time.deltaTime;
                if (gameRunningTime >= 10f)
                {
                    Add_Gold(400, Robot_color.All);
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
                    if (lastTime < 60f && gameRunningTime >= 60f)
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Send_Memsage("小能量机关可激活", 5f);
                            centerBuff.StartRotate(false);
                        }
                    if (lastTime < 150f && gameRunningTime >= 150f)
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Send_Memsage("小能量机关可激活", 5f);
                            centerBuff.StartRotate(false);
                        }
                    if (lastTime < 240f && gameRunningTime >= 240f)
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Send_Memsage("大能量机关可激活", 5f);
                            centerBuff.StartRotate(true);
                        }
                    if (lastTime < 315 && gameRunningTime >= 315f)
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Send_Memsage("大能量机关可激活", 5f);
                            centerBuff.StartRotate(true);
                        }
                    if (lastTime < 390f && gameRunningTime >= 390f)
                        if (PhotonNetwork.IsMasterClient)
                        {
                            Send_Memsage("大能量机关可激活", 5f);
                            centerBuff.StartRotate(true);
                        }
                    if (lastTime < 60 && gameRunningTime >= 60f)
                    {
                        Add_Gold(50, Robot_color.All);
                    }

                    if (lastTime < 120 && gameRunningTime >= 120f)
                    {
                        Add_Gold(50, Robot_color.All);
                    }

                    if (lastTime < 180 && gameRunningTime >= 180f)
                    {
                        Add_Gold(50, Robot_color.All);
                    }

                    if (lastTime < 240 && gameRunningTime >= 240f)
                    {
                        Add_Gold(50, Robot_color.All);
                    }

                    if (lastTime < 300 && gameRunningTime >= 300f)
                    {
                        Add_Gold(50, Robot_color.All);
                    }

                    if (lastTime < 360 && gameRunningTime >= 360f)
                    {
                        Add_Gold(150, Robot_color.All);
                    }
                }
                if (gameRunningTime >= 420)
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