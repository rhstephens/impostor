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
	- [ ] Look for improvements in the model
	- [ ] Final model
	- [ ] Integrate model with AI prefab.
	- [ ] Sync AI prefab on the network
	- [ ] Add AI section to readme
- [ ] Main Menu
	- [ ] Start local game
	- [ ] Return to menu after game is finished
