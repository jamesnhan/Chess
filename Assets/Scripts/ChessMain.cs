using System.Collections.Generic;
using UnityEngine;
public class ChessMain : MonoBehaviour {
    private Game game;

    public GameObject squarePrefab;
    public List<GameObject> piecePrefabs = new List<GameObject>();

    void Start() {
        game = this.gameObject.AddComponent<Game>();
        game.Init();
        game.Reset();
        game.BuildBoard(squarePrefab, piecePrefabs);
        game.NextPlayerTurn();
    }
}