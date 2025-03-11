using UnityEngine;
using UnityEngine.UI;

public class Image_UI:MonoBehaviour
{
    [SerializeField] Image supplyImage;
    private Robot_buff buff;

    public void Set_Buff_Type(Robot_buff buff)
    {
        this.buff = buff;
    }

    void OnGUI()
    {
        if ((buff & Robot_buff.supply) != 0)
        {
            supplyImage.enabled = true;
        }
        else
        {
            supplyImage.enabled = false;
        }
    }
}