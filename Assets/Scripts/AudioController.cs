using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    private const int SampleRate = 44100;
    private const float ClipDuration = 0.12f;

    private AudioSource source;
    private AudioClip hitClip;

    private void Start()
    {
        source = transform.GetComponent<AudioSource>();
        hitClip = CreateHitClip();
        source.playOnAwake = false;
        source.clip = hitClip;
    }

    public void PlayHit()
    {
        if (source == null)
        {
            return;
        }

        source.PlayOneShot(hitClip);
    }

    private AudioClip CreateHitClip()
    {
        int sampleCount = Mathf.CeilToInt(SampleRate * ClipDuration);
        float[] data = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float progress = (float)i / sampleCount;
            float frequency = Mathf.Lerp(880f, 220f, progress);
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * i / SampleRate);
            data[i] = wave * Mathf.Pow(1f - progress, 2f) * 0.4f;
        }

        AudioClip clip = AudioClip.Create("Hit", sampleCount, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
