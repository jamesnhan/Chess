/// <summary>
/// Entry point for chess game
/// </summary>
using System.Collections.Generic;
using UnityEngine;

public class ChessMain : MonoBehaviour {
    /// <summary>
    /// The reference to the current game
    /// </summary>
    private Game game;

    /// <summary>
    /// The prefab of the board square
    /// </summary>
    public GameObject squarePrefab;

    /// <summary>
    /// The list of prefabs of the chess pieces
    /// </summary>
    public List<GameObject> piecePrefabs = new List<GameObject>();

    public Player.Type player1Type = Player.Type.Human;
    public Player.Type player2Type = Player.Type.Human;

    /// <summary>
    /// This is the entry point for the Chess Game
    /// </summary>
    void Start() {
        // Instantiate a game
        this.game = this.gameObject.AddComponent<Game>();
        // Initialize the game and set up the board
        this.game.Init(player1Type, player2Type);
        this.game.Reset();
        this.game.BuildBoard(this.squarePrefab, this.piecePrefabs);

        // Start the first turn
        this.game.NextPlayerTurn();
    }
}