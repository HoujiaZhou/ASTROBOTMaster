using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rule_message_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redGold, blueGold, time, redWinpoint, blueWinpoint;
    private int RedGold, BlueGold, Time, RedWinpoint, BlueWinpoint;
    Game_State game_state;
    Robot_Data red1, blue1, red3, blue3;
    public Sprite level1, level2, level3, level4, level5, level6, level7, level8, level9, level10;
    [SerializeField] private Image red1Hp,red3Hp, blue1Hp, blue3Hp,red1Level,red3Level, blue1Level, blue3Level;
    private Sprite[] level = new Sprite[10];
    public void Set_Message(int red, int blue, int redWinpoint, int blueWinpoint, int time, Game_State state)
    {
        RedGold = red;
        BlueGold = blue;
        RedWinpoint = redWinpoint;
        BlueWinpoint = blueWinpoint;
        Time = time;
        game_state = state;
    }

    public void Set_Robot_Data(Robot_Data red1, Robot_Data blue1, Robot_Data red3, Robot_Data blue3)
    {
        this.red1 = red1;
        this.blue1 = blue1;
        this.red3 = red3;
        this.blue3 = blue3;
    }

    private void Start()
    {
        level[0] = level1;
        level[1] = level2;
        level[2] = level3;
        level[3] = level4;
        level[4] = level5;
        level[5] = level6;
        level[6] = level7;
        level[7] = level8;
        level[8] = level9;
        level[9] = level10;
    }

    void OnGUI()
    {
        redGold.text = RedGold.ToString();
        blueGold.text = BlueGold.ToString();
        redWinpoint.text = RedWinpoint.ToString();
        blueWinpoint.text = BlueWinpoint.ToString();
        if (game_state == Game_State.Running)
        {
            if (Time % 60 < 10)
                time.text = (Time / 60).ToString() + ":" + '0' + (Time % 60).ToString();
            else
                time.text = (Time / 60).ToString() + ":" + (Time % 60).ToString();
        }
        else if (game_state == Game_State.Waiting)
            time.text = "Waiting";
    
        red1Hp.fillAmount = (float)red1.Hp / (float)red1.MaxHp;
        blue1Hp.fillAmount = (float)blue1.Hp / (float)blue1.MaxHp;
        red3Hp.fillAmount = (float)red3.Hp / (float)red3.MaxHp;
        blue3Hp.fillAmount = (float)blue3.Hp / (float)blue3.MaxHp;
        
        red1Level.sprite = level[red1.level];
        blue1Level.sprite = level[blue1.level];
        red3Level.sprite = level[red3.level];
        blue3Level.sprite = level[blue3.level];
    }
}