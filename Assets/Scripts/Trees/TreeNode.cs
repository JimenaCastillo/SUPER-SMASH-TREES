using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode
{
    public int Value { get; set; }
    public TreeNode Left { get; set; }
    public TreeNode Right { get; set; }
    public int Height { get; set; }

    public TreeNode(int value)
    {
        Value = value;
        Left = null;
        Right = null;
        Height = 1;
    }
}

public enum TreeType
{
    BST,
    AVL,
    BTree
}

public interface ITree
{
    void Insert(int value);
    bool ValidateChallenge(string challengeType, int parameter);
    List<TreeNode> GetNodes();
    TreeType GetTreeType();
}