# Mi Camino

## Overview

Mi Camino is a 2D educational adventure game designed to help players learn and practise Spanish through interactive conversations, exploration, and everyday scenarios.

The project originally began at Georgia State University and is still in active development.

Players explore the fictional town of San Paloma, interact with characters, complete objectives, and use Spanish in situations such as introducing themselves, asking for directions, and visiting a restaurant.

## Features

- **Town exploration:** Explore a 2D environment and interact with different characters.
- **Interactive dialogue:** Choose responses in Spanish and experience branching conversations.
- **Fluency Points:** Earn points by selecting correct responses during conversations.
- **Audio and pronunciation:** Listen to spoken dialogue with the option to slow down audio playback.
- **Vocabulary assistance:** Hover over selected Spanish words and phrases to view their meanings.
- **Journal system:** Review previously learned Spanish phrases and their English translations.
- **Objective system:** Follow objectives and track progress throughout the game.
- **Save and load system:** Save and restore supported player progress locally.
- **Interactive puzzles:** Complete puzzles as part of the game's story.

## Technologies Used

| Technology | Purpose |
|---|---|
| C# | Gameplay programming |
| Unity 6 | Game development |
| ScriptableObjects | Dialogue and game data management |
| Unity Input System | Player controls |
| TextMesh Pro | Dialogue and user interfaces |
| Cinemachine | Camera management |
| Unity Timeline | Cutscenes |
| JSON | Save and load functionality |
| Git & GitHub | Version control and team collaboration |

## My Contributions

As the lead programmer, my primary responsibility is developing and maintaining the core gameplay systems.

My contributions include:

- Developing the NPC interaction and branching dialogue systems.
- Implementing dialogue choices, response feedback, and Fluency Points.
- Developing vocabulary assistance and dialogue audio controls.
- Building the objective management system.
- Developing the journal system to store learned Spanish phrases.
- Implementing game progression across different scenes.
- Developing the local JSON save and load system.
- Integrating gameplay systems with user interfaces and other team-developed features.
- Collaborating with team members to integrate gameplay, artwork, and level design.

## Game Architecture

The game is organized into separate systems that handle dialogue, player interactions, objectives, progression, and saving.

Character conversations are stored using ScriptableObjects, allowing dialogue content to be created and updated without directly modifying the gameplay scripts.

A shared progression system manages information such as the player's current objective, Fluency Points, learned phrases, and module progress.

The save system stores supported player progress in a local JSON file, allowing players to continue from previously saved progress.

## Getting Started

The project uses Unity 6, version `6000.0.47f1`.

1. Install Unity Hub and the required Unity Editor version.
2. Clone the repository:

```bash
git clone https://github.com/Christi1280/TeamLinguist.git
```

3. Add the project folder through Unity Hub.
4. Open the project and allow Unity to import the required assets and packages.
5. Open `Assets/Scenes/MainHub.unity`.
6. Press Play in the Unity Editor.

## Development Status

Mi Camino is currently in active development.

The current version includes town exploration, interactive conversations, vocabulary assistance, objectives, journal functionality, and restaurant-based learning scenarios.

Further development will focus on:

- Expanding the game's story and learning scenarios.
- Adding more Spanish vocabulary and conversations.
- Improving existing gameplay systems.
- Expanding save and load functionality.
- Improving artwork, audio, and the overall player experience.
- Testing and preparing the game for release.

## Team and Credits

Mi Camino is a collaborative project that originally began at Georgia State University.

The project brings together programming, game design, level design, artwork, and educational content from multiple contributors.

**Christian  — Lead Programmer**

**Donovan - Programmer**

**Ian  — Game Artist**

**Joshua - Game Artist**

## Third-Party Assets

The project incorporates third-party assets and packages that remain subject to their respective licenses and copyright terms.
