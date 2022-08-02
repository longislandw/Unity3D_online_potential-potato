using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    public void Open()
    {
        open = true;
        gameObject.SetActive(true); //open menu
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false); //close menu
    }
}
