using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Genetic : MonoBehaviour
{
    NeuralNet savedGenome;

    [Header("References")]
    public CarController controller;

    [Header("Controls")]
    public int initialPopulation = 85;
    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.055f;

    // Mutation Breed Options
    [Header("Crossover Controls")]
    public int bestAgentSelection = 8;
    public int worstAgentSelection = 3;
    public int numberToCrossover;

    // Alle ausgewählten Gene
    private List<int> genePool = new List<int>();

    // Wieviele ausgewählt wurden
    private int naturallySelected;

    public NeuralNet[] population;

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome;

    private bool isLoaded = false;

    private void Start()
    {
        CreatePopulation();
    }

    public NeuralNet SaveGenome()
    {
        NeuralNet _net = new NeuralNet();

        _net.inputLayer = population[currentGenome].inputLayer;
        _net.hiddenLayers = population[currentGenome].hiddenLayers;
        _net.outputLayer = population[currentGenome].outputLayer;
        _net.weights = population[currentGenome].weights;
        _net.biases = population[currentGenome].biases;

        return _net;
    }

    public void LoadGenome(NeuralNet _net)
    {
        savedGenome = _net;
        isLoaded = true;
    }

    private void CreatePopulation()
    {
        population = new NeuralNet[initialPopulation];
        FillPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome()
    {
        // Erstes Gen wird zurückgesetzt und durch das Gen in population getauscht
        controller.ResetWithNetwork(population[currentGenome]);
    }

    private void FillPopulationWithRandomValues(NeuralNet[] newPopulation, int startingIndex)
    {
        while (startingIndex < initialPopulation)
        {
            newPopulation[startingIndex] = new NeuralNet();
            newPopulation[startingIndex].Initilise(controller.LAYERS, controller.NEURONS);
            startingIndex++;
        }
    }

    public void Death(float fitness, NeuralNet _network)
    {
        if (currentGenome < population.Length - 1)
        {
            population[currentGenome].fitness = fitness;
            currentGenome++;
            ResetToCurrentGenome();
        }
        else
        {
            RePopulate();
        }
    }

    private void RePopulate()
    {
        genePool.Clear();
        currentGeneration++;
        naturallySelected = 0;
        SortPopulation();

        NeuralNet[] newPopulation = PickBestPopulation();

        Crossover(newPopulation);
        Mutate(newPopulation);

        FillPopulationWithRandomValues(newPopulation, naturallySelected);

        population = newPopulation;

        currentGenome = 0;

        ResetToCurrentGenome();
    }

    private void Mutate(NeuralNet[] newPopulation)
    {
        for (int i = 0; i < naturallySelected; i++)
        {
            for (int c = 0; c < newPopulation[i].weights.Count; c++)
            {
                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].weights[c] = MutateMatrix(newPopulation[i].weights[c]);
                }
            }
        }
    }

    Matrix MutateMatrix(Matrix _matrix)
    {
        // Picking random Matrizies and mutate them
        int randomPoints = Random.Range(1, (_matrix.rowCount * _matrix.columnCount) / 7);

        Matrix _currentMatrix  = _matrix;

        for (int i = 0; i < randomPoints; i++)
        {
            int randomColumn = Random.Range(0, _currentMatrix.columnCount);
            int randomRow = Random.Range(0, _currentMatrix.rowCount);

            _currentMatrix.matrix[randomColumn, randomRow] = Mathf.Clamp(_currentMatrix.matrix[randomColumn, randomRow] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return _currentMatrix;
    }

    private void Crossover(NeuralNet[] newPopulation)
    {
        for (int i = 0; i < numberToCrossover; i += 2)
        {
            // First and Second Parent
            int AIndex = i;
            int BIndex = i + 1;

            if (genePool.Count >= 1)
            {
                for (int l = 0; l < 100; l++)
                {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];

                    if (AIndex != BIndex)
                    {
                        break;
                    }
                }
            }

            NeuralNet Child1 = new NeuralNet();
            NeuralNet Child2 = new NeuralNet();

            Child1.Initilise(controller.LAYERS, controller.NEURONS);
            Child2.Initilise(controller.LAYERS, controller.NEURONS);

            Child1.fitness = 0;
            Child2.fitness = 0;

            for (int w = 0; w < Child1.weights.Count; w++)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.weights[w] = population[AIndex].weights[w];
                    Child2.weights[w] = population[BIndex].weights[w];
                }
                else
                {
                    Child2.weights[w] = population[AIndex].weights[w];
                    Child1.weights[w] = population[BIndex].weights[w];
                }
            }

            for (int w = 0; w < Child1.biases.Count; w++)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.biases[w] = population[AIndex].biases[w];
                    Child2.biases[w] = population[BIndex].biases[w];
                }
                else
                {
                    Child2.biases[w] = population[AIndex].biases[w];
                    Child1.biases[w] = population[BIndex].biases[w];
                }
            }

            newPopulation[naturallySelected] = Child1;
            naturallySelected++;

            newPopulation[naturallySelected] = Child2;
            naturallySelected++;
        }
    }

    private NeuralNet[] PickBestPopulation()
    {
        NeuralNet[] newPopulation = new NeuralNet[initialPopulation];

        if (isLoaded)
        {
            newPopulation[0] = savedGenome;
            naturallySelected++;
        }

        // Suche die besten Gene
        for (int i = 0; i < bestAgentSelection; i++)
        {
            // Übernimmt die besten Gene in die nächste Generation
            newPopulation[naturallySelected] = population[i].InitiliseCopy(controller.LAYERS, controller.NEURONS);
            newPopulation[naturallySelected].fitness = 0;
            naturallySelected++;
            // Chance das das Gen mit der Höchsten Fitness übernommen wird
            int _currentChance = Mathf.RoundToInt(population[i].fitness);

            for (int j = 0; j < _currentChance + 1; j++)
            {
                genePool.Add(i);
            }
        }
        //Suche die schlechtesten Gene
        for (int i = 0; i < worstAgentSelection; i++)
        {
            int last = population.Length - 1;
            last -= i;

            // Chance das das Gen mit der Höchsten Fitness übernommen wird
            int _currentChance = Mathf.RoundToInt(population[last].fitness);

            for (int j = 0; j < _currentChance + 1; j++)
            {
                genePool.Add(last);
            }
        }

        return newPopulation;
    }

    private void SortPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i; j < population.Length; j++)
            {
                if (population[i].fitness < population[j].fitness)
                {
                    NeuralNet temp = population[i];
                    population[i] = population[j];
                    population[j] = temp;
                }
            }
        }
    }
}
