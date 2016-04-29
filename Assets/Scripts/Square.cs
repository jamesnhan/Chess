using System.Collections;
using UnityEngine;
public class Square : MonoBehaviour {
    private GameObject gamePiece;
    private Piece piece;
    private Game game;
    private Cell cell;
    private Color originalColor;
    private ArrayList previouslyHighlightedSquares;

    private bool highlighted = false;

    void OnMouseEnter() {
        Highlight();
    }

    void OnMouseExit() {
        if (!highlighted) {
            Unhighlight();
        }
    }

    void OnMouseDown() {
        if (!game.ActivePlay.IsAI()) {
            if (game.lastSelectedSquare != null && this != game.lastSelectedSquare) {
                if (game.UserMove(game.lastSelectedSquare.cell.ToString(), this.cell.ToString())) {
                    this.game.lastSelectedSquare.UpdatePiece();
                    UpdatePiece();

                    this.game.NextPlayerTurn();
                }

                foreach (Cell cell in this.game.lastSelectedSquare.previouslyHighlightedSquares) {
                    Square square = this.game.GetBoardSquare(cell.ToString());
                    square.highlighted = false;
                    square.Unhighlight();
                }

                this.game.lastSelectedSquare.previouslyHighlightedSquares.Clear();

                game.lastSelectedSquare = null;
            } else {
                this.previouslyHighlightedSquares = game.GetLegalMoves(this.cell);

                foreach (Cell cell in this.previouslyHighlightedSquares) {
                    Square square = game.GetBoardSquare(cell.ToString());
                    square.highlighted = true;
                    square.Highlight();
                }
                
                game.lastSelectedSquare = this;
            }
        }
    }

    public void UpdatePiece() {
        Piece p = this.game.board[this.cell].Piece;
        if (this.piece!= p) {
            if (this.gamePiece != null) {
                Destroy(gamePiece);
            }
            if (!p.IsEmpty()) {
                gamePiece = Instantiate(this.game.piecePrefabs[(int)p.Type], new Vector3(this.cell.Row, this.cell.Column), Quaternion.identity) as GameObject;
                if (p.Side.isWhite()) {
                    MeshRenderer[] meshRenderers = gamePiece.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in meshRenderers) {
                        mr.material.color = Color.white;
                    }
                } else {
                    MeshRenderer[] meshRenderers = gamePiece.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in meshRenderers) {
                        mr.material.color = Color.black;
                    }
                }
                this.gamePiece.transform.SetParent(this.transform);
            }
            this.piece = p;
        }
    }

    private void Highlight() {
        this.GetComponent<Renderer>().material.color = Color.green;
    }

    private void Unhighlight() {
        this.GetComponent<Renderer>().material.color = originalColor;
    }

    public Cell Cell {
        get {
            return this.cell;
        }

        set {
            this.cell = value;
        }
    }

    public Game Game {
        get {
            return this.game;
        }

        set {
            this.game = value;
        }
    }

    public Color OriginalColor {
        get {
            return this.originalColor;
        }

        set {
            this.originalColor = value;
        }
    }

    public GameObject Piece {
        get {
            return this.gamePiece;
        }

        set {
            this.gamePiece = value;
        }
    }
}
