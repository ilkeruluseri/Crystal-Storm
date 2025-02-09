using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceScript : MonoBehaviour
{
    [SerializeField] private int increaseAmount = 1;
    [SerializeField] private int hitPoints = 10;
    [SerializeField] ParticleSystem particles;
    [SerializeField] AudioSource audioSource;
    Inventory inventory;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    public int GetIncrease()
    {
        return increaseAmount;
    }

    public bool isCollecting = false;
    public IEnumerator CollectAnimation()
    {
        isCollecting = true;

        particles.Play();

        float duration = 0.5f; // Total duration of the animation
        float maxScale = 1.2f; // Scale factor for the pop effect
        Vector3 originalScale = transform.localScale;

        // Scale up
        float elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(1f, maxScale, elapsedTime / (duration / 2));
            transform.localScale = originalScale * scale;
            yield return null;
        }

        // Scale down
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(maxScale, 1f, elapsedTime / (duration / 2));
            transform.localScale = originalScale * scale;
            yield return null;
        }

        hitPoints--;
        if (hitPoints <= 0)
        {
            audioSource.Play();
            yield return new WaitForSeconds(0.25f);
            var main = particles.main;
            main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.08f);
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2f);
            particles.Play();
            yield return new WaitForSeconds(0.25f);
            if (gameObject.CompareTag("Resource2"))
            {
                inventory.RemoveFromCrystals(gameObject);
            }
            Destroy(gameObject);
        }

        isCollecting = false;
    }
}
