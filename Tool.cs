using System;
using UnityEngine;


public class Tool : MonoBehaviour
{
    [SerializeField] private Transform RED1, RED2, BLUE1, BLUE2;
    [SerializeField] private GameObject RedOutpose, BlueOutpose ,RedBase,BlueBase,CenterBuff;
    public Transform Get_generate_place(string refereename_)
    {
        string refereename = refereename_;
        if(string.Equals(refereename, "Red1", StringComparison.OrdinalIgnoreCase)) return RED1;
        else if(string.Equals(refereename, "Red2", StringComparison.OrdinalIgnoreCase)) return RED2;
        else if(string.Equals(refereename, "BLUE1", StringComparison.OrdinalIgnoreCase)) return BLUE1;
        else if(string.Equals(refereename, "BLUE2", StringComparison.OrdinalIgnoreCase)) return BLUE2;
        else
        {
            
            Debug.LogError("未识别到该机器的型号");
            return null;
        }
    }

    public void StartGame()
    {
        RedOutpose.SetActive(true);
        BlueOutpose.SetActive(true);
        RedBase.SetActive(true);
        BlueBase.SetActive(true);
        CenterBuff.SetActive(true);
    }

    public void GameOver()
    {
        RedOutpose.SetActive(false);
        BlueOutpose.SetActive(false);
        RedBase.SetActive(false);
        BlueBase.SetActive(false);
        CenterBuff.SetActive(false);
    }
    public string Get_robot_prename(Robot_type type)
    {
        if (type == Robot_type.Hero)
        {
            return "Hero";
        }else if (type == Robot_type.Infantry)
        {
            return "Infantry";
        }
        else
        {
            Debug.LogError("不存在的机器人类型");
            return "";
        }
    }
}