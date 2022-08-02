using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public static GameEvent instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<string,int> onGoldCollected;
    public event Action<string,string,int> onItemCollected;
    public event Action<string,string, Vector3,Quaternion> onItemUsed;

    public void GoldCollected(string playername,int id)
    {
        if (onGoldCollected != null)
        {
            onGoldCollected(playername,id);
        }
    }

    public void ItemCollected(string playername,string itemtype,int id)
    {
        if (onItemCollected != null)
        {
            onItemCollected(playername, itemtype, id);
        }
    }

    public void ItemUsed(string playerinfo,string itemtype, Vector3 pos, Quaternion dir)
    {
        if (onItemUsed != null)
        {
            onItemUsed(playerinfo,itemtype,pos,dir);
        }
    }
}
