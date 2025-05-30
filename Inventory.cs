using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    private int CoinsCount = 0;
    public TMP_Text CoinsString;

    public void AddCoin() {
        CoinsCount++;
        CoinsString.text = "Coins: " + CoinsCount.ToString();
    }
}
