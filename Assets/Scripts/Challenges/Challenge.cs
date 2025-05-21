using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Challenge
{
    public TreeType treeType;
    public string challengeType;
    public int parameter;
    public string description;

    public Challenge(TreeType treeType, string challengeType, int parameter)
    {
        this.treeType = treeType;
        this.challengeType = challengeType;
        this.parameter = parameter;
        this.description = GenerateDescription();
    }

    private string GenerateDescription()
    {
        switch (treeType)
        {
            case TreeType.BST:
                switch (challengeType)
                {
                    case "depth":
                        return $"Construir un BST de profundidad {parameter} o mayor";
                    case "size":
                        return $"Construir un BST con al menos {parameter} nodos";
                    default:
                        return "Reto desconocido";
                }
            case TreeType.AVL:
                switch (challengeType)
                {
                    case "balanced":
                        return "Construir un árbol AVL perfectamente balanceado";
                    case "size":
                        return $"Construir un árbol AVL con al menos {parameter} nodos";
                    default:
                        return "Reto desconocido";
                }
            case TreeType.BTree:
                switch (challengeType)
                {
                    case "height":
                        return $"Construir un B-Tree de altura {parameter} o mayor";
                    case "nodes":
                        return $"Construir un B-Tree con al menos {parameter} nodos";
                    default:
                        return "Reto desconocido";
                }
            default:
                return "Reto desconocido";
        }
    }
}