/// <summary>
/// This is an individual cell
/// </summary>
using System;

public class Cell {
    /// <summary>
    /// The piece in this cell
    /// </summary>
    private Piece piece;

    /// <summary>
    /// The row of the cell
    /// </summary>
    private int row;

    /// <summary>
    /// The column of the cell
    /// </summary>
    private int column;
    
    /// <summary>
    /// Create a cell at (row, column)
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="column">The column</param>
    public Cell(int row, int column) {
        this.row = row;
        this.column = column;
    }
    
    /// <summary>
    /// The cell as a string in standard algebraic notation
    /// 
    /// "A1" through "H8"
    /// </summary>
    /// <returns>The cell as a string in standard algebraic notation</returns>
    public override string ToString() {
        string cell = "";

        cell = Convert.ToString(Convert.ToChar(this.column + 64));
        cell += this.row.ToString();

        return cell;
    }
    
    /// <summary>
    /// Checks whether or not the cell has a piece or not
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() {
        return (this.piece == null || this.piece.Type == Piece.PieceType.Empty);
    }
    
    /// <summary>
    /// Checks whether or not the cell is owned by the enemy side
    /// </summary>
    /// <param name="other">The other cell</param>
    /// <returns>True if this cell and the other cell are owned by different players</returns>
    public bool IsOwnedByEnemy(Cell other) {
        if (this.IsEmpty()) {
            return false;
        } else {
            return (this.piece.Side.Type != other.piece.Side.Type);
        }
    }
    
    /// <summary>
    /// Checks whether or not the cell is owned by the this cell's owner
    /// </summary>
    /// <param name="other">The other cell</param>
    /// <returns>True if this cell and the other cell are owned by the same player</returns>
    public bool IsOwned(Cell other) {
        if (this.IsEmpty()) {
            return false;
        } else {
            return (this.piece.Side.Type == other.piece.Side.Type);
        }
    }

    /// <summary>
    /// Property for the row
    /// </summary>
    public int Row {
        get {
            return this.row;
        }
        set {
            this.row = value;
        }
    }

    /// <summary>
    /// Property for the column
    /// </summary>
    public int Column {
        get {
            return this.column;
        }
        set {
            this.column = value;
        }
    }
    
    /// <summary>
    /// Property for the piece
    /// </summary>
    public Piece Piece {
        get {
            return this.piece;
        }
        set {
            this.piece = value;
        }
    }
}