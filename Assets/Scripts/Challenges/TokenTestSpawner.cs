using UnityEngine;

public class TokenTestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject tokenPrefab;

    void Start()
    {
        if (tokenPrefab == null)
        {
            Debug.LogError("Token prefab no asignado.");
            return;
        }

        Vector3 spawnPosition = new Vector3(0, 0, 0); // Centro de la pantalla
        GameObject tokenObj = Instantiate(tokenPrefab, spawnPosition, Quaternion.identity);

        Token token = tokenObj.GetComponent<Token>();
        if (token != null)
        {
            token.Initialize(10, 0f); // Valor 10, sin caída
            Debug.Log("Token instanciado correctamente en el centro.");
        }
        else
        {
            Debug.LogError("El prefab no tiene el script Token.");
        }
    }
}
