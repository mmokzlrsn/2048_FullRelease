using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;


public class GameManager : MonoBehaviour
{

    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private Ease _easeType;

    private List<Node> _nodes;
    private List<Block> _blocks;
    private GameState _currentState;

    private BlockType GetBlockTypeByValue(int value) => _types.First(t => t.Value == value);
    private Vector2 _center;
    private int _round = 0;


    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameState.GenerateLevel);


        //GenerateGrid();
        //SetCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentState != GameState.WaitInput ) return;
        if (_currentState == GameState.Moving) return;
        Movement();
    }

    void Movement()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Shift(Vector2.left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Shift(Vector2.right);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Shift(Vector2.up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Shift(Vector2.down);
        }
    }

    void GenerateGrid()
    {
        _round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();

        for(int x = 0; x < _width ; x++)
        {
            for(int y = 0; y < _height ; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x,y), Quaternion.identity);
                _nodes.Add(node);
            }
        }

        _center = new Vector2((float) _width/2f -0.5f, _height/ 2f - 0.5f);

        var board = Instantiate(_boardPrefab, _center, Quaternion.identity);
        board.size = new Vector2(_width, _height);

        ChangeState(GameState.SpawnBlocks);
    }

    void SetCameraPosition()
    {
        Camera.main.transform.position = new Vector3(_center.x, _center.y, -10);
    }

    void SpawnBlocks(int amount)
    {
        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null).OrderBy(b => Random.value).ToList();

        foreach (var node in freeNodes.Take(amount))
        {
            var block = Instantiate(_blockPrefab, node.Poz, Quaternion.identity);
            block.Init(GetBlockTypeByValue(Random.value > 0.8 ? 4 : 2));
            block.SetBlock(node);
            _blocks.Add(block);
        }

        if (freeNodes.Count() == 1)
        {
            //lose
            return;
        }


        ChangeState(GameState.WaitInput);
        
    }

    void Shift(Vector2 dir)
    {
        ChangeState(GameState.Moving);

        var ordererdBlocks = _blocks.OrderBy(b => b.Poz.x).ThenBy(b => b.Poz.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) ordererdBlocks.Reverse();

        foreach (var block in ordererdBlocks)
        {
            var next = block.Node;
            do
            {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Poz + dir);
                if (possibleNode != null)
                {
                    //if possible merge
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Value))
                    {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                    }
                    //or check for move next stop?
                    else if (possibleNode.OccupiedBlock == null) next = possibleNode;

                    //end while loop
                }

            } while (next != block.Node);

            //block.transform.position = block.Node.Poz;
            block.transform.DOMove(block.Node.Poz, _travelTime).SetEase(_easeType)
                .OnComplete(() =>
                {
                    ChangeState(GameState.WaitInput);
                });
        }

    }

    private Node GetNodeAtPosition(Vector2 poz)
    {
        return _nodes.FirstOrDefault(n => n.Poz == poz);
    }

     


    void ChangeState(GameState newState)
    {
        _currentState = newState;
        switch (newState)
        {
            case GameState.GenerateLevel:
                GenerateGrid();
                SetCameraPosition();
                break;
            case GameState.SpawnBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1); //if its first round spawn 2
                break;
            case GameState.WaitInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    
}

[Serializable]
public struct BlockType
{
    public int Value;
    public Color Color;
}

public enum GameState
{
    GenerateLevel, SpawnBlocks, WaitInput, Moving, Win, Lose
}
