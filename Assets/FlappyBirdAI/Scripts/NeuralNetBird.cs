using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetBird : MonoBehaviour
{
    public int inputLayerNeuronsCount = 3;
    public int outputLayerNeuronsCount = 2;

    public Matrix inputLayer = new Matrix(1, 3);

    public List<Matrix> hiddenLayers = new List<Matrix>();

    public Matrix outputLayer = new Matrix(2, 1);

    public List<Matrix> weights = new List<Matrix>();

    public List<float> biases = new List<float>();

    public float fitness;

    public NeuralNetBird()
    {

    }

    public NeuralNetBird(int _inputNeuronCount, int _outputNeuronCount, int _hiddenLayerCount, int _hiddenNeuronCount)
    {
        inputLayerNeuronsCount = _inputNeuronCount;
        outputLayerNeuronsCount = _outputNeuronCount;

        Initilise(_hiddenLayerCount, _hiddenNeuronCount);
    }

    public void Initilise(int _hiddenLayerCount, int _hiddenNeuronCount)
    {
        inputLayer = new Matrix(1, inputLayerNeuronsCount);
        outputLayer = new Matrix(outputLayerNeuronsCount, 1);

        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < _hiddenLayerCount; i++)
        {
            Matrix _currentMatrix = new Matrix(1, _hiddenNeuronCount);

            hiddenLayers.Add(_currentMatrix);

            biases.Add(Random.Range(-1, 1f));

            if (i == 0)
            {
                Matrix inputToFirstHiddenLayer = new Matrix(inputLayerNeuronsCount, _hiddenNeuronCount);
                weights.Add(inputToFirstHiddenLayer);
            }

            Matrix _hiddenToHidden = new Matrix(_hiddenNeuronCount, _hiddenNeuronCount);
            weights.Add(_hiddenToHidden);
        }

        Matrix _outputWeight = new Matrix(_hiddenNeuronCount, outputLayerNeuronsCount);

        weights.Add(_outputWeight);

        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();
    }

    public NeuralNetBird InitiliseCopy(int _hiddenLayerCount, int _hiddenNeuronCount)
    {
        NeuralNetBird _net = new NeuralNetBird();

        List<Matrix> _newWeights = new List<Matrix>();

        for (int i = 0; i < this.weights.Count; i++)
        {
            Matrix _currentWeight = new Matrix(weights[i].columnCount, weights[i].rowCount);

            for (int x = 0; x < this.weights[i].columnCount; x++)
            {
                for (int y = 0; y < this.weights[i].rowCount; y++)
                {
                    _currentWeight.matrix[x, y] = this.weights[i].matrix[x, y];
                    
                }
            }

            _newWeights.Add(_currentWeight);
        }

        List<float> _newBiases = new List<float>();

        _newBiases.AddRange(biases);

        _net.weights = _newWeights;
        _net.biases = _newBiases;

        inputLayer = new Matrix(1, inputLayerNeuronsCount);
        outputLayer = new Matrix(outputLayerNeuronsCount, 1);

        _net.InitiliseHidden(_hiddenLayerCount, _hiddenNeuronCount);

        return _net;
    }

    public void InitiliseHidden(int _hiddenLayerCount, int _hiddenNeuronCount)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();

        for (int i = 0; i < _hiddenLayerCount; i++)
        {
            Matrix _newHiddenLayer = new Matrix(1, _hiddenNeuronCount);
            hiddenLayers.Add(_newHiddenLayer);
        }
    }

    public (float, float) RunNetwork(float _inputA, float _inputB, float _inputC)
    {
        inputLayer.matrix[0, 0] = _inputA;
        inputLayer.matrix[0, 1] = _inputB;
        inputLayer.matrix[0, 2] = _inputC;

        inputLayer = inputLayer.ActivateTanh();

        hiddenLayers[0] = ((inputLayer.MultiplyMatrices(weights[0])).AddScalar(biases[0])).ActivateTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1].MultiplyMatrices(weights[i])).AddScalar(biases[i])).ActivateTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1].MultiplyMatrices(weights[weights.Count - 1])).AddScalar(biases[biases.Count - 1])).ActivateTanh();

        return (outputLayer.Sigmoid(outputLayer.matrix[0, 0]), outputLayer.Sigmoid(outputLayer.matrix[0, 1]));
    }

    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            weights[i].RandomValue();
        }
    }
}