# Teesside-AR-Darts
Third-year university AR Module

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/wNMX7QHwrc0/0.jpg)](https://www.youtube.com/watch?v=wNMX7QHwrc0)



Introduction

Teesside-Darts-AR is an attempt to combine the skill needed for traditional darts with the technology of Augmented Reality. Hanging up a full-size dartboard isn’t always an option due to space or budget constraints. Having the game installed on a phone allows the user to learn, practise and hone their darts abilities through a choice of game modes.  
The five single-player game modes are: Traditional 501 and 301 scoring games, Around the Clock and Cricket for accuracy training and finally, a Maths Quiz, to test the player’s arithmetic ability. Each mode has been developed to improve aspects of a player’s technique, from hand-eye coordination to sharpening numeric scoring. 
Blending the technology of AR Foundation and Unity’s 3D Engine, the game can detect flat, vertical surfaces, provided they have enough texture and variation for the plane to track.  
Whether you’re a beginner learning the game of darts, a seasoned pro looking to combine technology with their passion on the go, or an educator wanting to make mathematics a little more fun, Teesside-AR-Darts provides a portable and convenient alternative to physically using a board.




Technical Implementation

Development Environment:
Unity3D (2001.3.10f1)
AR Core (4.2.7)
AR Foundation (4.2.7)
TextMeshPro (3.0.6)
C#

Assets Used:
All assets are included in AR Dart Game - Augmented Reality Game (Tofaani) from the Unity Asset Store and are used under appropriate licence.
Mark.PNG – Indicator for dartboard placement.
DartboardPrefab – A dartboard with individual tiles (which have been later altered to be hit markers used in point scoring).
Dart – a blue dart model.
AR Feathered Plane and script – used for surface detection.
SampleSound.WAV – Used when a dart successfully hits the board.
wood.PNG – Dartboard material used for the cork effect.

Architecture Patterns:
For maintainability, the Strategy Pattern has been implemented with a base GameMode and interface to allow for the dynamic addition of future game modes. This encapsulation of game modes allows for a reduction in code duplication.






Features and Functionality

Game Modes:
Traditional Scoring (501/301)
Classic dart scoring starting from 501 or 301, Double-out requirement for finishing, three darts per turn, maximum throws: 50 (501) or 35 (301), "Bust" rule when score drops below zero, turn-based gameplay with automatic scoring.

Around the Clock
Sequential progression through numbers 1-20, any multiplier counts as a valid hit, maximum 30 throws to complete, progress tracking display, perfect for accuracy training.

Cricket
Strategic gameplay targeting numbers 15-20 and bullseye, three marks needed to "close" a number, point scoring system, maximum 40 throws, visual indicators for closed numbers.

Maths Quiz
Educational mode combining darts with math, ten possible questions, score points by hitting correct answers.
Gameplay Elements
Touch-based dart throwing, physics-based trajectories, accurate hit detection, multiple dart colours, sound effects for feedback.

User Interface
Clean, intuitive menus, real-time score updates, throw counter, game state messages, easy mode selection, restart functionality.

User Guide

Download and install the game on your Android phone (Oreo 8.0 or newer) and allow for installation from unknown sources.
Enable camera permissions, with the camera facing down, choose the game mode you’d like to play.
Aim the camera at a well-lit wall (preferably not featureless and white) and walk back slowly until the entire wall is covered by marker dots. Use the placement marker to place the dartboard.
To shoot a dart, aim at the dartboard and press the dart which will appear on the screen. An audio clip will play when successfully hitting a number (hitting the wire, another dart or missing the board won’t trigger the audio).
Play the game until a message will display congratulating the player on a win, or that they have ran out of darts. They may then decide to press the restart and/or then exit game buttons.
Future Enhancements

There is potential for a multiplayer mode as the foundations are already in place with the three darts per turn mechanics.
High Score Leaderboards would add to repeatability, especially if combined with online multiplayer mode. A championship with reward-based badge system could be one form of multiplayer mode.
Additional game modes may easily be added due to the architecture used. These could utilise a countdown timer or further expand on new educational modes. 


Built using Unity3D (2021.3.10f1)

All assets are from: AR Dart Game - Augmented Reality Game (Tofaani)

The instructions provided to get the board up and running didn't work, so it was decided to learn from official documentation and courses on Packt.
The scripts that were made available in the Asset package were used as a base and have all been heavily modified to implement additional features.


To get the dartboard scores correctly, there are several options. A series of rings could have been used from the circumference of the board, but the numbers would change if the board wasn't straight.
Instead, a better method is to map the hit areas and use a colour like hot pink to mark where these show up in-game. It was originally thought that the hit areas corresponded to the actual board numbers (1 is single 1, 2 is single 2), but as can be seen from manually mapping the hit areas this isn't the case.

Inner Singles:

1 was wood is single 1

2 was black is single 18

3 was wood is single 4

4 was black is single 13

5 was wood is single 6

6 was black is single 10

7 was wood is single 15

8 was black and is single 2

9 was black and is single 3

10 was wood and is single 19

11 was black and is single 7

12 was wood and is single 16

13 was black and is single 8

14 was wood and is single 11

15 was black and is single 14

16 was wood and is single 9

17 was black and is single 12

18 was wood and is single 5

79 was wood and is single 17

Ring was black and is single 20


Outer Single:

19 was black and is single 20

20 was wood and is single 1

21 was black and is single 18

22 was wood and is single 4

23 was black and is single 13

24 was wood and is single 6

25 was black and is single 10

26 was wood and is single 15

27 was black and is single 2

28 was wood and is single 17

29 was black and is single 3

30 was wood and is single 19

31 was black and is single 7

32 was wood and is single 16

33 was black and is single 8

34 was wood and is single 11

35 was black and is single 14

36 was wood and is single 9

37 was black and is single 12

38 was wood and is single 5


Triple Ring:

39 was red and is triple 20

40 was green and is triple 1

41 was red and is triple 18

42 was green and is triple 4

43 was red and is triple 13

44 was green and is triple 6

45 was red and is triple 10

46 was green and is triple 15

47 was red and is triple 2

48 was green and is triple 17

49 was red and is triple 3

50 was green and is triple 19

51 was red and is triple 7

52 was green and is triple 16

53 was red and is triple 8

54 was green and is triple 11

55 was red and is triple 14

56 was green and is triple 9

57 was red and is triple 12

58 was green and is triple 5


Double Ring:

59 was red and is double 20

60 was green and is double 1

61 was red and is double 18

62 was green and is double 4

63 was red and is double 13

64 was green and is double 6

65 was red and is double 10

66 was green and is double 15

67 was red and is double 2

68 was green and is double 17

69 was red and is double 3

70 was green and is double 19

71 was red and is double 7

72 was green and is double 16

73 was red and is double 8

74 was green and is double 11

75 was red and is double 14

76 was green and is double 9

77 was red and is double 12

78 was green and is double 5


Bulls:

Ring.007 was green and is the outer bull

Ring.008 was red and is the inner bull







The game was mostly built and tested while in the hospital for various procedures, and proved very entertaining.

  Click on the thumbnail to view the Demonstration video

[![YouTube Video Thumbnail](https://img.youtube.com/vi/wNMX7QHwrc0/0.jpg)](https://www.youtube.com/shorts/wNMX7QHwrc0)
