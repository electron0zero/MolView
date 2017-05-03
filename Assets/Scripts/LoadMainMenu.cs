using UnityEngine;
using System.Collections;

public class LoadMainMenu : MonoBehaviour {

    public void LoadScene(int level)
    {
        Application.LoadLevel(level);
    }
}