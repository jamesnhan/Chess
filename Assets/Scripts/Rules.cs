using System;
using System.Collections;

public class Rules {
    private Board m_Board;
    private Game m_Game;
    public Rules(Board board, Game game) {
        m_Board = board;
        m_Game = game;
    }
    public Board ChessBoard {
        get {
            return m_Board;
        }
    }
    public Game ChessGame {
        get {
            return m_Game;
        }
    }
    public bool IsCheckMate(Side.SideType PlayerSide) {
        if (IsUnderCheck(PlayerSide) && GetCountOfPossibleMoves(PlayerSide) == 0)
            return true;
        else
            return false;
    }
    public bool IsStaleMate(Side.SideType PlayerSide) {
        Stack moves = this.m_Game.MoveHistory.Clone() as Stack;
        bool captureOrPawnMove = false;
        int count = 0;

        while (moves.Count > 0) {
            Move move = moves.Pop() as Move;
            ++count;
            captureOrPawnMove = captureOrPawnMove || (move.IsCaptureMove() || move.Piece.IsPawn());
            if (count >= 50 && !captureOrPawnMove) {
                return true;
            } else if (count >= 50 || captureOrPawnMove) {
                break;
            }
        }

        if (!IsUnderCheck(PlayerSide) && GetCountOfPossibleMoves(PlayerSide) == 0)
            return true;
        else
            return false;
    }
    public int DoMove(Move move) {

        ArrayList LegalMoves = GetLegalMoves(m_Board[move.StartCell]);

        if (!LegalMoves.Contains(m_Board[move.EndCell]))
            return -2;
        SetMoveType(move);
        ExecuteMove(move);

        return 0;
    }
    public void ExecuteMove(Move move) {

        switch (move.Type) {
            case Move.MoveType.CaptureMove:
                DoNormalMove(move);
                break;

            case Move.MoveType.NormalMove:
                DoNormalMove(move);
                break;

            case Move.MoveType.TowerMove:
                DoTowerMove(move);
                break;

            case Move.MoveType.PromotionMove:
                DoPromoMove(move);
                break;

            case Move.MoveType.EnPassant:
                DoEnPassantMove(move);
                break;
        }
    }

    private void SetMoveType(Move move) {

        move.Type = Move.MoveType.NormalMove;
        if (move.EndCell.Piece != null && move.EndCell.Piece.Type != Piece.PieceType.Empty)
            move.Type = Move.MoveType.CaptureMove;
        if (move.StartCell.Piece != null && move.StartCell.Piece.Type == Piece.PieceType.King) {
            if (Math.Abs(move.EndCell.Column - move.StartCell.Column) > 1)
                move.Type = Move.MoveType.TowerMove;
        }
        if (move.StartCell.Piece != null && move.StartCell.Piece.Type == Piece.PieceType.Pawn) {

            if (move.EndCell.Row == 8 || move.EndCell.Row == 1)
                move.Type = Move.MoveType.PromotionMove;
        }
        if (move.StartCell.Piece != null && move.StartCell.Piece.Type == Piece.PieceType.Pawn) {

            if ((move.EndCell.Piece == null || move.EndCell.Piece.IsEmpty()) && move.StartCell.Column != move.EndCell.Column)
                move.Type = Move.MoveType.EnPassant;
        }
    }

    private void DoNormalMove(Move move) {
        m_Board[move.StartCell].Piece.MoveCount++;
        m_Board[move.EndCell].Piece = m_Board[move.StartCell].Piece;
        m_Board[move.StartCell].Piece = new Piece(Piece.PieceType.Empty);
    }
    private void DoTowerMove(Move move) {
        DoNormalMove(move);
        if (move.EndCell.Column > move.StartCell.Column) {
            Cell rockcell = m_Board.RightCell(move.EndCell);
            Move newmove = new Move(rockcell, m_Board.LeftCell(move.EndCell));
            DoNormalMove(newmove);
        } else {

            Cell rockcell = m_Board.LeftCell(move.EndCell);
            rockcell = m_Board.LeftCell(rockcell);
            Move newmove = new Move(rockcell, m_Board.RightCell(move.EndCell));
            DoNormalMove(newmove);
        }
    }
    private void DoPromoMove(Move move) {
        DoNormalMove(move);

        if (move.PromotedPiece == null)
            m_Board[move.EndCell].Piece = new Piece(Piece.PieceType.Queen, m_Board[move.EndCell].Piece.Side);
        else
            m_Board[move.EndCell].Piece = move.PromotedPiece;
    }
    private void DoEnPassantMove(Move move) {
        Cell EnPassantCell;

        if (move.StartCell.Piece.Side.isWhite())
            EnPassantCell = m_Board.BottomCell(move.EndCell);
        else
            EnPassantCell = m_Board.TopCell(move.EndCell);

        move.EnPassantPiece = EnPassantCell.Piece;
        EnPassantCell.Piece = new Piece(Piece.PieceType.Empty);
        DoNormalMove(move);
    }
    public void UndoMove(Move move) {
        if (move.Type == Move.MoveType.CaptureMove || move.Type == Move.MoveType.NormalMove || move.Type == Move.MoveType.PromotionMove)
            UndoNormalMove(move);
        if (move.Type == Move.MoveType.TowerMove) {
            UndoNormalMove(move);
            if (move.EndCell.Column > move.StartCell.Column) {

                Cell source = m_Board.LeftCell(move.EndCell);
                Cell target = m_Board[move.StartCell.Row, 8];

                m_Board[source].Piece.MoveCount--;
                m_Board[target].Piece = m_Board[source].Piece;
                m_Board[source].Piece = new Piece(Piece.PieceType.Empty);
            } else {

                Cell source = m_Board.RightCell(move.EndCell);
                Cell target = m_Board[move.StartCell.Row, 1];

                m_Board[source].Piece.MoveCount--;
                m_Board[target].Piece = m_Board[source].Piece;
                m_Board[source].Piece = new Piece(Piece.PieceType.Empty);
            }
        }
        if (move.Type == Move.MoveType.EnPassant) {
            Cell EnPassantCell;

            UndoNormalMove(move);
            if (move.StartCell.Piece.Side.isWhite())
                EnPassantCell = m_Board.BottomCell(move.EndCell);
            else
                EnPassantCell = m_Board.TopCell(move.EndCell);

            EnPassantCell.Piece = move.EnPassantPiece;
        }
    }
    private void UndoNormalMove(Move move) {
        m_Board[move.EndCell].Piece = move.CapturedPiece;
        m_Board[move.StartCell].Piece = move.Piece;
        m_Board[move.StartCell].Piece.MoveCount--;
    }
    public bool IsUnderCheck(Side.SideType PlayerSide) {
        Cell OwnerKingCell = null;
        ArrayList OwnerCells = m_Board.GetSideCell(PlayerSide);
        foreach (string CellName in OwnerCells) {
            if (m_Board[CellName].Piece.Type == Piece.PieceType.King) {
                OwnerKingCell = m_Board[CellName];
                break;
            }
        }
        ArrayList EnemyCells = m_Board.GetSideCell((new Side(PlayerSide)).Enemy());
        foreach (string CellName in EnemyCells) {
            ArrayList moves = GetPossibleMoves(m_Board[CellName]);

            if (moves.Contains(OwnerKingCell))
                return true;
        }
        return false;
    }
    private int GetCountOfPossibleMoves(Side.SideType PlayerSide) {
        int TotalMoves = 0;
        ArrayList PlayerCells = m_Board.GetSideCell(PlayerSide);
        foreach (string CellName in PlayerCells) {
            ArrayList moves = GetLegalMoves(m_Board[CellName]);
            TotalMoves += moves.Count;
        }
        return TotalMoves;
    }
    private bool CauseCheck(Move move) {
        bool CauseCheck = false;
        Side.SideType PlayerSide = move.StartCell.Piece.Side.type;
        ExecuteMove(move);
        CauseCheck = IsUnderCheck(PlayerSide);
        UndoMove(move);

        return CauseCheck;
    }
    public ArrayList GetLegalMoves(Cell source) {
        ArrayList LegalMoves;

        LegalMoves = GetPossibleMoves(source);
        ArrayList ToRemove = new ArrayList();
        foreach (Cell target in LegalMoves) {

            if (CauseCheck(new Move(source, target)))
                ToRemove.Add(target);
        }

        if (source.Piece.Type == Piece.PieceType.King && IsUnderCheck(source.Piece.Side.type)) {
            foreach (Cell target in LegalMoves) {

                if (Math.Abs(target.Column - source.Column) > 1)
                    ToRemove.Add(target);
            }
        }
        foreach (Cell cell in ToRemove) {
            LegalMoves.Remove(cell);
        }
        return LegalMoves;
    }

    public ArrayList GenerateAllLegalMoves(Side PlayerSide) {
        ArrayList TotalMoves = new ArrayList();
        ArrayList PlayerCells = m_Board.GetSideCell(PlayerSide.type);
        Move move;
        foreach (string CellName in PlayerCells) {
            ArrayList moves = GetLegalMoves(m_Board[CellName]);

            foreach (Cell dest in moves) {
                move = new Move(m_Board[CellName], dest);
                SetMoveType(move);

                if (move.IsPromoMove())
                    move.Score = 1000;
                else if (move.IsCaptureMove())
                    move.Score = move.EndCell.Piece.GetWeight();

                TotalMoves.Add(move);
            }
        }

        MoveCompare moveCompareObj = new MoveCompare();
        TotalMoves.Sort(moveCompareObj);

        return TotalMoves;
    }

    public ArrayList GenerateGoodCaptureMoves(Side PlayerSide) {
        ArrayList TotalMoves = new ArrayList();
        ArrayList PlayerCells = m_Board.GetSideCell(PlayerSide.type);
        Move move;
        foreach (string CellName in PlayerCells) {

            if (m_Board[CellName].Piece.GetWeight() > 100) {
                ArrayList moves = GetLegalMoves(m_Board[CellName]);

                foreach (Cell dest in moves) {

                    if (dest.Piece != null && !dest.Piece.IsEmpty()) {
                        move = new Move(m_Board[CellName], dest);

                        TotalMoves.Add(move);
                    }
                }
            }
        }
        return TotalMoves;
    }

    public ArrayList GetPossibleMoves(Cell source) {
        ArrayList LegalMoves = new ArrayList();
        switch (source.Piece.Type) {
            case Piece.PieceType.Empty:
                break;

            case Piece.PieceType.Pawn:
                GetPawnMoves(source, LegalMoves);
                break;

            case Piece.PieceType.Knight:
                GetKnightMoves(source, LegalMoves);
                break;

            case Piece.PieceType.Rook:
                GetRookMoves(source, LegalMoves);
                break;

            case Piece.PieceType.Bishop:
                GetBishopMoves(source, LegalMoves);
                break;

            case Piece.PieceType.Queen:
                GetQueenMoves(source, LegalMoves);
                break;

            case Piece.PieceType.King:
                GetKingMoves(source, LegalMoves);
                break;
        }

        return LegalMoves;
    }
    private Move LastMoveWasPawnBegin() {

        Move lastmove = m_Game.GetLastMove();

        if (lastmove != null) {
            if (lastmove.Piece.IsPawn() && lastmove.Piece.MoveCount == 1) {
                return lastmove;
            }
        }
        return null;
    }
    private void GetPawnMoves(Cell source, ArrayList moves) {
        Cell newcell;

        if (source.Piece.Side.isWhite()) {

            newcell = m_Board.TopCell(source);
            if (newcell != null && newcell.IsEmpty())
                moves.Add(newcell);
            if (newcell != null && newcell.IsEmpty()) {
                newcell = m_Board.TopCell(newcell);
                if (newcell != null && source.Piece.MoveCount == 0 && newcell.IsEmpty())
                    moves.Add(newcell);
            }
            newcell = m_Board.TopLeftCell(source);
            if (newcell != null && newcell.IsOwnedByEnemy(source))
                moves.Add(newcell);
            newcell = m_Board.TopRightCell(source);
            if (newcell != null && newcell.IsOwnedByEnemy(source))
                moves.Add(newcell);
            Move LastPawnMove = LastMoveWasPawnBegin();

            if (LastPawnMove != null) {
                if (source.Row == LastPawnMove.EndCell.Row) {
                    if (LastPawnMove.EndCell.Column == source.Column - 1) {
                        newcell = m_Board.TopLeftCell(source);
                        if (newcell != null && newcell.IsEmpty())
                            moves.Add(newcell);
                    }

                    if (LastPawnMove.EndCell.Column == source.Column + 1) {
                        newcell = m_Board.TopRightCell(source);
                        if (newcell != null && newcell.IsEmpty())
                            moves.Add(newcell);
                    }
                }
            }
        } else {

            newcell = m_Board.BottomCell(source);
            if (newcell != null && newcell.IsEmpty())
                moves.Add(newcell);
            if (newcell != null && newcell.IsEmpty()) {
                newcell = m_Board.BottomCell(newcell);
                if (newcell != null && source.Piece.MoveCount == 0 && newcell.IsEmpty())
                    moves.Add(newcell);
            }
            newcell = m_Board.BottomLeftCell(source);
            if (newcell != null && newcell.IsOwnedByEnemy(source))
                moves.Add(newcell);
            newcell = m_Board.BottomRightCell(source);
            if (newcell != null && newcell.IsOwnedByEnemy(source))
                moves.Add(newcell);
            Move LastPawnMove = LastMoveWasPawnBegin();

            if (LastPawnMove != null) {
                if (source.Row == LastPawnMove.EndCell.Row) {
                    if (LastPawnMove.EndCell.Column == source.Column - 1) {
                        newcell = m_Board.BottomLeftCell(source);
                        if (newcell != null && newcell.IsEmpty())
                            moves.Add(newcell);
                    }

                    if (LastPawnMove.EndCell.Column == source.Column + 1) {
                        newcell = m_Board.BottomRightCell(source);
                        if (newcell != null && newcell.IsEmpty())
                            moves.Add(newcell);
                    }
                }
            }
        }
    }
    private void GetKnightMoves(Cell source, ArrayList moves) {
        Cell newcell;
        newcell = m_Board.TopCell(source);
        if (newcell != null) {
            newcell = m_Board.TopLeftCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);

            newcell = m_Board.TopCell(source);
            newcell = m_Board.TopRightCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);
        }

        newcell = m_Board.BottomCell(source);
        if (newcell != null) {
            newcell = m_Board.BottomLeftCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);

            newcell = m_Board.BottomCell(source);
            newcell = m_Board.BottomRightCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);
        }

        newcell = m_Board.LeftCell(source);
        if (newcell != null) {
            newcell = m_Board.TopLeftCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);

            newcell = m_Board.LeftCell(source);
            newcell = m_Board.BottomLeftCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);
        }

        newcell = m_Board.RightCell(source);
        if (newcell != null) {
            newcell = m_Board.TopRightCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);

            newcell = m_Board.RightCell(source);
            newcell = m_Board.BottomRightCell(newcell);

            if (newcell != null && !newcell.IsOwned(source))
                moves.Add(newcell);
        }
    }
    private void GetRookMoves(Cell source, ArrayList moves) {
        Cell newcell;
        newcell = m_Board.TopCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.TopCell(newcell);
        }
        newcell = m_Board.LeftCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.LeftCell(newcell);
        }
        newcell = m_Board.RightCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.RightCell(newcell);
        }
        newcell = m_Board.BottomCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.BottomCell(newcell);
        }
    }
    private void GetBishopMoves(Cell source, ArrayList moves) {
        Cell newcell;
        newcell = m_Board.TopLeftCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.TopLeftCell(newcell);
        }
        newcell = m_Board.TopRightCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.TopRightCell(newcell);
        }
        newcell = m_Board.BottomLeftCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.BottomLeftCell(newcell);
        }
        newcell = m_Board.BottomRightCell(source);
        while (newcell != null) {
            if (newcell.IsEmpty())
                moves.Add(newcell);

            if (newcell.IsOwnedByEnemy(source)) {
                moves.Add(newcell);
                break;
            }

            if (newcell.IsOwned(source))
                break;

            newcell = m_Board.BottomRightCell(newcell);
        }
    }
    private void GetQueenMoves(Cell source, ArrayList moves) {

        GetRookMoves(source, moves);
        GetBishopMoves(source, moves);
    }
    private void GetKingMoves(Cell source, ArrayList moves) {
        Cell newcell;
        newcell = m_Board.TopCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);

        newcell = m_Board.LeftCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);

        newcell = m_Board.RightCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);

        newcell = m_Board.BottomCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);

        newcell = m_Board.TopLeftCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);

        newcell = m_Board.TopRightCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);

        newcell = m_Board.BottomLeftCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);

        newcell = m_Board.BottomRightCell(source);
        if (newcell != null && !newcell.IsOwned(source))
            moves.Add(newcell);
        if (m_Board[source].Piece.MoveCount == 0) {
            Cell CastlingTarget = null;
            newcell = m_Board.RightCell(source);
            if (newcell != null && newcell.IsEmpty()) {
                if (!CauseCheck(new Move(source, newcell))) {
                    newcell = m_Board.RightCell(newcell);
                    if (newcell != null && newcell.IsEmpty()) {
                        CastlingTarget = newcell;
                        newcell = m_Board.RightCell(newcell);
                        if (newcell != null && !newcell.IsEmpty() && newcell.Piece.MoveCount == 0)
                            moves.Add(CastlingTarget);
                    }
                }
            }
            newcell = m_Board.LeftCell(source);
            if (newcell != null && newcell.IsEmpty()) {
                if (!CauseCheck(new Move(source, newcell))) {
                    newcell = m_Board.LeftCell(newcell);
                    if (newcell != null && newcell.IsEmpty()) {
                        CastlingTarget = newcell;
                        newcell = m_Board.LeftCell(newcell);
                        if (newcell != null && newcell.IsEmpty()) {
                            newcell = m_Board.LeftCell(newcell);
                            if (newcell != null && !newcell.IsEmpty() && newcell.Piece.MoveCount == 0)
                                moves.Add(CastlingTarget);
                        }
                    }

                }
            }
        }
    }
    public int AnalyzeBoard(Side.SideType PlayerSide) {
        int Score = 0;
        ArrayList OwnerCells = m_Board.GetSideCell(PlayerSide);
        foreach (string ChessCell in OwnerCells) {
            Score += m_Board[ChessCell].Piece.GetWeight();
        }

        return Score;
    }
    public int Evaluate(Side PlayerSide) {
        int Score = 0;

        Score = AnalyzeBoard(PlayerSide.type) - AnalyzeBoard(PlayerSide.Enemy()) - 25;

        if (IsCheckMate(PlayerSide.Enemy()))
            Score = 1000000;

        return Score;
    }

}