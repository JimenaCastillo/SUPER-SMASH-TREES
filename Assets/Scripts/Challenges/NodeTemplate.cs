using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class NodeTemplate : MonoBehaviour
{
    [SerializeField] private TextMeshPro valueText;
    public int value { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Ini(int value)
    {
        this.value = value;
        valueText.text = value.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
