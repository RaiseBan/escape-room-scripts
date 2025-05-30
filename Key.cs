using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Key : MonoBehaviour
{
    [SerializeField] private KeyType keyType;

    public enum KeyType {
        Pink,
        Purple
    }

    public KeyType GetKeyType() {
        return keyType;
    }
}
