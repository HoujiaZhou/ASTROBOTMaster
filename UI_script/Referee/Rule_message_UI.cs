using System;
using TMPro;
using UnityEngine;

public class Rule_message_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI redGold, blueGold, time;
    private int RedGold, BlueGold, Time;
    Game_State game_state;
    public void Set_Message(int red, int blue, int time,Game_State state)
    {
        RedGold = red;
        BlueGold = blue;
        Time = time;
        game_state = state;
    }

    void OnGUI()
    {
        redGold.text = RedGold.ToString();
        blueGold.text = BlueGold.ToString();
        if(game_state == Game_State.Running)
            time.text = (Time / 60).ToString() + ":"+(Time % 60).ToString();
        else if(game_state == Game_State.Waiting)
            time.text = "Waiting";
        
    }
}