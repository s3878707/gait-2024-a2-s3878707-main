# Games and Artificial Intelligence Techniques<br>COSC2527 / COSC2528<br>Assignment 2
**Student ID:** \*\*\*s3878707\*\*\*

This is the README file for Assignment 2 in Games and Artificial Intelligence Techniques.

Notes:
* The starter project was created in Unity 2022.3.19f1. Please use the same Unity version for your submission.
* Please do not edit the contents of the .gitignore file.

Enhance MCTS Agent:

First attempt: Initially, I adjusted the exploration constant, setting it to the square root of 2, which led to improved performance for the TunedMCTSAgent. With this change, the agent achieved a record of 10 wins out of 2 matches.

Second attempt: In the TunedMCTSAgent, I implemented a strategy to terminate simulations early. This involved checking the result of the simulation if it was not undecided when the number of possible moves was reduced to 3, allowing the agent to explore more possibilities without increasing runtime cost too much. With this enhancement, the TunedMCTSAgent achieved a record of 10 wins out of 3 matches. To ensure efficiency, both of the prefab agents should be turned on the simulation time ms and c should be set to 0.5.
