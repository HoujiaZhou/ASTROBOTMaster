using UnityEngine;
public class Wheel_groud_check : MonoBehaviour
{
    public bool Wheel_Is_groud;
    private WheelCollider wheelCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        WheelHit hit;
        Wheel_Is_groud = false;
        if (wheelCollider.GetGroundHit(out hit))
        {
            // 检查接触物体的信息
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("groud"))
                {
                    Wheel_Is_groud = true;
                }
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("groud"))
        {
            Wheel_Is_groud = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("groud"))
        {
            Wheel_Is_groud = false;
        }
    }
}
