using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Ravernt.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] Sound[] soundEffects;
        [SerializeField] AudioMixerGroup setGroup;
        [SerializeField] AudioSource audioSourcePrefab;

        static Sound[] sounds;
        static AudioSource audioSourcePrefabStatic;
        public static AudioSource currentMusic;
        static MonoBehaviour source;
        Coroutine coroutine;
        static AudioMixerGroup group;

        void Awake()
        {
            sounds = soundEffects;
            group = setGroup;
            audioSourcePrefabStatic = audioSourcePrefab;
            source = this;

            if (coroutine != null)
                StopCoroutine(coroutine);

            if (currentMusic != null)
            {
                currentMusic.pitch = 1f;
                currentMusic.volume = 0.25f;
            }

            foreach (Sound sound in sounds)
                sound.lastTimePlayed = 0;
        }

        public static AudioSource PlaySound(string soundName, bool isLooping = false)
        {
            if (TryFindSound(soundName, out Sound sound))
            {
                AudioSource audioSource = Play(sound, isLooping, sound.delay);
                Transform audioTransform = audioSource.transform;
                audioTransform.parent = Camera.main.transform;//not really good, but  ¯\_:)_/¯
                audioTransform.localPosition = Vector3.zero;
                return audioSource;
            }

            return null;
        }

        public static AudioSource PlaySoundOnObject(string soundName, Transform transform, bool isLooping = false)
        {
            if (TryFindSound(soundName, out Sound sound))
            {
                AudioSource audioSource = Play(sound, isLooping, sound.delay);
                Transform audioTransform = audioSource.transform;
                audioTransform.parent = transform;
                audioTransform.localPosition = Vector3.zero;
                return audioSource;
            }

            return null;
        }

        private static bool TryFindSound(string soundName, out Sound sound)
        {
            sound = null;
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].soundName == soundName)
                {
                    if (CanPlay(sounds[i]))
                    {
                        sound = sounds[i];
                        return true;
                    }
                    return false;
                }
            }

            Debug.LogError("Couldn't find '" + sound + "' sound");
            return false;
        }

        static AudioSource Play(Sound sound, bool isLoop = false, float delay = 0)
        {
            AudioSource audioSource = Instantiate(audioSourcePrefabStatic);
            audioSource.clip = sound.soundClips[Random.Range(0, sound.soundClips.Length)];
            audioSource.pitch = Random.Range(sound.minPitch, sound.maxPitch);
            audioSource.volume = sound.volume;
            audioSource.loop = isLoop;
            audioSource.outputAudioMixerGroup = group;

            if (delay > 0)
                audioSource.PlayDelayed(delay);
            else
                audioSource.Play();

            if (!isLoop)
            {
                Destroy(audioSource.gameObject, audioSource.clip.length);
            }

            return audioSource;
        }

        static bool CanPlay(Sound sound)
        {
            if (sound.cooldown + sound.lastTimePlayed < Time.time)
            {
                sound.lastTimePlayed = Time.time;
                return true;
            }
            else
                return false;
        }

        public void AddSoundEffects(List<Sound> sounds)
        {
            soundEffects = new Sound[sounds.Count];

            for (int i = 0; i < sounds.Count; i++)
                soundEffects[i] = sounds[i];
        }
    }
}
