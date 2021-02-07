using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    public int inputNeuronsCount;
    public int outputNeuronsCount;
    public int hiddenLayerCount;
    public int hiddenNeuronsPerLayerCount;
    public int biasesCount;

    public List<float> inputLayer = new List<float>();
    public List<float> hiddenLayer = new List<float>();
    public List<float> outputLayer = new List<float>();
    public List<float> weights = new List<float>();
    public List<float> biases = new List<float>();
}
