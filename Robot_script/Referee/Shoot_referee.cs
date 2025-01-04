using Photon.Pun;
using UnityEngine;

public class Shoot_referee : MonoBehaviourPun
{
    private int[] Max_heat_ ={200,250,300,350,400,450,500,550,600,650};
    private int[] cooling_rate_ ={10,15,20,25,30,35,40,45,50,60};
    private int level = 0;
    public Referee_control referee;
    private int allow_bullet_num = 0;
    private int shoot_num = 0;
    private int Max_heat, now_heat, cooling_rate;
    private Robot_shoot robot_Shoot;

    
    public void Shoot_one_bullet(Robot_type bullet_type)
    {
        if (bullet_type == Robot_type.Infantry)
        {
            shoot_num++;
            now_heat += 10;
            referee.Add_Exp(1);
            if (now_heat >= Max_heat * 2)
            {
                referee.Damage_by_MaxHP(((float)(now_heat - 2 * Max_heat)) / 250);
                now_heat = 2 * Max_heat;
                
            }
        }
        else
        {
            shoot_num++;
            now_heat += 100;
            referee.Add_Exp(10);
            if (now_heat >= Max_heat * 2)
            {
                referee.Damage_by_MaxHP(((float)(now_heat - 2 * Max_heat)) / 250);
                now_heat = 2 * Max_heat;
            }
        }
    }
    public Referee_control Get_referee()
    {
        return referee;
    }
    public string Get_nickname()
    {
        return referee.Get_nickname();
    }
    public bool Shoot_permission()
    {
        if (allow_bullet_num > shoot_num)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Buy_bullet(int num)
    {
        allow_bullet_num += num;
        
    }
    public Robot_type Get_shoot_type()
    {
        return referee.Get_Robot_type();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Max_heat = Max_heat_[level];
        cooling_rate= cooling_rate_[level];
        allow_bullet_num = 0;
        shoot_num = 0;
    }

    private float time;
    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)return;
        level = referee.Get_robot_level();
        Max_heat = Max_heat_[level];
        cooling_rate= cooling_rate_[level];
        if(referee.Get_robotHP()==0)
        {
            now_heat=0;
        }
        time += Time.deltaTime;
        if (time >= 0.1)
        {
            if (now_heat > Max_heat)
            {
                referee.Damage_by_MaxHP(((float)(now_heat - Max_heat)) / 250 / 10);
            }
            now_heat -= (int)(cooling_rate * time);
            if (now_heat < 0) now_heat = 0;
            time = 0;
        }
        robot_Shoot.Max_heat = Max_heat;
        robot_Shoot.now_heat = now_heat;
        robot_Shoot.allow_bullet_num = allow_bullet_num;
        robot_Shoot.shoot_num = shoot_num;
        referee.Set_shoot_data(robot_Shoot);
    }
}
