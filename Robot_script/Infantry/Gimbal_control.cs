using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;
public class Gimbal_control : MonoBehaviourPun
{
    [SerializeField] private Transform Gimbal_body;
    [SerializeField] private Transform Pitch_body;
    [SerializeField] private Transform Chassis_body;
    [SerializeField] private Transform Gimbal_body_rotation_center1, Gimbal_body_rotation_center2;
    [SerializeField] private Transform Pitch_body_rotation_center1, Pitch_body_rotation_center2;
    [SerializeField] private Robot_control gimbal;
    private float Yaw_angle;
    private Vector3 yaw_position, yaw_rotate, pitch_position, pitch_rotate;
    // [PunRPC]
    // void Sync_angle(Vector3 yaw_position_, Vector3 yaw_rotate_, Vector3 pitch_position_, Vector3 pitch_rotate_)
    // {
    //     yaw_position = yaw_position_;
    //     yaw_rotate = yaw_rotate_;
    //     pitch_position = pitch_position_;
    //     pitch_rotate = pitch_rotate_;
    // }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (photonView.IsMine)
        {
            if (gimbal != null)
            {
                Gimbal_body.RotateAround(Gimbal_body_rotation_center1.position, Gimbal_body_rotation_center2.position - Gimbal_body_rotation_center1.position, gimbal.gimbal.Yaw - (Chassis_body.eulerAngles.y - Yaw_angle));
                Yaw_angle = Chassis_body.eulerAngles.y;
                float pitch_angle = Pitch_body.eulerAngles.x;
                while (pitch_angle >= 180 || pitch_angle <= -180)
                {
                    if (pitch_angle >= 180)
                    {
                        pitch_angle -= 360;
                    }
                    if (pitch_angle <= -180)
                    {
                        pitch_angle += 360;
                    }
                }
                if ((pitch_angle <= 42 && -gimbal.gimbal.Pitch < 0) || (pitch_angle >= -30 && -gimbal.gimbal.Pitch > 0))
                {
                    Pitch_body.RotateAround(Pitch_body_rotation_center1.position, Pitch_body_rotation_center1.position - Pitch_body_rotation_center2.position, -gimbal.gimbal.Pitch);
                }
                yaw_position = Gimbal_body.localPosition;
                yaw_rotate = Gimbal_body.localEulerAngles;
                pitch_position = Pitch_body.localPosition;
                pitch_rotate = Pitch_body.localEulerAngles;
                // photonView.RPC("Sync_angle", RpcTarget.Others, yaw_position, yaw_rotate, pitch_position, pitch_rotate);
            }
        }
        // else
        // {
        //     Gimbal_body.localPosition = yaw_position;
        //     Gimbal_body.localEulerAngles = yaw_rotate;
        //     Pitch_body.localPosition = pitch_position;
        //     Pitch_body.localEulerAngles = pitch_rotate;
        // }

    }
}
