# impostor
Psychological tactics and deception. 'imposter' is a straightforward approach to a multiplayer shooter.

---

## AI Controller

My first attempt at creating an AI Controller resulted in some undesirable effects. I used a collection of semi-randomized heuristics to guide the AI in the right direction. In theory, this seemed like the quick and simple approach.

However, in practice I was easily able to tell which character is an AI and which was a *real player* controlling an imposter. This made bluffing and deception, a large part of the game, not as fun as it should be. As such, I needed a way for the AI to mimic what a real player does.

### ML-Agents Integration
(When I first started this project, ML-Agents was not yet publicly available, and I had tried other approaches to simulate human-like behaviour. To see my thought processes behind these appraches, check out the other sections below.)

I recently attended GDC 2018 where a few developers at Unity talked about their experimental project, ML-Agents: a new open-source toolkit built to enable training and deploying Deep Learning models within Unity. It seemed like this was a perfect solution for my game, and a great opportunity to learn more about Unity's latest features.

#### Brains
In Impostor, we can have many different Brain types to simulate different types of Player behaviour (known as Policies). For example, we can have one Brain imitate a player that tries to avoid enemies as best it can. Another Brain could instead simulate a player that is aggressive and chases/follows players. When a Brain is attached to an AI's GameObject, that AI will start to exhibit the Brain's trained policy!

##### List of trained Policies in Impostor
- Avoiding Players: Agents with this Brain will actively avoid approaching Players with some degree of ambiguity
- Follow Players: Agents with this Brain will attempt to follow/approach opposing Players
- Avoid Obstacles: A Policy to avoid unpassable obstacles.

In addition to these trained Policies, ML-Agents allows the support of a Heuristic Brain. That is, without compromising the functionality of trained Agents, we can have other Agents that simply follow a set of rules / heuristics to make a decision. I created a Heuristic brain to add even more complexity into the AI's behaviour, making it harder to predict who is real and who is AI.

Together, the AI in Impostor will be composed of many different Player behaviours, each represented as a Brain in ML-Agents. This ensures each game will be played differently from the last.

Can you tell which Impostor is controlled by a real player?

![display](https://media.giphy.com/media/2ISiXWEmoouzL9hf6e/giphy.gif)

---

### \*\*Old approaches for AI\*\*
### Logistic Regression Approach

At any given point in time, we want our AI Controller to choose the most appropriate horizontal and vertical input to simulate a player pressing a combination of WASD. This can be done by calculating features that represent what a player may be thinking about to determine their next move.

At a fixed `limitRate` interval, we calculate the following features:

* X position on map
* Y position on map
* Distance from center
* X Distance from nearest opponent
* Y Distance from nearest oppoenent
* Is the AI in motion?
* Time since last direction change
* Time since last full start or stop

These distance features are passed to the Logistic Regression classifier which will then spit out the most likely horizontal and vertical directions for the AI's next input.

However after training the classifier on ~5000 training examples (and fine tuning along the way) the resulting model had a very ***high bias***. It was simply unable to capture the important (and non-linear) connections between the features.

Below is an example of the Logistic Regression classifier failing to make a seemingly normal player decision (avoiding an opponent close to them).

![Logistic Regression Failure](https://media.giphy.com/media/DjqyuC199ZjJ6/giphy.gif)

---

### Convolutional Neural Network Approach

Convolutional Neural Networks have traditionally performed *very well* on classifying images into multiple types.

I could represent each "map state" as a matrix, similar to how a picture is made up of a matrix of pixels. The bounds of the map itself are fixed, but the contents within are always changing. Thus the map state can be represented as multiple NxM matrices of objects, where NxM is the limits of the map.

For example, there are 3 main types of objects in Impostor.
* ***Player***. The Player currently being controlled.
* ***Obstacles***. Any impassable or destructible object.
* ***Enemies***. Either an opposing Real Player or opposing AI controlled Player.

Knowing this, I can construct matrices that represent the map state to be used as input for the Convolutional Neural Network. Below is an example of input data if my map was 5x5 units.

| Player Matrix | Obstacles Matrix | Enemies Matrix |
|:---------------:|:-----------------:|:---------------:|
| 0, 0, 0, 0, 0 | 1, 0, 0, 0, 0   | 0, 0, 0, 0, 0 |
| 0, 0, 0, 0, 0 | 1, 0, 0, 1, 0   | 0, 0, 0, 0, 0 |
| 0, 0, 0, 1, 0 | 1, 0, 0, 0, 0   | 0, 1, 0, 0, 1 |
| 0, 0, 0, 0, 0 | 1, 1, 0, 0, 0   | 0, 0, 0, 0, 0 |
| 0, 0, 0, 0, 0 | 1, 1, 1, 0, 0   | 0, 0, 0, 0, 0 |

In practice, the map covers around 60 units in width and 115 units in length. This is partitioned into a 100x200 grid to allow for more flexibility. Below is a visualization of the grid being used:

![Map Grid](https://imgur.com/WHKrEE5.png)

Now, we have some useful features that should be able to help our AI controller choose an appropriate direction for a given "map state".

#### The Training Pipeline
- During runtime, compute the map state as a set of 3x100x200 matrices in Unity.
- After gathering a play session's data, export the training data. This was easily done thanks to AWS's C# SDK for Unity.
- When ready to train, run a Python script ([see here](https://github.com/Codetroopa/impostor-convnet)) that loads training data from S3, and generates a TensorFLow graph.
- After training the model, run another script that freezes the TensorFlow graph and exports it back into S3.
- Finally, this model can be used in Unity with the C# bindings for TensorFlow.

#### The Results
After training and integrating my first version of a CNN, I noticed a few issues. First and foremost was a significant decrease in performance, mainly in small spikes that affected framerate. Whenever inference was performed on the model, there was a large spike in performance.

![Spike](https://imgur.com/S71AaWP.png)

Initially, I thought this was due to generating the large matrices to use as input data. I implemented caching so I only had to update the map state matrices when an obstacle/player has moved or been destroyed. However, this didn't solve the problem either. I had later learned that it was the sheer size of the Tensorflow graph that caused the issue. Since I was training on multiple thousand training samples, with each containing 60,000 points of data, the resulting Tensorflow model was large and took a long time to perform inference on.
