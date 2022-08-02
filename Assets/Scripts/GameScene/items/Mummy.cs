using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mummy : MonoBehaviour
{
    private float speed = 15;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //撞到牆兩秒後消滅,撞到玩家使玩家靜止1.5s
        if (collision.gameObject.tag== "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            StartCoroutine(player.freezed(1.5f));
        }
        else if (collision.gameObject.tag == "Wall") 
        {
            StartCoroutine(waitfordie(2));
        }
    }

    IEnumerator waitfordie(int countdown)
    {
        yield return new WaitForSeconds(countdown);
        Destroy(gameObject);
    }
}
