using Photon.Pun;
using UnityEngine;

public class camera_follow : MonoBehaviourPun
{
    private Camera Camera_;
    public Transform gimbal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!photonView.IsMine)return ;
        Camera_=Camera.main;
        Camera_.transform.position = transform.position;
        Camera_.transform.rotation = transform.rotation;
        Camera_.transform.SetParent(gimbal,true); 
    }

    // Update is called once per frame
    void Update()
    {

    }
}

