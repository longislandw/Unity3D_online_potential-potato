using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviour
{
    MazeController mazeController = MazeController.Instance;
    PhotonView PV;
    int id;


    private void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();
    }

    public void setID(int inp)
    {
        id = inp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && PV.IsMine)     
        {
            string itemtype = ItemController.getranditem();
            GameEvent.instance.ItemCollected(other.name, itemtype, id);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
