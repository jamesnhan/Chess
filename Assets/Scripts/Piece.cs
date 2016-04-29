public class Piece {
    int numMoves;
    Side side;
    PieceType type;

    public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King, Empty };
    public Piece() {
        this.Type = PieceType.Empty;
    }

    public Piece(PieceType type) {
        this.type = type;
    }

    public Piece(PieceType type, Side side) {
        this.type = type;
        this.side = side;
    }

    public bool IsEmpty() {
        return type == PieceType.Empty;
    }

    public bool IsPawn() {
        return type == PieceType.Pawn;
    }

    public bool IsKnight() {
        return type == PieceType.Knight;
    }

    public bool IsBishop() {
        return type == PieceType.Bishop;
    }

    public bool IsRook() {
        return type == PieceType.Rook;
    }

    public bool IsQueen() {
        return type == PieceType.Queen;
    }

    public bool IsKing() {
        return type == PieceType.King;
    }

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

    public PieceType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }

    public Side Side {
        get {
            return side;
        }
        set {
            side = value;
        }
    }

    public int MoveCount {
        get {
            return numMoves;
        }
        set {
            numMoves = value;
        }
    }
}