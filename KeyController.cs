using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeyController : MonoBehaviour
{
    public event EventHandler OnKeysChanged;

    private List<Key.KeyType> keyList;

    private void Awake() {
        keyList = new List<Key.KeyType>();
    }

    public List<Key.KeyType> GetKeyList() {
        return keyList;
    }

    public void AddKey(Key.KeyType keyType) {
        keyList.Add(keyType);
        OnKeysChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool ContainsKey(Key.KeyType keyType) {
        return keyList.Contains(keyType);
    }

    private void OnTriggerEnter(Collider col) {
        Key key = col.GetComponent<Key>();
        if (key != null) {
            AddKey(key.GetKeyType());
            Destroy(key.gameObject);
        }
    }
}
