using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations;
public class Bullet_control : MonoBehaviourPun
{

    public Robot_type robot_type;
    private float time;
    [SerializeField] private string shooter_nickname,local_nickname;
    private bool attacked_enable = true;
    private Referee_control referee;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Add_referee(Referee_control referee_)
    {
        referee = referee_;
    }
    public void Add_nickname(string name)
    {
        shooter_nickname = name;
    }
    void Start()
    {
        object obj;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("referee", out obj);
        local_nickname = (string)obj;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 5)
        {
            Destroy(gameObject);
        }
    }
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(0, -3.09f  * 100, 0, ForceMode.Acceleration);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Armour") && attacked_enable&&local_nickname == shooter_nickname)
        {
            int damageRate = referee.Get_DamageRate();
            
            armour_control armour = collision.gameObject.GetComponent<armour_control>();
            if (armour == null)
            {
                Debug.LogWarning("装甲板没有控制组件");
            }
            else
            {

                armour.photonView.RPC("armour_attacked_PUN", RpcTarget.All, robot_type,shooter_nickname,damageRate);
            }
            if (referee != null)
            {
                
            }
            else
            {
                Debug.LogWarning("弹丸没有检测到发出对象");
            }
            attacked_enable = false;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Armour") && attacked_enable&&local_nickname == shooter_nickname)
        {
            int damageRate = referee.Get_DamageRate();
            armour_control armour = collision.gameObject.GetComponent<armour_control>();
            if (armour == null)
            {
                Debug.LogWarning("装甲板没有控制组件");
            }
            else
            {
                armour.photonView.RPC("armour_attacked_PUN", RpcTarget.All, robot_type,shooter_nickname,damageRate);
            }
            if (referee != null)
            {
                
            }
            else
            {
                Debug.LogWarning("弹丸没有检测到发出对象");
            }
            attacked_enable = false;
        }

        if (collision.gameObject.CompareTag("CenterBuff") && attacked_enable && local_nickname == shooter_nickname&&robot_type  == Robot_type.Infantry)
        {
            CenterBuff_collider centerBuff = collision.gameObject.GetComponent<CenterBuff_collider>();
            if (centerBuff == null)
            {
                Debug.LogWarning("能量机关没有控制组件");
            }
            else
            {
                centerBuff.HitCenter();
            }
            attacked_enable = false;
        }
    }
}
