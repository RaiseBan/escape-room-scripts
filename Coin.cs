using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider col) {
        if (col.tag == "Player") {
            col.gameObject.GetComponent<Inventory>().AddCoin();
            Destroy(gameObject);
        }
    }
}
