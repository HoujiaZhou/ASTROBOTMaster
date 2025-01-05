using System;
using Photon.Pun;
using UnityEngine;

public class Center_buff : MonoBehaviour
{
    [SerializeField]private Rule_RMUL2025 rule;
    private float timer;
    private Robot_color occupyColor;
    private Referee_control referee;

    private void Start()
    {
        occupyColor = Robot_color.Null;
    }

    private void Update()
    {
        if (!rule)
        {
            rule = GameObject.FindGameObjectWithTag("Rule").GetComponent<Rule_RMUL2025>();
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                if (occupyColor == Robot_color.RED || occupyColor == Robot_color.BLUE)
                {
                    rule.photonView.RPC("Add_WinPoint", RpcTarget.All, referee.Get_nickname(), 1);
                }

                timer -= 1.0f;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("referee"))
        {
            if (occupyColor == Robot_color.Null)
            {
                Referee_control referee_ = collision.gameObject.GetComponent<Referee_control>();
                occupyColor = referee_.Get_Robot_color();
                referee = referee_;
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("referee"))
        {
            Referee_control referee_ = collider.gameObject.GetComponent<Referee_control>();
            if(referee_.Get_Robot_color() == occupyColor)
                occupyColor = Robot_color.Null;
        }
    }
}