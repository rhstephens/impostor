# impostor
Psychological tactics and deception. 'imposter' is a straightforward approach to a multiplayer shooter.

## AI Controller Model

My first attempt at creating an AI Controller resulted in some undesirable effects. I used simple randomizing techniques in order for the AI to randomly roam across the map. In theory, this seemed like the quick and simple solution to having AI. 

However, in practice I found that my friends were easily able to tell which character is an AI and which was a real Player controlling an imposter. This essentially defeated the purpose of the game. As such, I needed a way for the AI to mimic what a real Player does.

### Basic Approach
At any given point in time, we want our AI Controller to choose the most appropriate Horizontal and Vertical movement, to simulate a player pressing a combonation of WASD. At a fixed `limitRate` interval, we calculate the following features:

* X position on map
* Y position on map
* Distance from center
* X Distance from nearest opponent
* Y Distance from nearest oppoenent
* Is the ai in motion?
* Time since last direction change
* Time since last full start or stop

These distance features are then passed to the Logistic Regression model.
The model will then spit out the most likely horizontal and vertical directions for the AI's next input.
