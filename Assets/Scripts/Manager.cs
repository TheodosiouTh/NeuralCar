using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // UI components
    [SerializeField] Text Generation;
    [SerializeField] Text BestFitness;

    public static float timeToDeath = 5;
    [SerializeField] static int Population = 50;
    [SerializeField] float movementSpeed = 10.0f;
    [SerializeField] int rotationSpeed = 1;
    [SerializeField] GameObject characterPrephab;
    [SerializeField] int[] layers = { 5, 12, 9, 2 };
    [SerializeField] float mutationChance = 10f;
    [SerializeField] float mutationStrength = 0.0005f;

    public static int dead = 0;
    private GameObject[] characters = new GameObject[Population];

    public int currentCheckpoint = 0;

    int generation = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Population; i++)
        {
            NeuralNetwork temp = new NeuralNetwork(layers);
            characters[i] = createCharacter(temp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dead == Population)
        {
            newGeneration();

            dead = 0;
        }

    }

    float getBestNetworkFitness()
    {
        NeuralNetwork best = characters[0].GetComponent<Character>().brain;
        for (int i = 1; i < Population; i++)
        {
            if (characters[i].GetComponent<Character>().brain.fitness > best.fitness)
            {
                best = getBrain(characters[i]);
            }
        }
        return best.fitness;
    }
    
    NeuralNetwork[] getBestNetworks(){
        float bestFitness = getBestNetworkFitness();
         List<NeuralNetwork> bestList = new List<NeuralNetwork>();
        for (int i = 0; i < Population; i++)
        {
            if (characters[i].GetComponent<Character>().brain.fitness == bestFitness)
            {
                bestList.Add(getBrain(characters[i]));
            }
        }
        return bestList.ToArray();
    }

    void newGeneration()
    {
        generation += 1;
        Generation.text = "Generation " + generation;
        List<NeuralNetwork> nextGenration = new List<NeuralNetwork>();

        // keep the smartest characters
        NeuralNetwork[] smartesCharacters = getBestNetworks();
        for (int i = 0; i < smartesCharacters.Length; i++)
        {
            nextGenration.Add(smartesCharacters[i]);
        }
        BestFitness.text = "Best Fitness(last generation): " + smartesCharacters[0].fitness;

        // The rest of the spots are mutations of the smartes characters;
        while (nextGenration.ToArray().Length < Population)
        {
            int selectedSmartCharacter = UnityEngine.Random.Range(0, smartesCharacters.Length - 1);
            NeuralNetwork tempBrain = smartesCharacters[selectedSmartCharacter].Clone();
            tempBrain.Mutate(mutationChance, mutationStrength);
            nextGenration.Add(tempBrain);
        }

        // Instantiate next Gneration
        NeuralNetwork[] nextGenerationArray = nextGenration.ToArray();
        for (int i = 0; i < Population; i++)
        {
            Destroy(characters[i]);
            NeuralNetwork tempBrain = nextGenerationArray[i];
            tempBrain.fitness = 0;
            characters[i] = createCharacter(tempBrain);
        }

    }

    NeuralNetwork getBrain(GameObject character)
    {
        return character.GetComponent<Character>().brain;
    }


    GameObject createCharacter(NeuralNetwork brain)
    {
        GameObject character = Instantiate(characterPrephab, this.transform.position, transform.rotation);
        character.GetComponent<Character>().brain = brain;
        character.GetComponent<Character>().movementSpeed = movementSpeed;
        character.GetComponent<Character>().rotationSpeed = rotationSpeed;
        character.transform.parent = transform;
        return character;
    }
}
