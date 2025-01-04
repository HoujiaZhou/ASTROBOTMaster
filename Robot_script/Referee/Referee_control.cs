using System;
using System.Collections;
using System.Data;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public enum Robot_type
{
    Hero = 0,
    Engineer,
    Infantry,
    Null
};

public enum Robot_color
{
    BLUE,
    RED,
    All,
    Null,
};

public enum Robot_buff
{
    supply = 0b00000001,
};

public class Level_System
{
    public int level;
    public int exp;
    public int[] upgrade_exp = { 0, 400, 800, 1200, 1600, 2000, 2400, 2800, 3200, 4000, 99999999 };

    public Level_System()
    {
        level = 0;
        exp = 0;
    }

    public void give_exp(int num)
    {
        exp += num;
    }

    public bool Update()
    {
        bool Is_sync = false;
        while (exp > upgrade_exp[level + 1])
        {
            level++;
            Is_sync = true;
        }

        return Is_sync;
    }
}

public class Robot_power
{
    public bool Chassis_power = true, Gimbal_power = true, Shoot_Power = true;
    public Chassis_control chassis_Control;
    public Gimbal_control gimbal_Control;
    public Shoot_control shoot_Control;
    public Chassis_control_Hero chassis_Control_Hero;
    public Gimbal_control_Hero gimbal_Control_Hero;

    public Shoot_control_Hero shoot_Control_Hero;

    public void Check_power(Robot_Case robot_Case)
    {
        if (robot_Case == Robot_Case.dead)
        {
            Chassis_power = false;
            Gimbal_power = false;
            Shoot_Power = false;
        }
        else if (Shoot_Power == false && robot_Case == Robot_Case.Alive)
        {
            Chassis_power = true;
            Gimbal_power = true;
            Shoot_Power = false;
        }
    }

    public void Enable_shoot()
    {
        if (Chassis_power == true && Shoot_Power == false)
        {
            Shoot_Power = true;
        }
    }

    public void Update(Robot_type robot_Type)
    {
        if (robot_Type == Robot_type.Infantry)
        {
            if (!chassis_Control || !gimbal_Control || !shoot_Control)
            {
                Debug.LogWarning("裁判系统控制实例未导入");
            }
            else
            {
                chassis_Control.Set_power(Chassis_power);
                gimbal_Control.enabled = Gimbal_power;
                shoot_Control.enabled = Shoot_Power;
            }
        }
        else if (robot_Type == Robot_type.Hero)
        {
            if (!chassis_Control_Hero || !gimbal_Control_Hero || !shoot_Control_Hero)
            {
                Debug.LogWarning("裁判系统控制实例未导入");
            }
            else
            {
                chassis_Control_Hero.Set_power(Chassis_power);
                gimbal_Control_Hero.enabled = Gimbal_power;
                shoot_Control_Hero.enabled = Shoot_Power;
            }
        }
    }
}

public enum Robot_Case
{
    Alive = 0,
    dead,
    revive
};

public enum Chassis_referee_mode
{
    HP_first = 0,
    power_first
};

public class Robot_chassis
{
    bool Is_sync = false;
    public int robotHP, robot_MAXHP;
    public int chassis_defense_buff;
    private float Supply_rate;
    public float defense_exist_time;
    public float dead_time;
    public float whole_time;
    public Robot_Case robot_Case;
    public int buff;
    public Chassis_referee_mode chassis_mode;
    public int deadNum;
    private Rule_RMUL2025 rule;

    public Robot_chassis()
    {
        deadNum = 0;
        robot_MAXHP = 150;
        chassis_defense_buff = 0;
        defense_exist_time = 0;
        robot_Case = Robot_Case.Alive;
    }

    public void Add_rule(Rule_RMUL2025 rule)
    {
        this.rule = rule;
    }

    public void Set_supply_rate(float supply_rate)
    {
        Supply_rate = supply_rate;
    }

    public void Add_defense_buff(int defense_rate, float defense_time)
    {
        if (chassis_defense_buff < defense_rate)
        {
            chassis_defense_buff = defense_rate;
            defense_exist_time = -defense_time;
            Is_sync = true;
        }
        else if (chassis_defense_buff == defense_rate)
        {
            defense_exist_time -= defense_time;
            Is_sync = true;
        }
        else if (chassis_defense_buff > defense_rate)
        {
            return;
        }
    }

    public void Kill_robot(int time)
    {
        deadNum++;
        robotHP = 0;
        whole_time = time;
        Add_defense_buff(100, time);
        robot_Case = Robot_Case.dead;
        dead_time -= time;
    }

    public void Alive_robot()
    {
        if (dead_time >= 0)
        {
            Add_defense_buff(100, 10);
            robot_Case = Robot_Case.Alive;
            robotHP = robot_MAXHP / 5;
        }
    }

    private float supply_time;

    public bool Update()
    {
        Is_sync = false;
        if (Supply_rate != 0 && robot_Case == Robot_Case.Alive)
        {
            supply_time += Time.deltaTime;
            if (supply_time >= 1)
            {
                if (robotHP != robot_MAXHP)
                {
                    robotHP += (int)(Supply_rate / 100 * robot_MAXHP);
                    if (robotHP > robot_MAXHP)
                    {
                        robotHP = robot_MAXHP;
                    }

                    Is_sync = true;
                }

                supply_time = 0;
            }
        }
        else
        {
            supply_time = 0;
        }

        if (defense_exist_time > 0)
        {
            Is_sync = true;
            chassis_defense_buff = 0;
            defense_exist_time = 0;
        }
        else
        {
            defense_exist_time += Time.deltaTime;
        }

        if (robotHP <= 0)
        {
            if (robot_Case == Robot_Case.Alive)
            {
                Kill_robot((int)rule.Get_Alive_Time(deadNum));
            }
        }

        if (robot_Case == Robot_Case.dead)
        {
            if (dead_time >= 0)
            {
                dead_time = 0;
            }
            else
            {
                dead_time += Time.deltaTime;
            }
        }

        return Is_sync;
    }
}

public struct Robot_shoot
{
    public int allow_bullet_num;
    public int shoot_num;
    public int Max_heat, now_heat;
}

public class Referee_control : MonoBehaviourPun
{
    public Robot_type robot_type;
    public Robot_color robot_color;
    private string referee_nickname;
    int[] chassis_HPlevel_powerfirst = { 150, 175, 200, 225, 250, 275, 300, 325, 350, 400 };
    int[] chassis_HPlevel_HPfirst = { 200, 225, 250, 275, 300, 325, 350, 375, 400, 400 };

    [SerializeField] private Chassis_control chassis_Control;
    [SerializeField] private Gimbal_control gimbal_Control;
    [SerializeField] private Shoot_control shoot_Control;
    [SerializeField] private Chassis_control_Hero chassis_Control_Hero;
    [SerializeField] private Gimbal_control_Hero gimbal_Control_Hero;
    [SerializeField] private Shoot_control_Hero shoot_Control_Hero;
    [SerializeField] private Shoot_referee shootReferee;
    private Level_System level_system = new Level_System();
    private Robot_power robot_Power = new Robot_power();
    private Robot_chassis robot_Chassis = new Robot_chassis();
    private Robot_shoot robot_Shoot;
    public Rule_RMUL2025 rule;
    private Robot_control robot_Control;
    public bool initflag = false;

    [PunRPC]
    public void Sync_referee(Robot_type robot_Type, Robot_color robot_Color, string NickName)
    {
        referee_nickname = NickName;
        robot_type = robot_Type;
        robot_color = robot_Color;
    }

    [PunRPC]
    public void Sync_Chassis_data(int Hp, int rate)
    {
        robot_Chassis.robotHP = Hp;
        robot_Chassis.chassis_defense_buff = rate;
    }

    [PunRPC]
    public void Sync_level(int level)
    {
        level_system.level = level;
    }

    public void Add_Exp(int exp)
    {
        level_system.give_exp(exp);
    }

    public void Init(Robot_type robot_Type, Robot_color robot_Color, string NickName, Rule_RMUL2025 rule)
    {
        robot_type = robot_Type;
        robot_color = robot_Color;
        referee_nickname = NickName;
        this.rule = rule;
        robot_Chassis.Add_rule(rule);
        photonView.RPC("Sync_referee", RpcTarget.Others, robot_Type, robot_Color, NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            rule.Start_Game();
        }

        initflag = true;
        StartCoroutine(ExecutePeriodically());
    }

    public void Set_shoot_data(Robot_shoot shoot_data)
    {
        robot_Shoot = shoot_data;
    }

    public string Get_nickname()
    {
        return referee_nickname;
    }

    public Robot_Case Get_robot_case()
    {
        return robot_Chassis.robot_Case;
    }

    public Robot_shoot Get_shoot_data()
    {
        return robot_Shoot;
    }


    public Robot_color Get_Robot_color()
    {
        return robot_color;
    }

    public Robot_type Get_Robot_type()
    {
        return robot_type;
    }

    public int Get_robot_exp()
    {
        return level_system.exp - level_system.upgrade_exp[level_system.level];
    }

    public float Get_Alive_time()
    {
        return robot_Chassis.dead_time;
    }

    public float Get_Alive_TotalTime()
    {
        return robot_Chassis.whole_time;
    }

    public int Get_robot_next_exp()
    {
        return level_system.upgrade_exp[level_system.level + 1] - level_system.upgrade_exp[level_system.level];
    }

    public int Get_defense_buff()
    {
        return robot_Chassis.chassis_defense_buff;
    }

    public int Get_robotHP()
    {
        return robot_Chassis.robotHP;
    }

    public int Get_robotMaxHP()
    {
        return robot_Chassis.robot_MAXHP;
    }

    public int Get_robot_level()
    {
        return level_system.level;
    }

    public int Get_robot_buff()
    {
        return robot_Chassis.buff;
    }

    public void Add_buff(Robot_buff buff_type, int buff_time)
    {
        if (buff_type == Robot_buff.supply)
        {
            if (buff_time == 1)
            {
                if (robot_Chassis.robot_Case == Robot_Case.Alive)
                {
                    robot_Power.Shoot_Power = true;
                }

                robot_Chassis.buff = robot_Chassis.buff | (int)buff_type;
                robot_Chassis.Set_supply_rate(25f);
            }
            else
            {
                if ((robot_Chassis.buff & (int)buff_type) != 0)
                {
                    robot_Chassis.buff -= (int)buff_type;
                }

                robot_Chassis.Set_supply_rate(0f);
            }
        }
    }

    public void Buy_bullet(int num)
    {
        shootReferee.Buy_bullet(num);
        if (robot_type == Robot_type.Hero)
            rule.Add_Gold(num * -10, robot_color);
        else rule.Add_Gold(num * -1, robot_color);
    }

    public void Damage_settle(int total_damage)
    {
        robot_Chassis.robotHP -= total_damage * (1 - robot_Chassis.chassis_defense_buff / 100);
        photonView.RPC("Sync_Chassis_data", RpcTarget.Others, robot_Chassis.robotHP,
            robot_Chassis.chassis_defense_buff);
    }

    public void Damage_settle(int total_damage, bool Real_injury)
    {
        if (Real_injury == true)
            robot_Chassis.robotHP -= total_damage;
        photonView.RPC("Sync_Chassis_data", RpcTarget.Others, robot_Chassis.robotHP,
            robot_Chassis.chassis_defense_buff);
    }

    public void Damage_by_MaxHP(float scale)
    {
        robot_Chassis.robotHP -= (int)(scale * robot_Chassis.robot_MAXHP);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        robot_Chassis.robotHP = chassis_HPlevel_powerfirst[0];
        robot_Chassis.robot_MAXHP = chassis_HPlevel_powerfirst[0];
        robot_Power.chassis_Control = chassis_Control;
        robot_Power.gimbal_Control = gimbal_Control;
        robot_Power.shoot_Control = shoot_Control;
        robot_Power.chassis_Control_Hero = chassis_Control_Hero;
        robot_Power.gimbal_Control_Hero = gimbal_Control_Hero;
        robot_Power.shoot_Control_Hero = shoot_Control_Hero;
    }

    public void Stop_control()
    {
        robot_Control.Stop_Control();
    }

    public void Enable_control()
    {
        robot_Control.Enable_Control();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        if (!rule || !robot_Control)
        {
            if (!rule)
            {
                GameObject rules = GameObject.FindGameObjectWithTag("Rule");
                if (rules)
                {
                    rule = rules.GetComponent<Rule_RMUL2025>();
                }
            }

            if (!robot_Control)
                robot_Control = GetComponentInParent<Robot_control>();
            if (rule)
                robot_Chassis.Add_rule(rule);
        }
        else
        {
            if (initflag == false)
            {
                Stop_control();
                return;
            } // 在为初始化之前不进行操作

            if (robot_Chassis.Update())
            {
                
            }

            robot_Power.Check_power(robot_Chassis.robot_Case);
            robot_Power.Update(robot_type);
            if (level_system.Update())
            {
            }

            if (robot_Chassis.robot_MAXHP < chassis_HPlevel_powerfirst[level_system.level])
            {
                robot_Chassis.robotHP += chassis_HPlevel_powerfirst[level_system.level] - robot_Chassis.robot_MAXHP;
                robot_Chassis.robot_MAXHP = chassis_HPlevel_powerfirst[level_system.level];
            }

            int newhp = robot_Chassis.robotHP;
        }
    }

    IEnumerator ExecutePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (photonView.IsMine)
            {
                photonView.RPC("Sync_level", RpcTarget.Others, level_system.level);
                photonView.RPC("Sync_Chassis_data", RpcTarget.Others, robot_Chassis.robotHP,
                    robot_Chassis.chassis_defense_buff);
            }
        }
    }

    public void Alive_robot()
    {
        robot_Chassis.Alive_robot();
    }
}