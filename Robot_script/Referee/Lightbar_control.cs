using UnityEngine;

public class Lightbar_control : MonoBehaviour
{
    public Renderer light1;
    public Material blue, red, die;
    private Material main_color;
    public Referee_control referee;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (referee != null)
        {
            if (referee.Get_Robot_color() == Robot_color.RED)
            {
                main_color = red;
            }
            else
            {
                main_color = blue;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (referee != null)
        {
            if (referee.Get_robotHP() == 0)
            {
                light1.material = die;
            }
            else
            {
                light1.material = main_color;
            }
        }
        else
        {
            Debug.LogWarning("灯条没有绑定裁判系统");
        }
    }
}
