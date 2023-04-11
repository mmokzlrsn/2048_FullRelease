using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int Value;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMeshPro _textMeshPro;

    public void Init(BlockType type)
    {
        Value = type.Value;
        _renderer.color = type.Color;
        _textMeshPro.text = type.Value.ToString();

    }
}
