using UnityEngine;

// Plays all sound effects. One AudioSource handles one-shot clips, and a second
// one handles the looping reel spin so the two never interrupt each other.
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip leverPull;
    [SerializeField] private AudioClip reelSpin;     // Must have Loop enabled in its import settings
    [SerializeField] private AudioClip reelStop;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip jackpot;
    [SerializeField] private AudioClip buttonPress;

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float loopVolume = 0.5f;

    private AudioSource sfxSource;    // One-shot sounds
    private AudioSource loopSource;   // Reel spin loop

    private void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.playOnAwake = false;
        loopSource.loop = true;
        loopSource.clip = reelSpin;
        loopSource.volume = loopVolume;
    }

    public void PlayLever() => PlayOneShot(leverPull);
    public void PlayReelStop() => PlayOneShot(reelStop);
    public void PlayWin() => PlayOneShot(win);
    public void PlayJackpot() => PlayOneShot(jackpot);
    public void PlayButton() => PlayOneShot(buttonPress);

    // Starts the spin loop if it isn't already playing.
    public void StartSpinLoop()
    {
        if (reelSpin == null || loopSource.isPlaying) return;
        loopSource.Play();
    }

    // Stops the spin loop immediately.
    public void StopSpinLoop()
    {
        loopSource.Stop();
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}