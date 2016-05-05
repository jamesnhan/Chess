/// <summary>
/// A move in chess
/// </summary>

public class Move {
    /// <summary>
    /// An enum of all the possible types of chess moves
    /// </summary>
    public enum MoveType { NormalMove, CaptureMove, TowerMove, PromotionMove, EnPassant };

    /// <summary>
    /// The starting cell
    /// </summary>
    private Cell startCell;

    /// <summary>
    /// The ending cell
    /// </summary>
    private Cell endCell;

    /// <summary>
    /// The piece moving
    /// </summary>
    private Piece piece;

    /// <summary>
    /// The potential captured piece
    /// </summary>
    private Piece capturedPiece;

    /// <summary>
    /// The potential promoted piece
    /// </summary>
    private Piece promotedPiece;

    /// <summary>
    /// The potential piece captured in en passant
    /// </summary>
    private Piece enPassantPiece;

    /// <summary>
    /// The type of move this is
    /// </summary>
    private MoveType type;

    /// <summary>
    /// The score of this move for AI purposes
    /// </summary>
    private int score;

    /// <summary>
    /// Create a basic move from one cell to another
    /// </summary>
    /// <param name="begin">The starting cell</param>
    /// <param name="end">The ending cell</param>
    public Move(Cell begin, Cell end) {
        startCell = begin;
        endCell = end;
        piece = begin.Piece;
        capturedPiece = end.Piece;
        score = 0;
    }

    /// <summary>
    /// Property for the starting cell
    /// </summary>
    public Cell StartCell {
        get {
            return startCell;
        }
        set {
            startCell = value;
        }
    }

    /// <summary>
    /// Property for the ending cell
    /// </summary>
    public Cell EndCell {
        get {
            return endCell;
        }
        set {
            endCell = value;
        }
    }

    /// <summary>
    /// Property for the piece
    /// </summary>
    public Piece Piece {
        get {
            return piece;
        }
        set {
            piece = value;
        }
    }
    
    /// <summary>
    /// Property for the captured piece
    /// </summary>
    public Piece CapturedPiece {
        get {
            return capturedPiece;
        }
        set {
            capturedPiece = value;
        }
    }

    /// <summary>
    /// Property for the move type
    /// </summary>
    public MoveType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }
    
    /// <summary>
    /// Property for the promoted piece
    /// </summary>
    public Piece PromotedPiece {
        get {
            return promotedPiece;
        }
        set {
            promotedPiece = value;
        }
    }

    /// <summary>
    /// Property for the en passant piece
    /// </summary>
    public Piece EnPassantPiece {
        get {
            return enPassantPiece;
        }
        set {
            enPassantPiece = value;
        }
    }
    
    /// <summary>
    /// Property for the score
    /// </summary>
    public int Score {
        get {
            return score;
        }
        set {
            score = value;
        }
    }

    /// <summary>
    /// Checks whether or not this move is a promotion move
    /// </summary>
    /// <returns>True if this move is a promotion move</returns>
    public bool IsPromotionMove() {
        return type == MoveType.PromotionMove;
    }

    /// <summary>
    /// Checks whether or not this move is a capture move
    /// </summary>
    /// <returns>True if this move is a capture move</returns>
    public bool IsCaptureMove() {
        return type == MoveType.CaptureMove;
    }

    /// <summary>
    /// The move in standard algebraic notation
    /// </summary>
    /// <returns>The move as a string in standard algebraic notation</returns>
    public override string ToString() {
        if (type == MoveType.CaptureMove) {
            return piece + " " + startCell.ToString() + "x" + endCell.ToString();
        } else {
            return piece + " " + startCell.ToString() + "-" + endCell.ToString();
        }
    }
}

/// <summary>
/// An IComparer to compare two moves based on score for sorting
/// </summary>
public class MoveComparer : System.Collections.IComparer {
    /// <summary>
    /// Create a new move comparer
    /// </summary>
    public MoveComparer() { }

    /// <summary>
    /// The compare method for sorting
    /// </summary>
    /// <param name="firstObj">The first move</param>
    /// <param name="secondObj">The second move</param>
    /// <returns>The score comparison</returns>
    public int Compare(object firstObj, object secondObj) {
        Move firstMove = (Move)firstObj;
        Move secondMove = (Move)secondObj;

        return -firstMove.Score.CompareTo(secondMove.Score);
    }
}