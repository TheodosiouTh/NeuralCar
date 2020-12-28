# NeuralCar

A project where a car learns how to drive, made using Unity3d.

## Solution Explanation

---

1. ### Initialization

   At the start of the game we wait for the user to input the number of population (N characters) he wants to run, and wether he/she wants to load and save to a [flie](#file). with a specified name. If the user inputs a file path but does not check the toggle then the best NeuralNetwork from each generation would be saved to that file. If the user sets the toggle to true then the Best network will be read from the specified file.If the user does not want to read from file then N neural networks with random biases and weights and the training will bein. If the user want to read from a file then a neural network will be initialized using the biases and weights from the file and the rest of the spots of the Population will be filled by mutated versions of that Neural Network. Then the Training will begin.

2. ### Characters

   The character emmits 8 rays like so:

    <p align="center">
        <img src="RayCasted.png" alt="Sublime's custom image" width="600"/>
    </p>

   Getting the respective distances from the walls. Then It uses these distances as inputs to it's Neural network and gets two values back. The first value represents the amount of horizontal rotation needed (positive values represent rotation to the right while negative values represent rotation to the left) and the second represents the amount of movement (positive values represent forward movement while negative values represent backwards movement).

   Spread across the track are checkpoints:

    <p align="center">
        <img src="CheckPoints.png" alt="Sublime's custom image" width="600"/>
    </p>

   If the character crosses one checkpoint then 5 points are added to it's Neural Network's fitness. Each character has 5 seconds to cross the next checkpoint. If he does not cross the checkpoint within that timeframe then he dies and 5 points are removed from it's Neural Network's fitness. The character can also die if he hits a wall. If that happens then 10 points are removed from it's Neural Network's fitness.

3. ### Training
   During training we wait for all the characters to die. If all of them are dead then we get the character with the best fitness score. We treat this character as the previous generation's smartest character. Then we start creating the characters for the next generation. The first character of the next generation is the smartest character of the previous generation and the remaining characters of the generation are mutated versions of the previous generation's smartest character. When all the spots are filled with characters we initialize them and start the next training episode.

## Results

---

After a lot of experimentation I have reached to these values as the best ones:

- Mutation Chance: 5.0
- Mutation Strength: 0.03
- Population: 100

After training for 350 generations with the values above I got this result:

<p align="center">
  <img src="ResultSoFar.gif" alt="Sublime's custom image"/>
</p>

Still have not made a full circle but so far I am happy with the result so far:

## <a name="file"></a>File Format

---

```
line 1:  layers array without brackets, with each line representing the number of the nodes in that layer (ex. 8, 4, 5, 2).
lines 2...(<number-of biases> + 1): float number representing the current bias
lines (<number-of biases> + 2)...(<number-of biases> + <number-of-weights> + 3):  float number representing the current weight
```

**The layers array should always have 8 nodes in the input layer and 2 nodes in the output layer.**

## Bibliography

- [Building a neural network in C#](https://towardsdatascience.com/building-a-neural-network-framework-in-c-16ef56ce1fef), by [Kip Parker](https://medium.com/@kipgparker)
- [Implementing Deep Cloning via Serializing objects](http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx), by [](https://www.codeproject.com/script/Membership/View.aspx?mid=4869534)
