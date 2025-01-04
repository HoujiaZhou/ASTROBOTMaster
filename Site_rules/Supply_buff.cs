using UnityEngine;

public class Supply_buff : MonoBehaviour
{
    [SerializeField]
    Robot_color robot_Color;
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("referee"))
        {
            Referee_control referee_ = collider.gameObject.GetComponent<Referee_control>();
            if(referee_.Get_Robot_color()==robot_Color)
            {
                referee_.Add_buff(Robot_buff.supply,1);
            }
        }
    }
    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.CompareTag("referee"))
        {
            Referee_control referee_ = collider.gameObject.GetComponent<Referee_control>();
            if(referee_.Get_Robot_color()==robot_Color)
            {
                referee_.Add_buff(Robot_buff.supply,1);
            }
        }
    }
     
    void  OnTriggerExit(Collider collider)
    {
        
        if(collider.gameObject.CompareTag("referee"))
        {
            Referee_control referee_ = collider.gameObject.GetComponent<Referee_control>();
            if(referee_.Get_Robot_color()==robot_Color)
            {
                referee_.Add_buff(Robot_buff.supply,0);
            }
        }
    }
}
