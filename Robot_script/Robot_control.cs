using System;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;


public enum Chassis_Mode
{
    Normal_mode = 0,
    Rotate_mode
};

public class Chassis_Data
{
    
    public Chassis_Mode Chassis_Mode;
    public float Vx_Speed, Vy_Speed, Vw_Speed;
    private int Max_speed;
    private int Speed;
    public bool Is_using_Cap;
    public float Super_cap_used_time = 0f, Super_cap_Max_time = 8f;
    private float last_time, deltatime;

    public Chassis_Data()
    {
        Chassis_Mode = Chassis_Mode.Normal_mode;
        Vx_Speed = 0;
        Vy_Speed = 0;
    }

    public void Set_Maxspeed(int maxspeed)
    {
        Max_speed = maxspeed;
    }

    public void UpdateChassis_data(bool iscontrol)
    {
        if (iscontrol)
        {
            deltatime = Time.deltaTime;
            Vx_Speed = -Input.GetAxis("Vertical") * Speed;
            Vy_Speed = -Input.GetAxis("Horizontal") * Speed;
            if (Vx_Speed * Vx_Speed + Vy_Speed * Vy_Speed > Speed * Speed)
            {
                float vx = Vx_Speed, vy = Vy_Speed;
                Vx_Speed = Speed * (vx / Mathf.Sqrt(vx * vx + vy * vy));
                Vy_Speed = Speed * (vy / Mathf.Sqrt(vx * vx + vy * vy));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (Chassis_Mode == Chassis_Mode.Rotate_mode)
                {
                    Chassis_Mode = Chassis_Mode.Normal_mode;
                }
                else
                {
                    Chassis_Mode = Chassis_Mode.Rotate_mode;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Super_cap_used_time < Super_cap_Max_time)
                {
                    Is_using_Cap = true;
                }
                else
                {
                    Is_using_Cap = false;
                }
            }
            else
            {
                Is_using_Cap = false;
            }

            if (Is_using_Cap == true)
            {
                if (Vx_Speed * Vx_Speed + Vy_Speed * Vy_Speed != 0)
                    Super_cap_used_time += Time.deltaTime;
                Speed = Max_speed * 2;
            }
            else
            {
                if (Super_cap_used_time > 0)
                    Super_cap_used_time -= Time.deltaTime / 3;
                if (Super_cap_used_time < 0) Super_cap_used_time = 0;
                Speed = Max_speed;
            }

            if (Chassis_Mode == Chassis_Mode.Rotate_mode)
            {
                Vw_Speed = 5;
            }
            else
            {
                Vw_Speed = 0;
            }
        }
        else
        {
            Vx_Speed = 0;
            Vy_Speed = 0;
            Is_using_Cap = false;
        }
    }
}

public class Gimbal_Data
{
    public float Yaw_target, pitch_target;
    public float Yaw, Pitch;
    public int bullet_num;
    private float SensitivityX, SensitivityY;
    private float last_time, deltatime;

    public Gimbal_Data(float Yaw_init, float Pitch_init, float sensitivityX, float sensitivityY)
    {
        Yaw_target = Yaw_init;
        pitch_target = Pitch_init;
        SensitivityX = sensitivityX;
        SensitivityY = sensitivityY;
    }

    public Gimbal_Data(float Yaw_init, float Pitch_init)
    {
        Yaw_target = Yaw_init;
        pitch_target = Pitch_init;
    }

    public void Set_sensitiviy(float x, float y)
    {
        SensitivityX = x;
        SensitivityY = y;
    }

    public void UpdataGimbal_Data(bool iscontrol)
    {
        deltatime = Time.deltaTime;
        if (iscontrol)
        {
            Yaw = Input.GetAxis("Mouse X") * SensitivityX * deltatime;
            Pitch = Input.GetAxis("Mouse Y") * SensitivityY * deltatime;
        }
        else
        {
            Yaw = 0;
            Pitch = 0;
        }
    }
}

public class Shoot_Data
{
    public bool isfire;
    public bool bullet_type;

    public void Set_bullet_type(bool type)
    {
        bullet_type = type;
    }

    public void UpdataShoot_Data(bool iscontrol)
    {
        if (iscontrol)
        {
            isfire = Input.GetMouseButton(0);
        }
        else isfire = false;
    }

    public Shoot_Data()
    {
        isfire = false;
        bullet_type = false;
    }
}

public class Robot_control : MonoBehaviourPun
{
    private float initSensitivityX= 300f, initSensitivityY= 300f;
    public float SensitivityX = 300f, SensitivityY = 300f;
    public bool iscontrol = true;
    public Gimbal_Data gimbal = new Gimbal_Data(0, 0);
    public Chassis_Data chassis = new Chassis_Data();
    public Shoot_Data shoot = new Shoot_Data();
    [SerializeField] private Referee_control referee_;
    private int maxSpeed;

    public void Set_MaxSpeed(int speed)
    {
        maxSpeed = speed;
    }

    public void Set_Sensitivity(float num)
    {
        SensitivityX = initSensitivityX * num;
        SensitivityY = initSensitivityY * num;
        gimbal.Set_sensitiviy(SensitivityX, SensitivityY);
    }
    void Start()
    {
        if (!photonView.IsMine) return;
        gimbal.Set_sensitiviy(SensitivityX, SensitivityY);
    }

    public void Stop_Control()
    {
        iscontrol = false;
    }

    public void Enable_Control()
    {
        iscontrol = true;
    }

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true) return;

        chassis.Set_Maxspeed(maxSpeed);
        chassis.UpdateChassis_data(iscontrol);
        gimbal.UpdataGimbal_Data(iscontrol);
        shoot.UpdataShoot_Data(iscontrol);
    }

    void FixedUpdate()
    {
    }
}