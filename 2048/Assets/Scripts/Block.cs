using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Node Node;
    public int Value;
    public Block MergingBlock;
    public bool Merging;

    public Vector2 Poz => transform.position;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _textMeshPro;

    

    public void Init(BlockType type)
    {
        Value = type.Value;
        _renderer.color = type.Color;
        _textMeshPro.text = type.Value.ToString();
    }

    public void SetBlock(Node node)
    {
        if (Node != null)
        {
            Node.OccupiedBlock = null;
        }

        Node = node;
        Node.OccupiedBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith)
    {
        MergingBlock = blockToMergeWith;

        Node.OccupiedBlock = null;

        blockToMergeWith.Merging = true;
    }

    public bool CanMerge(int value) => value == Value & !Merging && MergingBlock == null;

}
