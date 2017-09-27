# impostor
Psychological tactics and deception. 'imposter' is a straightforward approach to a multiplayer shooter.

## AI Controller Model

My first attempt at creating an AI Controller resulted in some undesirable effects. I used simple randomizing techniques in order for the AI to randomly roam across the map. In theory, this seemed like the quick and simple solution to having AI. 

However, in practice I found that my friends were easily able to tell which character is an AI and which was a *real player* controlling an imposter. This essentially defeated the purpose of the game. As such, I needed a way for the AI to mimic what a real player does.

The solution was machine learning. 

At any given point in time, we want our AI Controller to choose the most appropriate horizontal and vertical input to simulate a player pressing a combination of WASD. This can be done by calculating features that represent what a player may be thinking about to determine their next move.

### Logistic Regression Approach

At a fixed `limitRate` interval, we calculate the following features:

* X position on map
* Y position on map
* Distance from center
* X Distance from nearest opponent
* Y Distance from nearest oppoenent
* Is the ai in motion?
* Time since last direction change
* Time since last full start or stop

These distance features are passed to the Logistic Regression classifier which will then spit out the most likely horizontal and vertical directions for the AI's next input.

However after training the classifier on ~5000 training examples (and fine tuning along the way) the resulting model had a very ***high bias***. It was simply unable to capture the important (and non-linear) connections between the features. 

Below is an example of the Logistic Regression classifier failing to make a seemingly normal player decision (avoiding an opponent close to them).

![Logistic Regression Failure](https://media.giphy.com/media/DjqyuC199ZjJ6/giphy.gif)


### Convolutional Neural Network Approach

Convolutional Neural Networks have traditionally performed *very well* on classifying images into multiple types. The input into the first layer, the convolutional layer, would typically be an NxMxC matrix. Where NxM is the dimensions of the image, and C is the number of channels in the image (3 for RGB). 

It wasn't long before I could draw some parallels between image classification and the problem with Impostor's AI. Since my game is a topdown shooter, I can represent each "map state" as a matrix. The bounds of the map itself are fixed, but the contents within are always changing. Thus the map state can be represented as multiple NxM matrices of objects, where NxM is the limits of the map.

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

Now, we have some useful features that should be able to help our AI controller choose an appropriate direction for a given "map state". All that is left is to gather some Real Player data for supervised learning! 

*To be continued...*
