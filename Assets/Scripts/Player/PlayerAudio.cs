using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This class should hold references to [count] audioInstances
 * all instances are originally stored in soundQueue, where the
 * instances that most recently held sound are put at the end of the queue
 * when a new PlayOnce sound is added, the sound is played on the first free instance, and
 * the queue is resorted so any busy audioinstances that were read are put at the end
 * when a new PlayLoop sound is added, it is removed from the queue and added to the loopSounds list
 * until the loop is told to end
 * 
 */
public class PlayerAudio
{
    AudioInstance oneshotPlayer;
    Queue<AudioInstance> soundQueue;
    AudioInstance[] loopSounds;
    int count;
    float soundScale;

    public PlayerAudio(AudioSource[] _sourceArr, float _soundScale = 1)
    {
        soundScale = _soundScale;
        //push forward the sources into instances
        count = _sourceArr.Length;
        soundQueue = new Queue<AudioInstance>();
        loopSounds = new AudioInstance[count];

        oneshotPlayer = new AudioInstance(_sourceArr[0]);
        for (int n = 1; n < count; n++)
        {
            soundQueue.Enqueue(new AudioInstance(_sourceArr[n]));
            loopSounds[n] = null;
        }
    }

    /// <summary>
    /// Plays sound clip on first available source
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="soundLength">time (in seconds) to play clip. leave blank for length of clip</param>
    /// <param name="volume">0.0-0.1 volume of sound</param>
    /// <param name="delay">delay before audio starts in seconds</param>
    /// <returns>true if successful, false if unsucecessful</returns>
    public bool PlayOnce(AudioClip clip, float volume = 0.5f)
    {
        volume = Mathf.Clamp(volume, 0, 1) * soundScale;
        oneshotPlayer.PlayOnce(clip, volume);
        return false;
    }
    /// <summary>
    /// Plays loop on first available source
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume">0.0-1.0 sound scale for clip</param>
    /// <returns>-1 if unsuccessful, index of the clip if successful</returns>
    public int PlayLoop(AudioClip clip, float volume = 0.5f, float delay = 0)
    {
        volume = Mathf.Clamp(volume, 0, 1) * soundScale;
        for (int n = 0; n < soundQueue.Count; n++)
        {
            AudioInstance snd = soundQueue.Dequeue();
            if (snd.IsFree())
            {
                for (int j = 0; j < count; j++)
                {
                    if (loopSounds[j] == null)
                    {
                        snd.PlayLoop(clip, volume, delay);
                        loopSounds[j] = snd;
                        return j;
                    }
                }
                soundQueue.Enqueue(snd);
                return -1;
            }
            else
            {
                soundQueue.Enqueue(snd);
            }
        }
        return -1;
    }
    /// <summary>
    /// Stops the loop with specified index
    /// </summary>
    /// <param name="index"></param>
    public void StopLoop(int index)
    {
        AudioInstance snd = loopSounds[index];
        snd.StopLoop();
        loopSounds[index] = null;
        soundQueue.Enqueue(snd);
    }
}

public class AudioInstance
{
    AudioSource source;
    bool isLooping = false; //if the source is indefinitely in use

    public AudioInstance(AudioSource _source)
    {
        source = _source;
        isLooping = false;
    }
    /// <summary>
    /// Plays the specified clip for specified time
    /// </summary>
    /// <param name="clip">clip to play</param>
    /// <param name="soundLength">time it should take to finish</param>
    /// <param name="volume">0-1 scale applied to volume of clip</param>
    /// <param name="delay">time in seconds to delay start of clip</param>
    public void PlayOnce(AudioClip clip, float volume)
    {
        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        source.PlayOneShot(clip);
        isLooping = false;

    }
    /// <summary>
    /// Plays the specified clip until told to stop
    /// </summary>
    /// <param name="clip">clip to play</param>
    public void PlayLoop(AudioClip clip, float volume, float delay)
    {
        source.loop = true;
        source.clip = clip;
        source.volume = volume;
        source.PlayDelayed(delay);
        isLooping = true;
    }
    /// <summary>
    /// stops the current loop
    /// </summary>
    public void StopLoop()
    {
        source.Stop();
        isLooping = false;
    }

    //getter/setter functions
    /// <summary>
    /// returns whether or not the source is free to use
    /// </summary>
    /// <returns></returns>
    public bool IsFree()
    {
        return !isLooping;
    }

}