using System;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PID_controller
{
    private float Kp, Ki, Kd;
    private float time, last_time, deltatime;
    private float error, last_error;
    private float target, current;
    private float Pout, Iout, Dout, Iterm;
    public float output;
    public float max_output;

    public PID_controller()
    {
        Kp = 0;
        Ki = 0;
        Kd = 0;
    }

    public void Set_parameter(float kp, float ki, float kd)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
    }

    public void Set_parameter(float kp, float ki, float kd, float max_out)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
        max_output = max_out;
    }

    public void Set_target(float target)
    {
        this.target = target;
    }

    public float Get_output()
    {
        return output;
    }

    public void Set_measure(float measure)
    {
        current = measure;
    }

    public void Update()
    {
        time = Time.time;
        deltatime = time - last_time;
        error = target - current;
        Pout = error * Kp;
        Iterm = error * Ki * deltatime;
        Dout = Kd * (error - last_error) / deltatime;
        Iout += Iterm;
        output = Pout + Iout + Dout;
        output = Math.Clamp(output, -max_output, max_output);
        last_error = error;
        last_time = time;
    }
}


public class PID : MonoBehaviourPun
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PID_controller[] PID_instance = new PID_controller[30];
    private int PID_num = -1;

    public PID_controller Creat_PID()
    {
        return PID_instance[++PID_num];
    }

    void Awake()
    {
        for (int i = 0; i < 30; i++)
        {
            PID_instance[i] = new PID_controller();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        for (int i = 0; i <= PID_num; i++)
        {
            PID_instance[i].Update();
        }
    }
}