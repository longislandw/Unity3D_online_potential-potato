using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemController : MonoBehaviour
{
    public GameObject car;
    public GameObject mummy;
    static string[] itemlist = new string[] { "車子","木乃伊"};
    
    // Start is called before the first frame update
    void Start()
    {
        GameEvent.instance.onItemUsed += useitem;
    }

    //回傳道具的列表,其他腳本想找道具全都要來這裡,以確保道具表的獨一性
    public static string[] getitemlist()
    {
        return itemlist;
    }

    //回傳隨機的道具,給item的腳本使用
    public static string getranditem()
    {
        int itemtype = Random.Range(0, itemlist.Length);
        return itemlist[itemtype];
    }

    //在這裡根據傳入的位置和方向使道具產生作用
    public void useitem(string playerinfo,string itemtype, Vector3 pos,Quaternion dir)
    {
        //在 pos 使用了 itemtype;
        if (itemtype == itemlist[0])
        {
            PhotonNetwork.Instantiate("Prefabs/Car", pos, dir);
        }
        if (itemtype == itemlist[1])
        {
            PhotonNetwork.Instantiate("Prefabs/Mummy", pos, dir);
        }
    }
}
