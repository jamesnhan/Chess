/// <summary>
/// The rules of a chess game
/// </summary>
using System;
using System.Collections;

public class Rules {
    /// <summary>
    /// The game board
    /// </summary>
    private Board board;

    /// <summary>
    /// The game instance
    /// </summary>
    private Game game;

    /// <summary>
    /// Create a new set of rules for this board and game
    /// </summary>
    /// <param name="board">The board</param>
    /// <param name="game">The game</param>
    public Rules(Board board, Game game) {
        this.board = board;
        this.game = game;
    }

    /// <summary>
    /// Check if the player is under checkmate
    /// </summary>
    /// <param name="playerSide">The player</param>
    /// <returns>True if the player is in checkmate</returns>
    public bool IsCheckMate(Side.SideType playerSide) {
        if (this.IsUnderCheck(playerSide) && this.GetCountOfPossibleMoves(playerSide) == 0) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Check if the player is under stalemate
    /// </summary>
    /// <param name="playerSide">The player</param>
    /// <returns>True if the player is in stalemate</returns>
    public bool IsStaleMate(Side.SideType playerSide) {
        if (!this.IsUnderCheck(playerSide) && this.GetCountOfPossibleMoves(playerSide) == 0) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Check if the players are under fifty move draw
    /// </summary>
    /// <returns>True if the players are under fifty move draw</returns>
    public bool IsFiftyMoveDraw() {
        // Clone the current move history for iteration
        Stack moves = this.game.MoveHistory.Clone() as Stack;
        bool captureOrPawnMove = false;
        int count = 0;

        // Iterate through the past moves
        while (moves.Count > 0) {
            Move move = moves.Pop() as Move;
            ++count;
            // If the move is a capture move or the moved piece is a pawn, set it to true
            captureOrPawnMove = captureOrPawnMove || (move.IsCaptureMove() || move.Piece.IsPawn());
            // If we have gone through over 50 moves and not had a capture or pawn move we draw
            if (count >= 50 && !captureOrPawnMove) {
                return true;
            } else if (captureOrPawnMove) {
                // If we have a capture or pawn move, break
                break;
            }
        }

        return false;
    }

    /// <summary>
    /// Make a move
    /// </summary>
    /// <param name="move"></param>
    /// <returns>0 if successful, -2 if the move is illegal</returns>
    public int DoMove(Move move) {
        // Get the list of legal moves
        ArrayList legalMoves = this.GetLegalMoves(board[move.StartCell]);

        // If the move is not in the list of legal moves return -2
        if (!legalMoves.Contains(board[move.EndCell])) {
            return -2;
        }

        // Set the move type and execute it
        this.SetMoveType(move);
        this.ExecuteMove(move);

        return 0;
    }

    /// <summary>
    /// Execute a move
    /// </summary>
    /// <param name="move">The move</param>
    public void ExecuteMove(Move move) {
        switch (move.Type) {
            case Move.MoveType.CaptureMove:
                this.DoNormalMove(move);
                break;
            case Move.MoveType.NormalMove:
                this.DoNormalMove(move);
                break;
            case Move.MoveType.TowerMove:
                this.DoTowerMove(move);
                break;
            case Move.MoveType.PromotionMove:
                this.DoPromotionMove(move);
                break;
            case Move.MoveType.EnPassant:
                this.DoEnPassantMove(move);
                break;
        }
    }

    /// <summary>
    /// Set the move type and move information
    /// </summary>
    /// <param name="move">The move</param>
    private void SetMoveType(Move move) {
        // The move starts out as a normal move
        move.Type = Move.MoveType.NormalMove;

        // If the move would kill a piece, make it a capture type
        if (move.EndCell.Piece != null && move.EndCell.Piece.Type != Piece.PieceType.Empty) {
            move.Type = Move.MoveType.CaptureMove;
        }

        // If the move is a king and it is moving more than one space, it's a tower move
        if (move.StartCell.Piece != null && move.StartCell.Piece.Type == Piece.PieceType.King) {
            if (Math.Abs(move.EndCell.Column - move.StartCell.Column) > 1) {
                move.Type = Move.MoveType.TowerMove;
            }
        }

        // If the move is a pawn and it is at the end row, it's a promotion move
        if (move.StartCell.Piece != null && move.StartCell.Piece.Type == Piece.PieceType.Pawn) {
            if (move.EndCell.Row == 8 || move.EndCell.Row == 1) {
                move.Type = Move.MoveType.PromotionMove;
            }
        }

        // If the piece is a move and it is changing columns and the ending space is empty, it's en passant
        if (move.StartCell.Piece != null && move.StartCell.Piece.Type == Piece.PieceType.Pawn) {
            if ((move.EndCell.Piece == null || move.EndCell.Piece.IsEmpty()) && move.StartCell.Column != move.EndCell.Column) {
                move.Type = Move.MoveType.EnPassant;
            }
        }
    }

    /// <summary>
    /// Do a normal move
    /// </summary>
    /// <param name="move">The move</param>
    private void DoNormalMove(Move move) {
        this.board[move.StartCell].Piece.MoveCount++;
        this.board[move.EndCell].Piece = this.board[move.StartCell].Piece;
        this.board[move.StartCell].Piece = new Piece(Piece.PieceType.Empty);

        // Update the pieces
        this.game.GetBoardSquare(move.StartCell.ToString()).UpdatePiece();
        this.game.GetBoardSquare(move.EndCell.ToString()).UpdatePiece();
    }

    /// <summary>
    /// Do a tower move
    /// </summary>
    /// <param name="move">The move</param>
    private void DoTowerMove(Move move) {
        // First do a normal move to move the king
        this.DoNormalMove(move);

        // Move the corresponding rook
        if (move.EndCell.Column > move.StartCell.Column) {
            Cell rookCell = this.board.RightCell(move.EndCell);
            Move towerMove = new Move(rookCell, this.board.LeftCell(move.EndCell));
            this.DoNormalMove(towerMove);

            // Update the rook cells
            this.game.GetBoardSquare(towerMove.StartCell.ToString()).UpdatePiece();
            this.game.GetBoardSquare(towerMove.EndCell.ToString()).UpdatePiece();
        } else {
            // The left rook is further from the king by one cell
            Cell rockcell = this.board.LeftCell(move.EndCell);
            rockcell = this.board.LeftCell(rockcell);
            Move towerMove = new Move(rockcell, this.board.RightCell(move.EndCell));
            this.DoNormalMove(towerMove);

            // Update the rook cells
            this.game.GetBoardSquare(towerMove.StartCell.ToString()).UpdatePiece();
            this.game.GetBoardSquare(towerMove.EndCell.ToString()).UpdatePiece();
        }
    }

    /// <summary>
    /// Do a promotion move
    /// </summary>
    /// <param name="move">The move</param>
    private void DoPromotionMove(Move move) {
        this.DoNormalMove(move);

        if (move.PromotedPiece == null) {
            this.board[move.EndCell].Piece = new Piece(Piece.PieceType.Queen, this.board[move.EndCell].Piece.Side);
            this.game.GetBoardSquare(move.EndCell.ToString()).UpdatePiece();
        } else {
            this.board[move.EndCell].Piece = move.PromotedPiece;
            this.game.GetBoardSquare(move.EndCell.ToString()).UpdatePiece();
        }
    }

    /// <summary>
    /// Do an enpassant move
    /// </summary>
    /// <param name="move">The move</param>
    private void DoEnPassantMove(Move move) {
        Cell enPassantCell;

        if (move.StartCell.Piece.Side.IsWhite()) {
            enPassantCell = this.board.BottomCell(move.EndCell);
        } else {
            enPassantCell = this.board.TopCell(move.EndCell);
        }

        // Update the cell
        this.game.GetBoardSquare(move.EndCell.ToString()).UpdatePiece();

        move.EnPassantPiece = enPassantCell.Piece;
        enPassantCell.Piece = new Piece(Piece.PieceType.Empty);
        this.DoNormalMove(move);
    }

    /// <summary>
    /// Undo a move
    /// </summary>
    /// <param name="move">The move</param>
    public void UndoMove(Move move) {
        // If it is a capture, normal, or promotion move, first undo the underlying normal move
        if (move.Type == Move.MoveType.CaptureMove || move.Type == Move.MoveType.NormalMove || move.Type == Move.MoveType.PromotionMove) {
            this.UndoNormalMove(move);
        }

        // If the move is a tower move, undo the underlying normal move and then move the rook back
        if (move.Type == Move.MoveType.TowerMove) {
            this.UndoNormalMove(move);

            if (move.EndCell.Column > move.StartCell.Column) {
                Cell source = this.board.LeftCell(move.EndCell);
                Cell target = this.board[move.StartCell.Row, 8];

                this.board[source].Piece.MoveCount--;
                this.board[target].Piece = board[source].Piece;
                this.board[source].Piece = new Piece(Piece.PieceType.Empty);

                // Update the rooks
                this.game.GetBoardSquare(this.board[source].ToString()).UpdatePiece();
                this.game.GetBoardSquare(this.board[target].ToString()).UpdatePiece();
            } else {
                Cell source = this.board.RightCell(move.EndCell);
                Cell target = this.board[move.StartCell.Row, 1];

                this.board[source].Piece.MoveCount--;
                this.board[target].Piece = board[source].Piece;
                this.board[source].Piece = new Piece(Piece.PieceType.Empty);

                // Update the rooks
                this.game.GetBoardSquare(this.board[source].ToString()).UpdatePiece();
                this.game.GetBoardSquare(this.board[target].ToString()).UpdatePiece();
            }
        }

        // If the move is en passant undo the normal move
        if (move.Type == Move.MoveType.EnPassant) {
            Cell enPassantCell;

            this.UndoNormalMove(move);
            if (move.StartCell.Piece.Side.IsWhite()) {
                enPassantCell = this.board.BottomCell(move.EndCell);
            } else {
                enPassantCell = this.board.TopCell(move.EndCell);
            }

            // Update the cell
            this.game.GetBoardSquare(move.EndCell.ToString()).UpdatePiece();

            enPassantCell.Piece = move.EnPassantPiece;
        }
    }

    /// <summary>
    /// Undo a normal move
    /// </summary>
    /// <param name="move">The move</param>
    private void UndoNormalMove(Move move) {
        this.board[move.EndCell].Piece = move.CapturedPiece;
        this.board[move.StartCell].Piece = move.Piece;
        this.board[move.StartCell].Piece.MoveCount--;

        // Update the cells
        this.game.GetBoardSquare(move.StartCell.ToString()).UpdatePiece();
        this.game.GetBoardSquare(move.EndCell.ToString()).UpdatePiece();
    }

    /// <summary>
    /// Check if a player is under check
    /// </summary>
    /// <param name="playerSide">The player</param>
    /// <returns>True if the player is under check</returns>
    public bool IsUnderCheck(Side.SideType playerSide) {
        Cell kingCell = null;
        ArrayList ownedCells = this.board.GetSideCell(playerSide);

        // First get the king cell
        foreach (string cell in ownedCells) {
            if (this.board[cell].Piece.Type == Piece.PieceType.King) {
                kingCell = this.board[cell];
                break;
            }
        }

        // Get the enemy cells and all of the possible moves of each enemy cell
        ArrayList EnemyCells = board.GetSideCell((new Side(playerSide)).Enemy());
        foreach (string CellName in EnemyCells) {
            ArrayList moves = this.GetPossibleMoves(this.board[CellName]);

            // If the king cell is in this set, then the player is under check
            if (moves.Contains(kingCell)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get the number of possible moves
    /// </summary>
    /// <param name="playerSide">The player</param>
    /// <returns>The number of possible moves</returns>
    private int GetCountOfPossibleMoves(Side.SideType playerSide) {
        int moveCount = 0;
        ArrayList playerCells = this.board.GetSideCell(playerSide);

        foreach (string cell in playerCells) {
            ArrayList moves = this.GetLegalMoves(this.board[cell]);
            moveCount += moves.Count;
        }

        return moveCount;
    }

    /// <summary>
    /// Check whether or not a move causes check
    /// </summary>
    /// <param name="move">The move</param>
    /// <returns>True if the move causes check</returns>
    private bool CausesCheck(Move move) {
        bool causesCheck = false;
        Side.SideType playerSide = move.StartCell.Piece.Side.Type;

        // Do the move
        this.ExecuteMove(move);

        // Check if we're under check
        causesCheck = this.IsUnderCheck(playerSide);

        // Undo the move
        this.UndoMove(move);

        return causesCheck;
    }

    /// <summary>
    /// Get all the legal moves for a cell
    /// </summary>
    /// <param name="source">The cell</param>
    /// <returns>An ArrayList of all legal moves for a cell</returns>
    public ArrayList GetLegalMoves(Cell source) {
        ArrayList allPossibleMoves = this.GetPossibleMoves(source);
        ArrayList movesToRemove = new ArrayList();

        foreach (Cell target in allPossibleMoves) {
            // If the move will cause this player to go into check, remove it
            if (this.CausesCheck(new Move(source, target))) {
                movesToRemove.Add(target);
            }
        }

        // Remove tower moves if the king is under check
        if (source.Piece.Type == Piece.PieceType.King && this.IsUnderCheck(source.Piece.Side.Type)) {
            foreach (Cell target in allPossibleMoves) {
                if (Math.Abs(target.Column - source.Column) > 1) {
                    movesToRemove.Add(target);
                }
            }
        }

        // Remove all cells that are illegal
        foreach (Cell cell in movesToRemove) {
            allPossibleMoves.Remove(cell);
        }

        return allPossibleMoves;
    }

    /// <summary>
    /// Get all the legal moves
    /// </summary>
    /// <param name="playerSide">The player side</param>
    /// <returns>An ArrayList of all legal moves</returns>
    public ArrayList GenerateAllLegalMoves(Side playerSide) {
        ArrayList allLegalMoves = new ArrayList();
        ArrayList playerCells = this.board.GetSideCell(playerSide.Type);
        Move move;

        // Get all of the legal moves for each cell the player owns
        foreach (string cell in playerCells) {
            ArrayList moves = this.GetLegalMoves(this.board[cell]);

            foreach (Cell dest in moves) {
                move = new Move(board[cell], dest);
                this.SetMoveType(move);

                // Set the move's score
                if (move.IsPromotionMove()) {
                    move.Score = 1000;
                } else if (move.IsCaptureMove()) {
                    move.Score = move.EndCell.Piece.GetWeight();
                }

                allLegalMoves.Add(move);
            }
        }

        // Sort the moves
        MoveComparer moveCompareObj = new MoveComparer();
        allLegalMoves.Sort(moveCompareObj);

        return allLegalMoves;
    }

    /// <summary>
    /// Get the good capture moves with a score over 100
    /// </summary>
    /// <param name="playerSide">The player side</param>
    /// <returns>An ArrayList of good capture moves</returns>
    public ArrayList GenerateGoodCaptureMoves(Side playerSide) {
        ArrayList goodCaptureMoves = new ArrayList();
        ArrayList playerCells = this.board.GetSideCell(playerSide.Type);
        Move move;

        foreach (string cell in playerCells) {
            // If the weight is over 100 add its legal moves
            if (this.board[cell].Piece.GetWeight() > 100) {
                ArrayList moves = this.GetLegalMoves(board[cell]);

                foreach (Cell dest in moves) {
                    if (dest.Piece != null && !dest.Piece.IsEmpty()) {
                        move = new Move(board[cell], dest);
                        goodCaptureMoves.Add(move);
                    }
                }
            }
        }
        return goodCaptureMoves;
    }

    /// <summary>
    /// Get all the possible moves from a cell
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <returns>An ArrayList of possible moves</returns>
    public ArrayList GetPossibleMoves(Cell source) {
        ArrayList possibleMoves = new ArrayList();

        switch (source.Piece.Type) {
            case Piece.PieceType.Empty:
                break;
            case Piece.PieceType.Pawn:
                this.GetPawnMoves(source, possibleMoves);
                break;
            case Piece.PieceType.Knight:
                this.GetKnightMoves(source, possibleMoves);
                break;
            case Piece.PieceType.Rook:
                this.GetRookMoves(source, possibleMoves);
                break;
            case Piece.PieceType.Bishop:
                this.GetBishopMoves(source, possibleMoves);
                break;
            case Piece.PieceType.Queen:
                this.GetQueenMoves(source, possibleMoves);
                break;
            case Piece.PieceType.King:
                this.GetKingMoves(source, possibleMoves);
                break;
        }

        return possibleMoves;
    }

    /// <summary>
    /// Gets the move if the pawn's last move was its first
    /// </summary>
    /// <returns>The pawn's last move if it was the first or null</returns>
    private Move LastMoveWasPawnFirst() {
        Move lastmove = this.game.GetLastMove();

        if (lastmove != null) {
            if (lastmove.Piece.IsPawn() && lastmove.Piece.MoveCount == 1) {
                return lastmove;
            }
        }

        return null;
    }

    /// <summary>
    /// Get the pawn moves
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="moves">The output list of moves</param>
    private void GetPawnMoves(Cell source, ArrayList moves) {
        Cell cell;

        // White
        if (source.Piece.Side.IsWhite()) {
            // Top
            cell = this.board.TopCell(source);
            if (cell != null && cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Double first move
            if (cell != null && cell.IsEmpty()) {
                cell = board.TopCell(cell);
                if (cell != null && source.Piece.MoveCount == 0 && cell.IsEmpty()) {
                    moves.Add(cell);
                }
            }

            // Top left
            cell = board.TopLeftCell(source);
            if (cell != null && cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
            }

            // Top right
            cell = board.TopRightCell(source);
            if (cell != null && cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
            }

            // En Passant moves
            Move lastPawnMove = this.LastMoveWasPawnFirst();
            if (lastPawnMove != null) {
                if (source.Row == lastPawnMove.EndCell.Row) {
                    // Right
                    if (lastPawnMove.EndCell.Column == source.Column - 1) {
                        cell = this.board.TopLeftCell(source);
                        if (cell != null && cell.IsEmpty()) {
                            moves.Add(cell);
                        }
                    }

                    // Left
                    if (lastPawnMove.EndCell.Column == source.Column + 1) {
                        cell = this.board.TopRightCell(source);
                        if (cell != null && cell.IsEmpty()) {
                            moves.Add(cell);
                        }
                    }
                }
            }
        } else {
            // Black
            // Bottom
            cell = this.board.BottomCell(source);
            if (cell != null && cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Double first move
            if (cell != null && cell.IsEmpty()) {
                cell = board.BottomCell(cell);
                if (cell != null && source.Piece.MoveCount == 0 && cell.IsEmpty()) {
                    moves.Add(cell);
                }
            }

            // Bottom left
            cell = board.BottomLeftCell(source);
            if (cell != null && cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
            }

            // Bottom right
            cell = board.BottomRightCell(source);
            if (cell != null && cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
            }

            // En Passant moves
            Move lastPawnMove = this.LastMoveWasPawnFirst();
            if (lastPawnMove != null) {
                if (source.Row == lastPawnMove.EndCell.Row) {
                    // Right
                    if (lastPawnMove.EndCell.Column == source.Column - 1) {
                        cell = this.board.TopLeftCell(source);
                        if (cell != null && cell.IsEmpty()) {
                            moves.Add(cell);
                        }
                    }

                    // Left
                    if (lastPawnMove.EndCell.Column == source.Column + 1) {
                        cell = this.board.TopRightCell(source);
                        if (cell != null && cell.IsEmpty()) {
                            moves.Add(cell);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get the knight moves
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="moves">The output list of moves</param>
    private void GetKnightMoves(Cell source, ArrayList moves) {
        Cell cell;
        // Top Top Left and Right
        cell = this.board.TopCell(source);
        if (cell != null) {
            // Top Top Left
            cell = this.board.TopLeftCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }

            // Top Top Right
            cell = this.board.TopCell(source);
            cell = this.board.TopRightCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }
        }

        // Bottom Bototm Left and Right
        cell = this.board.BottomCell(source);
        if (cell != null) {
            // Bottom Bottom Left
            cell = this.board.BottomLeftCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }

            // Bottom Bottom Right
            cell = this.board.BottomCell(source);
            cell = this.board.BottomRightCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }
        }
        
        // Left Left Top and Bottom
        cell = this.board.LeftCell(source);
        if (cell != null) {
            // Left Left Top
            cell = this.board.TopLeftCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }

            // Left Left Bottom
            cell = this.board.LeftCell(source);
            cell = this.board.BottomLeftCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }
        }

        // Right Right Top and Bottom
        cell = this.board.RightCell(source);
        if (cell != null) {
            // Right Right Top
            cell = this.board.TopRightCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }

            // Right Right Bottom
            cell = this.board.RightCell(source);
            cell = this.board.BottomRightCell(cell);
            if (cell != null && !cell.IsOwned(source)) {
                moves.Add(cell);
            }
        }
    }

    /// <summary>
    /// Get rook moves
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="moves">The output list of moves</param>
    private void GetRookMoves(Cell source, ArrayList moves) {
        Cell cell;

        // Up
        cell = this.board.TopCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }
            
            // Iterate
            cell = this.board.TopCell(cell);
        }

        // Left
        cell = this.board.LeftCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }

            // Iterate
            cell = this.board.LeftCell(cell);
        }

        // Right
        cell = this.board.RightCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }

            // Iterate
            cell = this.board.RightCell(cell);
        }

        // Down
        cell = this.board.BottomCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }

            // Iterate
            cell = this.board.BottomCell(cell);
        }
    }

    /// <summary>
    /// Get bishop moves
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="moves">The output list of moves</param>
    private void GetBishopMoves(Cell source, ArrayList moves) {
        Cell cell;

        // Top Left
        cell = this.board.TopLeftCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }

            // Iterate
            cell = this.board.TopLeftCell(cell);
        }

        // Top Right
        cell = board.TopRightCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }

            // Iterate
            cell = this.board.TopRightCell(cell);
        }

        // Bottom Left
        cell = board.BottomLeftCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }

            // Iterate
            cell = this.board.BottomLeftCell(cell);
        }

        // Bottom Right
        cell = board.BottomRightCell(source);
        while (cell != null) {
            // Add Empty
            if (cell.IsEmpty()) {
                moves.Add(cell);
            }

            // Add Enemy (end)
            if (cell.IsOwnedByEnemy(source)) {
                moves.Add(cell);
                break;
            }

            // End Own Cells
            if (cell.IsOwned(source)) {
                break;
            }

            // Iterate
            cell = this.board.BottomRightCell(cell);
        }
    }

    /// <summary>
    /// Get queen moves
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="moves">The output list of moves</param>
    private void GetQueenMoves(Cell source, ArrayList moves) {
        // Queen is rook and bishop combined
        GetRookMoves(source, moves);
        GetBishopMoves(source, moves);
    }
    
    /// <summary>
    /// Get king moves
    /// </summary>
    /// <param name="source">The source cell</param>
    /// <param name="moves">The output list of moves</param>
    private void GetKingMoves(Cell source, ArrayList moves) {
        Cell cell;
        // Top
        cell = this.board.TopCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Left
        cell = this.board.LeftCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Right
        cell = this.board.RightCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Bottom
        cell = this.board.BottomCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Top Left
        cell = this.board.TopLeftCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Top Right
        cell = this.board.TopRightCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Bottom Left
        cell = this.board.BottomLeftCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Bottom Right
        cell = this.board.BottomRightCell(source);
        if (cell != null && !cell.IsOwned(source)) {
            moves.Add(cell);
        }

        // Tower Moves
        if (this.board[source].Piece.MoveCount == 0) {
            Cell castlingTarget = null;
            // Right Side
            cell = this.board.RightCell(source);
            // The bishop has to be empty
            if (cell != null && cell.IsEmpty()) {
                // That cell can't be attacked
                if (!this.CausesCheck(new Move(source, cell))) {
                    cell = this.board.RightCell(cell);
                    // The knight has to be empty
                    if (cell != null && cell.IsEmpty()) {
                        castlingTarget = cell;
                        cell = this.board.RightCell(cell);
                        // The rook has to be there and unmoved
                        if (cell != null && !cell.IsEmpty() && cell.Piece.MoveCount == 0) {
                            moves.Add(castlingTarget);
                        }
                    }
                }
            }

            // Left Side
            cell = board.LeftCell(source);
            // The queen has to be empty
            if (cell != null && cell.IsEmpty()) {
                // That cell can't be attacked
                if (!CausesCheck(new Move(source, cell))) {
                    cell = board.LeftCell(cell);
                    // The bishop has to be empty
                    if (cell != null && cell.IsEmpty()) {
                        castlingTarget = cell;
                        cell = board.LeftCell(cell);
                        // T he knight has to be empty
                        if (cell != null && cell.IsEmpty()) {
                            cell = board.LeftCell(cell);
                            // The rook has to be there and unmoved
                            if (cell != null && !cell.IsEmpty() && cell.Piece.MoveCount == 0)
                                moves.Add(castlingTarget);
                        }
                    }

                }
            }
        }
    }

    /// <summary>
    /// Analyze the board score state
    /// </summary>
    /// <param name="playerSide">The player side</param>
    /// <returns>The player board score</returns>
    public int AnalyzeBoard(Side.SideType playerSide) {
        int score = 0;
        ArrayList ownedCells = this.board.GetSideCell(playerSide);

        // Add up the scores of the player cells
        foreach (string cell in ownedCells) {
            score += this.board[cell].Piece.GetWeight();
        }

        return score;
    }

    /// <summary>
    /// Evaluate the board and player state
    /// </summary>
    /// <param name="PlayerSide">The player</param>
    /// <returns>The score difference</returns>
    public int Evaluate(Side PlayerSide) {
        int score = 0;

        // Calculate the difference in score
        score = AnalyzeBoard(PlayerSide.Type) - AnalyzeBoard(PlayerSide.Enemy()) - 25;

        // Checkmate is infinity score
        if (IsCheckMate(PlayerSide.Enemy())) {
            score = int.MaxValue;
        }

        return score;
    }

    /// <summary>
    /// Property for the board
    /// </summary>
    public Board Board {
        get {
            return board;
        }
    }

    /// <summary>
    /// Property for the game
    /// </summary>
    public Game Game {
        get {
            return game;
        }
    }
}