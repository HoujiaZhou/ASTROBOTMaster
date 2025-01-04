using UnityEngine;
using System;
using Unity.VisualScripting;
using Photon.Pun;
using Unity.Mathematics;

public class Chassis_control_Hero : MonoBehaviourPun
{
    //     Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Wheel_groud_check FLWheel, FRWheel, BLWheel, BRWheel;
    [SerializeField] private PID pID;
    [SerializeField] private Plane Chassis_plane;
    [SerializeField] private float wheel_Kp, wheel_Ki, wheel_Kd, rotate_kp, rotate_ki, rotate_kd;
    public PID_controller vx_Pid, vy_Pid, vw_Pid;
    [SerializeField] private Rigidbody movebody;
    [SerializeField] private Transform movebody_;
    [SerializeField] private Robot_control chassis_Data;
    private bool Isgroud;
    //     public float Robot_Speed = 1000f;
    [SerializeField] private Transform gimbal_angle;
    private bool power = true;
    private float Isgroud_time;
    //     // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Set_power(bool power_)
    {
        power = power_;
    }
    void Start()
    {
        Chassis_plane = new Plane(FLWheel.transform.position, FRWheel.transform.position, BLWheel.transform.position);
        if (pID != null)
        {
            vx_Pid = pID.Creat_PID();
            vy_Pid = pID.Creat_PID();
            vw_Pid = pID.Creat_PID();
            vx_Pid.Set_parameter(wheel_Kp, wheel_Ki, wheel_Kd);
            vy_Pid.Set_parameter(wheel_Kp, wheel_Ki, wheel_Kd);
            vw_Pid.Set_parameter(rotate_kp, rotate_ki, rotate_kd);
            vx_Pid.Set_measure(movebody.linearVelocity.x);
            vy_Pid.Set_measure(movebody.linearVelocity.z);
            vw_Pid.Set_measure(movebody.angularVelocity.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true) return;
        Chassis_plane = new Plane(FLWheel.transform.position, FRWheel.transform.position, BLWheel.transform.position);
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected == true) return;
        if (chassis_Data != null)
        {

            Vector3 x_plane, y_plane, w_plane;
            x_plane = new Vector3(Chassis_plane.normal.y, -Chassis_plane.normal.x, 0).normalized;
            y_plane = new Vector3(0, -Chassis_plane.normal.z, Chassis_plane.normal.y).normalized;
            w_plane = new Vector3(Chassis_plane.normal.x, Chassis_plane.normal.y, Chassis_plane.normal.z);
            vx_Pid.Set_measure(movebody.linearVelocity.x * x_plane.x + movebody.linearVelocity.y * x_plane.y);
            vy_Pid.Set_measure(movebody.linearVelocity.z * y_plane.z + movebody.linearVelocity.y * y_plane.y);
            
            vw_Pid.Set_measure(movebody.angularVelocity.y);
            //更新一下PID当前值
            if (FLWheel.Wheel_Is_groud || FRWheel.Wheel_Is_groud || BLWheel.Wheel_Is_groud || BRWheel.Wheel_Is_groud)
            {

                Isgroud = true;
            }
            else
            {
                Isgroud = false;
            }
            float Vy_Speed = chassis_Data.chassis.Vy_Speed;
            float Vx_Speed = chassis_Data.chassis.Vx_Speed;
            float Vw_Speed = chassis_Data.chassis.Vw_Speed;
            float Angle_gimbal = gimbal_angle.rotation.eulerAngles.y * (float)Math.PI / 180;
            float Angle_chassis = transform.rotation.eulerAngles.y * (float)Math.PI / 180;
            Vector3 movement = new Vector3(0, 0, 0);
            while (Angle_gimbal > (float)Math.PI || Angle_gimbal < -(float)Math.PI)
            {
                if (Angle_gimbal > (float)Math.PI)
                {
                    Angle_gimbal -= (float)Math.PI * 2;
                }
                if (Angle_gimbal < -(float)Math.PI)
                {
                    Angle_gimbal += (float)Math.PI * 2;
                }
            }
            while (Angle_chassis > (float)Math.PI || Angle_chassis < -(float)Math.PI)
            {
                if (Angle_chassis > (float)Math.PI)
                {
                    Angle_chassis -= (float)Math.PI * 2;
                }
                if (Angle_chassis < -(float)Math.PI)
                {
                    Angle_chassis += (float)Math.PI * 2;
                }
            }
            if (chassis_Data.chassis.Chassis_Mode == Chassis_Mode.Normal_mode)
            {
                Vw_Speed += 1 * (Angle_gimbal - Angle_chassis);
                while (Vw_Speed > (float)Math.PI || Vw_Speed < -(float)Math.PI)
                {
                    if (Vw_Speed > (float)Math.PI)
                    {
                        Vw_Speed -= (float)Math.PI * 2;
                    }
                    if (Vw_Speed < -(float)Math.PI)
                    {
                        Vw_Speed += (float)Math.PI * 2;
                    }
                }
                Vw_Speed *= 5;
            }
            movement.x = - (Vy_Speed * (float)Math.Cos(Angle_gimbal) + Vx_Speed * (float)Math.Sin(Angle_gimbal));
            movement.z = - (-Vy_Speed * (float)Math.Sin(Angle_gimbal) + Vx_Speed * (float)Math.Cos(Angle_gimbal));
            vx_Pid.Set_target(movement.x);
            vy_Pid.Set_target(movement.z);
            vw_Pid.Set_target(Vw_Speed);
            if (Isgroud && power)
            {
                
                Vector3 force = new Vector3(0, 0, 0);
                force = vx_Pid.Get_output() * x_plane + y_plane * vy_Pid.Get_output();
                if (float.IsNaN(force.x) || float.IsNaN(force.y) || float.IsNaN(force.z))
                {
                    Debug.LogWarning("力向量包含无效的数值！");
                }
                else
                {
                    movebody.AddForce(force);
                }
                movebody.AddTorque(w_plane * vw_Pid.Get_output());
                Isgroud_time = 0;

            }
            else if (!Isgroud)
            {
                Isgroud_time += Time.deltaTime;
                movebody.AddForce(0, -9.8f * 100, 0, ForceMode.Acceleration);
                if (Isgroud_time >= 5)
                {
                    movebody_.eulerAngles = new Vector3(0, movebody_.eulerAngles.y, 0);
                    movebody_.position = new Vector3(movebody_.position.x, movebody_.position.y + 10, movebody_.position.z);
                    Isgroud_time = 0;
                }
            }
        }
    }

}
