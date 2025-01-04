using UnityEngine;
using Photon.Pun;
public class Shoot_control : MonoBehaviourPun
{

    public Robot_control Shoot;
    [SerializeField] private GameObject Big_bullet, small_bullet;
    public int num_of_bullet = 0;
    private float time;
    public Transform bullet_place;
    private int bullet_speed;
    public Shoot_referee referee;
    private GameObject Bullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (referee.Get_shoot_type() == Robot_type.Hero)
            Bullet = Big_bullet;
        else Bullet = small_bullet;
        bullet_speed = 2500;
    }
    [PunRPC]
    void Spawn_bullet()
    {
        GameObject bullet_new = Instantiate(Bullet, bullet_place.position, bullet_place.rotation);
        Rigidbody rb = bullet_new.GetComponent<Rigidbody>();
        rb.linearVelocity = bullet_place.forward * bullet_speed;
        bullet_new.GetComponent<Bullet_control>().Add_referee(referee.Get_referee());
        bullet_new.GetComponent<Bullet_control>().Add_nickname(referee.Get_nickname());
    }
    // Update is called once per frame
    void Update()
    {

        if (time > 0.08)
        {
            if (Shoot.shoot.isfire == true && referee.Shoot_permission())
            {
                referee.Shoot_one_bullet(referee.Get_shoot_type());
                num_of_bullet++;
                photonView.RPC("Spawn_bullet", RpcTarget.All);
                time = 0;
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }
}
