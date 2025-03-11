using System;
using Photon.Pun;
using UnityEngine;

public class CenterBuff_collider : MonoBehaviour
{
    public int points;
    private CenterBuff_eachborad borad;
    void Start()
    {
        while (!borad)
        {
            borad = GetComponentInParent<CenterBuff_eachborad>();
        }
    }
    
    public void HitCenter()
    {
        borad.photonView.RPC("HitCenterBuff",RpcTarget.All,points);
    }
} 