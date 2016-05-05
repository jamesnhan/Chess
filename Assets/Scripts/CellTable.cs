/// <summary>
/// This is a table of cells
/// </summary>
using System.Collections;

public class CellTable {
    /// <summary>
    /// The hashtable of cells
    /// </summary>
    private Hashtable cells;

    /// <summary>
    /// Create an empty cell table
    /// </summary>
    public CellTable() {
        this.cells = new Hashtable();
    }
    
    /// <summary>
    /// Get the key of the cell
    /// </summary>
    /// <param name="cell">The cell</param>
    /// <returns>The string key of the cell</returns>
    private string GetKey(Cell cell) {
        return "" + cell.Row + cell.Column;
    }
    
    /// <summary>
    /// Get the key of a cell coordinate
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="col">The column</param>
    /// <returns>The string key of the cell coordinate</returns>
    private string GetKey(int row, int col) {
        return "" + row + col;
    }
    
    // Add a cell to the cell table
    public void Add(Cell cell) {
        this.cells.Add(this.GetKey(cell), cell);
    }
    
    /// <summary>
    /// Clear the cell table
    /// </summary>
    public void Clear() {
        this.cells.Clear();
    }
    
    /// <summary>
    /// Operator accessor for a cell at a specific cell coordinate
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="col">The column</param>
    /// <returns>The cell at (row, col)</returns>
    public Cell this[int row, int col] {
        get {
            return (Cell)this.cells[this.GetKey(row, col)];
        }
    }
    
    /// <summary>
    /// Operator accessor for a cell at the specified standard algebraic notation coordinate
    /// </summary>
    /// <param name="cell">A cell coordinate in standard algebraic notation</param>
    /// <returns></returns>
    public Cell this[string cell] {
        get {
            int col = char.Parse(cell.Substring(0, 1).ToUpper()) - 64;
            int row = int.Parse(cell.Substring(1, 1));
            return (Cell)this.cells[this.GetKey(row, col)];
        }
    }
}