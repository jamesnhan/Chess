using System;

[Serializable]
public class Move {
    public enum MoveType { NormalMove, CaptureMove, TowerMove, PromotionMove, EnPassant };

    private Cell startCell;
    private Cell endCell;
    private Piece piece;
    private Piece capturedPiece;
    private Piece promotedPiece;
    private Piece enPassantPiece;
    private MoveType type;
    private bool causesCheck;
    private int score;

    public Move(Cell begin, Cell end) {
        startCell = begin;
        endCell = end;
        piece = begin.Piece;
        capturedPiece = end.Piece;
        score = 0;
    }
    public Cell StartCell {
        get {
            return startCell;
        }
        set {
            startCell = value;
        }
    }
    public Cell EndCell {
        get {
            return endCell;
        }
        set {
            endCell = value;
        }
    }
    public Piece Piece {
        get {
            return piece;
        }
        set {
            piece = value;
        }
    }
    public Piece CapturedPiece {
        get {
            return capturedPiece;
        }
        set {
            capturedPiece = value;
        }
    }
    public MoveType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }
    public bool CausesCheck {
        get {
            return causesCheck;
        }
        set {
            causesCheck = value;
        }
    }
    public Piece PromotedPiece {
        get {
            return promotedPiece;
        }
        set {
            promotedPiece = value;
        }
    }
    public Piece EnPassantPiece {
        get {
            return enPassantPiece;
        }
        set {
            enPassantPiece = value;
        }
    }
    public int Score {
        get {
            return score;
        }
        set {
            score = value;
        }
    }
    public bool IsPromoMove() {
        return type == MoveType.PromotionMove;
    }
    public bool IsCaptureMove() {
        return type == MoveType.CaptureMove;
    }

    public override string ToString() {
        if (type == MoveType.CaptureMove)
            return piece + " " + startCell.ToString() + "x" + endCell.ToString();
        else
            return piece + " " + startCell.ToString() + "-" + endCell.ToString();
    }
}

public class MoveCompare : System.Collections.IComparer {

    public MoveCompare() {
    }

    public int Compare(object firstObj, object SecondObj) {
        Move firstMove = (Move)firstObj;
        Move secondMove = (Move)SecondObj;

        return -firstMove.Score.CompareTo(secondMove.Score);
    }
}