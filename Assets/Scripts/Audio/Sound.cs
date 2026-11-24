namespace Ravernt.Audio
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Utility/Sound")]
    public class Sound : ScriptableObject
    {
        public string soundName;
        public AudioClip[] soundClips;
        public float minPitch = 0.85f;
        public float maxPitch = 1.15f;
        public float volume = 0.6f;
        public float cooldown = 0.05f;
        public float delay = 0;
        [HideInInspector] public float lastTimePlayed = 0;
    }
}
