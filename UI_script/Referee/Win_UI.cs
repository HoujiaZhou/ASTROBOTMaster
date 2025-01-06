using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Win_UI:MonoBehaviourPun
{
    private Robot_Data red1, red3, blue1, blue3;
    [SerializeField] TextMeshProUGUI redWinpoint,blueWinpoint,red1Winpoint,blue1Winpoint,red3Winpoint,blue3Winpoint,red1Killnum,blue1Killnum,red3Killnum,blue3Killnum;
    private int redWinPoint,blueWinPoint;
    private UI_parent parent;
    public void OnExitRoom()
    {
        parent.Set_Game_State(false);
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.farClipPlane = 2000f;
        // 设置标签为 Main Camera
        cameraObject.tag = "MainCamera";
        PhotonNetwork.LeaveRoom();
    }

    public void Set_Robot_Data(Robot_Data red1, Robot_Data red3, Robot_Data blue1, Robot_Data blue3)
    {
        this.red1 = red1;
        this.red3 = red3;
        this.blue1 = blue1;
        this.blue3 = blue3;
        if(!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public void Set_Point(int redPoint, int bluePoint)
    {
        this.redWinPoint = redPoint;
        this.blueWinPoint = bluePoint;
        if(!gameObject.activeSelf) gameObject.SetActive(true);
    }
    void OnGUI()
    {
        if(!parent)parent = parent = gameObject.GetComponentInParent<UI_parent>();
        redWinpoint.text = redWinPoint.ToString();
        blueWinpoint.text = blueWinPoint.ToString();
        red1Winpoint.text = red1.WinPoint.ToString();
        blue1Winpoint.text = blue1.WinPoint.ToString();
        red3Winpoint.text = red3.WinPoint.ToString();
        blue3Winpoint.text = blue3.WinPoint.ToString();
    }
}