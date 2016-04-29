using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public Board board;
    public Side.SideType currentTurn;
    public Square lastSelectedSquare;
    public List<GameObject> piecePrefabs;

    private ArrayList squares;
    private Stack moveHistory;
    private Stack redoMoveHistory;
    private Rules rules;
    private Player whitePlayer;
    private Player blackPlayer;
    private List<Move> moves = new List<Move>();

    public bool DoNullMovePruning;
    public bool DoPrincipleVariation;
    public bool DoQuiescentSearch;

    public void Init() {
        board = new Board();

        rules = new Rules(board, this);
        moveHistory = new Stack();
        redoMoveHistory = new Stack();
        whitePlayer = new Player(new Side(Side.SideType.White), Player.Type.AI, rules);
        blackPlayer = new Player(new Side(Side.SideType.Black), Player.Type.AI, rules);
    }

    public void BuildBoard(GameObject squarePrefab, List<GameObject> piecePrefabs) {
        this.piecePrefabs = piecePrefabs;
        squares = new ArrayList();
        bool white = false;

        for (int row = 1; row <= 8; row++) {
            for (int col = 1; col <= 8; col++) {
                Square square = ((GameObject)(Instantiate(squarePrefab, new Vector3(row, col), Quaternion.identity))).GetComponent<Square>();
                square.Cell = board[row, col];
                square.Game = this;
                if (white) {
                    square.GetComponent<Renderer>().material.color = Color.white;
                    square.OriginalColor = Color.white;
                } else {
                    square.GetComponent<Renderer>().material.color = Color.black;
                    square.OriginalColor = Color.black;
                }
                if (!board[row, col].Piece.IsEmpty()) {
                    square.Piece = Instantiate(piecePrefabs[(int)board[row, col].Piece.Type], new Vector3(row, col), Quaternion.identity) as GameObject;
                }
                white = !white;
                square.UpdatePiece();
                square.transform.SetParent(this.transform);
                squares.Add(square);
            }
            white = !white;
        }
    }

    public void NextPlayerTurn() {
        if (ActivePlay.IsAI()) {
            Move nextMove = ActivePlay.GetBestMove();

            if (nextMove != null) {
                if (UserMove(nextMove.StartCell.ToString(), nextMove.EndCell.ToString())) {
                    GetBoardSquare(nextMove.StartCell.ToString()).GetComponent<Square>().UpdatePiece();
                    GetBoardSquare(nextMove.EndCell.ToString()).GetComponent<Square>().UpdatePiece();
                    
                    if (!IsCheckMate(currentTurn) && !IsStaleMate(currentTurn)) {
                        NextPlayerTurn();
                    }
                }
            }
        }
    }

    public bool UserMove(string source, string dest) {
        bool success = true;
        int MoveResult = DoMove(source, dest);

        if (MoveResult == 0) {
            Move move = GetLastMove();
            moves.Add(move);

            if (move.IsPromoMove() && !ActivePlay.IsAI()) {
                SetPromotionPiece(new Piece(Piece.PieceType.Queen, move.Piece.Side));
            }

            if (currentTurn == Side.SideType.White) {
                currentTurn = Side.SideType.Black;
            } else {
                currentTurn = Side.SideType.White;
            }


            if (IsCheckMate(currentTurn)) {
                string movesList = "";
                while (moveHistory.Count > 0) {
                    movesList = ((Move)moveHistory.Pop()) + ", " + movesList;
                }
                Debug.Log(movesList);
                Debug.Log("CHECK MATE!");
                return false;
            }

            if (IsStaleMate(currentTurn)) {
                string movesList = "";
                while (moveHistory.Count > 0) {
                    movesList = ((Move)moveHistory.Pop()) + ", " + movesList;
                }
                Debug.Log(movesList);
                Debug.Log("STALE MATE!");
                return false;
            }
        } else {
            success = false;
        }

        return success;
    }

    public Square GetBoardSquare(string cell) {
        foreach (Square square in squares) {
            if (square.Cell.ToString() == cell) {
                return square;
            }
        }

        return null;
    }

    public Cell this[int row, int col] {
        get {
            return board[row, col];
        }
    }


    public Cell this[string cell] {
        get {
            return board[cell];
        }
    }

    public void Reset() {
        moveHistory.Clear();
        redoMoveHistory.Clear();

        currentTurn = Side.SideType.White;
        board.Init();
    }

    public Player WhitePlayer {
        get {
            return whitePlayer;
        }
    }

    public Player BlackPlayer {
        get {
            return blackPlayer;
        }
    }

    public Player ActivePlay {
        get {
            if (BlackTurn()) {
                return blackPlayer;
            } else {
                return whitePlayer;
            }
        }
    }

    public Player EnemyPlayer(Side Player) {
        if (Player.isBlack()) {
            return whitePlayer;
        } else {
            return blackPlayer;
        }
    }

    public Player GetPlayerBySide(Side.SideType type) {
        if (type == Side.SideType.Black) {
            return blackPlayer;
        } else {
            return whitePlayer;
        }
    }

    public bool BlackTurn() {
        return (currentTurn == Side.SideType.Black);
    }

    public bool WhiteTurn() {
        return (currentTurn == Side.SideType.White);
    }

    public ArrayList GetLegalMoves(Cell source) {
        return rules.GetLegalMoves(source);
    }

    public int DoMove(string source, string dest) {
        int MoveResult;

        if (this.board[source].Piece.Type != Piece.PieceType.Empty && this.board[source].Piece.Side.type == currentTurn) {
            Move UserMove = new Move(this.board[source], this.board[dest]);
            MoveResult = rules.DoMove(UserMove);

            if (MoveResult == 0) {
                moveHistory.Push(UserMove);
            }
        } else {
            MoveResult = -1;
        }

        return MoveResult;
    }

    public bool UndoMove() {
        if (moveHistory.Count > 0) {
            Move UserMove = (Move)moveHistory.Pop();
            redoMoveHistory.Push(UserMove);
            rules.UndoMove(UserMove);
            NextPlayerTurn();
            return true;
        } else {
            return false;
        }
    }

    public bool ReDoMove() {
        if (redoMoveHistory.Count > 0) {
            Move UserMove = (Move)redoMoveHistory.Pop();
            moveHistory.Push(UserMove);
            rules.DoMove(UserMove);
            NextPlayerTurn();
            return true;
        } else {
            return false;
        }
    }
    
    public Stack MoveHistory {
        get { return moveHistory; }
    }

    public bool IsCheckMate(Side.SideType PlayerSide) {
        return rules.IsCheckMate(PlayerSide);
    }

    public bool IsStaleMate(Side.SideType PlayerSide) {
        return rules.IsStaleMate(PlayerSide);
    }

    public bool IsUnderCheck() {
        return rules.IsUnderCheck(currentTurn);
    }

    public Move GetLastMove() {
        if (moveHistory.Count > 0) {
            return (Move)moveHistory.Peek();
        }

        return null;
    }

    public void SetPromotionPiece(Piece promotionPiece) {
        if (moveHistory.Count > 0) {
            Move move = (Move)moveHistory.Peek();
            move.EndCell.Piece = promotionPiece;
            move.PromotedPiece = promotionPiece;
        }
    }
}