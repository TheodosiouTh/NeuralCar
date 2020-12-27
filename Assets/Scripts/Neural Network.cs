using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class NeuralNetwork
{
    private int[] layers;
    private float[][] neurons;
    private float[][] biases;
    private float[][][] weights; 
    public float fitness;

    public NeuralNetwork(int[] layers)
    {
        this.fitness = 0f;
        InitializeLayers(layers);
        InitializeNeurons();
        InitializeBiases();
        InitializeWeights();
    }

    private void InitializeLayers(int[] layers) {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
    }

    private void InitializeNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitializeBiases()
    {
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    private void InitializeWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = (float)System.Math.Tanh((double)(value + biases[i][j])) * 10;
            }
        }
        return neurons[neurons.Length - 1];
    }

    float Relu(float value)
    {
        if (value < 0)
        {
            return 0;
        }
        return value;
    }

    public static NeuralNetwork Load(string path)
    {
        int[] tempLayers = { 1, 2 };
        NeuralNetwork loadedNeuralNetwork = new NeuralNetwork(tempLayers);

        TextReader tr = new StreamReader(path);
        int NumberOfLines = (int)new FileInfo(path).Length;
        string[] ListLines = new string[NumberOfLines];
        int index = 1;
        for (int i = 1; i < NumberOfLines; i++)
        {
            ListLines[i] = tr.ReadLine();
        }
        tr.Close();
        if (new FileInfo(path).Length > 0)
        {
            List<int> readLayers = new List<int>();
            string[] LayerLine = ListLines[index].Split(',');
            for (int i = 0; i < LayerLine.Length; i++) {
                int tempLayer = 0;
                Int32.TryParse(LayerLine[i], out tempLayer);
                readLayers.Add(tempLayer);
            }
            loadedNeuralNetwork = new NeuralNetwork(readLayers.ToArray());
            index++;

            for (int i = 0; i < loadedNeuralNetwork.biases.Length; i++)
            {
                for (int j = 0; j < loadedNeuralNetwork.biases[i].Length; j++)
                {
                    loadedNeuralNetwork.biases[i][j] = float.Parse(ListLines[index]);
                    index++;
                }
            }
            for (int i = 0; i < loadedNeuralNetwork.weights.Length; i++)
            {
                for (int j = 0; j < loadedNeuralNetwork.weights[i].Length; j++)
                {
                    for (int k = 0; k < loadedNeuralNetwork.weights[i][j].Length; k++)
                    {
                        loadedNeuralNetwork.weights[i][j][k] = float.Parse(ListLines[index]);
                        index++;
                    }
                }
            }
        }
        return loadedNeuralNetwork;
    }

    public void Save(string path)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            string layerStirng = "";
            for (int i = 0; i < layers.Length; i++)
            {
                if (i != layers.Length - 1) {
                    layerStirng += layers[i].ToString() + ',';
                }
                else {
                    layerStirng += layers[i];
                }
            }
            sw.WriteLine(layerStirng.ToString());

            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    sw.WriteLine(biases[i][j]);
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        sw.WriteLine(weights[i][j][k]);
                    }
                }
            }
        }
    }

    public void Mutate(float chance, float val)
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5) ? biases[i][j] += UnityEngine.Random.Range(-val, val) : biases[i][j];
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5) ? weights[i][j][k] += UnityEngine.Random.Range(-val, val) : weights[i][j][k];

                }
            }
        }
    }
}
