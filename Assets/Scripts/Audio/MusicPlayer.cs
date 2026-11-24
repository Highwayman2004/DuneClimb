namespace Ravernt.Audio
{
    using System.Collections;
    using UnityEngine;

    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip[] musicTracks;
        static MusicPlayer instance;

        AudioSource source;
        int currentTrack = 0;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            source = GetComponent<AudioSource>();
            StartCoroutine(Play());
        }

        IEnumerator Play()
        {
            while (true)
            {
                source.clip = musicTracks[currentTrack];
                source.Play();
                yield return new WaitUntil(() => !source.isPlaying);

                currentTrack++;
                if (currentTrack == musicTracks.Length)
                    currentTrack = 0;
            }
        }
    }
}
