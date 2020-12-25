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
    [SerializeField] static int Population = 25;
    [SerializeField] float movementSpeed = 2.0f;
    [SerializeField] int rotationSpeed = 1;
    [SerializeField] GameObject characterPrephab;
    [SerializeField] int[] layers = { 5, 32, 12, 1 };
    [SerializeField] float mutationChance = 7f;
    [SerializeField] float mutationStrength = 0.5f;

    [SerializeField] int numberOfMutatedCharactersToKeep = 5;
    [SerializeField] float MutationCuttoff;
    [SerializeField] static float GameSpeed = 1.0f;

    public static int dead = 0;
    public static int numberOfNonSmartBrainsToBreed = 5;
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
        MutationCuttoff = 2;
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
                best = characters[i].GetComponent<Character>().brain;
                
            }
        }
        return best.fitness;
    }

    int[] getBestCharacters()
    {
        float bestAchievedFitness = getBestNetworkFitness();
        MutationCuttoff = MutationCuttoff > bestAchievedFitness? MutationCuttoff : bestAchievedFitness;
        List<int> bestOnes = new List<int>();
        for (int i = 0; i < Population; i++)
        {
            if (getBrain(characters[i]).fitness >= bestAchievedFitness)
            {
                bestOnes.Add(i);
            }
        }

        return bestOnes.ToArray();
    }

    NeuralNetwork[] sortByFitness(GameObject[] objectsToBeSorted) 
    {
        List<NeuralNetwork> charactersToBeSortedList = new List<NeuralNetwork>();
        for (int i = 0; i < objectsToBeSorted.Length; i++)
        {
            charactersToBeSortedList.Add(getBrain(objectsToBeSorted[i]));
        }

        NeuralNetwork[] charactersToBeSorted = charactersToBeSortedList.ToArray();
        for (int i = 0; i < charactersToBeSorted.Length; i += 1)
        {
            for (int j = 0; j < charactersToBeSorted.Length - i - 1; j += 1)
            {
                if(charactersToBeSorted[j].CompareTo(charactersToBeSorted[j+1]) == 1)
                {
                    NeuralNetwork temp = charactersToBeSorted[j + 1];
                    charactersToBeSorted[j + 1] = charactersToBeSorted[j];
                    charactersToBeSorted[j] = temp;
                }
            }
        }
        return charactersToBeSorted;
    }

    void newGeneration()
    {
        generation += 1;
        List<NeuralNetwork> nextGenration = new List<NeuralNetwork>();
        
        //add smart people to new generation
        int[] smartestCharacters = getBestCharacters();
        for (int i = 0; i < smartestCharacters.Length; i += 1)
        {
            nextGenration.Add(getBrain(characters[smartestCharacters[i]]));
        }

        Generation.text = "Generation " + generation;
        BestFitness.text = "Best Fitness Achieved: " + MutationCuttoff;

        // breed smart brains with non smart ones to make the latter smarter
        if(smartestCharacters.Length > 0 && smartestCharacters.Length < Population - numberOfNonSmartBrainsToBreed)
        {
            NeuralNetwork[] sortredCharacters = sortByFitness(characters);
            int beforeBreeding = nextGenration.ToArray().Length;
            int notSmartBrainIndex = 0;
            while (nextGenration.ToArray().Length - beforeBreeding < numberOfNonSmartBrainsToBreed)
            {
                int smarterBrainIndex = UnityEngine.Random.Range(0, smartestCharacters.Length - 1);
                Debug.Log("Smartest Index: " + smarterBrainIndex + " Length: " + smartestCharacters.Length);
                NeuralNetwork smarterBrain = getBrain(characters[smartestCharacters[smarterBrainIndex]]);

                NeuralNetwork selectedBrain = sortredCharacters[notSmartBrainIndex];
                if (!nextGenration.Contains(selectedBrain))
                {
                    selectedBrain.BreedWith(smarterBrain);
                    nextGenration.Add(selectedBrain);
                }
                notSmartBrainIndex += 1;
            }
        }


        // mutate the rest
        int index = 0;
        while (nextGenration.ToArray().Length < Population)
        {
            NeuralNetwork selectedBrain = getBrain(characters[index]);
            if (!nextGenration.Contains(selectedBrain))
            {
                selectedBrain.Mutate(mutationChance, mutationStrength);
                nextGenration.Add(selectedBrain);
            }
            index += 1;
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
