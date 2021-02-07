using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public void OnButtonClickLoadScene(int _buildIndex)
    {
        SceneManager.LoadScene(_buildIndex);
    }

    public void OnButtonClickQuit()
    {
        Application.Quit();
    }
}
