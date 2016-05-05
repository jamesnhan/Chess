/// <summary>
/// A player in a chess game
/// </summary>
using System.Collections;

public class Player {
    /// <summary>
    /// An enum for the type of player
    /// </summary>
    [System.Serializable]
    public enum Type { Human, AI };

    /// <summary>
    /// The type of player
    /// </summary>
    private Type type;

    /// <summary>
    /// The sie of the player
    /// </summary>
    private Side side;

    /// <summary>
    /// The rules of the game
    /// </summary>
    private Rules rules;

    /// <summary>
    /// The number of moves an AI has analyzed
    /// </summary>
    private int totalMovesAnalyzed;

    /// <summary>
    /// Whether or not the game is near the end
    /// </summary>
    private bool gameNearEnd;

    /// <summary>
    /// Internal constructor
    /// </summary>
    internal Player() {
        side = PlayerSide;
        type = PlayerType;
    }

    /// <summary>
    /// Create a player on a side and of a type
    /// </summary>
    /// <param name="PlayerSide">The side</param>
    /// <param name="PlayerType">The type</param>
    public Player(Side PlayerSide, Type PlayerType) {
        side = PlayerSide;
        type = PlayerType;
    }

    /// <summary>
    /// Create a player on a side, with a type, and with specific rules
    /// </summary>
    /// <param name="PlayerSide">The side</param>
    /// <param name="PlayerType">The type</param>
    /// <param name="rules">The rules</param>
    public Player(Side PlayerSide, Type PlayerType, Rules rules) : this(PlayerSide, PlayerType) {
        this.rules = rules;
    }

    /// <summary>
    /// Checks whether or not this player is an AI
    /// </summary>
    /// <returns>True if this player is an AI</returns>
    public bool IsAI() {
        return type == Type.AI;
    }

    /// <summary>
    /// Gets the best move for the player
    /// </summary>
    /// <returns>The best Move for a player</returns>
    public Move GetBestMove() {
        int alpha, beta;
        int depth;
        Move bestMove = null;

        const int MIN_SCORE = -10000000;
        const int MAX_SCORE = 10000000;

        ArrayList allLegalMoves = this.rules.GenerateAllLegalMoves(side);
        int moveCounter;

        // If we have very few pieces or moves left, we're near the end
        if (this.rules.Board.GetSideCell(side.Type).Count <= 5 || allLegalMoves.Count <= 5) {
            gameNearEnd = true;
        }

        Side EnemySide;

        // Get the enemy side
        if (side.IsBlack()) {
            EnemySide = rules.Game.WhitePlayer.PlayerSide;
        } else {
            EnemySide = rules.Game.BlackPlayer.PlayerSide;
        }

        // If the enemy player has very few pieces or moves left, we're near the end
        if (rules.Board.GetSideCell(side.Enemy()).Count <= 5 || rules.GenerateAllLegalMoves(EnemySide).Count <= 5) {
            gameNearEnd = true;
        }

        totalMovesAnalyzed = 0;

        // Check for moves up to a depth of MAX_DEPTH
        for (depth = 1; depth <= this.rules.Game.maxAIDepth; depth++) {
            // Start with the min and max scores
            alpha = MIN_SCORE;
            beta = MAX_SCORE;

            moveCounter = 0;

            // Go through each legal move
            foreach (Move move in allLegalMoves) {
                moveCounter++;
                // Try the move
                this.rules.ExecuteMove(move);

                // Score the move with alpha-beta
                move.Score = -this.AlphaBeta(this.rules.Game.EnemyPlayer(side).PlayerSide, depth - 1, -beta, -alpha);
                this.totalMovesAnalyzed++;

                // Undo the move
                this.rules.UndoMove(move);

                // If this move's score is higher than the current alpha, update the best move
                if (move.Score > alpha) {
                    bestMove = move;
                    alpha = move.Score;
                }
            }
        }
        
        return bestMove;
    }

    /// <summary>
    /// Alpha-Beta Pruning
    /// </summary>
    /// <param name="playerSide">The player side</param>
    /// <param name="depth">The depth</param>
    /// <param name="alpha">Alpha value</param>
    /// <param name="beta">Beta value</param>
    /// <returns></returns>
    private int AlphaBeta(Side playerSide, int depth, int alpha, int beta) {
        int val;
        int R = (depth > 6) ? 3 : 2;

        // If we're deep or we want to do null pruning
        if (depth >= 2 && !this.gameNearEnd && this.rules.Game.DoNullMovePruning) {
            // Recurse alpha-beta
            val = -this.AlphaBeta(this.rules.Game.EnemyPlayer(playerSide).PlayerSide, depth - R - 1, -beta, -beta + 1);
            if (val >= beta) {
                return beta;
            }
        }

        // Check if we want to do quiescent searching to prevent horizon effects
        bool bFoundPv = false;
        if (depth <= 0) {
            if (this.rules.Game.DoQuiescentSearch) {
                return this.QuiescentSearch(playerSide, alpha, beta);
            } else {
                return this.rules.Evaluate(playerSide);
            }
        }
        
        // Evaluate the moves
        ArrayList allLegalMoves = rules.GenerateAllLegalMoves(playerSide);
        foreach (Move move in allLegalMoves) {
            this.rules.ExecuteMove(move);
            // Do we do principle variation?
            if (bFoundPv && this.rules.Game.DoPrincipleVariation) {
                val = -this.AlphaBeta(this.rules.Game.EnemyPlayer(playerSide).PlayerSide, depth - 1, -alpha - 1, -alpha);
                if ((val > alpha) && (val < beta))
                    val = -this.AlphaBeta(this.rules.Game.EnemyPlayer(playerSide).PlayerSide, depth - 1, -beta, -alpha);
            } else
                val = -this.AlphaBeta(this.rules.Game.EnemyPlayer(playerSide).PlayerSide, depth - 1, -beta, -alpha);

            this.totalMovesAnalyzed++;
            this.rules.UndoMove(move);
            if (val >= beta) {
                return beta;
            }

            if (val > alpha) {
                alpha = val;
                bFoundPv = true;
            }
        }

        return alpha;
    }

    /// <summary>
    /// Quiescent Search Algorithm
    /// </summary>
    /// <param name="playerSide">The player side</param>
    /// <param name="alpha">Alpha value</param>
    /// <param name="beta">Beta value</param>
    /// <returns></returns>
    int QuiescentSearch(Side playerSide, int alpha, int beta) {
        int val = this.rules.Evaluate(playerSide);

        if (val >= beta) {
            return beta;
        }

        if (val > alpha) {
            alpha = val;
        }

        // Do quiesecent searching on the moves
        ArrayList allGoodCaptureMoves = rules.GenerateGoodCaptureMoves(playerSide);
        foreach (Move move in allGoodCaptureMoves) {
            this.rules.ExecuteMove(move);
            val = -QuiescentSearch(this.rules.Game.EnemyPlayer(playerSide).PlayerSide, -beta, -alpha);
            rules.UndoMove(move);

            if (val >= beta) {
                return beta;
            }

            if (val > alpha) {
                alpha = val;
            }
        }

        return alpha;
    }

    /// <summary>
    /// Property for player type
    /// </summary>
    public Type PlayerType {
        get {
            return type;
        }
        set {
            type = value;
        }
    }

    /// <summary>
    /// Property for player side
    /// </summary>
    public Side PlayerSide {
        get {
            return side;
        }
        set {
            side = value;
        }
    }
}
