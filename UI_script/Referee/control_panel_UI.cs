using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class control_panel_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI robotname,sensitivityText;
    private string robotName;
    [SerializeField] private TextMeshProUGUI chassis, shoot;
    [SerializeField] private Slider sensitivity;
    public Chassis_Referee_type chassisValu;
    public Shoot_Referee_type shootValu;
    public float sensitivityValu = 0.5f;

    public void Set_Robot_Name(string robotName)
    {
        if (robotName == "RED1")
            this.robotName = "R1 - Hero";
        if (robotName == "RED2")
            this.robotName = "R2 - Standard";
        if (robotName == "BLUE1")
            this.robotName = "B1 - Hero";
        if (robotName == "BLUE2")
            this.robotName = "B2 - Standard";
    }

    public void OnSetHpfirst()
    {
        chassisValu = Chassis_Referee_type.HpFirst;
    }

    public void OnSetPowerfirst()
    {
        chassisValu = Chassis_Referee_type.PowerFirst;
    }

    public void OnSetCoolfirst()
    {
        shootValu = Shoot_Referee_type.CoolFirst;
    }

    public void OnSetHeatfirst()
    {
        shootValu = Shoot_Referee_type.HeatFirst;
    }

    private void OnGUI()
    {
        sensitivityValu = sensitivity.value;
        if (chassisValu == Chassis_Referee_type.HpFirst)
            chassis.text = "血量优先";
        else if (chassisValu == Chassis_Referee_type.PowerFirst)
            chassis.text = "功率优先";
        if (shootValu == Shoot_Referee_type.CoolFirst)
            shoot.text = "冷却优先";
        else if (shootValu == Shoot_Referee_type.HeatFirst)
            shoot.text = "热量优先";
        robotname.text = robotName;
        sensitivityText.text = ((int)(sensitivityValu * 100)).ToString();
    }
}