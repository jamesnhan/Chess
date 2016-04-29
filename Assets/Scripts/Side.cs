public class Side {
    SideType m_side;
    public enum SideType { White, Black };


    public Side() {
    }


    public Side(SideType side) {
        m_side = side;
    }


    public SideType type {
        get {
            return m_side;
        }
        set {
            m_side = value;
        }
    }


    public bool isWhite() {
        return (this.type == SideType.White);
    }


    public bool isBlack() {
        return (this.type == SideType.Black);
    }


    public SideType Enemy() {
        if (this.type == SideType.White)
            return SideType.Black;
        else
            return SideType.White;
    }


    public bool isEnemy(Side other) {
        return (this.type != other.type);
    }
}