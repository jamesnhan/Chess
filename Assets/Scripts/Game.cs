/// <summary>
/// A chess game
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    /// <summary>
    /// The board of the game
    /// </summary>
    private Board board;

    /// <summary>
    /// The current turn
    /// </summary>
    private Side.SideType currentTurn;

    /// <summary>
    /// The last selected square
    /// </summary>
    private Square lastSelectedSquare;

    /// <summary>
    /// A list of the piece prefabs
    /// </summary>
    public List<GameObject> piecePrefabs;

    /// <summary>
    /// The squares of the board
    /// </summary>
    private ArrayList squares;

    /// <summary>
    /// The move history
    /// </summary>
    private Stack moveHistory;

    /// <summary>
    /// The rules of the game
    /// </summary>
    private Rules rules;

    /// <summary>
    /// The white player
    /// </summary>
    private Player whitePlayer;

    /// <summary>
    /// The black player
    /// </summary>
    private Player blackPlayer;

    /// <summary>
    /// Whether or not the computer should do null move pruning to speed up alpha-beta
    /// </summary>
    public bool DoNullMovePruning;

    /// <summary>
    /// Whether or not the computer should do principal variation search instead of alpha-beta
    /// </summary>
    public bool DoPrincipleVariation;

    /// <summary>
    /// Whether or not the computer should use quiescence searching to prevent the horizon problem
    /// </summary>
    public bool DoQuiescentSearch;

    /// <summary>
    /// The maximum depth for the AI to search
    /// </summary>
    public int maxAIDepth = 3;

    /// <summary>
    /// Initialize the board
    /// </summary>
    /// <param name="player1Type">The type of player for player 1</param>
    /// <param name="player2Type">The type of player for player 2</param>
    public void Init(Player.Type player1Type, Player.Type player2Type) {
        this.board = new Board();
        this.rules = new Rules(this.board, this);
        this.moveHistory = new Stack();
        this.whitePlayer = new Player(new Side(Side.SideType.White), player1Type, this.rules);
        this.blackPlayer = new Player(new Side(Side.SideType.Black), player2Type, this.rules);
    }

    /// <summary>
    /// Build the board squares
    /// </summary>
    /// <param name="squarePrefab">The prefab for the board squares</param>
    /// <param name="piecePrefabs">The list of prefabs for the pieces</param>
    public void BuildBoard(GameObject squarePrefab, List<GameObject> piecePrefabs) {
        // Keep a reference of the list for future use
        this.piecePrefabs = piecePrefabs;
        
        // Initialize the squares
        this.squares = new ArrayList();

        // Starts with white
        bool white = false;

        for (int row = 1; row <= 8; row++) {
            for (int col = 1; col <= 8; col++) {
                // Instantiate a new square and set up its properties
                Square square = ((GameObject)(Instantiate(squarePrefab, new Vector3(row, col), Quaternion.identity))).GetComponent<Square>();
                square.Cell = this.board[row, col];
                square.Game = this;
                if (white) {
                    square.GetComponent<Renderer>().material.color = Color.white;
                    square.OriginalColor = Color.white;
                } else {
                    square.GetComponent<Renderer>().material.color = Color.black;
                    square.OriginalColor = Color.black;
                }

                // Columns flip flop
                white = !white;

                // Update the square piece so that we create the model and set its color
                square.UpdatePiece();

                // Parent the square to the game's game object so that we can move everything all together if need be
                square.transform.SetParent(this.transform);

                // Add the square to our list
                this.squares.Add(square);
            }

            // Rows flip flop
            white = !white;
        }
    }

    /// <summary>
    /// Try to do the next AI turn
    /// </summary>
    public void NextPlayerTurn() {
        // Automate the move if the active player is an AI
        if (this.ActivePlay.IsAI()) {
            // Get the best move
            Move nextMove = this.ActivePlay.GetBestMove();

            // If we have a move
            if (nextMove != null) {
                // Try making it
                if (this.TryMove(nextMove.StartCell.ToString(), nextMove.EndCell.ToString())) {
                    // Update the squares at each end of the move
                    this.GetBoardSquare(nextMove.StartCell.ToString()).GetComponent<Square>().UpdatePiece();
                    this.GetBoardSquare(nextMove.EndCell.ToString()).GetComponent<Square>().UpdatePiece();
                    
                    // If we we are not in checkmate, stalemate, or draw
                    if (!this.IsCheckMate(this.currentTurn) && !this.IsStaleMate(this.currentTurn) && !this.IsFiftyMoveDraw()) {
                        // Try to do the next turn
                        this.NextPlayerTurn();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Try making a move
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="dest">The destination cell</param>
    /// <returns>Success of the move attempt</returns>
    public bool TryMove(string source, string dest) {
        bool success = true;
        // Try the move
        int MoveResult = this.DoMove(source, dest);

        // If the move was successful
        if (MoveResult == 0) {
            Move move = this.GetLastMove();

            // If the move is a promotion move, set the promotion piece to a queen
            // TODO: Allow the user to select what kind of piece he wants (queen versus knight)
            if (move.IsPromotionMove()) {
                this.SetPromotionPiece(new Piece(Piece.PieceType.Queen, move.Piece.Side));
            }

            // Swap the current turn
            if (this.currentTurn == Side.SideType.White) {
                this.currentTurn = Side.SideType.Black;
            } else {
                this.currentTurn = Side.SideType.White;
            }

            // Check for check mate
            if (this.IsCheckMate(currentTurn)) {
                string movesList = "";
                while (moveHistory.Count > 0) {
                    movesList = ((Move)moveHistory.Pop()) + ", " + movesList;
                }
                Debug.Log(movesList);
                Debug.Log("CHECK MATE!");
                return false;
            }

            // Check for stale mate
            if (this.IsStaleMate(currentTurn)) {
                string movesList = "";
                while (moveHistory.Count > 0) {
                    movesList = ((Move)moveHistory.Pop()) + ", " + movesList;
                }
                Debug.Log(movesList);
                Debug.Log("STALE MATE!");
                return false;
            }

            // Check for fifty move draws
            if (this.IsFiftyMoveDraw()) {
                string movesList = "";
                while (moveHistory.Count > 0) {
                    movesList = ((Move)moveHistory.Pop()) + ", " + movesList;
                }
                Debug.Log(movesList);
                Debug.Log("FIFTY MOVE DRAW!");
                return false;
            }

            // TODO: Check for threefold repetition draw
        } else {
            success = false;
        }

        return success;
    }

    /// <summary>
    /// Get the square in a cell
    /// </summary>
    /// <param name="cell">The cell in standard algebraic notation</param>
    /// <returns>The square or null if no square</returns>
    public Square GetBoardSquare(string cell) {
        foreach (Square square in this.squares) {
            if (square.Cell.ToString() == cell) {
                return square;
            }
        }

        return null;
    }

    /// <summary>
    /// Resets the game
    /// </summary>
    public void Reset() {
        this.moveHistory.Clear();

        this.currentTurn = Side.SideType.White;
        this.board.Init();
    }

    /// <summary>
    /// Get the enemy player
    /// </summary>
    /// <param name="Player">The player</param>
    /// <returns>The enemy of the player</returns>
    public Player EnemyPlayer(Side Player) {
        if (Player.IsBlack()) {
            return this.whitePlayer;
        } else {
            return this.blackPlayer;
        }
    }

    /// <summary>
    /// Checks whether or not it is black's turn
    /// </summary>
    /// <returns>True if it is black's turn</returns>
    public bool BlackTurn() {
        return (this.currentTurn == Side.SideType.Black);
    }

    /// <summary>
    /// Get all of the legal moves from a cell
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <returns>An ArrayList of legal Moves</returns>
    public ArrayList GetLegalMoves(Cell source) {
        return this.rules.GetLegalMoves(source);
    }

    /// <summary>
    /// Attempts to make a move
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="dest">The destination cell</param>
    /// <returns>0 for success, -1 for failure</returns>
    public int DoMove(string source, string dest) {
        int MoveResult;

        if (this.board[source].Piece.Type != Piece.PieceType.Empty && this.board[source].Piece.Side.Type == this.currentTurn) {
            Move UserMove = new Move(this.board[source], this.board[dest]);
            MoveResult = this.rules.DoMove(UserMove);

            if (MoveResult == 0) {
                this.moveHistory.Push(UserMove);
            }
        } else {
            MoveResult = -1;
        }

        return MoveResult;
    }

    /// <summary>
    /// Checks if it is check mate
    /// </summary>
    /// <param name="PlayerSide">The player side</param>
    /// <returns>True if the player is in check mate</returns>
    public bool IsCheckMate(Side.SideType PlayerSide) {
        return this.rules.IsCheckMate(PlayerSide);
    }

    /// <summary>
    /// Checks if it is stale mate
    /// </summary>
    /// <param name="PlayerSide">The player side</param>
    /// <returns>True if the player is in stale mate</returns>
    public bool IsStaleMate(Side.SideType PlayerSide) {
        return this.rules.IsStaleMate(PlayerSide);
    }

    /// <summary>
    /// Checks if it is fifty move draw
    /// </summary>
    /// <param name="PlayerSide">The player side</param>
    /// <returns>True if the player is in fifty move draw</returns>
    public bool IsFiftyMoveDraw() {
        return this.rules.IsFiftyMoveDraw();
    }

    /// <summary>
    /// Gets the last move made
    /// </summary>
    /// <returns>The last move made</returns>
    public Move GetLastMove() {
        if (this.moveHistory.Count > 0) {
            return (Move)this.moveHistory.Peek();
        }

        return null;
    }

    /// <summary>
    /// Sets the promotion piece
    /// </summary>
    /// <param name="promotionPiece"></param>
    public void SetPromotionPiece(Piece promotionPiece) {
        if (this.moveHistory.Count > 0) {
            Move move = (Move)this.moveHistory.Peek();
            move.EndCell.Piece = promotionPiece;
            move.PromotedPiece = promotionPiece;
        }
    }

    /// <summary>
    /// Property for the white player
    /// </summary>
    public Player WhitePlayer {
        get {
            return this.whitePlayer;
        }
    }

    /// <summary>
    /// Property for the black player
    /// </summary>
    public Player BlackPlayer {
        get {
            return this.blackPlayer;
        }
    }

    /// <summary>
    /// Property for the active player
    /// </summary>
    public Player ActivePlay {
        get {
            if (this.BlackTurn()) {
                return this.blackPlayer;
            } else {
                return this.whitePlayer;
            }
        }
    }

    /// <summary>
    /// Property for the move history
    /// </summary>
    public Stack MoveHistory {
        get {
            return this.moveHistory;
        }
    }

    public Board Board {
        get {
            return this.board;
        }
    }

    public Side.SideType CurrentTurn {
        get {
            return this.currentTurn;
        }

        set {
            this.currentTurn = value;
        }
    }

    public Square LastSelectedSquare {
        get {
            return this.lastSelectedSquare;
        }

        set {
            this.lastSelectedSquare = value;
        }
    }
}