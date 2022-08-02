using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;  //count() function

public class UI_player : MonoBehaviour
{
    //UI的物件
    [SerializeField] Image[] slots; //道具欄
    public Canvas canvas;
    public GameObject arrow;
    public Text text_distance;
    public Text text_playerinfo;

    //世界的物件
    public GameObject player;
    public Camera cam;
    public GameObject LoseEffect;

    //函式內使用的物件
    private RectTransform arrowrect;
    private Vector3 goldpos;
    private MazeController mazeController = MazeController.Instance;
    private Player[] players = PhotonNetwork.PlayerList;
    private int[] score;
    // Start is called before the first frame update
    void Start()
    {
        score = new int[players.Count()];
        changescore("");
        arrowrect = arrow.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //判斷目標的距離
        JudgeDistance(10);
        ChangeArrow();
    }

    //低於limit則不顯示
    private void JudgeDistance(int limit)
    {
        int distantance = (int)Vector3.Distance(goldpos, player.transform.position);
        text_distance.text = "距離 :" + distantance;
        if (distantance < limit)
        {
            arrow.GetComponent<CanvasGroup>().alpha = 0;
        }
        else
        {
            arrow.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    //改變指向標的位置
    private void ChangeArrow()
    {
        goldpos = mazeController.GetGoldPos();
        Vector3 point=cam.WorldToScreenPoint(goldpos);
        if (point.z < 0)
        {
            if (point.x < arrowrect.sizeDelta.x / 2)
            {
                point.x = 10000;
            }
            else
            {
                point.x = 0;
            }
        }
        arrowrect.position = GetClampPos(point, CalRectByCanvas(canvas, arrowrect.sizeDelta));
    }
    // 防止箭頭跑出畫面
    private Vector2 GetClampPos(Vector2 pos, Rect area)
    {
        Vector2 safePos = Vector2.zero;
        safePos.x = Mathf.Clamp(pos.x, area.xMin, area.xMax);
        safePos.y = Mathf.Clamp(pos.y, area.yMin, area.yMax);

        return safePos;
    }
    //取得畫布(Canvas)的範圍
    private Rect CalRectByCanvas(Canvas c, Vector2 uiSize)
    {
        Rect rect = Rect.zero;
        Vector2 area = c.GetComponent<RectTransform>().sizeDelta;

        //减去uiSize的一半是为了防止UI元素一般溢出屏幕
        rect.xMax = area.x - uiSize.x / 2;
        rect.yMax = area.y - uiSize.y / 2;
        rect.xMin = uiSize.x / 2;
        rect.yMin = uiSize.y / 2;

        return rect;
    }

    //呈現道具在畫面上
    public void showitem(string item,int index)
    {
        slots[index].sprite = Resources.Load("Item/" + item, typeof(Sprite)) as Sprite;
        
    }

    //使用道具後將道具從畫面上移除
    public void deleteitem(int index)
    {
        slots[index].sprite = Resources.Load("Item/blank", typeof(Sprite)) as Sprite;
    }

    public void changescore(string palyername)
    {
        text_playerinfo.text = "";
        for (int i = 0; i < players.Count(); i++)
        {
            if (palyername == players[i].NickName)
            {
                score[i]++;
            }
            text_playerinfo.text += players[i].NickName + " : " + score[i] + "\n";
        }
    }

    public void Lose(string WinnerName)
    {
        LoseEffect.SetActive(true);
    }

    //Debug的時候用的顯示位置
    IEnumerator showpos()
    {
        while (true)
        {
            Debug.Log("targetpos:" + goldpos);
            Debug.Log("playerpos:" + gameObject.transform.position);
            yield return new WaitForSeconds(1);
        }
    }
}
