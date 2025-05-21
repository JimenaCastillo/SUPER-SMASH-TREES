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

    public void StartSpawning()
    {
        isSpawning = true;
        ClearAllTokens();
    }

    public void StopSpawning()
    {
        isSpawning = false;
        ClearAllTokens();
    }

    public void SpawnToken(int value)
    {
        if (!isSpawning) return;

        GameObject tokenObj = Instantiate(tokenPrefab,
                              new Vector3(Random.Range(minX, maxX), spawnY, 0),
                              Quaternion.identity);

        Token token = tokenObj.GetComponent<Token>();
        if (token != null)
        {
            token.Initialize(value, fallSpeed);
            activeTokens.Add(token);
        }
    }

    public int GetActiveTokenCount()
    {
        return activeTokens.Count;
    }

    public void RemoveToken(Token token)
    {
        if (activeTokens.Contains(token))
        {
            activeTokens.Remove(token);
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