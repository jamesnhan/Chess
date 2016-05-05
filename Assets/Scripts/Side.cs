/// <summary>
/// A player side in chess
/// </summary>
/// 
public class Side {
    /// <summary>
    /// A side type enum for white or black
    /// </summary>
    public enum SideType { White, Black };

    /// <summary>
    /// The side type of this side
    /// </summary>
    private SideType side;

    /// <summary>
    /// Create a side from a side type
    /// </summary>
    /// <param name="side"></param>
    public Side(SideType side) {
        this.side = side;
    }

    /// <summary>
    /// Property for the side type
    /// </summary>
    public SideType Type {
        get {
            return this.side;
        }
        set {
            this.side = value;
        }
    }
    
    /// <summary>
    /// Check whether or not this side is white
    /// </summary>
    /// <returns>True if this side is white</returns>
    public bool IsWhite() {
        return (this.Type == SideType.White);
    }

    /// <summary>
    /// Check whether or not this side is black
    /// </summary>
    /// <returns>True if this side is black</returns>
    public bool IsBlack() {
        return (this.Type == SideType.Black);
    }

    /// <summary>
    /// Get the enemy side
    /// </summary>
    /// <returns>The enemy side of this side</returns>
    public SideType Enemy() {
        if (this.Type == SideType.White) {
            return SideType.Black;
        } else {
            return SideType.White;
        }
    }
}