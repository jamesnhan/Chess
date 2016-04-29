using System;

public class Cell {
    private Piece piece;
    private int row;
    private int column;
    
    public Cell() {
        this.row = 0;
        this.column = 0;
    }
    
    public Cell(int row, int column) {
        this.row = row;
        this.column = column;
    }
    
    public Cell(string cell) {
        if (cell.Length == 2) {
            this.column = char.Parse(cell.Substring(0, 1).ToUpper()) - 64;
            this.row = int.Parse(cell.Substring(1, 1));
        }
    }
    
    public bool IsDark {
        get {
            return ((this.row + Column) % 2 == 0);
        }
    }
    
    public override string ToString() {
        string cell = "";

        cell = Convert.ToString(Convert.ToChar(Column + 64));
        cell += this.row.ToString();

        return cell;
    }
    
    public override bool Equals(object obj) {
        if (obj is Cell) {
            Cell cellObj = (Cell)obj;

            return (cellObj.row == this.row && cellObj.Column == Column);
        }
        return false;
    }
    
    public override int GetHashCode() {
        return base.GetHashCode();
    }
    
    public int Row {
        get {
            return this.row;
        }
        set {
            this.row = value;
        }
    }
    
    public bool IsEmpty() {
        return piece == null || piece.Type == Piece.PieceType.Empty;
    }
    
    public bool IsOwnedByEnemy(Cell other) {
        if (IsEmpty())
            return false;
        else
            return piece.Side.type != other.piece.Side.type;
    }
    
    public bool IsOwned(Cell other) {
        if (IsEmpty())
            return false;
        else
            return piece.Side.type == other.piece.Side.type;
    }
    
    public int Column {
        get {
            return this.column;
        }
        set {
            this.column = value;
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
}