# AudioMachine
A versatile and modular audio management system for Unity, supporting sound effects and music with smooth transitions. This script allows for easy integration of both music and sound effects, using an efficient pooling system for sound effects and a crossfade mechanism for music to ensure smooth transitions

## Features
- Static Methods for Audio Control: Easily play or stop music and sound effects using static methods.
- Crossfade Music: Automatically crossfade between music tracks on the same channel for smooth transitions.
- Audio Mixer Integration: Integrate with Unity's Audio Mixer to control volumes independently for music and sound effects.
- Object Pooling for SFX: Utilizes a pooling system to reuse sound effect GameObjects, improving performance by minimizing the creation and destruction of objects.

## Requirements
- [DOTween](https://dotween.demigiant.com/) for customizable and smooth fade transitions.

## Installation
- Clone or download the repository.
- Add the `AudioManager.cs` and `AutoReturnToPool.cs` scripts to your Unity project.
- Create Audio Mixer Groups for music and sfx, and place the Audio Mixer in the Ressources folder.
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

## Usage
### Playing Sound Effects
To play a sound effect by name, ensure that the audio clip is placed under the folder defined by `SFX_PATH`.</br>
``` C#
AudioManager.PlaySFX("<sfx name>");
```
Alternatively, if you already have an AudioClip reference:
``` C#
AudioManager.PlaySFX(clip);
```

### Playing Music
To play a music track by name on a specific channel, ensure that the audio clip is placed under the folderdefined by `MUSIC_PATH`.</br>
``` C#
AudioManager.PlayMusic("Background", "MainTheme");
```
