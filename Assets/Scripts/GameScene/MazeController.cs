using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.IO;

public class MazeController : MonoBehaviourPun
{
    int itemcount = 0;
    private static GameObject gold;
    static Vector3 goldpos;
    string[] itemlist = ItemController.getitemlist();
    bool[] itemmap=new bool[81] ;
    PhotonView PV;

    private void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();

        //初始化itemmap
        for(int i = 0; i < itemmap.Length; i++)
        {
            itemmap[i] = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            StartCoroutine(MakeGold()); 
            StartCoroutine(CreateItem(10, 5));
            //PhotonNetwork.Instantiate("Prefabs/item", new Vector3(5, 1, 10), Quaternion.identity);
            //PhotonNetwork.Instantiate("Prefabs/item", new Vector3(5, 1, 11), Quaternion.identity);
            //PhotonNetwork.Instantiate("Prefabs/item", new Vector3(5, 1, 12), Quaternion.identity);
            //PhotonNetwork.Instantiate("Prefabs/item", new Vector3(5, 1, 13), Quaternion.identity);
        }
        
        GameEvent.instance.onGoldCollected += score;
        GameEvent.instance.onItemCollected += getitem;
        //GameEvent.instance.onItemUsed += useitem;
    }

    //得分
    public void score(string playername,int itemid)
    {
        itemcount--;
        itemmap[itemid] = false;
        StartCoroutine(MakeGold());
    }

    //創建新的金子
    public IEnumerator MakeGold()
    {
        

        int id = Random.Range(0, 80);
        while(itemmap[id] == true)//確認該位置是否已經有物品了
        {
            id = Random.Range(0, 80);
        }
        itemcount++;
        itemmap[id] = true;
        int x = 5 + 10 * (id / 9);
        int z = 5 + 10 * (id % 9);
        Vector3 newPos = new Vector3(x, 1, z);

        yield return new WaitForSeconds(2.0f);
        gold = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "gold"), newPos, Quaternion.identity);
        gold.GetComponent<Score>().setID(id);
    }

    //回傳金子的位置
    public Vector3 GetGoldPos()
    {
        return goldpos;
    }

    //固定時間建立 count 數量的道具
    IEnumerator CreateItem(float timestep,int count)
    {
        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                int id = Random.Range(0, 80);
                if (itemmap[id]==true)//確認該位置是否已經有物品了
                {
                    if (itemcount <= 30)
                    {
                        i--;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                    
                }
                else//若沒有物品則新建
                {
                    itemcount++;
                    itemmap[id] = true;
                    int x = 5 + 10 * (id / 9);
                    int z = 5 + 10 * (id % 9);
                    Vector3 newPos = new Vector3(x, 1, z);
                    GameObject newItem = PhotonNetwork.Instantiate("Prefabs/item", newPos, Quaternion.identity);
                    Item newItemclass = newItem.GetComponent<Item>();
                    newItemclass.setID(id);
                }  
            }
            yield return new WaitForSeconds(timestep);

            /*隨機生成道具(可能重複生成在同一個地方)
            for (int i = 0; i < count; i++)
            {
                int x = 5 + 10 * Random.Range(1, 9);
                int z = 5 + 10 * Random.Range(1, 9);
                Vector3 newPos = new Vector3(x, 1, z);
                PhotonNetwork.Instantiate("Prefabs/item", newPos, Quaternion.identity);
            }*/
            
        }
    }

    //玩家取得物件
    public void getitem(string player, string item, int itemid)
    {
        itemcount--;
        itemmap[itemid] = false;
        Debug.Log(player + ": 取得道具--" + item);
    }

    //在地圖上使道具作用
    public void useitem(string playerinfo,string itemtype,Vector3 pos,Quaternion dir)
    {
        //playerinfo 包含了玩家名稱和使用的道具在道具欄的排行
        char index = playerinfo[playerinfo.Length-1];
        string playername = playerinfo.Remove(playerinfo.Length-1);
        Debug.Log(playername + "在 " + pos + " 使用了第 " +index+" 個道具: "+ itemtype);
    }

    public void SetGoldPos(Vector3 newPos)
    {
        goldpos = newPos;
    }

    #region Singleton
    private static MazeController instance = null;
    private MazeController()
    {
        instance = this;
    }
    public static MazeController Instance
    {
        get
        {
            return instance ?? new MazeController();
        }
    }
    #endregion

}
