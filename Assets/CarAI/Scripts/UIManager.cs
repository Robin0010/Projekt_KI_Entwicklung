using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Save System Attributes")]
    public GameObject UISaveParent;
    public GameObject UILoadParent;
    public GameObject saveFileButtonPrefab;
    public GameObject saveFileParent;
    public Text saveInputFieldText;

    public GameObject mainCam;
    public GameObject insideCarCam;
    public GameObject outsideCarCam;
    public GameObject editCam;

    public Text generationText;
    public Text genomeText;

    public GameObject mainMenu;
    public GameObject editMenu;
    public GameObject startButton;

    public GameObject map;

    public Slider timeSlider;

    public CarController controller;
    private Genetic geneticHandler;

    private List<string> fileNames = new List<string>();
    private List<GameObject> loadButtons = new List<GameObject>();

    float currentTimeScale;

    int camIndex = 0;

    private void Awake()
    {
        Time.timeScale = 0;

        //mainCam = GameObject.FindGameObjectWithTag("Main");
    }

    private void Start()
    {
        UISaveParent.SetActive(false);
        UILoadParent.SetActive(false);
        geneticHandler = GameObject.FindObjectOfType<Genetic>();



        editMenu.SetActive(false);
        insideCarCam.gameObject.SetActive(false);
        outsideCarCam.gameObject.SetActive(false);
        editCam.gameObject.SetActive(false);
    }

    private void Update()
    {
        generationText.text = "Generation: " +  geneticHandler.currentGeneration;
        genomeText.text = "Genome: " + geneticHandler.currentGenome;
    }

    public void OnButtonClickOpenSave()
    {
        UISaveParent.SetActive(true);
    }

    public void OnButtonClickOpenLoad()
    {
        UILoadParent.SetActive(true);

        fileNames = SaveManager.GetSaveFiles();

        Transform[] _buttons = saveFileParent.GetComponentsInChildren<Transform>();

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (i != 0)
            {
                Destroy(_buttons[i].gameObject);
            }
        }

        for (int i = 0; i < fileNames.Count; i++)
        {
            GameObject _button = Instantiate(saveFileButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            loadButtons.Add(_button);

            _button.name = "" + i;

            _button.transform.parent = saveFileParent.transform;

            _button.GetComponentInChildren<Text>().text = fileNames[i];

            int _temp = i;

            _button.GetComponent<Button>().onClick.AddListener(delegate { OnButtonClickLoad(_temp); });
        }
    }

    public void OnButtonClickSave()
    {
        NeuralNet _net = geneticHandler.SaveGenome();

        string _fileName = saveInputFieldText.text;

        SaveManager.Save(_net, _fileName);

        UISaveParent.SetActive(false);
    }

    public void OnButtonClickLoad(int _index)
    {
        string _fileName = fileNames[_index];

        NeuralNet _net = SaveManager.Load(_fileName);

        geneticHandler.LoadGenome(_net);

        UILoadParent.SetActive(false);

        foreach (GameObject _button in loadButtons)
        {
            Destroy(_button.gameObject);
        }
    }

    public void OnButtonClickReturn()
    {
        UISaveParent.SetActive(false);
        UILoadParent.SetActive(false);
    }

    public void OnButtonClickChangeCamera()
    {
        camIndex++;

        if (camIndex >= 3)
        {
            camIndex = 0;
        }

        if (camIndex == 0)
        {
            mainCam.gameObject.SetActive(true);
            insideCarCam.gameObject.SetActive(false);
            outsideCarCam.gameObject.SetActive(false);
        }
        else if (camIndex == 1)
        {
            mainCam.gameObject.SetActive(false);
            insideCarCam.gameObject.SetActive(false);
            outsideCarCam.gameObject.SetActive(true);
        }
        else if (camIndex == 2)
        {
            mainCam.gameObject.SetActive(false);
            insideCarCam.gameObject.SetActive(true);
            outsideCarCam.gameObject.SetActive(false);
        }

    }

    public void OnButtonClickStart()
    {
        Time.timeScale = 1;
        startButton.SetActive(false);
    }

    public void OnButtonClickEnterEditMode()
    {
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        mainMenu.SetActive(false);
        editMenu.SetActive(true);
        mainCam.gameObject.SetActive(false);
        insideCarCam.gameObject.SetActive(false);
        outsideCarCam.gameObject.SetActive(false);
        editCam.gameObject.SetActive(true);
    }

    public void OnButtonClickExitEditMode()
    {
        Time.timeScale = currentTimeScale;
        editMenu.SetActive(false);
        mainMenu.SetActive(true);
        mainCam.gameObject.SetActive(true);
        insideCarCam.gameObject.SetActive(false);
        outsideCarCam.gameObject.SetActive(false);
        editCam.gameObject.SetActive(false);
    }

    public void OnButtonClickResetLevel()
    {
        Transform[] mapParts = map.GetComponentsInChildren<Transform>();

        for (int i = 0; i < mapParts.Length; i++)
        {
            if (mapParts[i].gameObject.GetComponent<MeshRenderer>() != null && mapParts[i].gameObject.GetComponent<BoxCollider>() != null)
            {
                mapParts[i].gameObject.GetComponent<MeshRenderer>().enabled = true;
                mapParts[i].gameObject.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    public void OnButtonClickMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnSliderValueChangeTimeScale()
    {
        Time.timeScale = timeSlider.value;
    }
}
