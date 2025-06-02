using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public void LoadLevel(string LevelName) {
        SceneManager.LoadScene(LevelName);
    }
}
