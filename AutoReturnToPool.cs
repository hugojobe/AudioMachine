using UnityEngine;

public class AutoReturnToPool : MonoBehaviour
{
    public AudioSource source;

    public void Initialize(AudioSource source) {
        this.source = source;
        Invoke(nameof(ReturnToPool), source.clip.length);
    }
    
    private void ReturnToPool() {
        AudioManager.StopSfx(source);
        source.gameObject.name = string.Format(AudioManager.SFX_GAMEOBJECT_NAME, "POOL");
    }
}
