using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private SpriteRenderer _spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateGrid()
    {
        for(int x = 0; x < _width ; x++)
        {
            for(int y = 0; y < _height ; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x,y), Quaternion.identity);
            }
        }

        // var center = 
    }
}
