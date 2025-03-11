using System;
using UnityEngine;

public class watcher_control : MonoBehaviour
{
    private float Vx_Speed, Vy_Speed,Yaw,Pitch;
    public float Speed,Sensitivity;

    private void Start()
    {
         
    }

    void Update()
    {
        
        float angleX, angleY;
        angleX = transform.localEulerAngles.x;
        angleY = transform.localEulerAngles.y;
        Vector3 verb,rotate = Vector3.zero;
        Vx_Speed = Input.GetAxis("Vertical")* Speed;
        Vy_Speed = Input.GetAxis("Horizontal")* Speed;
        Yaw = Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        Pitch = -Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;
        verb = Vx_Speed * new Vector3(Mathf.Sin(angleY / 180 * Mathf.PI),-Mathf.Sin(angleX / 180 * Mathf.PI),Mathf.Cos(angleY/ 180 * Mathf.PI) ) + Vy_Speed * new Vector3(Mathf.Cos(angleY / 180 * Mathf.PI),0,Mathf.Sin(angleY/ 180 * Mathf.PI) );
        transform.Translate(verb,Space.World);
        Debug.Log(new Vector3(Mathf.Sin(angleY / 180 * Mathf.PI),0,Mathf.Cos(angleY/ 180 * Mathf.PI) ));
        float nextangle = transform.localEulerAngles.x;
        
        while (nextangle > 180 || nextangle < -180)
        {
            if(nextangle > 180)
                nextangle = nextangle - 360;
            if(nextangle < -180)
                nextangle = nextangle + 360;
        }
        if ((nextangle <= 60 && nextangle >= -60)||(nextangle > 60 && Pitch < 0) || (nextangle < -60 && Pitch > 0))
        {
            rotate += new Vector3(Pitch,0,0);
        }
        rotate += new Vector3(0,Yaw,0);
        transform.eulerAngles += rotate;
    }

    
}