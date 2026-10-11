using System.Collections;
using UnityEngine;

// Rotates the lever handle down when a spin starts and back up when the reels stop.
public class LeverAnimator : MonoBehaviour
{
    [SerializeField] private float pullAngle = -45f;   // Angle the lever swings to when pulled
    [SerializeField] private float duration = 0.25f;   // Seconds for each swing

    private Quaternion restRotation;
    [SerializeField] private float restAngle = 0f;     // Z angle when the lever is upright

    private void Awake()
    {
        restRotation = Quaternion.Euler(0, 0, restAngle);
        transform.localRotation = restRotation;
    }

    // Swing down. Called by SlotMachine at the start of a spin.
    public IEnumerator Pull()
    {
        yield return Rotate(Quaternion.Euler(pullAngle, 0, 0) * restRotation);
    }

    // Swing back up. Called by SlotMachine after the reels stop.
    public IEnumerator Release()
    {
        yield return Rotate(restRotation);
    }

    // Smoothly rotates from the current angle to the target over 'duration' seconds.
    private IEnumerator Rotate(Quaternion target)
    {
        Quaternion start = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(start, target, elapsed / duration);
            yield return null;
        }

        transform.localRotation = target;
    }
}