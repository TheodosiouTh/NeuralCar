using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] Button startButton;
    protected bool start = false;
    [SerializeField] InputField filepath;
    string BEST_BRAIN_SAVE_PATH;
    [SerializeField] Toggle readSavedFile;
    bool readFromFile = false;


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
    void startTraining()
    {
        dead = 0;
        if (!readFromFile) { 
            for (int i = 0; i < Population; i++)
            {
                NeuralNetwork temp = new NeuralNetwork(layers);
                characters[i] = createCharacter(temp);
            }
        }
        else {
            NeuralNetwork readCharacter = NeuralNetwork.Load(BEST_BRAIN_SAVE_PATH);
            newGeneration(readCharacter);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            startTraining();
            start = false;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
            if (dead == Population)
            {
                newGeneration(null);
                dead = 0;
            }
        }


    }

    NeuralNetwork getBestNetwork()
    {
        NeuralNetwork best = characters[0].GetComponent<Character>().brain;
        for (int i = 1; i < Population; i++)
        {
            if (characters[i].GetComponent<Character>().brain.fitness > best.fitness)
            {
                best = getBrain(characters[i]);
            }
        }
        best.Save(BEST_BRAIN_SAVE_PATH);
        return best;
    }

    void newGeneration(NeuralNetwork readFromFile)
    {
        List<NeuralNetwork> nextGenration = new List<NeuralNetwork>();
        NeuralNetwork smartesCharacter;
        if (readFromFile != null) {
            smartesCharacter = readFromFile;
        }
        else {
            generation += 1;
            Generation.text = "Generation " + generation;
            smartesCharacter = getBestNetwork();
        }
        nextGenration.Add(smartesCharacter);
        BestFitness.text = "Best Fitness(last generation): " + smartesCharacter.fitness;

        // The rest of the spots are mutations of the smartes characters;
        while (nextGenration.ToArray().Length < Population)
        {
            NeuralNetwork tempBrain = smartesCharacter.Clone();
            tempBrain.Mutate(mutationChance, mutationStrength);
            nextGenration.Add(tempBrain);
        }

        // Instantiate next Gneration
        NeuralNetwork[] nextGenerationArray = nextGenration.ToArray();
        for (int i = 0; i < Population; i++)
        {
            if(characters[i] != null) Destroy(characters[i]);
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

    public void initializeTraining()
    {
        readFromFile = readSavedFile.isOn;
        string inputfieldText = filepath.text.ToString();
        BEST_BRAIN_SAVE_PATH = inputfieldText != "" ? inputfieldText : "./savedNetwork.txt";
        Debug.Log(BEST_BRAIN_SAVE_PATH);
        filepath.gameObject.SetActive(false);
        readSavedFile.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        start = true;
    }
}
