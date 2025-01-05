using System;
using TMPro;
using UnityEngine;

public class Rule_message_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redGold, blueGold, time,redWinpoint,blueWinpoint;
    private int RedGold, BlueGold, Time,RedWinpoint, BlueWinpoint;
    Game_State game_state;

    public void Set_Message(int red, int blue,int redWinpoint,int blueWinpoint, int time, Game_State state)
    {
        RedGold = red;
        BlueGold = blue;
        RedWinpoint = redWinpoint;
        BlueWinpoint = blueWinpoint;
        Time = time;
        game_state = state;
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
    }
}