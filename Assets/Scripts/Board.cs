using System.Collections;

public class Board {
    private Side whiteSide;
    private Side blackSide;
    private Cells cells;

    public Board() {
        this.whiteSide = new Side(Side.SideType.White);
        this.blackSide = new Side(Side.SideType.Black);

        this.cells = new Cells();
    }
    
    public void Init() {
        this.cells.Clear();
        
        for (int row = 1; row <= 8; row++)
            for (int col = 1; col <= 8; col++) {
                this.cells.Add(new Cell(row, col));
            }
        
        this.cells["A1"].Piece = new Piece(Piece.PieceType.Rook, this.whiteSide);
        this.cells["H1"].Piece = new Piece(Piece.PieceType.Rook, this.whiteSide);
        this.cells["B1"].Piece = new Piece(Piece.PieceType.Knight, this.whiteSide);
        this.cells["G1"].Piece = new Piece(Piece.PieceType.Knight, this.whiteSide);
        this.cells["C1"].Piece = new Piece(Piece.PieceType.Bishop, this.whiteSide);
        this.cells["F1"].Piece = new Piece(Piece.PieceType.Bishop, this.whiteSide);
        this.cells["E1"].Piece = new Piece(Piece.PieceType.King, this.whiteSide);
        this.cells["D1"].Piece = new Piece(Piece.PieceType.Queen, this.whiteSide);
        for (int col = 1; col <= 8; col++)
            this.cells[2, col].Piece = new Piece(Piece.PieceType.Pawn, this.whiteSide);
        
        this.cells["A8"].Piece = new Piece(Piece.PieceType.Rook, this.blackSide);
        this.cells["H8"].Piece = new Piece(Piece.PieceType.Rook, this.blackSide);
        this.cells["B8"].Piece = new Piece(Piece.PieceType.Knight, this.blackSide);
        this.cells["G8"].Piece = new Piece(Piece.PieceType.Knight, this.blackSide);
        this.cells["C8"].Piece = new Piece(Piece.PieceType.Bishop, this.blackSide);
        this.cells["F8"].Piece = new Piece(Piece.PieceType.Bishop, this.blackSide);
        this.cells["E8"].Piece = new Piece(Piece.PieceType.King, this.blackSide);
        this.cells["D8"].Piece = new Piece(Piece.PieceType.Queen, this.blackSide);
        for (int col = 1; col <= 8; col++)
            this.cells[7, col].Piece = new Piece(Piece.PieceType.Pawn, this.blackSide);

        for (int col = 1; col <= 8; ++col) {
            for (int row = 3; row <= 6; ++row) {
                this.cells[row, col].Piece = new Piece();
            }
        }
    }
    
    public Cell this[int row, int col] {
        get {
            return this.cells[row, col];
        }
    }
    
    public Cell this[string cell] {
        get {
            return this.cells[cell];
        }
    }
    
    public Cell this[Cell cell] {
        get {
            return this.cells[cell.ToString()];
        }
    }
    
    public ArrayList GetAllCells() {
        ArrayList CellNames = new ArrayList();
        
        for (int row = 1; row <= 8; row++)
            for (int col = 1; col <= 8; col++) {
                CellNames.Add(this[row, col].ToString());
            }

        return CellNames;
    }
    
    public ArrayList GetSideCell(Side.SideType PlayerSide) {
        ArrayList CellNames = new ArrayList();
        
        for (int row = 1; row <= 8; row++)
            for (int col = 1; col <= 8; col++) {
                if (this[row, col].Piece != null && !this[row, col].IsEmpty() && this[row, col].Piece.Side.type == PlayerSide)
                    CellNames.Add(this[row, col].ToString());
            }

        return CellNames;
    }
    
    public Cell TopCell(Cell cell) {
        return this[cell.Row + 1, cell.Column];
    }
    
    public Cell LeftCell(Cell cell) {
        return this[cell.Row, cell.Column + 1];
    }
    
    public Cell RightCell(Cell cell) {
        return this[cell.Row, cell.Column - 1];
    }
    
    public Cell BottomCell(Cell cell) {
        return this[cell.Row - 1, cell.Column];
    }
    
    public Cell TopLeftCell(Cell cell) {
        return this[cell.Row + 1, cell.Column + 1];
    }
    
    public Cell TopRightCell(Cell cell) {
        return this[cell.Row + 1, cell.Column - 1];
    }
    
    public Cell BottomLeftCell(Cell cell) {
        return this[cell.Row - 1, cell.Column + 1];
    }
    
    public Cell BottomRightCell(Cell cell) {
        return this[cell.Row - 1, cell.Column - 1];
    }
}