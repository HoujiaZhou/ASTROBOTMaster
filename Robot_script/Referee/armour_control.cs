using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;
public class armour_control : MonoBehaviourPun
{
   [SerializeField] private Renderer light1, light2;
   [SerializeField] private Material blue, red, blink;
   [SerializeField] private Material now_color;
    private Robot_color color_;
   [SerializeField] private Material main_color;
   [SerializeField] private Referee_control Robot;
    private int damage_total;
    private int attacked_count = 0;
    private bool Color_=true;
    public void armour_attacked(Robot_type bullet_type)
    {
        if (bullet_type == Robot_type.Infantry)
        {
            attacked_count++;
            damage_total += 10;
        }
        if (bullet_type == Robot_type.Hero)
        {
            attacked_count++;
            damage_total += 100;
        }
    }
    [PunRPC]
    public void armour_attacked_PUN(Robot_type bullet_type)
    {
        if (bullet_type == Robot_type.Infantry)
        {
            attacked_count++;
            damage_total += 10;
        }
        if (bullet_type == Robot_type.Hero)
        {
            attacked_count++;
            damage_total += 100;
        }
    }
    public int Get_robot_defense_buff()
    {
        return Robot.Get_defense_buff();
    }
    private bool blink_enabled;
    [PunRPC]
    public void Sync_color(bool IS_alive)
    {
        Debug.Log("调用了全局");
        if (!IS_alive)
        {
            light1.material = blink;
            light2.material = blink;
        }
        else
        {
            light1.material = main_color;
            light2.material = main_color;
        }
    }
    public void armour_blink(float time)
    {
        if (blink_enabled)
        {
            if (now_color != blink)
            {
                now_color = blink;
                light1.material = now_color;
                light2.material = now_color;
                Color_ = false;
                photonView.RPC("Sync_color", RpcTarget.Others, Color_);
            }
            if (time >= 0.02)
            {
                if (now_color != main_color)
                {
                    blink_enabled = false;
                    now_color = main_color;
                    light1.material = now_color;
                    light2.material = now_color;
                    Color_ = true;
                    photonView.RPC("Sync_color", RpcTarget.Others, Color_);
                }
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        color_ = Robot.Get_Robot_color();
        if (color_ == Robot_color.RED)
        {
            main_color = red;
        }
        else
        {
            main_color = blue;
        }
        light1.material = main_color;
        light2.material = main_color;
    }
    private float time;
    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)return ;
        if (Robot == null)
        {

            Debug.LogWarning("装甲板没有绑定机器");
        }
        else
        {
            if (Robot.Get_robotHP() == 0)
            {
                damage_total = 0;
                time = 0;
                if (now_color != blink)
                {
                    now_color = blink;
                    light1.material = now_color;
                    light2.material = now_color;
                    Color_ = false;
                    photonView.RPC("Sync_color", RpcTarget.Others, Color_);
                }
            }
            else
            {
                if (Robot.Get_robotHP() != 0 && blink_enabled == false && now_color != main_color)
                {
                    now_color = main_color;
                    light1.material = now_color;
                    light2.material = now_color;
                    Color_ = true;
                    photonView.RPC("Sync_color", RpcTarget.Others, Color_);
                }
                armour_blink(time);
                time += Time.deltaTime;
                if (time >= 0.05)
                {
                    int damage = damage_total;
                    if(damage!=0)
                    Robot.Damage_settle(damage);
                    //referee 造成伤害

                    damage_total -= damage;
                    time = 0;
                    if (damage != 0)
                    { blink_enabled = true; }
                }
            }

        }
    }
}
