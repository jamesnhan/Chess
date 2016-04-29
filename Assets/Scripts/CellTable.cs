using System.Collections;

public class Cells {
    Hashtable cells;

    public Cells() {
        this.cells = new Hashtable();
    }
    
    private string GetKey(Cell newcell) {
        return "" + newcell.Row + newcell.Column;
    }
    
    private string GetKey(int row, int col) {
        return "" + row + col;
    }
    
    public void Add(Cell newcell) {
        this.cells.Add(GetKey(newcell), newcell);
    }
    
    public void Remove(int row, int col) {
        string key = GetKey(row, col);
        if (this.cells.ContainsKey(key)) {
            this.cells.Remove(key);
        }
    }
    
    public void Clear() {
        this.cells.Clear();
    }
    
    public Cell this[int row, int col] {
        get {
            return (Cell)this.cells[GetKey(row, col)];
        }
    }
    
    public Cell this[string cell] {
        get {
            int col = char.Parse(cell.Substring(0, 1).ToUpper()) - 64;
            int row = int.Parse(cell.Substring(1, 1));
            return (Cell)this.cells[GetKey(row, col)];
        }
    }
}