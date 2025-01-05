
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HP_barcontrol_UI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private TextMeshProUGUI HPtext;
    private bool Set_color = false;
    void Start()
    {
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Horizontal;
    }
    public void Set_hp(int now_hp, int max_hp, Robot_color robot_Color)
    {
        if (!Set_color)
        {
            if (robot_Color == Robot_color.RED)
            {
                image.color = Color.red;
            }
            else
            {
                image.color = Color.blue;
            }
            Set_color = true;
        }
        image.fillAmount = (float)now_hp / (float)max_hp;
        HPtext.text = now_hp.ToString() + '/' + max_hp.ToString();
    }
}
