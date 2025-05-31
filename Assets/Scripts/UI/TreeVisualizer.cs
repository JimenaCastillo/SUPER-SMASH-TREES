using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreeVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject nodeTemplate;
    [SerializeField] private RectTransform treeContainer;
    [SerializeField] private float horizontalSpacing = 50f;
    [SerializeField] private float verticalSpacing = 40f;
    [SerializeField] private GameObject lineImagePrefab; // Nuevo prefab de línea UI

    private List<GameObject> nodeObjects = new List<GameObject>();
    private List<GameObject> lineObjects = new List<GameObject>();
    private int inOrderX = 0;

    public void UpdateTreeVisualization(ITree tree)
    {
        ClearVisualization();
        if (tree == null) return;

        List<TreeNode> nodes = tree.GetNodes();

        if (tree.GetTreeType() == TreeType.BST || tree.GetTreeType() == TreeType.AVL)
        {
            TreeNode root = nodes.Count > 0 ? nodes[0] : null;
            if (root != null)
            {
                inOrderX = 0;
                float containerCenterX = (treeContainer.rect.width / 2f) - 40f; // más a la izquierda
                float verticalOffsetY = 240f; // más arriba

                Dictionary<TreeNode, Vector2> nodePositions = new Dictionary<TreeNode, Vector2>();
                CalculateNodePositions(root, 0, nodePositions, containerCenterX, verticalOffsetY);

                CreateVisualNodes(root, nodePositions);
            }
        }
        else if (tree.GetTreeType() == TreeType.BTree)
        {
            // Visualización simple para B-Tree
            float yPos = 0;
            float xPos = 0;

            foreach (TreeNode node in nodes)
            {
                GameObject nodeObj = CreateNodeObject(node.Value.ToString(), new Vector2(xPos, yPos));
                nodeObjects.Add(nodeObj);

                xPos += horizontalSpacing;
                if (xPos > treeContainer.rect.width - horizontalSpacing)
                {
                    xPos = 0;
                    yPos -= verticalSpacing;
                }
            }
        }
    }

    private void CalculateNodePositions(TreeNode node, int level, Dictionary<TreeNode, Vector2> nodePositions, float centerOffsetX, float verticalOffsetY)
    {
        if (node == null) return;

        CalculateNodePositions(node.Left, level + 1, nodePositions, centerOffsetX, verticalOffsetY);

        float xPos = inOrderX * horizontalSpacing;
        float yPos = -level * verticalSpacing;
        nodePositions[node] = new Vector2(xPos + centerOffsetX, yPos + verticalOffsetY); // 👈 APLICAMOS offset vertical
        inOrderX++;

        CalculateNodePositions(node.Right, level + 1, nodePositions, centerOffsetX, verticalOffsetY);
    }

    private void CreateVisualNodes(TreeNode node, Dictionary<TreeNode, Vector2> nodePositions)
    {
        if (node == null) return;

        Vector2 nodePos = nodePositions[node];
        GameObject nodeObj = CreateNodeObject(node.Value.ToString(), nodePos);
        nodeObjects.Add(nodeObj);

        if (node.Left != null)
        {
            Vector2 leftPos = nodePositions[node.Left];
            CreateLineBetween(nodePos, leftPos);
            CreateVisualNodes(node.Left, nodePositions);
        }

        if (node.Right != null)
        {
            Vector2 rightPos = nodePositions[node.Right];
            CreateLineBetween(nodePos, rightPos);
            CreateVisualNodes(node.Right, nodePositions);
        }
    }

    private GameObject CreateNodeObject(string value, Vector2 position)
    {
        GameObject nodeObj = Instantiate(nodeTemplate, treeContainer);
        RectTransform rectTransform = nodeObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;

        TextMeshProUGUI valueText = nodeObj.GetComponentInChildren<TextMeshProUGUI>();
        if (valueText != null)
        {
            valueText.text = value;
        }

        nodeObj.SetActive(true);
        return nodeObj;
    }

    private void CreateLineBetween(Vector2 start, Vector2 end)
    {
        GameObject lineObj = Instantiate(lineImagePrefab, treeContainer);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        Vector2 direction = end - start;
        float distance = direction.magnitude;
        Vector2 center = start + direction / 2;

        lineRect.anchoredPosition = center;
        lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, distance); // Width stays the same, height = distance
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle - 90); // Correct rotation

        lineObj.SetActive(true);
        lineObjects.Add(lineObj);
    }

    private void ClearVisualization()
    {
        foreach (GameObject nodeObj in nodeObjects)
        {
            Destroy(nodeObj);
        }
        nodeObjects.Clear();

        foreach (GameObject lineObj in lineObjects)
        {
            Destroy(lineObj);
        }
        lineObjects.Clear();
    }
}