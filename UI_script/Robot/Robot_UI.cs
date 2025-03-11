
using System;
using UnityEngine;

public class Robot_UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private UnityEngine.UI.Image Super_cap;
    private Robot_control robot_data;
    private GameObject robot;
    private UI_parent parent;

    private void Start()
    {
        parent = gameObject.GetComponent<UI_parent>();
        robot = parent.Get_Robot();
    }

    // Update is called once per frame
    void Update()
    {
        if (robot_data) return;
        if (robot)
        {
            robot_data = robot.GetComponentInChildren<Robot_control>();
        }
        else
        {
            parent = gameObject.GetComponent<UI_parent>();
        }
    }
    void OnGUI()
    {
        if (!robot_data) return;
        Super_cap.fillAmount = (robot_data.chassis.superCapCapacity - robot_data.chassis.superCapCapacityUsed) /
                                robot_data.chassis.superCapCapacity;
        if (robot_data.chassis.Is_using_Cap == true)
        {
            Super_cap.color = Color.red;
        }
        else
        {
            Super_cap.color = Color.green;
        }
    }
}
