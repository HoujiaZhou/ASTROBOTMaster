using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public class CenterBuff_control : MonoBehaviourPun
{
    Rule _rule;
    [SerializeField] private Transform rotateCenter1, rotateCenter2;
    private double a, w, b;
    public bool rotateMode = false, rotate = false,redBuff,blueBuff;
    private float rotateTime;
    public int redHitNum = 0, redHitPoint = 0, blueHitNum = 0, blueHitPoint = 0;
    public int redNumber = 0, blueNumber = 0;
    private Random random = new Random();
    private float time;
    
    private bool[] red = new bool[5],
        blue = new bool[5];

    [SerializeField] private GameObject
        redHitLight1,
        redHitLight2,
        redHitLight3,
        redHitLight4,
        redHitLight5,
        blueHitLight1,
        blueHitLight2,
        blueHitLight3,
        blueHitLight4,
        blueHitLight5;

    private GameObject[] redHitLight = new GameObject[5], blueHitLight = new GameObject[5];

    [PunRPC]
    void Sync_CenterBuff_Data(bool mode, bool state, int redHitNum, int blueHitNum, int redHitPoint, int blueHitPoint,
        int redNumber, int blueNumber)
    {
        this.redHitNum = redHitNum;
        this.redHitPoint = redHitPoint;
        this.blueHitNum = blueHitNum;
        this.blueHitPoint = blueHitPoint;
        this.redNumber = redNumber;
        rotate = state;
        rotateMode = mode;
    }

    public void StartRotate(bool mode) //true为大符
    {
        rotateMode = mode;
        rotate = true;
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("Sync_CenterBuff_Data", RpcTarget.Others, rotateMode, rotate, redHitNum, blueHitNum,
                redHitPoint, blueHitPoint, redNumber, blueNumber);
    }

    public void HitCenter(int point, Robot_color robotColor)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (robotColor == Robot_color.RED)
        {
            red[redNumber] = false;
            redNumber = random.Next(0, 5);
            redHitPoint += point;
            redHitNum++;
            if (redHitNum == 5) return;
            while (red[redNumber] == false)
            {
                redNumber = random.Next(0, 5);
            }
        }
        else
        {
            blue[blueNumber] = false;
            blueNumber = random.Next(0, 5);
            blueHitPoint += point;
            blueHitNum++;
            if (blueHitNum == 5) return;
            while (blue[blueNumber] == false)
            {
                blueNumber = random.Next(0, 5);
            }
        }

        photonView.RPC("Sync_CenterBuff_Data", RpcTarget.Others, rotateMode, rotate, redHitNum, blueHitNum,
            redHitPoint, blueHitPoint, redNumber, blueNumber);
    }

    [PunRPC]
    void Set_Light(int redNumber, int blueNumber)
    {
        redHitLight[redNumber].SetActive(true);
        blueHitLight[blueNumber].SetActive(true);
    }
    void Close_Light()
    {
        for (int i = 0; i < 5; i++)
        {
            redHitLight[i].SetActive(false);
            blueHitLight[i].SetActive(false);
        }
    }

    void Start()
    {
        redHitLight[0] = redHitLight1;
        redHitLight[1] = redHitLight2;
        redHitLight[2] = redHitLight3;
        redHitLight[3] = redHitLight4;
        redHitLight[4] = redHitLight5;
        blueHitLight[0] = blueHitLight1;
        blueHitLight[1] = blueHitLight2;
        blueHitLight[2] = blueHitLight3;
        blueHitLight[3] = blueHitLight4;
        blueHitLight[4] = blueHitLight5;
        redNumber = random.Next(0, 4);
        blueNumber = random.Next(0, 4);
        a = 0.780 + random.NextDouble() * (1.045 - 0.780);
        w = 1.884 + random.NextDouble() * (2.000 - 1.884);
        b = 2.092 - a;
        StartCoroutine(ExecutePeriodically());
        while (!_rule)
        {
            _rule = GameObject.FindGameObjectWithTag("Rule").GetComponent<Rule>();
        }
    }

    void SmallBuffRotate()
    {
        if (redHitNum == 5)
        {
            //给红队增加防御增益_
            _rule.Send_Memsage("红队已激活小能量机关\n 获得25%防御增益",3f);
            _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,25,45f);
            rotate = false;
        }
        else if (blueHitNum == 5)
        {
            //给蓝队增加增益
            _rule.Send_Memsage("蓝队已激活小能量机关\n 获得25%防御增益",3f);
            _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,25,45f);
            rotate = false;
        }
        rotateTime = 0;
        transform.RotateAround(rotateCenter1.position, rotateCenter1.position - rotateCenter2.position,
            1f / 3f * 180f * Time.deltaTime);
        
    }

    void BigBuffRotate()
    {
        if (redHitNum == 5 && redBuff)
        {
            //给红队增加防御增益_
            redBuff = false;
            if (redHitPoint >= 5 && redHitPoint <= 15)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 25%防御增益 150%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,150,45f);
            }else if (redHitPoint >= 15 && redHitPoint <= 25)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 25%防御增益 155%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,155,45f);
            }else if (redHitPoint >= 25 && redHitPoint <= 35)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 25%防御增益 160%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,160,45f);
            }else if (redHitPoint >= 35 && redHitPoint <= 40)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 25%防御增益 200%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,200,45f);
            }else if (redHitPoint >= 40 && redHitPoint <= 45)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 25%防御增益 300%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,300,45f);
            }else if (redHitPoint == 46)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 30%防御增益 340%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,30,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,340,45f);
            }else if (redHitPoint == 47)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 35%防御增益 380%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,35,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,380,45f);
            }else if (redHitPoint == 48)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 40%防御增益 420%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,40,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,420,45f);
            }else if (redHitPoint == 49)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 45%防御增益 460%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,45,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,460,45f);
            }else if (redHitPoint == 50)
            {
                _rule.Send_Memsage("红队已激活大能量机关\n 50%防御增益 500%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.RED,50,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.RED,500,45f);
            }
            _rule.photonView.RPC("Give_Xp", RpcTarget.All,"",500);
        }
        else if (blueHitNum == 5 && blueBuff)
        {
            //给蓝队增加增益
            blueBuff = false;
            if (blueHitPoint >= 5 && blueHitPoint <= 15)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 25%防御增益 150%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,150,45f);
            }else if (blueHitPoint >= 15 && blueHitPoint <= 25)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 25%防御增益 155%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,155,45f);
            }else if (blueHitPoint >= 25 && blueHitPoint <= 35)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 25%防御增益 160%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,160,45f);
            }else if (blueHitPoint >= 35 && blueHitPoint <= 40)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 25%防御增益 200%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,200,45f);
            }else if (blueHitPoint >= 40 && blueHitPoint <= 45)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 25%防御增益 300%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,25,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,300,45f);
            }else if (blueHitPoint == 46)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 30%防御增益 340%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,30,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,340,45f);
            }else if (blueHitPoint == 47)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 35%防御增益 380%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,35,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,380,45f);
            }else if (blueHitPoint == 48)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 40%防御增益 420%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,40,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,420,45f);
            }else if (blueHitPoint == 49)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 45%防御增益 460%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,45,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,460,45f);
            }else if (blueHitPoint == 50)
            {
                _rule.Send_Memsage("蓝队已激活大能量机关\n 50%防御增益 500%伤害增益",3f);
                _rule.photonView.RPC("Give_Defense", RpcTarget.All,Robot_color.BLUE,50,45f);
                _rule.photonView.RPC("Give_DamageRate", RpcTarget.All,Robot_color.BLUE,500,45f);
            }
            _rule.photonView.RPC("Give_Xp", RpcTarget.All,"",500);
        }
        double speed = a * (float)(Math.Sin(w * rotateTime)) + b;
        transform.RotateAround(rotateCenter1.position, rotateCenter1.position - rotateCenter2.position,
            (float)(speed) * 180f / (float)Math.PI * Time.deltaTime);
        rotateTime += Time.deltaTime;
    }

    private void Update()
    {
        if (rotate)
        {
            if (rotateMode == false)
            {
                
                
            }
            else
            {
                
            }
            
        }
        else
        {
            Close_Light();
        }

        if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) return;
        if (rotate)
        {
            photonView.RPC("Set_Light", RpcTarget.All,redNumber,blueNumber);
            time -= Time.deltaTime;
            if (time < 0) rotate = false;
            if (rotateMode == false)
            {
                rotateTime = 0;
                SmallBuffRotate();
            }
            else
            {
                BigBuffRotate();
            }
        }
        else
        {
            redHitNum = 0;
            blueHitNum = 0;
            redHitPoint = 0;
            blueHitPoint = 0;
            rotateTime = 0;
            time = 30f;
            red = Enumerable.Repeat(true, 5).ToArray();
            redBuff = true;
            blueBuff = true;
            blue = Enumerable.Repeat(true, 5).ToArray();
        }
    }

    IEnumerator ExecutePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Sync_CenterBuff_Data", RpcTarget.Others, rotateMode, rotate, redHitNum, blueHitNum,
                    redHitPoint, blueHitPoint, redNumber, blueNumber);
            }
        }
    }
}