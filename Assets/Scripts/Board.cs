/// <summary>
/// This is a board for a chess game
/// </summary>
using System.Collections;

public class Board {
    /// <summary>
    /// The white side object
    /// </summary>
    private Side whiteSide;

    /// <summary>
    /// The black side object
    /// </summary>
    private Side blackSide;

    /// <summary>
    /// A CellTable for all of the board positions
    /// </summary>
    private CellTable cells;

    /// <summary>
    /// Create an empty board
    /// </summary>
    public Board() {
        this.whiteSide = new Side(Side.SideType.White);
        this.blackSide = new Side(Side.SideType.Black);

        this.cells = new CellTable();
    }

    /// <summary>
    /// Initialize the board with the initial pieces
    /// </summary>
    public void Init() {
        this.cells.Clear();

        // Initialize the cell table
        for (int row = 1; row <= 8; row++) {
            for (int col = 1; col <= 8; col++) {
                this.cells.Add(new Cell(row, col));
            }
        }
        
        // Create the back row white pieces
        this.cells["A1"].Piece = new Piece(Piece.PieceType.Rook, this.whiteSide);
        this.cells["H1"].Piece = new Piece(Piece.PieceType.Rook, this.whiteSide);
        this.cells["B1"].Piece = new Piece(Piece.PieceType.Knight, this.whiteSide);
        this.cells["G1"].Piece = new Piece(Piece.PieceType.Knight, this.whiteSide);
        this.cells["C1"].Piece = new Piece(Piece.PieceType.Bishop, this.whiteSide);
        this.cells["F1"].Piece = new Piece(Piece.PieceType.Bishop, this.whiteSide);
        this.cells["E1"].Piece = new Piece(Piece.PieceType.King, this.whiteSide);
        this.cells["D1"].Piece = new Piece(Piece.PieceType.Queen, this.whiteSide);

        // Create the white pawns
        for (int col = 1; col <= 8; col++) {
            this.cells[2, col].Piece = new Piece(Piece.PieceType.Pawn, this.whiteSide);
        }
        
        // Create the back row black pieces
        this.cells["A8"].Piece = new Piece(Piece.PieceType.Rook, this.blackSide);
        this.cells["H8"].Piece = new Piece(Piece.PieceType.Rook, this.blackSide);
        this.cells["B8"].Piece = new Piece(Piece.PieceType.Knight, this.blackSide);
        this.cells["G8"].Piece = new Piece(Piece.PieceType.Knight, this.blackSide);
        this.cells["C8"].Piece = new Piece(Piece.PieceType.Bishop, this.blackSide);
        this.cells["F8"].Piece = new Piece(Piece.PieceType.Bishop, this.blackSide);
        this.cells["E8"].Piece = new Piece(Piece.PieceType.King, this.blackSide);
        this.cells["D8"].Piece = new Piece(Piece.PieceType.Queen, this.blackSide);

        // Create the black pawns
        for (int col = 1; col <= 8; col++) {
            this.cells[7, col].Piece = new Piece(Piece.PieceType.Pawn, this.blackSide);
        }

        // Fill the empty space with empty pieces
        for (int col = 1; col <= 8; ++col) {
            for (int row = 3; row <= 6; ++row) {
                this.cells[row, col].Piece = new Piece();
            }
        }
    }

    /// <summary>
    /// Operator accessor for a specific cell on the board
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="col">The column</param>
    /// <returns>The cell at (row, col)</returns>
    public Cell this[int row, int col] {
        get {
            return this.cells[row, col];
        }
    }

    /// <summary>
    /// Operator accessor for a specific cell on the board by standard algebraic notation
    /// 
    /// "A1" through "H8"
    /// </summary>
    /// <param name="cell">The cell in standard algebraic notation</param>
    /// <returns>The cell defined by the standard algebraic notation</returns>
    public Cell this[string cell] {
        get {
            return this.cells[cell];
        }
    }

    /// <summary>
    /// Operator accessor for a specific cell on the board by cell name
    /// </summary>
    /// <param name="cell">The cell with the name to access</param>
    /// <returns>The cell defined by the cell's name</returns>
    public Cell this[Cell cell] {
        get {
            return this.cells[cell.ToString()];
        }
    }

    /// <summary>
    /// Accessor for all cells controlled by a player side
    /// </summary>
    /// <param name="PlayerSide">The player side white or black</param>
    /// <returns>An ArrayList of all of the cells controlled by the player side</returns>
    public ArrayList GetSideCell(Side.SideType PlayerSide) {
        ArrayList CellNames = new ArrayList();

        for (int row = 1; row <= 8; row++) {
            for (int col = 1; col <= 8; col++) {
                // If the piece in (row, col) is not empty and is owned by the player, add it to the ArrayList
                if (this[row, col].Piece != null && !this[row, col].IsEmpty() && this[row, col].Piece.Side.Type == PlayerSide)
                    CellNames.Add(this[row, col].ToString());
            }
        }

        return CellNames;
    }

    /// <summary>
    /// Get the cell above the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell above the current cell</returns>
    public Cell TopCell(Cell cell) {
        return this[cell.Row + 1, cell.Column];
    }

    /// <summary>
    /// Get the cell to the left of the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell to the left of the current cell</returns>
    public Cell LeftCell(Cell cell) {
        return this[cell.Row, cell.Column - 1];
    }

    /// <summary>
    /// Get the cell to the right of the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell to the right of the current cell</returns>
    public Cell RightCell(Cell cell) {
        return this[cell.Row, cell.Column + 1];
    }

    /// <summary>
    /// Get the cell below the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell below the current cell</returns>
    public Cell BottomCell(Cell cell) {
        return this[cell.Row - 1, cell.Column];
    }

    /// <summary>
    /// Get the cell to the top left of the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell to the top left of the current cell</returns>
    public Cell TopLeftCell(Cell cell) {
        return this[cell.Row + 1, cell.Column - 1];
    }

    /// <summary>
    /// Get the cell to the top right of the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell to the top right of the current cell</returns>
    public Cell TopRightCell(Cell cell) {
        return this[cell.Row + 1, cell.Column + 1];
    }

    /// <summary>
    /// Get the cell to the bottom right the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell to the bottom right the current cell</returns>
    public Cell BottomLeftCell(Cell cell) {
        return this[cell.Row - 1, cell.Column - 1];
    }

    /// <summary>
    /// Get the cell to the bottom right of the current cell
    /// </summary>
    /// <param name="cell">The current cell</param>
    /// <returns>The cell to the bottom right of the current cell</returns>
    public Cell BottomRightCell(Cell cell) {
        return this[cell.Row - 1, cell.Column + 1];
    }

    /// <summary>
    /// Property for the cell table
    /// </summary>
    /// <returns>The cell table</returns>
    public CellTable CellTable {
        get {
            return this.cells;
        }
    }
}