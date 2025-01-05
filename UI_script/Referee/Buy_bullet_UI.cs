using System;
using TMPro;
using UnityEngine;

public class Buy_bullet_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buyNum, allowNum, text;
    [SerializeField] private TextMeshProUGUI Buy10, Buy20, Buy50, Buy100;
    [SerializeField] private TextMeshProUGUI Reduce10, Reduce20, Reduce50, Reduce100;
    private int buyBulletNum;
    private int maxBullet, nowBullet;
    private Robot_type robotType;
    private Referee_control referee;
    private void OnEnable()
    {
        buyBulletNum = 0;
    }

    public void Bullet_Add_10()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum += 1;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum += 10;
        }
        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Bullet_Add_20()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum += 2;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum += 20;
        }
        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Bullet_Add_50()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum += 5;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum += 50;
        }
        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Bullet_Add_100()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum += 10;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum += 100;
        }
        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Bullet_Reduce_10()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum -= 1;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum -= 10;
        }
        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Bullet_Reduce_20()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum -= 2;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum -= 20;
        }
        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Bullet_Reduce_50()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum -= 5;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum -= 50;
        }
        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Bullet_Reduce_100()
    {
        if (robotType == Robot_type.Hero)
        {
            buyBulletNum -= 10;
        }
        else if (robotType == Robot_type.Infantry)
        {
            buyBulletNum -= 100;
        }

        if (buyBulletNum < 0)
        {
            buyBulletNum = 0;
        }

        if (buyBulletNum > maxBullet)
        {
            buyBulletNum = maxBullet;
        }
    }

    public void Confirm_Buying()
    {
        // 调用接口购买弹丸
        if(referee)
        {
            referee.Buy_bullet(buyBulletNum);
        }
        else
            Debug.Log("faild to buy bullet");
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Message_Updata(int goldNum)
    {
        if (robotType == Robot_type.Hero)
        {
            maxBullet = (int)(goldNum / 10);

        }
        else if (robotType == Robot_type.Infantry)
        {
            maxBullet = goldNum;
        }
    }
    public void Init(int nowBullet, int goldNum, Robot_type robotType, int isSupply, Referee_control referee_control)
    {
        referee = referee_control;
        maxBullet = 100;
        this.robotType = robotType;
        this.nowBullet = nowBullet;
        if (robotType == Robot_type.Hero)
        {
            maxBullet = (int)(goldNum / 10);
            text.text = "42mm弹丸补给面板";
            Buy10.text = "+1";
            Buy20.text = "+2";
            Buy50.text = "+5";
            Buy100.text = "+10";
            Reduce10.text = "-1";
            Reduce20.text = "-2";
            Reduce50.text = "-5";
            Reduce100.text = "-10";
        }
        else if (robotType == Robot_type.Infantry)
        {
            maxBullet = goldNum;
            text.text = "17mm弹丸补给面板";
            Buy10.text = "+10";
            Buy20.text = "+20";
            Buy50.text = "+50";
            Buy100.text = "+100";
            Reduce10.text = "-10";
            Reduce20.text = "-20";
            Reduce50.text = "-50";
            Reduce100.text = "-100";
        }

        if (isSupply != 0)
        {
            allowNum.color = Color.white;
            allowNum.text = nowBullet.ToString() + "/" + maxBullet.ToString();
        }
        else
        {
            allowNum.color = Color.red;
            allowNum.text = "未在补给区";
        }
    }
    private void OnGUI()
    {
        buyNum.text = buyBulletNum.ToString();
    }
}