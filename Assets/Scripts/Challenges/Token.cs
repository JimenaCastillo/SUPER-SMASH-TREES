using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    [SerializeField] private TextMeshPro valueText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public int value { get; private set; }
    private float fallSpeed;
    private bool isCollected = false;
    private bool isDestroyed = false;
    

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

            // Si el token cae fuera de la pantalla
            if (transform.position.y < -6f)
            {
                TokenSpawner spawner = FindObjectOfType<TokenSpawner>();
                if (spawner != null)
                    spawner.RemoveToken(this);

                SafeDestroy();
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

            // 1. Notifica al ChallengeManager para insertar el valor
            ChallengeManager.Instance.CollectToken(player.GetPlayerIndex(), value);

            // 2. Da feedback visual
            StartCoroutine(CollectAnimation(player.transform));
        }
    }

    private IEnumerator CollectAnimation(Transform target)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, elapsed / duration);
            spriteRenderer.color = new Color(1, 1, 1, 1 - (elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3. Quita el token de la lista activa y destruye el objeto
        TokenSpawner spawner = FindObjectOfType<TokenSpawner>();
        if (spawner != null)
            spawner.RemoveToken(this);

        SafeDestroy();
    }

    private void SafeDestroy()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        spriteRenderer.enabled = false;
        if (valueText != null) valueText.enabled = false;
        gameObject.SetActive(false);

        Destroy(gameObject); // Elimina el objeto después
    }
}
