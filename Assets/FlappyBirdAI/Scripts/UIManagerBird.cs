using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManagerBird : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject visuCam;

    private bool camSwitch = false;

    private void Awake()
    {
        visuCam.SetActive(false);

        Time.timeScale = 1;
    }

    public void OnButtonClickVisu()
    {
        camSwitch = !camSwitch;

        mainCam.SetActive(!camSwitch);
        visuCam.SetActive(camSwitch);

        GetComponent<NNetVisualiser>().OnButtonClickUpdateNodes();
    }

    public void OnButtonClickMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
