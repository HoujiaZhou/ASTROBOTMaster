using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class Outpost_control : MonoBehaviour
{
    [SerializeField] private Referee_control referee;
    [SerializeField] private Transform Rotation_center1, Rotation_center2;
    [SerializeField] private Transform RotateBody;
    public bool rotateState;
    private int gameTime, hp = 1500;
    public int causedDamage = 0;

    void Update()
    {
        gameTime = referee.rule.Get_time();
        int nowHp = referee.Get_robotHP();
        if (!PhotonNetwork.IsConnected) return;
        if (PhotonNetwork.IsMasterClient)
        {
            if (gameTime >= 4 * 60f)
            {
                causedDamage += nowHp - hp;
                hp = nowHp;
                if (causedDamage >= 500)
                {
                    referee.rule.photonView.RPC("Give_Xp", RpcTarget.All,
                        referee.Get_damage_Log().LastDamagenickname, 100);
                    causedDamage -= 500;
                }
            }
            else
                hp = referee.Get_robotHP();
        }

        if (hp > 0 && gameTime >= 4 * 60f)
            rotateState = true;
        else rotateState = false;
        if (rotateState)
        {
            RotateBody.RotateAround(Rotation_center1.position,
                Rotation_center2.position - Rotation_center1.position, (float)(0.8 * 180f) * Time.deltaTime);
        }
    }
}