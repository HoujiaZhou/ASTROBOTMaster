using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shoot_datacontrol_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heat_text;
    [SerializeField] private TextMeshProUGUI num_text;
    [SerializeField] private Image Heatcir;
    public void Set_shoot(Robot_shoot robot_Shoot)
    {
        num_text.text = robot_Shoot.shoot_num.ToString() + '/' + robot_Shoot.allow_bullet_num.ToString();
        heat_text.text = robot_Shoot.now_heat.ToString() + '/' + robot_Shoot.Max_heat.ToString();
        float scale;
        if (robot_Shoot.now_heat == 0)
        {
            scale = 0;
        }
        else
        {
            scale = (float)robot_Shoot.now_heat / (float)robot_Shoot.Max_heat;
        }
        if(scale<=1)
        {
            Heatcir.color = Color.white;
            Heatcir.fillAmount = scale;
        }
        else
        {
            Heatcir.color = Color.red;
            Heatcir.fillAmount = 1;
        }
    }
}
