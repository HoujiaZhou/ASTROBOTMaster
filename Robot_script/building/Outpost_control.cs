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
    private int gameTime, hp;

    void Update()
    {
        if (!PhotonNetwork.IsConnected) return;
        if (PhotonNetwork.IsMasterClient)
        {
            gameTime = referee.rule.Get_time();
            if (gameTime >= 4 * 60f)
            {
                if (hp > 1000)
                {
                    hp = referee.Get_robotHP();
                    if (hp <= 1000)
                    {
                        referee.rule.photonView.RPC("Give_Xp", RpcTarget.AllBuffered,
                            referee.Get_damage_Log().LastDamagenickname);
                    }
                }
                else if (hp > 500)
                {
                    hp = referee.Get_robotHP();
                    if (hp <= 500)
                    {
                        referee.rule.photonView.RPC("Give_Xp", RpcTarget.AllBuffered,
                            referee.Get_damage_Log().LastDamagenickname);
                    }
                }
                else if (hp > 0)
                {
                    hp = referee.Get_robotHP();
                    if (hp <= 0)
                    {
                        referee.rule.photonView.RPC("Give_Xp", RpcTarget.AllBuffered,
                            referee.Get_damage_Log().LastDamagenickname);
                    }
                }
            }else referee.Get_robotHP();

            if (hp >= 0 && gameTime >= 4 * 60f)
                rotateState = true;
            else rotateState = false;
            if (rotateState)
            {
                RotateBody.RotateAround(Rotation_center1.position,
                    Rotation_center2.position - Rotation_center1.position, (float)(0.8 * 180f) * Time.deltaTime);
            }
            else ;
        }
    }
}