# Beer Pong Game - A00325134 - After Hours Arcade Beer Pong.

### VR Application Development 3 — Continuous Assessment

**Technological University of the Shannon**  
**Module:** GAME07008-39752 – VR Application Development 3  

---

## 1. Introduction

For this project I created a VR game called **After Hours Arcade** using Unity and the XR Interaction Toolkit.

The idea behind the game was to create a small arcade-style baseball throwing game. The player has to grab a baseball and throw it into a set of bucket-style cups. The aim is to clear all six cups and reach the winning score.

I wanted to make the game feel like an actual arcade activity instead of just having a ball that the player could pick up and throw. I added a TV scoreboard, sound effects, controller haptics, a ball holder, start and restart controls, and a trajectory prediction line to help the player aim.

I also tried to use techniques from the VR tutorials throughout the project, especially grabbable objects, sockets, direct and ray interaction, UI, audio and haptics.

## 2. Game Overview

The main objective is to get the baseball into all six cups. Each cup is worth one point, so the player needs to score six points to win.

The game starts with a message on the TV telling the player to press start. Once the game begins, the player can grab the baseball from its holder and start throwing at the cups.

When the ball successfully enters a cup, the player receives a point, the cup is removed from the game and the ball returns to its starting position. The player can then pick it up and try again.

I kept the objective simple because I wanted the main focus to be on the VR interactions and making the throwing mechanic work properly.

## 3. VR Setup and Interaction

I used the XR Interaction Toolkit to set up the VR interactions. The baseball uses an `XRGrabInteractable` and a Rigidbody, which allows the player to pick it up and throw it using the VR controller.

I also used a socket for the ball holder so that the baseball has a starting place between throws. This makes it easier for the player to find and pick up the ball again after a missed throw or a successful score.

I used direct interaction for grabbing the baseball and ray interaction for the UI controls. This lets the player interact with the game without needing to use a keyboard or mouse.

I added start and restart controls that call the relevant functions in my score manager. The scoreboard is displayed on the TV in the arcade environment, so the player can see their score and the current game message while playing.

## 4. Game State and Scoring System

I created a script called `TossScoreManager` to control the score and the different states of the game.

I used three game states:

- `Ready`
- `Playing`
- `Won`

When the game first starts, it is in the Ready state and the scoreboard displays "PRESS START TO PLAY!". When the player starts the game, it changes to Playing and displays "GAME STARTED - THROW!".

I set the winning score to six because there are six cups. Each cup gives the player one point when it is hit successfully.

When the score reaches six, the game changes to the Won state. The scoreboard displays "YOU WIN!" and the victory sound plays. I also made sure the score cannot go above the winning score.

I added a restart function that resets the score, restores all the cups and returns the baseball to its starting position. This means the player can play again without having to reload the whole scene.


## 5. Cup Detection and Scoring

I created a separate script called `CupTarget` for the cups. Each cup has a trigger collider, which detects when the baseball enters it.

When the trigger detects the ball, the script checks that the object is actually the throwable baseball and that the game is currently being played. It then asks the ball if that throw has already scored before adding the points.

I added this check because I did not want one throw to accidentally score more than once. I also used a `hasBeenHit` variable so that a cup cannot be scored again after it has already been cleared.

Once the score is accepted, the cup plays its hit sound, sends haptic feedback to the controller and is removed from play. The ball is also sent back to its starting position.


## 6. Baseball Reset System

I created a `ThrowableBall` script to handle the grabbing, throwing and resetting of the baseball.

One feature I added was an automatic reset for missed throws. When the player releases the ball, a three-second countdown begins. If the ball has not scored and the player has not picked it up again, it returns to the holder.

When the ball successfully scores, it uses a shorter one-second reset delay instead. I also made sure the reset does not happen while the player is still holding the ball.

When the ball resets, I clear its linear and angular velocity before moving it back to its original position and rotation. This stops it from continuing to move after being returned.

## 7. Trajectory Prediction

One of the extra features I added was a trajectory preview using a `LineRenderer`. I wanted to give the player some help with aiming before releasing the baseball.

I created a `TrajectoryPreview` script that tracks the movement of the ball while it is being held. It uses the change in position over time to estimate the ball's velocity. I added velocity smoothing so that the preview is not constantly jumping around due to small movements.

The script then uses the estimated velocity and Unity's gravity value to draw a predicted path. I set it to use 30 points over a prediction time of 1.2 seconds.

The calculation is based on the following equation:

```text
position = startPosition
         + startVelocity * time
         + 0.5 * gravity * time * time
```

The preview only appears while the player is holding and moving the ball. It disappears when the ball is released or when the player is not moving it quickly enough.

This was one of the main extra technical features I worked on because it uses vector calculations and gives the player useful feedback during gameplay.

One limitation is that the line is only a prediction of the ball's movement under gravity. It does not fully simulate collisions with the cups or other objects.


## 8. Audio and Haptic Feedback

I added audio to make the game feel more responsive. There is a sound when the game starts, a sound when a cup is hit and a separate victory sound when all six cups have been cleared.

I also added haptic feedback when the player successfully scores. The `ThrowableBall` script remembers the controller that last grabbed the ball and sends a haptic impulse when a cup is hit.

I set the haptic amplitude to 0.75 and the duration to 0.2 seconds. This gives the player a small vibration when they successfully land a throw.

## 9. Problems I Had and How I Fixed Them

One issue I had to deal with was making sure one throw could not score multiple times. I fixed this by adding a `scoredThisThrow` variable to the ball script. Once a score is registered, the same throw cannot register another one. I also added a separate check to each cup so that a cup cannot be scored twice.

Another issue was managing the ball reset after different types of throws. I wanted successful throws and missed throws to behave differently, so I used separate reset delays. I also added checks to stop the reset if the player grabs the ball again or is still holding it.

I also had to make sure the ball did not keep its old physics velocity after being moved back to the holder. I fixed this by setting its linear and angular velocity to zero before restoring its starting position and rotation.

## 10. Reflection and Conclusion

Overall, I learned a lot from creating After Hours Arcade. It gave me a chance to use the VR interaction techniques from the tutorials in a project of my own rather than just following a tutorial example.

The scoring system, ball reset behaviour and trajectory preview took the most work because I had to make sure the different scripts worked together properly. I am happy with the trajectory preview because it adds something useful to the throwing mechanic instead of just being a visual extra.

There are still improvements I could make, such as making the trajectory prediction more accurate when objects are in the way and adding more gameplay features. However, I think the current version gives the player a clear objective and useful feedback when playing.

I found this project useful because it helped me understand how to combine VR grabbing, physics, UI, audio, haptics and scripting into one working application.

---

## 11. Screenshots

The Full Enviroment:
<img width="2552" height="1352" alt="image of Enviroment" src="https://github.com/user-attachments/assets/aba4efbb-b438-49cf-a4b1-b722462a3727" />

Ball in its Holder:
<img width="1065" height="601" alt="Ball in Holder" src="https://github.com/user-attachments/assets/60d3ded2-fcaa-424f-9191-cbdd1bcf264f" />

Ball In Hand: 
<img width="569" height="462" alt="Ball in Hand" src="https://github.com/user-attachments/assets/21e3cc35-d0de-4d55-a554-89b99aad913f" />

Trajectory Line:
<img width="694" height="680" alt="TJL" src="https://github.com/user-attachments/assets/d0dc6df9-c488-4dcb-9363-a43bbd0b9a75" />

Cup Disappears After Ball enters:
<img width="380" height="256" alt="CupGone" src="https://github.com/user-attachments/assets/512098ea-a93f-49dd-abe1-ae58ac0e796c" />

Scoreboard:
<img width="1171" height="914" alt="SB" src="https://github.com/user-attachments/assets/8e0439ab-89a9-4c76-963f-4b9a7abfa39f" />

Win:
<img width="1277" height="879" alt="Win" src="https://github.com/user-attachments/assets/c153cd74-6394-45fe-9d37-22e06a9660a8" />






