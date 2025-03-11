using System;
using Photon.Pun;
using UnityEngine;

public class CenterBuff_eachborad : MonoBehaviourPun
{
    private CenterBuff_control control;
    public Robot_color color;
    private bool isHit;
    [SerializeField] private GameObject lightCircle;

    [PunRPC]
    public void HitCenterBuff(int point)
    {
        if (isHit == false)
        {
            isHit = true;
            control.HitCenter(point,color);
        }
    }

    private void Start()
    {
        
        while (!control)
        {
            control = GetComponentInParent<CenterBuff_control>();
        }
    }

    private void OnEnable()
    {
        isHit = false;
    }

    private void Update()
    {
        if (!isHit)
        {
            lightCircle.SetActive(true);
        }
        else
        {
            lightCircle.SetActive(false);
        }
    }
}