using UnityEngine;

namespace Again.Runtime.Components.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public AudioSource audioSource;

        public void PlayAudio(string audioName)
        {
            var audioClip = Resources.Load<AudioClip>($"audio/{audioName}");
            audioSource.PlayOneShot(audioClip);
        }
    }
}