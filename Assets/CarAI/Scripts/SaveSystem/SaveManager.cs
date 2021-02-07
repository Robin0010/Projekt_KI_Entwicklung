using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SaveManager : MonoBehaviour
{
    [Header("Path Attributes")]
    //public static string path = "C:/Users/Robin/AppData/LocalLow/RobinGmBH";
    public static string path = Application.persistentDataPath;
    public static string directory = "/SavedGenome/";
    public static string fileName = "Genome";
    public static string dataType = ".txt";

    public static void Save(NeuralNet _net, string _fileName)
    {
        Genome _genome = ConvertToGenome(_net);

        fileName = _fileName;

        string dir = path + directory;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string json = JsonUtility.ToJson(_genome, true);
        File.WriteAllText(dir + fileName + dataType, json);
        Debug.Log("File Saved!");
    }

    public static NeuralNet Load(string _fileName)
    {
        NeuralNet _net = new NeuralNet();

        fileName = _fileName;

        string fullPath = path + directory + fileName + dataType;
        Genome _genome = new Genome();

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            _genome = JsonUtility.FromJson<Genome>(json);
            Debug.Log("File Loaded");
        }
        else
        {
            Debug.Log("File does not exist");
        }

        _net = ConvertFromGenome(_genome);

        return _net;
    }

    public static List<string> GetSaveFiles()
    {
        List<string> _fileNames = new List<string>();

        DirectoryInfo _dir = new DirectoryInfo(path + directory);
        FileInfo[] _fileInfo = _dir.GetFiles();

        foreach (FileInfo _info in _fileInfo)
        {
            int _fileLength = _info.Name.Length;

            string _fileName = "";

            for (int i = 0; i < _fileLength - 4; i++)
            {
                _fileName += _info.Name[i].ToString();
            }

            _fileNames.Add(_fileName);
        }

        return _fileNames;
    }

    private static Genome ConvertToGenome(NeuralNet _net)
    {
        Genome _genome = new Genome();

        int inputNeuronsCount;
        int outputNeuronsCount;
        int hiddenLayerCount;
        int hiddenNeuronsPerLayerCount;
        int biasesCount;

        List<float> inputLayer = new List<float>();
        List<float> hiddenLayer = new List<float>();
        List<float> outputLayer = new List<float>();
        List<float> weights = new List<float>();
        List<float> biases = new List<float>();

        inputNeuronsCount = _net.inputLayer.rowCount;
        outputNeuronsCount = _net.outputLayer.rowCount;
        hiddenLayerCount = _net.hiddenLayers.Count;
        hiddenNeuronsPerLayerCount = _net.hiddenLayers[0].rowCount;
        biasesCount = _net.biases.Count;

        for (int i = 0; i < inputNeuronsCount; i++)
        {
            inputLayer.Add(_net.inputLayer.matrix[0, i]);
        }

        for (int i = 0; i < hiddenLayerCount; i++)
        {
            for (int j = 0; j < hiddenNeuronsPerLayerCount; j++)
            {
                hiddenLayer.Add(_net.hiddenLayers[i].matrix[0, j]);
            }
        }

        for (int i = 0; i < outputNeuronsCount; i++)
        {
            outputLayer.Add(_net.outputLayer.matrix[0, i]);
        }

        for (int i = 0; i < _net.weights.Count - 1; i++)
        {
            for (int j = 0; j < _net.weights[i].columnCount; j++)
            {
                for (int k = 0; k < _net.weights[i].rowCount; k++)
                {
                    weights.Add(_net.weights[i].matrix[j, k]);
                }
            }
        }

        for (int i = 0; i < biasesCount; i++)
        {
            biases.Add(_net.biases[i]);
        }

        _genome.inputNeuronsCount = inputNeuronsCount;
        _genome.outputNeuronsCount = outputNeuronsCount;
        _genome.hiddenLayerCount = hiddenLayerCount;
        _genome.hiddenNeuronsPerLayerCount = hiddenNeuronsPerLayerCount;
        _genome.biasesCount = biasesCount;

        _genome.inputLayer = inputLayer;
        _genome.hiddenLayer = hiddenLayer;
        _genome.outputLayer = outputLayer;
        _genome.weights = weights;
        _genome.biases = biases;

        return _genome;
    }

    private static NeuralNet ConvertFromGenome(Genome _genome)
    {
        NeuralNet _net = new NeuralNet(_genome.inputNeuronsCount, _genome.outputNeuronsCount, _genome.hiddenLayerCount, _genome.hiddenNeuronsPerLayerCount);

        for (int i = 0; i < _genome.inputNeuronsCount; i++)
        {
            _net.inputLayer.matrix[0, i] = _genome.inputLayer[i];
        }

        int _hiddenLayerIndex = 0;

        for (int i = 0; i < _genome.hiddenLayerCount; i++)
        {
            for (int j = 0; j < _genome.hiddenNeuronsPerLayerCount; j++)
            {
                _net.hiddenLayers[i].matrix[0, j] = _genome.hiddenLayer[_hiddenLayerIndex];
                _hiddenLayerIndex ++;
            }
        }

        for (int i = 0; i < _genome.outputNeuronsCount; i++)
        {
            _net.outputLayer.matrix[i, 0] = _genome.outputLayer[i];
        }

        int _weightsIndex = 0;

        for (int i = 0; i < _net.weights.Count -1; i++)
        {
            for (int j = 0; j < _net.weights[i].columnCount; j++)
            {
                for (int k = 0; k < _net.weights[i].rowCount; k++)
                {
                    _net.weights[i].matrix[j, k] = _genome.weights[_weightsIndex];
                    _weightsIndex++;
                }
            }
        }

        for (int i = 0; i < _genome.biasesCount; i++)
        {
            _net.biases[i] = _genome.biases[i];
        }

        return _net;
    }
}
