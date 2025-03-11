using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Rule_message_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redGold, blueGold, time,redOutpostText,blueOutpostText,redBaseText,blueBaseText;
    private int RedGold, BlueGold, Time, RedWinpoint, BlueWinpoint;
    Game_State game_state;
    Robot_Data red1, blue1, red3, blue3,red4,blue4,redOutpost,blueOutpost,redBase,blueBase;
    public Sprite level1, level2, level3, level4, level5, level6, level7, level8, level9, level10;
    [SerializeField] private Image red1Hp,red3Hp, blue1Hp, blue3Hp,blue4Hp,red4Hp,red1Level,red3Level, blue1Level, blue3Level,blue4Level,red4Level;
    [SerializeField] private Image redOutpostHp, blueOutpostHp, redBaseHp, blueBaseHp;
    private Sprite[] level = new Sprite[10];
    public Rule rule;
    public void SetRule(Rule rule)
    {
        this.rule = rule;
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

    void Update()
    {
        if (rule)
        {
            RedGold = rule.redGold;
            BlueGold = rule.blueGold;
            red1 = rule.red1;
            blue1 = rule.blue1;
            red3 = rule.red3;
            blue3 = rule.blue3;
            red4 = rule.red4;
            blue4 = rule.blue4;
            redBase = rule.redBase;
            blueBase = rule.blueBase;
            redOutpost = rule.redOutpost;
            blueOutpost = rule.blueOutpost;
            game_state = rule.gameState;
            Time = rule.Get_time();
        }
    }
    void OnGUI()
    {
        redGold.text = RedGold.ToString();
        blueGold.text = BlueGold.ToString();
        if (game_state == Game_State.Running)
        {
            if (Time % 60 < 10)
                time.text = (Time / 60).ToString() + ":" + '0' + (Time % 60).ToString();
            else
                time.text = (Time / 60).ToString() + ":" + (Time % 60).ToString();
        }
        else if (game_state == Game_State.Waiting)
            time.text = "Waiting";
        redOutpostText.text = redOutpost.Hp.ToString();
        blueOutpostText.text = blueOutpost.Hp.ToString();
        redBaseText.text = redBase.Hp.ToString();
        blueBaseText.text = blueBase.Hp.ToString();
        red1Hp.fillAmount = (float)red1.Hp / (float)red1.MaxHp;
        blue1Hp.fillAmount = (float)blue1.Hp / (float)blue1.MaxHp;
        red3Hp.fillAmount = (float)red3.Hp / (float)red3.MaxHp;
        blue3Hp.fillAmount = (float)blue3.Hp / (float)blue3.MaxHp;
        red4Hp.fillAmount = (float)red4.Hp / (float)red4.MaxHp;
        blue4Hp.fillAmount = (float)blue4.Hp / (float)blue4.MaxHp;
        redOutpostHp.fillAmount = (float)redOutpost.Hp / (float)redOutpost.MaxHp;
        blueOutpostHp.fillAmount = (float)blueOutpost.Hp / (float)blueOutpost.MaxHp;
        redBaseHp.fillAmount = (float)redBase.Hp / (float)redBase.MaxHp;
        blueBaseHp.fillAmount = (float)blueBase.Hp / (float)blueBase.MaxHp;
        red1Level.sprite = level[red1.level];
        blue1Level.sprite = level[blue1.level];
        red3Level.sprite = level[red3.level];
        blue3Level.sprite = level[blue3.level];
        red4Level.sprite = level[red4.level];
        blue4Level.sprite = level[blue4.level];
    }
}