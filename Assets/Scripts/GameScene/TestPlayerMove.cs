using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder, ItemSend;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    string[] itemhold = new string[] { "", "", "", "" };

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        GameEvent.instance.onItemCollected += getitem;
    }

    void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Space))
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
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void getitem(string name, string itemtype,int itemid)
    {

        if (name == this.name)
        {
            int change = 0;
            for (int i = 0; i < 4; i++)
            {
                if (itemhold[i] == "")
                {
                    itemhold[i] = itemtype;
                    change = 1;
                    break;
                }
            }
            if (change == 0) Debug.Log("道具已滿");
        }

    }

    //透過按下 1,2,3,4 使用道具
    //觸發GameEvent的ItemUsed事件
    //傳入 玩家資訊(名稱加道具位置), 道具類型, 道具產生的位置, 道具的方向
    public void UseItem()
    {
        if (itemhold[0] != "" && Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameEvent.instance.ItemUsed(gameObject.name + "0", itemhold[0], ItemSend.transform.position, this.transform.rotation);
            itemhold[0] = "";
        }
        if (itemhold[1] != "" && Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameEvent.instance.ItemUsed(gameObject.name + "1", itemhold[1], ItemSend.transform.position, this.transform.rotation);
            itemhold[1] = "";
        }
        if (itemhold[2] != "" && Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameEvent.instance.ItemUsed(gameObject.name + "2", itemhold[2], ItemSend.transform.position, this.transform.rotation);
            itemhold[2] = "";
        }
        if (itemhold[3] != "" && Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameEvent.instance.ItemUsed(gameObject.name + "3", itemhold[3], ItemSend.transform.position, this.transform.rotation);
            itemhold[3] = "";
        }
    }
}
