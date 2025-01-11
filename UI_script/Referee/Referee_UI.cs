using System.Net.Mime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Referee_UI : MonoBehaviour
{
    public enum Panel_exist
    {
        dead = 0b00000001,
        control_panel = 0b00000010,
        buy_panel = 0b00000100,
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Referee_control referee_;
    [SerializeField] private HP_barcontrol_UI hP_bar;
    [SerializeField] private Shoot_datacontrol_UI shoot;
    [SerializeField] private Level_UI level;
    [SerializeField] private Dead_Alive_UI dead_alive;
    [SerializeField] private Buy_bullet_UI buy_bullet;
    [SerializeField] private Rule_message_UI rule_message;
    [SerializeField] private Win_UI win_ui;
    [SerializeField] private kill_memsage_UI kill_memsage;
    [SerializeField] private control_panel_UI control_panel_UI;
    private GameObject robot;
    private UI_parent parent;
    private Robot_Case robot_case;
    private Robot_type robot_type;
    private Rule_RMUL2025 rule;
    private Robot_shoot _shootData;
    public int panel_;

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
            level.set_level(referee_.Get_robot_level(), referee_.Get_robot_exp(), referee_.Get_robot_next_exp(),
                referee_.Get_nickname());
            if (robot_case == Robot_Case.dead)
            {
                dead_alive.Set_Alive_Time(referee_.Get_Alive_TotalTime(), -referee_.Get_Alive_time());

                robot_case = referee_.Get_robot_case();
                if (robot_case == Robot_Case.Alive)
                {
                    if ((panel_ & (int)Panel_exist.dead) != 0)
                    {
                        panel_ -= (int)Panel_exist.dead;
                    }

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
                    panel_ = 0;
                    if ((panel_ & (int)Panel_exist.dead) == 0)
                    {
                        panel_ += (int)Panel_exist.dead;
                    }

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    buy_bullet.gameObject.SetActive(false);
                    control_panel_UI.gameObject.SetActive(false);
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
                if (Input.GetKeyDown(KeyCode.O) && robot_type == Robot_type.Infantry &&
                    rule.gameState == Game_State.Running && (panel_ | 0) == 0)
                {
                    if (buy_bullet.gameObject.activeSelf == false)
                    {
                        if ((panel_ & (int)Panel_exist.buy_panel) == 0)
                        {
                            panel_ |= (int)Panel_exist.buy_panel;
                        }

                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.Confined;
                        buy_bullet.gameObject.SetActive(true);
                        referee_.Stop_control();
                        buy_bullet.Init(_shootData.allow_bullet_num, rule.Get_Gold_Num(referee_.Get_Robot_color()),
                            robot_type, referee_.Get_robot_buff() & (int)(Robot_buff.supply), referee_);
                    }
                }

                if (Input.GetKeyDown(KeyCode.I) && robot_type == Robot_type.Hero &&
                    rule.gameState == Game_State.Running && (panel_ | 0) == 0)
                {
                    if (buy_bullet.gameObject.activeSelf == false)
                    {
                        if ((panel_ & (int)Panel_exist.buy_panel) == 0)
                        {
                            panel_ += (int)Panel_exist.buy_panel;
                        }

                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.Confined;
                        buy_bullet.gameObject.SetActive(true);
                        referee_.Stop_control();
                        buy_bullet.Init(_shootData.allow_bullet_num, rule.Get_Gold_Num(referee_.Get_Robot_color()),
                            robot_type, referee_.Get_robot_buff() & (int)(Robot_buff.supply), referee_);
                    }
                }

                if ((panel_ & (int)Panel_exist.buy_panel) != 0 && buy_bullet.gameObject.activeSelf == false)
                {
                    panel_ -= (int)Panel_exist.buy_panel;
                }

                if (buy_bullet.gameObject.activeSelf == true)
                {
                    buy_bullet.Message_Updata(rule.Get_Gold_Num(referee_.Get_Robot_color()));
                }

                rule_message.Set_Message(rule.Get_Gold_Num(Robot_color.RED), rule.Get_Gold_Num(Robot_color.BLUE),
                    rule.Get_Win_Points(Robot_color.RED), rule.Get_Win_Points(Robot_color.BLUE),
                    rule.Get_time(), rule.Get_State());
                rule_message.Set_Robot_Data(rule.red1, rule.blue1, rule.red3, rule.blue3);
                kill_memsage.Set_Kill_Memsage(rule.killMemsageUpdate, rule.KillMemsages[rule.killNum].killer_nickname,
                    rule.KillMemsages[rule.killNum].killed_nickname);
                rule.killMemsageUpdate = false;
            }

            if (Input.GetKeyDown((KeyCode.P)))
            {
                control_panel_UI.Set_Robot_Name(referee_.Get_nickname());
                if (control_panel_UI.gameObject.activeSelf == false && (panel_ | 0) == 0 &&
                    rule.gameState == Game_State.Waiting)
                {
                    if ((panel_ & (int)Panel_exist.control_panel) == 0)
                    {
                        panel_ += (int)Panel_exist.control_panel;
                    }

                    referee_.Stop_control();
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                    control_panel_UI.gameObject.SetActive(true);
                }
                else if (control_panel_UI.gameObject.activeSelf == true &&
                         (panel_ & (int)Panel_exist.control_panel) != 0)
                {
                    if ((panel_ & (int)Panel_exist.control_panel) != 0)
                    {
                        panel_ -= (int)Panel_exist.control_panel;
                    }

                    referee_.Enable_control();
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    control_panel_UI.gameObject.SetActive(false);
                }
            }

            referee_.Set_Shoot_Referee(control_panel_UI.shootValu);
            referee_.Set_Chassis_Referee(control_panel_UI.chassisValu);
            if (control_panel_UI.sensitivityValu == 0)
                referee_.Set_Sensitivity(1);
            else
                referee_.Set_Sensitivity(control_panel_UI.sensitivityValu * 2);
            if (rule.gameState == Game_State.BlueWin || rule.gameState == Game_State.RedWin)
            {
                referee_.Stop_control();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                win_ui.Set_Robot_Data(rule.red1, rule.red3, rule.blue1, rule.blue3);
                win_ui.Set_Point(rule.redWinPoints, rule.blueWinPoints);
            }

            if ((panel_ & (int)Panel_exist.control_panel) == 0 && (panel_ & (int)Panel_exist.buy_panel) == 0)
            {
                referee_.Enable_control();
            }
        }
    }
}