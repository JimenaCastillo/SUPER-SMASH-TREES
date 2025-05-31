using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class TokenSpawner : MonoBehaviour
{
    [SerializeField] private GameObject tokenPrefab;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float spawnY = 6f;
    [SerializeField] private float fallSpeed = 2f;

    private List<Token> activeTokens = new List<Token>();
    private bool isSpawning = false;
    private Coroutine spawnRoutine;

    private void Start()
    {
        StartSpawning(); // Inicia el spawn
        SpawnToken(5);   // Genera un token de prueba
    }

    public void StartSpawning()
    {
        isSpawning = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
        ClearAllTokens();
    }

    public void StopSpawning()
    {
        ClearAllTokens();
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void SpawnToken(int value)
    {
        if (!isSpawning) return;

        if (tokenPrefab == null)
        {
            Debug.LogError("tokenPrefab no asignado en el inspector.");
            return;
        }

        GameObject tokenObj = Instantiate(tokenPrefab,
                              new Vector3(Random.Range(minX, maxX), spawnY, 0),
                              Quaternion.identity);

        Token token = tokenObj.GetComponent<Token>();
        if (token != null)
        {
            token.Initialize(value, fallSpeed);
            activeTokens.Add(token);
            Debug.Log($"Token {token.gameObject.name} añadido a la lista de activos. Total: {activeTokens.Count}");
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            SpawnToken(Random.Range(1, 50)); // Valor aleatorio entre 1 y 9
            yield return new WaitForSeconds(2f); // Espera 2 segundos entre cada token
        }
    }

    public int GetActiveTokenCount()
    {
        return activeTokens.Count;
    }

    public void RemoveToken(Token token)
    {
        if (token == null)
        {
            Debug.LogWarning("Intento de eliminar un token nulo.");
            return;
        }

        if (activeTokens.Contains(token))
        {
            activeTokens.Remove(token);
            Debug.Log($"Token {token.gameObject.name} eliminado correctamente. Tokens restantes: {activeTokens.Count}");
        }
        else
        {
            Debug.LogWarning($"Token {token.gameObject.name} no encontrado en la lista de activos. Puede que haya sido eliminado antes.");
        }
    }



    private void ClearAllTokens()
    {
        foreach (Token token in activeTokens.ToArray())
        {
            Destroy(token.gameObject);
        }
        activeTokens.Clear();
    }
}