using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NNetVisualiser : MonoBehaviour
{
    public GameObject lineRendererParent;
    public GameObject nodePrefab;
    public GameObject nodeParent;
    public GameObject nodeLayerPrefab;
    public GameObject lineRendererLayerPrefab;

    private List<GameObject> nodes = new List<GameObject>();
    private List<GameObject> lines = new List<GameObject>();

    [Header("Node Layer")]
    private GameObject inputLayerNodesParent;
    private GameObject outputLayerNodesParent;
    private List<GameObject> hiddenLayerNodesParent = new List<GameObject>();

    private List<GameObject> inputLayerNodes = new List<GameObject>();
    private List<GameObject> outputLayerNodes = new List<GameObject>();
    private GameObject[,] hiddenLayerNodes;

    [Header("Line Layer")]
    private GameObject inputLayerLinesParent;
    private GameObject outputLayerLinesParent;
    private List<GameObject> hiddenLayerLinesParent = new List<GameObject>();

    private List<GameObject> inputLayerLines = new List<GameObject>();
    private List<GameObject> outputLayerLines = new List<GameObject>();
    private GameObject[,] hiddenLayerLines;

    public Material weightOffMaterial;
    public Material weightOnMaterial;

    public NeuralNetBird currentNet;

    private Color nodeOffColor = Color.red;
    private Color nodeOnColor = Color.green;

    private Bird player;

    private int neuronsPerLayer;
    private int hiddenLayerCount;

    private int inputLayerNeurons = 3;
    private int outputLayerNeurons = 2;


    private float timer = 0.5f;
    private bool isConnected = false;
    private bool update = false;
    private void Awake()
    {
        player = GameObject.FindObjectOfType<Bird>();

        hiddenLayerCount = player.LAYERS;
        neuronsPerLayer = player.NEURONS;

        Initilise();
    }

    private void Start()
    {
        GenerateNodes();
    }

    private void Update()
    {
        //UpdateNodes();

        if (timer > 0 && !isConnected)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0 && !isConnected)
        {
            ConnectNodes();
            isConnected = true;
        }

        if (update)
        {
            UpdateNodes();
        }
    }

    private void Initilise()
    {
        hiddenLayerNodes = new GameObject[hiddenLayerCount, neuronsPerLayer];
        hiddenLayerLines = new GameObject[hiddenLayerCount, neuronsPerLayer * neuronsPerLayer];

        // Initilise Nodes
        inputLayerNodesParent = Instantiate(nodeLayerPrefab, transform.position, Quaternion.identity);
        inputLayerNodesParent.name = "Input Layer";
        inputLayerNodesParent.transform.parent = nodeParent.transform;
        inputLayerNodesParent.transform.localScale = new Vector3(1, 1, 1);

        for (int i = 0; i < hiddenLayerCount; i++)
        {
            GameObject _nodeParent = Instantiate(nodeLayerPrefab, transform.position, Quaternion.identity);
            hiddenLayerNodesParent.Add(_nodeParent);
            hiddenLayerNodesParent[i].name = "Hidden Layer " + (i + 1);
            hiddenLayerNodesParent[i].transform.parent = nodeParent.transform;

            hiddenLayerNodesParent[i].transform.localScale = new Vector3(1, 1, 1);
        }

        outputLayerNodesParent = Instantiate(nodeLayerPrefab, transform.position, Quaternion.identity);
        outputLayerNodesParent.name = "Output Layer";
        outputLayerNodesParent.transform.parent = nodeParent.transform;
        outputLayerNodesParent.transform.localScale = new Vector3(1, 1, 1);

        // Initilise Line Renderer
        inputLayerLinesParent = new GameObject();
        inputLayerLinesParent.name = "Input Layer";
        inputLayerLinesParent.transform.parent = lineRendererParent.transform;

        for (int i = 0; i < hiddenLayerCount; i++)
        {
            GameObject _newLine = new GameObject();
            hiddenLayerLinesParent.Add(_newLine);
            hiddenLayerLinesParent[i].name = "Hidden Layer " + (i + 1);
            hiddenLayerLinesParent[i].transform.parent = lineRendererParent.transform;
        }

        outputLayerLinesParent = new GameObject();
        outputLayerLinesParent.name = "Output Layer";
        outputLayerLinesParent.transform.parent = lineRendererParent.transform;
    }

    private void GenerateNodes()
    {
        // Generate Input Nodes

        for (int i = 0; i < inputLayerNeurons; i++)
        {
            GameObject _node = Instantiate(nodePrefab, new Vector3(0, 0, 0), Quaternion.identity);

            _node.transform.parent = inputLayerNodesParent.transform;

            _node.name = "Node " + (i + 1);

            inputLayerNodes.Add(_node);
        }

        // Generate Input Lines

        for (int i = 0; i < inputLayerNeurons * neuronsPerLayer; i++)
        {
            GameObject _line = new GameObject();

            _line.transform.parent = inputLayerLinesParent.transform;

            _line.name = "LineRenderer " + (i + 1);

            _line.AddComponent<LineRenderer>();

            inputLayerLines.Add(_line);
        }

        // Generate Hidden Layer Nodes

        for (int i = 0; i < hiddenLayerCount; i++)
        {
            for (int j = 0; j < neuronsPerLayer; j++)
            {
                GameObject _node = Instantiate(nodePrefab, new Vector3(0, 0, 0), Quaternion.identity);

                _node.transform.parent = hiddenLayerNodesParent[i].transform;

                _node.name = "Node " + (i + 1) + ", " + (j + 1);

                hiddenLayerNodes[i, j] = _node;
            }
        }

        // Generate Hidden Layer Lines

        for (int i = 0; i < hiddenLayerCount - 1; i++)
        {
            for (int j = 0; j < neuronsPerLayer * neuronsPerLayer; j++)
            {
                GameObject _line = new GameObject();

                _line.transform.parent = hiddenLayerLinesParent[i].transform;

                _line.name = "LineRenderer " + (j + 1);

                _line.AddComponent<LineRenderer>();

                hiddenLayerLines[i, j] = _line;
            }
        }

        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < neuronsPerLayer * outputLayerNeurons; j++)
            {
                GameObject _line = new GameObject();

                _line.transform.parent = hiddenLayerLinesParent[i + 2].transform;

                _line.name = "LineRenderer " + (j + 1);

                _line.AddComponent<LineRenderer>();

                hiddenLayerLines[i + 2, j] = _line;
            }
        }

        // Generate Output Nodes

        for (int i = 0; i < outputLayerNeurons; i++)
        {
            GameObject _node = Instantiate(nodePrefab, new Vector3(0, 0, 0), Quaternion.identity);

            _node.transform.parent = outputLayerNodesParent.transform;

            _node.name = "Node " + (i + 1);

            outputLayerNodes.Add(_node);

            GameObject _line = new GameObject();

            _line.transform.parent = outputLayerLinesParent.transform;

            _line.name = "LineRenderer " + (i + 1);

            _line.AddComponent<LineRenderer>();

            outputLayerLines.Add(_line);
        }

        for (int i = 0; i < inputLayerLines.Count; i++)
        {
            inputLayerLines[i].GetComponent<LineRenderer>().material = weightOnMaterial;

            inputLayerLines[i].GetComponent<LineRenderer>().SetWidth(0.3f, 0.3f);
        }

        for (int i = 0; i < hiddenLayerLines.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < hiddenLayerLines.GetLength(1); j++)
            {
                hiddenLayerLines[i, j].GetComponent<LineRenderer>().material = weightOnMaterial;

                hiddenLayerLines[i, j].GetComponent<LineRenderer>().SetWidth(0.3f, 0.3f);
            }
        }

        for (int i = 0; i < hiddenLayerLines.GetLength(0) - 2; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                hiddenLayerLines[i + 2, j].GetComponent<LineRenderer>().material = weightOnMaterial;

                hiddenLayerLines[i + 2, j].GetComponent<LineRenderer>().SetWidth(0.3f, 0.3f);
            }
        }
    }

    private void UpdateNodes()
    {
        if (update)
        {
            // Update Input Weights
            inputLayerLines[0].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[0, 0]), NormalizeLineWidth(currentNet.weights[0].matrix[0, 0]));
            inputLayerLines[3].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[0, 1]), NormalizeLineWidth(currentNet.weights[0].matrix[0, 1]));
            inputLayerLines[6].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[0, 2]), NormalizeLineWidth(currentNet.weights[0].matrix[0, 2]));
            inputLayerLines[9].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[0, 3]), NormalizeLineWidth(currentNet.weights[0].matrix[0, 3]));
            inputLayerLines[12].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[0, 4]), NormalizeLineWidth(currentNet.weights[0].matrix[0, 4]));

            inputLayerLines[1].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[1, 0]), NormalizeLineWidth(currentNet.weights[0].matrix[1, 0]));
            inputLayerLines[4].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[1, 1]), NormalizeLineWidth(currentNet.weights[0].matrix[1, 1]));
            inputLayerLines[7].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[1, 2]), NormalizeLineWidth(currentNet.weights[0].matrix[1, 2]));
            inputLayerLines[10].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[1, 3]), NormalizeLineWidth(currentNet.weights[0].matrix[1, 3]));
            inputLayerLines[13].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[1, 4]), NormalizeLineWidth(currentNet.weights[0].matrix[1, 4]));

            inputLayerLines[2].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[2, 0]), NormalizeLineWidth(currentNet.weights[0].matrix[2, 0]));
            inputLayerLines[5].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[2, 1]), NormalizeLineWidth(currentNet.weights[0].matrix[2, 1]));
            inputLayerLines[8].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[2, 2]), NormalizeLineWidth(currentNet.weights[0].matrix[2, 2]));
            inputLayerLines[11].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[2, 3]), NormalizeLineWidth(currentNet.weights[0].matrix[2, 3]));
            inputLayerLines[14].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[0].matrix[2, 4]), NormalizeLineWidth(currentNet.weights[0].matrix[2, 4]));


            // Update First Hidden Layer Weights
            hiddenLayerLines[0, 0].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[0, 0]), NormalizeLineWidth(currentNet.weights[1].matrix[0, 0]));
            hiddenLayerLines[0, 5].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[0, 1]), NormalizeLineWidth(currentNet.weights[1].matrix[0, 1]));
            hiddenLayerLines[0, 10].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[0, 2]), NormalizeLineWidth(currentNet.weights[1].matrix[0, 2]));
            hiddenLayerLines[0, 15].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[0, 3]), NormalizeLineWidth(currentNet.weights[1].matrix[0, 3]));
            hiddenLayerLines[0, 20].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[0, 4]), NormalizeLineWidth(currentNet.weights[1].matrix[0, 4]));

            hiddenLayerLines[0, 1].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[1, 0]), NormalizeLineWidth(currentNet.weights[1].matrix[1, 0]));
            hiddenLayerLines[0, 6].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[1, 1]), NormalizeLineWidth(currentNet.weights[1].matrix[1, 1]));
            hiddenLayerLines[0, 11].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[1, 2]), NormalizeLineWidth(currentNet.weights[1].matrix[1, 2]));
            hiddenLayerLines[0, 16].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[1, 3]), NormalizeLineWidth(currentNet.weights[1].matrix[1, 3]));
            hiddenLayerLines[0, 21].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[1, 4]), NormalizeLineWidth(currentNet.weights[1].matrix[1, 4]));

            hiddenLayerLines[0, 2].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[2, 0]), NormalizeLineWidth(currentNet.weights[1].matrix[2, 0]));
            hiddenLayerLines[0, 7].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[2, 1]), NormalizeLineWidth(currentNet.weights[1].matrix[2, 1]));
            hiddenLayerLines[0, 12].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[2, 2]), NormalizeLineWidth(currentNet.weights[1].matrix[2, 2]));
            hiddenLayerLines[0, 17].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[2, 3]), NormalizeLineWidth(currentNet.weights[1].matrix[2, 3]));
            hiddenLayerLines[0, 22].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[2, 4]), NormalizeLineWidth(currentNet.weights[1].matrix[2, 4]));

            hiddenLayerLines[0, 3].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[3, 0]), NormalizeLineWidth(currentNet.weights[1].matrix[3, 0]));
            hiddenLayerLines[0, 8].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[3, 1]), NormalizeLineWidth(currentNet.weights[1].matrix[3, 1]));
            hiddenLayerLines[0, 13].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[3, 2]), NormalizeLineWidth(currentNet.weights[1].matrix[3, 2]));
            hiddenLayerLines[0, 18].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[3, 3]), NormalizeLineWidth(currentNet.weights[1].matrix[3, 3]));
            hiddenLayerLines[0, 23].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[3, 4]), NormalizeLineWidth(currentNet.weights[1].matrix[3, 4]));

            hiddenLayerLines[0, 4].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[4, 0]), NormalizeLineWidth(currentNet.weights[1].matrix[4, 0]));
            hiddenLayerLines[0, 9].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[4, 1]), NormalizeLineWidth(currentNet.weights[1].matrix[4, 1]));
            hiddenLayerLines[0, 14].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[4, 2]), NormalizeLineWidth(currentNet.weights[1].matrix[4, 2]));
            hiddenLayerLines[0, 19].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[4, 3]), NormalizeLineWidth(currentNet.weights[1].matrix[4, 3]));
            hiddenLayerLines[0, 24].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[1].matrix[4, 4]), NormalizeLineWidth(currentNet.weights[1].matrix[4, 4]));


            // Update Second Hidden Layer Weights
            hiddenLayerLines[1, 0].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[0, 0]), NormalizeLineWidth(currentNet.weights[2].matrix[0, 0]));
            hiddenLayerLines[1, 5].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[0, 1]), NormalizeLineWidth(currentNet.weights[2].matrix[0, 1]));
            hiddenLayerLines[1, 10].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[0, 2]), NormalizeLineWidth(currentNet.weights[2].matrix[0, 2]));
            hiddenLayerLines[1, 15].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[0, 3]), NormalizeLineWidth(currentNet.weights[2].matrix[0, 3]));
            hiddenLayerLines[1, 20].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[0, 4]), NormalizeLineWidth(currentNet.weights[2].matrix[0, 4]));

            hiddenLayerLines[1, 1].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[1, 0]), NormalizeLineWidth(currentNet.weights[2].matrix[1, 0]));
            hiddenLayerLines[1, 6].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[1, 1]), NormalizeLineWidth(currentNet.weights[2].matrix[1, 1]));
            hiddenLayerLines[1, 11].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[1, 2]), NormalizeLineWidth(currentNet.weights[2].matrix[1, 2]));
            hiddenLayerLines[1, 16].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[1, 3]), NormalizeLineWidth(currentNet.weights[2].matrix[1, 3]));
            hiddenLayerLines[1, 21].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[1, 4]), NormalizeLineWidth(currentNet.weights[2].matrix[1, 4]));

            hiddenLayerLines[1, 2].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[2, 0]), NormalizeLineWidth(currentNet.weights[2].matrix[2, 0]));
            hiddenLayerLines[1, 7].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[2, 1]), NormalizeLineWidth(currentNet.weights[2].matrix[2, 1]));
            hiddenLayerLines[1, 12].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[2, 2]), NormalizeLineWidth(currentNet.weights[2].matrix[2, 2]));
            hiddenLayerLines[1, 17].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[2, 3]), NormalizeLineWidth(currentNet.weights[2].matrix[2, 3]));
            hiddenLayerLines[1, 22].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[2, 4]), NormalizeLineWidth(currentNet.weights[2].matrix[2, 4]));

            hiddenLayerLines[1, 3].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[3, 0]), NormalizeLineWidth(currentNet.weights[2].matrix[3, 0]));
            hiddenLayerLines[1, 8].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[3, 1]), NormalizeLineWidth(currentNet.weights[2].matrix[3, 1]));
            hiddenLayerLines[1, 13].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[3, 2]), NormalizeLineWidth(currentNet.weights[2].matrix[3, 2]));
            hiddenLayerLines[1, 18].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[3, 3]), NormalizeLineWidth(currentNet.weights[2].matrix[3, 3]));
            hiddenLayerLines[1, 23].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[3, 4]), NormalizeLineWidth(currentNet.weights[2].matrix[3, 4]));

            hiddenLayerLines[1, 4].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[4, 0]), NormalizeLineWidth(currentNet.weights[2].matrix[4, 0]));
            hiddenLayerLines[1, 9].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[4, 1]), NormalizeLineWidth(currentNet.weights[2].matrix[4, 1]));
            hiddenLayerLines[1, 14].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[4, 2]), NormalizeLineWidth(currentNet.weights[2].matrix[4, 2]));
            hiddenLayerLines[1, 19].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[4, 3]), NormalizeLineWidth(currentNet.weights[2].matrix[4, 3]));
            hiddenLayerLines[1, 24].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[2].matrix[4, 4]), NormalizeLineWidth(currentNet.weights[2].matrix[4, 4]));


            // Update Third Hidden Layer Weights
            hiddenLayerLines[2, 0].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[0, 0]), NormalizeLineWidth(currentNet.weights[3].matrix[0, 0]));
            hiddenLayerLines[2, 5].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[0, 1]), NormalizeLineWidth(currentNet.weights[3].matrix[0, 1]));
            hiddenLayerLines[2, 1].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[0, 2]), NormalizeLineWidth(currentNet.weights[3].matrix[0, 2]));
            hiddenLayerLines[2, 6].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[0, 3]), NormalizeLineWidth(currentNet.weights[3].matrix[0, 3]));
            hiddenLayerLines[2, 2].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[0, 4]), NormalizeLineWidth(currentNet.weights[3].matrix[0, 4]));

            hiddenLayerLines[2, 7].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[1, 0]), NormalizeLineWidth(currentNet.weights[3].matrix[1, 0]));
            hiddenLayerLines[2, 3].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[1, 1]), NormalizeLineWidth(currentNet.weights[3].matrix[1, 1]));
            hiddenLayerLines[2, 8].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[1, 2]), NormalizeLineWidth(currentNet.weights[3].matrix[1, 2]));
            hiddenLayerLines[2, 4].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[1, 3]), NormalizeLineWidth(currentNet.weights[3].matrix[1, 3]));
            hiddenLayerLines[2, 9].GetComponent<LineRenderer>().SetWidth(NormalizeLineWidth(currentNet.weights[3].matrix[1, 4]), NormalizeLineWidth(currentNet.weights[3].matrix[1, 4]));

            inputLayerNodes[0].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.inputLayer.matrix[0, 0]), 255, 255);
            inputLayerNodes[1].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.inputLayer.matrix[0, 1]), 255, 255);
            inputLayerNodes[2].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.inputLayer.matrix[0, 2]), 255, 255);

            hiddenLayerNodes[0, 0].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[0].matrix[0, 0]), 255, 255);
            hiddenLayerNodes[0, 1].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[0].matrix[0, 1]), 255, 255);
            hiddenLayerNodes[0, 2].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[0].matrix[0, 2]), 255, 255);
            hiddenLayerNodes[0, 3].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[0].matrix[0, 3]), 255, 255);
            hiddenLayerNodes[0, 4].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[0].matrix[0, 4]), 255, 255);

            hiddenLayerNodes[1, 0].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[1].matrix[0, 0]), 255, 255);
            hiddenLayerNodes[1, 1].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[1].matrix[0, 1]), 255, 255);
            hiddenLayerNodes[1, 2].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[1].matrix[0, 2]), 255, 255);
            hiddenLayerNodes[1, 3].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[1].matrix[0, 3]), 255, 255);
            hiddenLayerNodes[1, 4].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[1].matrix[0, 4]), 255, 255);

            hiddenLayerNodes[2, 0].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[2].matrix[0, 0]), 255, 255);
            hiddenLayerNodes[2, 1].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[2].matrix[0, 1]), 255, 255);
            hiddenLayerNodes[2, 2].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[2].matrix[0, 2]), 255, 255);
            hiddenLayerNodes[2, 3].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[2].matrix[0, 3]), 255, 255);
            hiddenLayerNodes[2, 4].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.hiddenLayers[2].matrix[0, 4]), 255, 255);

            outputLayerNodes[0].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.outputLayer.matrix[0, 0]), 255, 255);
            if (currentNet.outputLayer.matrix.GetLength(1) > 1)
            {
                outputLayerNodes[1].GetComponent<RawImage>().color = new Vector4(0, NormalizeNodeColor(currentNet.outputLayer.matrix[0, 1]), 255, 255);
            }
        }
    }

    private float NormalizeLineWidth(float x)
    {
        int rMin = 0;
        int rMax = 2;
        int tMin = 0;
        float tMax = 0.5f;

        x += 1;

        float y = (x - rMin) / (rMax - rMin) * (tMax - tMin) + tMin;

        return y;
    }

    private float NormalizeNodeColor(float x)
    {
        int rMin = 0;
        int rMax = 2;
        int tMin = 0;
        int tMax = 1;

        x += 1;

        float y = (x - rMin) / (rMax - rMin) * (tMax - tMin) + tMin;

        return y;
    }

    private void ConnectNodes()
    {
        // Connect Inputlayer and first Hidden Layer
        for (int j = 0; j < inputLayerNodes.Count; j++)
        {
            inputLayerLines[j].GetComponent<LineRenderer>().SetPosition(0, inputLayerNodes[j].transform.position);
            inputLayerLines[j].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[0, 0].transform.position);
        }
        for (int j = 0; j < inputLayerNodes.Count; j++)
        {
            inputLayerLines[j + 3].GetComponent<LineRenderer>().SetPosition(0, inputLayerNodes[j].transform.position);
            inputLayerLines[j + 3].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[0, 1].transform.position);
        }
        for (int j = 0; j < inputLayerNodes.Count; j++)
        {
            inputLayerLines[j + 6].GetComponent<LineRenderer>().SetPosition(0, inputLayerNodes[j].transform.position);
            inputLayerLines[j + 6].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[0, 2].transform.position);
        }
        for (int j = 0; j < inputLayerNodes.Count; j++)
        {
            inputLayerLines[j + 9].GetComponent<LineRenderer>().SetPosition(0, inputLayerNodes[j].transform.position);
            inputLayerLines[j + 9].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[0, 3].transform.position);
        }
        for (int j = 0; j < inputLayerNodes.Count; j++)
        {
            inputLayerLines[j + 12].GetComponent<LineRenderer>().SetPosition(0, inputLayerNodes[j].transform.position);
            inputLayerLines[j + 12].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[0, 4].transform.position);
        }

        // Connects first Hiddenlayer to second Hiddenlayer
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[0, j].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[0, j].transform.position);
            hiddenLayerLines[0, j].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[1, 0].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[0, j + 5].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[0, j].transform.position);
            hiddenLayerLines[0, j + 5].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[1, 1].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[0, j + 10].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[0, j].transform.position);
            hiddenLayerLines[0, j + 10].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[1, 2].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[0, j + 15].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[0, j].transform.position);
            hiddenLayerLines[0, j + 15].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[1, 3].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[0, j + 20].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[0, j].transform.position);
            hiddenLayerLines[0, j + 20].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[1, 4].transform.position);
        }

        // Connects second Hiddenlayer to third Hiddenlayer
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[1, j].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[1, j].transform.position);
            hiddenLayerLines[1, j].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[2, 0].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[1, j + 5].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[1, j].transform.position);
            hiddenLayerLines[1, j + 5].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[2, 1].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[1, j + 10].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[1, j].transform.position);
            hiddenLayerLines[1, j + 10].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[2, 2].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[1, j + 15].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[1, j].transform.position);
            hiddenLayerLines[1, j + 15].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[2, 3].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[1, j + 20].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[1, j].transform.position);
            hiddenLayerLines[1, j + 20].GetComponent<LineRenderer>().SetPosition(1, hiddenLayerNodes[2, 4].transform.position);
        }

        // Connects third Hiddenlayer to Outputlayer
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[2, j].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[2, j].transform.position);
            hiddenLayerLines[2, j].GetComponent<LineRenderer>().SetPosition(1, outputLayerNodes[0].transform.position);
        }
        for (int j = 0; j < 5; j++)
        {
            hiddenLayerLines[2, j + 5].GetComponent<LineRenderer>().SetPosition(0, hiddenLayerNodes[2, j].transform.position);
            hiddenLayerLines[2, j + 5].GetComponent<LineRenderer>().SetPosition(1, outputLayerNodes[1].transform.position);
        }
    }

    public void OnButtonClickUpdateNodes()
    {
        update = !update;
    }
}
