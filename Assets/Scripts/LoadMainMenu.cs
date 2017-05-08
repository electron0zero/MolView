using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour {

    public void LoadScene(int level)
    {
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
}