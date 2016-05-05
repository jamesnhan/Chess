/// <summary>
/// A chess piece
/// </summary>
public class Piece {
    /// <summary>
    /// An enum of types of chess pieces
    /// </summary>
    public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King, Empty };

    /// <summary>
    /// The number of moves this piece has made
    /// </summary>
    int numMoves;

    /// <summary>
    /// The side this piece belongs to
    /// </summary>
    Side side;
    
    /// <summary>
    /// The type of piece
    /// </summary>
    PieceType type;

    /// <summary>
    /// Create an empty piece
    /// </summary>
    public Piece() {
        this.Type = PieceType.Empty;
    }

    /// <summary>
    /// Create a specific piece
    /// </summary>
    /// <param name="type">The type</param>
    public Piece(PieceType type) {
        this.type = type;
    }

    /// <summary>
    /// Create a specific piece on a specific side
    /// </summary>
    /// <param name="type">The type</param>
    /// <param name="side">The side</param>
    public Piece(PieceType type, Side side) {
        this.type = type;
        this.side = side;
    }

    /// <summary>
    /// Checks whether or not the piece is empty
    /// </summary>
    /// <returns>True if the piece is empty</returns>
    public bool IsEmpty() {
        return type == PieceType.Empty;
    }

    /// <summary>
    /// Checks whether or not the piece is a pawn
    /// </summary>
    /// <returns>True if the piece is a pawn</returns>
    public bool IsPawn() {
        return type == PieceType.Pawn;
    }

    /// <summary>
    /// Checks whether or not the piece is a knight
    /// </summary>
    /// <returns>True if the piece is a knight</returns>
    public bool IsKnight() {
        return type == PieceType.Knight;
    }

    /// <summary>
    /// Checks whether or not the piece is a bishop
    /// </summary>
    /// <returns>True if the piece is a bishop</returns>
    public bool IsBishop() {
        return type == PieceType.Bishop;
    }

    /// <summary>
    /// Checks whether or not the piece is a rook
    /// </summary>
    /// <returns>True if the piece is a rook</returns>
    public bool IsRook() {
        return type == PieceType.Rook;
    }

    /// <summary>
    /// Checks whether or not the piece is a queen
    /// </summary>
    /// <returns>True if the piece is a queen</returns>
    public bool IsQueen() {
        return type == PieceType.Queen;
    }

    /// <summary>
    /// Checks whether or not the piece is a king
    /// </summary>
    /// <returns>True if the piece is a king</returns>
    public bool IsKing() {
        return type == PieceType.King;
    }

    /// <summary>
    /// The piece as a string
    /// </summary>
    /// <returns>The piece name as as tring</returns>
    public override string ToString() {
        switch (type) {
            case PieceType.King:
                return "King";
            case PieceType.Queen:
                return "Queen";
            case PieceType.Bishop:
                return "Bishop";
            case PieceType.Rook:
                return "Rook";
            case PieceType.Knight:
                return "Knight";
            case PieceType.Pawn:
                return "Pawn";
            default:
                return "Empty";
        }
    }

    /// <summary>
    /// Get the weight of a piece for AI purposes
    /// </summary>
    /// <returns>The weight of a piece</returns>
    public int GetWeight() {
        switch (type) {
            case PieceType.King:
                return 0;
            case PieceType.Queen:
                return 900;
            case PieceType.Rook:
                return 500;
            case PieceType.Bishop:
                return 325;
            case PieceType.Knight:
                return 300;
            case PieceType.Pawn:
                return 100;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Property for the type
    /// </summary>
    public PieceType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }

    /// <summary>
    /// Property for the side
    /// </summary>
    public Side Side {
        get {
            return side;
        }
        set {
            side = value;
        }
    }

    /// <summary>
    /// Property for the move count
    /// </summary>
    public int MoveCount {
        get {
            return numMoves;
        }
        set {
            numMoves = value;
        }
    }
}