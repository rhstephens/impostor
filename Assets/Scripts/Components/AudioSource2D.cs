using UnityEngine;

/// <summary>
/// An audio source that has its volume managed by the inverse square law on the X and Y axis ONLY.
/// </summary>
public class AudioSource2D : MonoBehaviour {

    public AudioClip audioClip;
    public bool playOnAwake = true;

    // Anything as close as this distance can be heard at full volume
    public int minDistance = 3;

    // The max distance in which this audio source can no longer be heard
    public int maxDistance = 20;

    AudioSource _src;

	void Start () {
        _src = gameObject.AddComponent<AudioSource>();
        _src.clip = audioClip;
        _src.spatialBlend = 0;

        if (playOnAwake) {
            Play();
        }
	}

    // Play the audio source with 2D spatial blending
    public void Play() {
        float distance = GetListenerDistance();
        float scale = CalculateAudioScale(distance);

        Debug.Log("Playing at %" + (scale * 100).ToString() + " volume");
        PlayAt(scale);
    }

    // Play the audio source with no spatial blending (max volume no matter where the listener is)
    public void PlayMax() {
        _src.Play();
    }

    // Play at a given scale in the range [0, 1]
    public void PlayAt(float scale) {
        scale = Mathf.Clamp01(scale);
        _src.PlayOneShot(audioClip, scale);
    }

    // Returns the distance between the LocalPlayer's audio listener and this audio source
    float GetListenerDistance() {
        GameObject player = GameManager.Instance.GetLocalPlayer();
        return Vector3.Distance(player.transform.position, transform.position);
    }

    // Calculates audio intensity based off of distance from target (X and Y axis)
    float CalculateAudioScale(float distance) {
        Debug.Log(distance);
        if (distance >= maxDistance) {
            return 0f;
        }
        if (distance <= minDistance) {
            return 1f;
        }

        float ratio = 5f / (((Mathf.Pow(distance, 2)) - (minDistance ^ 2)) + 1f);
        return Mathf.Clamp01(ratio);
    }
}
