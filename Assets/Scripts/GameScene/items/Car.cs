using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 10000);
        StartCoroutine(waitfordie(5));
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(collision.gameObject.transform.forward * 1000);
        }
    }*/

    IEnumerator waitfordie(int countdown)
    {
        yield return new WaitForSeconds(countdown);
        Destroy(gameObject);
    }
}
