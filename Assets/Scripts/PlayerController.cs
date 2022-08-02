using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder, ItemSend, WinEffect;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    string[] itemhold = new string[] { "", "", "", "" };

    Rigidbody rb;

    PhotonView PV;
    int score = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        gameObject.name = PV.Owner.NickName;
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(GetComponentInChildren<Canvas>().gameObject);
            Destroy(GetComponentInChildren<UI_player>());
            //Destroy(rb);
        }
        GameEvent.instance.onGoldCollected += AddScore;
        GameEvent.instance.onItemCollected += getitem;
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }
        Look();
        Move();
        Jump();
        UseItem();
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        //rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        gameObject.transform.Translate(moveAmount*Time.fixedDeltaTime);
    }

    //透過按下 1,2,3,4 使用道具
    //觸發GameEvent的ItemUsed事件
    //傳入 玩家資訊(名稱加道具位置), 道具類型, 道具產生的位置, 道具的方向
    public void UseItem()
    {
        if (itemhold[0]!=""&&Input.GetKeyDown(KeyCode.Alpha1) && grounded)
        {
            GameEvent.instance.ItemUsed(gameObject.name + "0", itemhold[0], ItemSend.transform.position, this.transform.rotation);
            gameObject.GetComponent<UI_player>().deleteitem(0);
            itemhold[0] = "";
        }
        if (itemhold[1] != "" && Input.GetKeyDown(KeyCode.Alpha2) && grounded)
        {
            GameEvent.instance.ItemUsed(gameObject.name + "1", itemhold[1], ItemSend.transform.position, this.transform.rotation);
            gameObject.GetComponent<UI_player>().deleteitem(1);
            itemhold[1] = "";
        }
        if (itemhold[2] != "" && Input.GetKeyDown(KeyCode.Alpha3) && grounded)
        {
            GameEvent.instance.ItemUsed(gameObject.name + "2", itemhold[2], ItemSend.transform.position, this.transform.rotation);
            gameObject.GetComponent<UI_player>().deleteitem(2);
            itemhold[2] = "";
        }
        if (itemhold[3] != "" && Input.GetKeyDown(KeyCode.Alpha4) && grounded)
        {
            GameEvent.instance.ItemUsed(gameObject.name + "3", itemhold[3], ItemSend.transform.position, this.transform.rotation);
            gameObject.GetComponent<UI_player>().deleteitem(3);
            itemhold[3] = "";
        }
    }

    public void AddScore(string playername,int itemid)
    {
        PV.RPC("RPC_AddScore", RpcTarget.All,playername);
    }

    public void getitem(string name, string itemtype,int itemid)
    {
        PV.RPC("RPC_GetItem", RpcTarget.All, name, itemtype);
    }

    private void win()
    {
        WinEffect.SetActive(true);
        PV.RPC("RPC_EndGame", RpcTarget.Others,gameObject.name);
    }

    public void getfreezed()
    {
        StartCoroutine(freezed(1.5f));
    }

    public IEnumerator freezed(float countdown)
    {
        float a = sprintSpeed;
        float b = walkSpeed;
        sprintSpeed = 0;
        walkSpeed = 0;
        yield return new WaitForSeconds(countdown);
        sprintSpeed = a;
        walkSpeed = b;
    }

    [PunRPC]
    void RPC_AddScore(string playername)
    {
        if (!PV.IsMine) return;
        gameObject.GetComponent<UI_player>()?.changescore(playername);
        if (gameObject.name == playername) score++;
        if (score >= 5)
        {
            win();
        }
    }

    [PunRPC]
    void RPC_GetItem(string name,string itemtype)
    {
        if (!PV.IsMine) return;
        if (name == this.name)//用名字來判斷是不是自己拿到道具
        {
            int change = 0;
            for (int i = 0; i < 4; i++)
            {
                if (itemhold[i] == "")
                {
                    itemhold[i] = itemtype;
                    gameObject.GetComponent<UI_player>()?.showitem(itemtype,i);
                    change = 1;
                    break;
                }
            }
            if (change == 0) Debug.Log("道具已滿");
        }
    }

    [PunRPC]
    void RPC_EndGame(string name)
    {
        Debug.Log("進入EndGame狀態" + gameObject.name);
        if (!PV.IsMine) return;
        gameObject.GetComponent<UI_player>()?.Lose(name);
    }
}
