/// <summary>
/// A square on a chess board
/// </summary>
using System.Collections;
using UnityEngine;
public class Square : MonoBehaviour {
    /// <summary>
    /// The game piece object
    /// </summary>
    private GameObject gamePiece;

    /// <summary>
    /// The piece information
    /// </summary>
    private Piece piece;

    /// <summary>
    /// The current game
    /// </summary>
    private Game game;

    /// <summary>
    /// The square's cell
    /// </summary>
    private Cell cell;

    /// <summary>
    /// The original color of the square after unhighlighting
    /// </summary>
    private Color originalColor;

    /// <summary>
    /// The previously highlighted squares from legal move highlighting
    /// </summary>
    private ArrayList previouslyHighlightedSquares;

    /// <summary>
    /// Whether or not this square is to remain highlighted on mouse exit
    /// </summary>
    private bool highlighted = false;

    /// <summary>
    /// On mouse enter, highlight the square
    /// </summary>
    void OnMouseEnter() {
        this.Highlight();
    }

    /// <summary>
    /// On mouse exit, if the square is not to remain highlighted, unhighlight it
    /// </summary>
    void OnMouseExit() {
        if (!this.highlighted) {
            this.Unhighlight();
        }
    }

    /// <summary>
    /// On mouse down, attempt to highlight legal moves or make a legal move
    /// </summary>
    void OnMouseDown() {
        // Only for human players
        if (!game.ActivePlay.IsAI()) {
            // If the last square is not this square
            if (game.LastSelectedSquare != null && this != game.LastSelectedSquare) {
                // Try to make a move from the last square to this square
                if (game.TryMove(game.LastSelectedSquare.cell.ToString(), this.cell.ToString())) {
                    // Update the game pieces
                    this.game.LastSelectedSquare.UpdatePiece();
                    this.UpdatePiece();

                    // Try to do the next AI move
                    this.game.NextPlayerTurn();
                }

                // Unhighlight previously highlighted legal move squares
                foreach (Cell cell in this.game.LastSelectedSquare.previouslyHighlightedSquares) {
                    Square square = this.game.GetBoardSquare(cell.ToString());
                    square.highlighted = false;
                    square.Unhighlight();
                }

                // Clear the list of previously highlighted squares
                this.game.LastSelectedSquare.previouslyHighlightedSquares.Clear();

                // Clear the last selected square
                this.game.LastSelectedSquare = null;
            } else {
                // Else show all possible legal moves
                this.previouslyHighlightedSquares = game.GetLegalMoves(this.cell);

                // Highlight all possible legal moves
                foreach (Cell cell in this.previouslyHighlightedSquares) {
                    Square square = game.GetBoardSquare(cell.ToString());
                    square.highlighted = true;
                    square.Highlight();
                }

                // Set this as the last selected square
                this.game.LastSelectedSquare = this;
            }
        }
    }

    /// <summary>
    /// Update the piece object based on the piece information
    /// </summary>
    public void UpdatePiece() {
        // Get the piece in this square
        Piece p = this.game.Board[this.cell].Piece;
        // If this piece has changed
        if (this.piece!= p) {
            // Destroy the existing game piece object
            if (this.gamePiece != null) {
                Destroy(gamePiece);
            }
            // If the piece is not empty
            if (!p.IsEmpty()) {
                // Create a new game piece using the prefabs
                gamePiece = Instantiate(this.game.piecePrefabs[(int)p.Type], new Vector3(this.cell.Row, this.cell.Column), Quaternion.identity) as GameObject;

                // Set its color based on the piece information
                if (p.Side.IsWhite()) {
                    // Have to set all of the mesh renderer colors
                    MeshRenderer[] meshRenderers = gamePiece.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in meshRenderers) {
                        mr.material.color = Color.white;
                    }
                } else {
                    // Have to set all of the mesh renderer colors
                    MeshRenderer[] meshRenderers = gamePiece.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in meshRenderers) {
                        mr.material.color = Color.black;
                    }
                }

                // Knight rotation needs to be fixed
                if (p.Type == Piece.PieceType.Knight) {
                    if (p.Side.IsWhite()) {
                        gamePiece.transform.Rotate(0, 0, 270.0f);
                    } else {
                        gamePiece.transform.Rotate(0, 0, 90.0f);
                    }
                }

                // Parent the piece to this square so twe move together
                this.gamePiece.transform.SetParent(this.transform);
            }

            // Update the piece information
            this.piece = p;
        }
    }

    /// <summary>
    /// Highlight the square green
    /// </summary>
    private void Highlight() {
        this.GetComponent<Renderer>().material.color = Color.green;
    }

    /// <summary>
    /// Unhighlight the square back to its original color
    /// </summary>
    private void Unhighlight() {
        this.GetComponent<Renderer>().material.color = this.originalColor;
    }

    /// <summary>
    /// Property for the square cell
    /// </summary>
    public Cell Cell {
        get {
            return this.cell;
        }

        set {
            this.cell = value;
        }
    }

    /// <summary>
    /// Accessor for the current game
    /// </summary>
    public Game Game {
        get {
            return this.game;
        }

        set {
            this.game = value;
        }
    }

    /// <summary>
    /// Accessor for the original color
    /// </summary>
    public Color OriginalColor {
        get {
            return this.originalColor;
        }

        set {
            this.originalColor = value;
        }
    }
}
