using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Score : MonoBehaviour
{
    private MazeController mazeController = MazeController.Instance;
    PhotonView PV;
    int id;

    // Start is called before the first frame update
    private void Awake()
    {
        PV = gameObject.GetComponent<PhotonView>();
    }

    void Start()
    {
        mazeController.SetGoldPos(gameObject.transform.position);
        Debug.Log("New Gold Spawn at"+ gameObject.transform.position);
    }

    public void setID(int inp)
    {
        id = inp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && PV.IsMine)
        {
            GameEvent.instance.GoldCollected(other.name,id);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
