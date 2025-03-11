using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;

public class armour_control : MonoBehaviourPun
{
    [SerializeField] private Renderer light1, light2;
    [SerializeField] private Material blue, red, blink;
    [SerializeField] private Material now_color;
    private Robot_color color_;

    public int bigBulletDamageDealt = 100,
        smallBulletDamageDealt = 10,
        bigBulletExpCoefficient = 4,
        smallBulletExpCoefficient = 4;

    [SerializeField] private Material main_color;
    [SerializeField] private Referee_control Robot;
    private int attacked_count = 0;
    private bool Color_ = true;
    Damage_Log damage_log;

    public int GetExp(Robot_type robotType)
    {
        if (robotType == Robot_type.Hero)
        {
            return bigBulletExpCoefficient * bigBulletDamageDealt;
        }
        else
        {
            return smallBulletExpCoefficient * smallBulletDamageDealt;
        }
    }

    public void armour_attacked(Robot_type bullet_type, string nickname)
    {
        if (bullet_type == Robot_type.Infantry)
        {
            attacked_count++;
            damage_log.SmallBulletDamage += 10;
            damage_log.LastDamagenickname = nickname;
        }

        if (bullet_type == Robot_type.Hero)
        {
            attacked_count++;
            damage_log.BigBulletDamage += 100;
            damage_log.LastDamagenickname = nickname;
        }
    }

    [PunRPC]
    public void armour_attacked_PUN(Robot_type bullet_type, string nickname)
    {
        if (bullet_type == Robot_type.Infantry)
        {
            attacked_count++;
            damage_log.SmallBulletDamage += smallBulletDamageDealt;
            damage_log.LastDamagenickname = nickname;
            Robot.rule.photonView.RPC("Give_Xp", RpcTarget.All, nickname,(int)(smallBulletDamageDealt * smallBulletExpCoefficient *
                (100 - Robot.Get_defense_buff()) / 100) );
        }

        if (bullet_type == Robot_type.Hero)
        {
            attacked_count++;
            damage_log.BigBulletDamage += bigBulletDamageDealt;
            damage_log.LastDamagenickname = nickname;
            Robot.rule.photonView.RPC("Give_Xp", RpcTarget.All,nickname,(int)(bigBulletDamageDealt * bigBulletExpCoefficient *
                (100 - Robot.Get_defense_buff()) / 100));
        }
    }
    [PunRPC]
    public void armour_attacked_PUN(Robot_type bullet_type, string nickname,int damageRate)
    {
        int damage;
        if (bullet_type == Robot_type.Infantry)
            damage = smallBulletDamageDealt;
        else damage = bigBulletDamageDealt;
        
        damage = (int)((float)(damage) * ((float)(damageRate) / 100f));
        if (bullet_type == Robot_type.Infantry)
        {
            attacked_count++;
            damage_log.SmallBulletDamage += damage;
            damage_log.LastDamagenickname = nickname;
            Robot.rule.photonView.RPC("Give_Xp", RpcTarget.All, nickname,(int)(damage * smallBulletExpCoefficient *
                (100 - Robot.Get_defense_buff()) / 100) );
        }

        if (bullet_type == Robot_type.Hero)
        {
            attacked_count++;
            damage_log.BigBulletDamage += damage;
            damage_log.LastDamagenickname = nickname;
            Robot.rule.photonView.RPC("Give_Xp", RpcTarget.All,nickname,(int)(damage * bigBulletExpCoefficient *
                (100 - Robot.Get_defense_buff()) / 100));
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

        damage_log.clear();
        light1.material = main_color;
        light2.material = main_color;
    }

    private float time;

    // Update is called once per frame
    void Update()
    {
        if ((Robot.Get_Robot_type() == Robot_type.Base || Robot.Get_Robot_type() == Robot_type.Outpost) &&
            (!PhotonNetwork.IsMasterClient)) return;
        if (!photonView.IsMine &&
            (Robot.Get_Robot_type() == Robot_type.Hero || Robot.Get_Robot_type() == Robot_type.Infantry)) return;
        if (!Robot)
        {
            Debug.LogWarning("装甲板没有绑定机器");
        }
        else
        {
            if (Robot.Get_robotHP() == 0)
            {
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
                    int bigDamage = damage_log.BigBulletDamage;
                    int smallDamage = damage_log.SmallBulletDamage;
                    if (bigDamage != 0)
                        Robot.Damage_settle(bigDamage, Damage_type.BigBullet, damage_log.LastDamagenickname);
                    //referee 造成伤害
                    if (smallDamage != 0)
                        Robot.Damage_settle(smallDamage, Damage_type.SmallBullet, damage_log.LastDamagenickname);
                    damage_log.BigBulletDamage -= bigDamage;
                    damage_log.SmallBulletDamage -= smallDamage;
                    time = 0;
                    if (bigDamage + smallDamage != 0)
                    {
                        blink_enabled = true;
                    }
                }
            }
        }
    }
}