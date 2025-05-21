using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    [SerializeField] private TextMeshPro valueText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int value;
    private float fallSpeed;
    private bool isCollected = false;

    public void Initialize(int value, float fallSpeed)
    {
        this.value = value;
        this.fallSpeed = fallSpeed;
        valueText.text = value.ToString();
    }

    private void Update()
    {
        if (!isCollected)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Destroy if out of screen
            if (transform.position.y < -6f)
            {
                FindObjectOfType<TokenSpawner>().RemoveToken(this);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            isCollected = true;

            // Notify challenge manager
            ChallengeManager challengeManager = FindObjectOfType<ChallengeManager>();
            challengeManager.CollectToken(player.GetPlayerIndex(), value);

            // Visual feedback
            StartCoroutine(CollectAnimation(player.transform));
        }
    }

    private IEnumerator CollectAnimation(Transform target)
    {
        float duration = 0.5f;
        float elapsed = 0;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, elapsed / duration);
            spriteRenderer.color = new Color(1, 1, 1, 1 - (elapsed / duration));

            elapsed += Time.deltaTime;
            yield return null;
        }

        FindObjectOfType<TokenSpawner>().RemoveToken(this);
        Destroy(gameObject);
    }
}