## Machine Learning Model for AI:
###  Feature Set:
- Position on map (x, y)
- Time (ms? 10ms? every frame?)  // I don't think we need this. Time elapsed should be independent of whether the AI moves or not
- Time since last direction change?
- Time while in motion / stationary?
- Previous frame's input vector  // I don't think we need this. The player's input gets inferred by other features
- Is player stationary?
- Distance from center
- Distance from border (any border? North,South,East,West?)
- Distance from nearest obstacle
- Distance from nearest player/ai

#### Output:
- Vector of player input
- [0 1 0 1] which maps to WASD

## Convolutional Neural Network Approach:
#### Input: 200x100x3 matrix.
- A 200x100 plane in the X/Y axis that either contains 0 or 1 in each spot, depending on whether an object is there or not.
- Stack 3 of these matrices together, where each one represents the locations of Obstacles/Player/Enemies

#### Output:
- Vector that represents a single direction for the Player. (Represented in Compass notation)
- Index 0 is North, 1 is North-East ... 7 is North-West. 8 is a special case of no direction at all
- e.g. [0 0 0 0 1 0 0 0 0] represents the South direction.

#### Enhancements:
- [ ] Cache the Obstacle Matrix.
	- Obstacles generally stay in the same position for a large majority of the game.
	- Computing this matrix every time an AI needs to select a new direction is *very* expensive.
	- Instead, add some listener that gets triggered any-time an Obstacle/Wall is destroyed. Only then will it re-calculate and cache the Obstacle Matrix.
- [ ] Better training data generation.
	- Right now, AI are standing still. As such, the Enemy Matrices are almost always the same
	- A better approach would be to randomly move all AI to a different grid position that DOES NOT contain an Obstacle/Wall

### The flaws of using a neural network
After training and integrating my first version of a ConvNet, I noticed a few issues. First and foremost was a significant decrease in performance, mainly in small spikes that affected framerate. Whenever inference was performed on the model, there was a large spike in performance.

Initially, I thought this was due to generating the large matrices to use as input data. I implemented caching so I only had to update the matrix when an obstacle/player has moved or been destroyed but that didn't solve the problem either. I had later learned that it was the sheer size of the Tensorflow graph that caused the issue. Since I was training on multiple thousand training samples, with each containing 60,000 points of data, the resulting Tensorflow model was large and took a long time to perform inference on.

I recently attended GDC 2018 where a few developers at Unity talked about their experimental project, ML-Agents: a new open-source toolkit built to enable training and deploying Deep Learning models within Unity. It seemed like this was a perfect solution for my game, and a great opportunity to learn more about Unity's Engine.

### ML-Agents
#### Academy
Controls any live AI Agents to determine a prediction. Can control many Brains across many agents and is responsible for giving certain impostors certain Brains.

#### Brains
In Impostor, we can have many different Brain types to simulate different types of Player movement. For example, we can have one Brain imitate the behaviour of a player that tries to avoid other players as best it can. And another Brain to simulate a player that is aggressive and chases/follows players.

#### Agents
All agents will appear the same as real Players visually, but will have different policies trained into each of their brains. Technically, the Main character is also an Agent, although their Brain is always set to "Player"
The ImpostorAgent GameObject will be responsible for collecting observations from the environment, as well as the corresponding actions for movement. Will have to refactor PlayerMotor logic into ImpostorAgent logic.

## TODO:
- [ ] Finish map
	- [X] Warehouse at east side
	- [X] Shrubbery
	- [X] Cars and fence along west side
	- [ ] River
	- [ ] Explosion and fire effects on vehicles
- [ ] Artificial Intelligence
	- [X] Complete class to calculate Features
	- [X] Export Features to CSV
	- [X] Store CSV on AWS S3
	- [X] Begin gathering training set data
	- [X] Train a first generation logistic regression model
	- [X] Look for improvements in the model
	- [X] Train CNN model
		- [X] Enhance `GameManager` to generate Player/Obstacle/Enemy Matrices for training.
		- [X] Script to import training data from S3
		- [X] Script to generate a model from training data
		- [X] Gather data and create first generation model
	- [X] Integrate Tensorflow model into C#
		- [X] Save first model with Tensorflow
		- [X] Import Tensorflow model from S3 to C# app
		- [X] Load Tensorflow model to be ready for prediction
		- [X] Use actual freeze_graph.py script when freezing model
		- [X] Prepare input data for inference on C# model
	- [ ] Final model
	- [ ] Integrate model with AI prefab.
	- [ ] Sync AI prefab on the network
	- [X] Add AI section to readme
- [ ] Main Menu
	- [ ] Start local game
	- [ ] Return to menu after game is finished
