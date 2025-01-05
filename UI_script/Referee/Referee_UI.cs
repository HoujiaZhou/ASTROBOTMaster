using UnityEngine;

public class Referee_UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Referee_control referee_;
    [SerializeField] private HP_barcontrol_UI hP_bar;
    [SerializeField] private Shoot_datacontrol_UI shoot;
    [SerializeField] private Level_UI level;
    [SerializeField] private Dead_Alive_UI dead_alive;
    [SerializeField] private Buy_bullet_UI buy_bullet;
    [SerializeField] private Rule_message_UI rule_message;
    private GameObject robot;
    private UI_parent parent;
    private Robot_Case robot_case;
    private Robot_type robot_type;
    private Rule_RMUL2025 rule;
    private Robot_shoot _shootData;

    void Start()
    {
        parent = gameObject.GetComponent<UI_parent>();
        robot = parent.Get_Robot();
    }

    public void OnAlive()
    {
        referee_.Alive_robot();
    }

    public void OnAlive_byGold()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!referee_)
        {
            if (robot)
            {
                referee_ = robot.GetComponentInChildren<Referee_control>();
            }
            else
            {
                parent = gameObject.GetComponent<UI_parent>();
                robot = parent.Get_Robot();
            }
        }
        else
        {
            if (referee_.initflag == false) return;
            _shootData = referee_.Get_shoot_data();
            robot_type = referee_.Get_Robot_type();
            hP_bar.Set_hp(referee_.Get_robotHP(), referee_.Get_robotMaxHP(), referee_.Get_Robot_color());
            shoot.Set_shoot(_shootData);
            level.set_level(referee_.Get_robot_level(), referee_.Get_robot_exp(), referee_.Get_robot_next_exp());
            if (robot_case == Robot_Case.dead)
            {
                dead_alive.Set_Alive_Time(referee_.Get_Alive_TotalTime(), -referee_.Get_Alive_time());

                robot_case = referee_.Get_robot_case();
                if (robot_case == Robot_Case.Alive)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    dead_alive.Set_Alive_Time(0, 0);
                }
            }
            else
            {
                robot_case = referee_.Get_robot_case();
                if (robot_case == Robot_Case.dead)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    dead_alive.Set_Damage_Log(referee_.Get_damage_Log());
                    dead_alive.Set_Alive_Time(referee_.Get_Alive_TotalTime(), -referee_.Get_Alive_time());
                }
            }

            if (!rule)
            {
                rule = referee_.rule;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.O) && robot_type == Robot_type.Infantry)
                {
                    if (buy_bullet.gameObject.activeSelf == false)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.Confined;
                        buy_bullet.gameObject.SetActive(true);
                        referee_.Stop_control();
                        buy_bullet.Init(_shootData.allow_bullet_num, rule.Get_Gold_Num(referee_.Get_Robot_color()),
                            robot_type, referee_.Get_robot_buff() & (int)(Robot_buff.supply), referee_);
                    }
                }

                if (Input.GetKeyDown(KeyCode.I) && robot_type == Robot_type.Hero)
                {
                    if (buy_bullet.gameObject.activeSelf == false)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.Confined;
                        buy_bullet.gameObject.SetActive(true);
                        referee_.Stop_control();
                        buy_bullet.Init(_shootData.allow_bullet_num, rule.Get_Gold_Num(referee_.Get_Robot_color()),
                            robot_type, referee_.Get_robot_buff() & (int)(Robot_buff.supply), referee_);
                    }
                }

                if (buy_bullet.gameObject.activeSelf == true)
                {
                    buy_bullet.Message_Updata(rule.Get_Gold_Num(referee_.Get_Robot_color()));
                }

                rule_message.Set_Message(rule.Get_Gold_Num(Robot_color.RED), rule.Get_Gold_Num(Robot_color.BLUE),
                    rule.Get_Win_Points(Robot_color.RED), rule.Get_Win_Points(Robot_color.BLUE),
                rule.Get_time(), rule.Get_State());
            }

            if (buy_bullet.gameObject.activeSelf == false)
            {
                referee_.Enable_control();
            }
        }
    }
}