# 2D Creature Battle Prototype

## Overview
This is a prototype designed to showcase a turn-based combat system and architecture inspired by classic RPG mechanics. Not meant to represent final visuals, polish, or scope.

The core gameplay loop is simple: 
Walk among the pointy trees -> trigger a battle -> fight.

Included systems: Turn-based combat system, HUD and dialogue UI, save/load system, and basic 2D overworld animations and movement.

## Features
- Turn-based battle system with player controls and simple enemy AI (random moves).
- Creature leveling and move system via XP.
- Creatures and moves are driven by Unity's ScriptableObjects. 
- Tilemap-based overworld with simple player movement.
- Basic UI including health bars, move and creature selection, and dialogue.
- Seamless transition between overworld and battle scenes.

## Tech

- Unity Version 6000.0.54f1
- C#
- ScriptableObjects
- Unity Tilemap, Sprite animation

## Project Structure
- *Scripts/* - Contains all the scripts for the project, with each subfolder pertaining to a different area of the game.
- *Prefabs/* - Contains all the prefabs for the project.
- *Resources/* - Contains the ScriptableObject bases for Creatures, Moves, and EncounterTables.

## How to Play
//insert instructions here - TBD

### Controls:
Overword:
- WASD - Move
- H - Heal party.
- F - Save.
- Game will automatically load when launching.

Combat:
- Simply left click the action you wish to use, whether a move or swapping currently used Creature.

## Known Limitations and Issues
- Minimal and placeholder sound effects.
- Limited number of creatures and moves (2 creatures, 2 moves per creature as of 11/18/2025).
- No damage typing or elemental interactions.
- Enemy decision-making is simple (random choice).
- No creature-catching system.
- All visuals are placeholder, either self-made or third-party (see below).

## Credits
- Tileset textures from [Monochrome RPG](https://www.kenney.nl/assets/monochrome-rpg) by [Kenney](kenney.nl), licensed under [CC0](https://creativecommons.org/publicdomain/zero/1.0/).
- Overworld player textures from [Free TopDown RPG Retro Sprites!](https://elvgames.itch.io/free-retro-game-world-sprites) by [ElvGames](https://elvgames.itch.io/).
- All code, sounds, and creature textures created by me.