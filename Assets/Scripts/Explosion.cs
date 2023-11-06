using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    
    public bool destroyOnFinish = false;
    private ParticleSystem[] _particles;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var system in _particles)
        {
            system.Stop();
        }
        gameObject.SetActive(false);
    }

    public void ExplodeAt(Vector3 position)
    {
        _audioSource.pitch = Time.timeScale;
        gameObject.SetActive(true);
        transform.position = position;
        float duration = 0;
        foreach (var system in _particles)
        {
            system.Play();
            duration = Mathf.Max(duration, system.main.duration);
        }
        _audioSource.PlayOneShot(clip);
        if(destroyOnFinish) Destroy(gameObject, duration);
    }
}