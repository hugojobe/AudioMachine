# AudioMachine
The AudioManager script is a robust solution for managing audio in Unity projects. It allows for easy playback, control, and pooling of sound effects (SFX) and background music, with additional support for crossfading between audio tracks. The script is designed to work seamlessly with Unity's Audio Mixers, and it integrates with a pooling system to optimize performance by reusing SFX objects.

## Table of Contents
1. [Features](#features)
2. [Requirements](#requirements)
3. [Installation](#installation)
4. [How to Use](#how-to-use)
   - [Setup](#setup)
   - [Playing Sound Effects](#playing-sound-effects)
   - [Playing Music](#playing-music)
   - [Stopping Audio](#stopping-audio)
   - [Object Pooling](#object-pooling)
5. [Notes](#notes)
6. [License](#license)

## Features
- Background Music Control: Play, stop, crossfade, and loop background music across different channels.
- Sound Effects Management: Play sound effects with optional volume and pitch control. Efficiently uses an object pool to manage SFX instances.
- Crossfading: Smoothly fades in/out music when switching between tracks.
- Pooling System: Reuses audio sources to reduce runtime allocation and improve performance.

## Requirements
- [DOTween](https://dotween.demigiant.com/) for customizable and smooth fade transitions.
- Audio Mixers: Ensure you have audio mixer groups set up in Unity for music and sound effects.

## Installation
- Clone or download the repository.
- Add the `AudioManager.cs` and `AutoReturnToPool.cs` scripts to your Unity project.
- Create Audio Mixer Groups for music and sfx, and place the Audio Mixer in the Ressources folder.
- Create a folder named Audio in the Resources folder, with subfolders Musics and SFXs to store your audio files. (Optional, you may need to change variables in `AudioManager.cs`
- If needed, modify the variables in `AudioManager.cs`.
``` C#
private const string AUDIO_MIXER_PATH = "Audio/Main";  // Path to the main audio mixer
private const string MUSIC_PATH = "Audio/Musics/";     // Path to the musics
private const string SFX_PATH = "Audio/SFXs/";         // Path to the sfxs
private const string AUDIO_MIXER_MUSIC = "Music";      // Name of the music mixer group
private const string AUDIO_MIXER_SFX = "Sfx";          // Name of the sfx mixer group

private const string MUSIC_GAMEOBJECT_NAME = "Music - [{0}]";  // {0} = Music channel id
public const string SFX_GAMEOBJECT_NAME = "SFX - [{0}]";       // {0} = SFX name
```

## How to Use
## Setup
Attach the AudioManager script to a GameObject in your scene (e.g., an AudioManager GameObject).<br>
I recommand to setup this gameobject in the first scene of your project, as the AudioManager has the flag `DontDestroyOnLoad` for persistent audio control across scenes.

### Playing Sound Effects
``` C#
AudioManager.PlaySfx("sfxName");
```
Returns an AudioSource. </br>
**Parameters:**
- `sfxName` (string, **required**): The name of the SFX clip to play.
- `volume` (float, optional): The volume of the sound effect. Default is `1f`.
- `pitch` (float, optional): The pitch of the sound effect. Default is `1f`.

Alternatively, you can also play an SFX clip from a reference :
``` C#
AudioManager.PlaySfx(clip);
```
Returns an AudioSource. </br>

### Playing Music
``` C#
AudioManager.PlayMusic("trackName");
```
- PlayMusic: Plays a music clip from the Resources/Audio/Musics/ path on the specified channel.
- Supports crossfading if there is a track already playing on the channel.
- 
**Parameters:**
- `musicName` (string, **required**): The name of the music clip to play.
- `channel`(int, **required**): The channel on which to play the music.
- `volume`(float, optional): The volume of the music. Default is `1f`.
- `pitch` (float, optional): The pitch of the music. Default is `1f`.
- `loop` (bool, optional): Whether the music should loop. Default is `true`.
- `instant`(bool, optional): Whether to play the music instantly without fading. Default is `false`.
- `fadeDuration`(float, optional): Duration of the crossfade in seconds. Default is `1f`.

### Stopping Audio
- **StopSfx:** Stops a specific sound effect and returns it to the pool.
  ``` C#
  AudioManager.StopSfx(audioSource);
  ```
  **Parameters:**
  - `source`(AudioSource, **required**): The audio source to stop.
  
- **StopMusic:** Stops the currently playing music on a given channel.
  ``` C#
  AudioManager.StopMusic(channel);
  ```
  **Parameters:**
  - `channel` (int, **required**): The channel on which to stop the music.
  - `instant` (bool, optional): Whether to stop the music instantly without fading. Default is `false`.
  - `fadeDuration` (float, optional): Duration of the fade out in seconds. Default is `1f`.
    
## Object Pooling
The `AutoReturnToPool` script ensures SFX objects are returned to the pool after their clip has finished playing, reducing the need for frequent allocations.

## Notes
- Ensure all audio files are correctly named and placed under their respective folders (Musics and SFXs) for the AudioManager to find them.
- The script uses DOTween to smoothly handle volume changes during fades. Make sure DOTween is installed in your project.

## License
This script is licensed under the MIT License. You are free to use, modify, and distribute it as needed.




  
