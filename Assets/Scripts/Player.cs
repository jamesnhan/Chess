using System.Collections;
using UnityEngine;

public class Player {
    private Type m_Type;
    private Side m_Side;
    private Rules m_Rules;

    private int m_TotalMovesAnalyzed;
    private bool m_GameNearEnd;

    public enum Type { Human, AI };

    internal Player() {
        m_Side = PlayerSide;
        m_Type = PlayerType;
    }
    public Player(Side PlayerSide, Type PlayerType) {
        m_Side = PlayerSide;
        m_Type = PlayerType;
    }

    public Player(Side PlayerSide, Type PlayerType, Rules rules) : this(PlayerSide, PlayerType) {
        m_Rules = rules;
    }

    public bool IsAI() {
        return (m_Type == Type.AI);
    }

    internal Rules GameRules {
        set {
            m_Rules = value;
        }
    }
    public Move GetFixBestMove() {
        int alpha, beta;
        int depth;
        Move BestMove = null;
        const int MIN_SCORE = -1000000;
        const int MAX_SCORE = 1000000;

        ArrayList TotalMoves = m_Rules.GenerateAllLegalMoves(m_Side);

        alpha = MIN_SCORE;
        beta = MAX_SCORE;

        depth = 3;
        foreach (Move move in TotalMoves) {
            m_Rules.ExecuteMove(move);
            move.Score = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(m_Side).PlayerSide, depth - 1, -beta, -alpha);
            m_Rules.UndoMove(move);

            if (move.Score > alpha) {
                BestMove = move;
                alpha = move.Score;
            }
        }

        return BestMove;
    }

    public Move GetBestMove() {
        int alpha, beta;
        int depth;
        Move BestMove = null;
        const int MIN_SCORE = -10000000;
        const int MAX_SCORE = 10000000;

        ArrayList TotalMoves = m_Rules.GenerateAllLegalMoves(m_Side);
        int MoveCounter;
        if (m_Rules.ChessBoard.GetSideCell(m_Side.type).Count <= 5 || TotalMoves.Count <= 5)
            m_GameNearEnd = true;
        Side EnemySide;

        if (m_Side.isBlack())
            EnemySide = m_Rules.ChessGame.WhitePlayer.PlayerSide;
        else
            EnemySide = m_Rules.ChessGame.BlackPlayer.PlayerSide;

        if (m_Rules.ChessBoard.GetSideCell(m_Side.Enemy()).Count <= 5 || m_Rules.GenerateAllLegalMoves(EnemySide).Count <= 5)
            m_GameNearEnd = true;

        m_TotalMovesAnalyzed = 0;

        for (depth = 1; depth <= 5; depth++) {
            alpha = MIN_SCORE;
            beta = MAX_SCORE;
            MoveCounter = 0;
            foreach (Move move in TotalMoves) {
                MoveCounter++;
                m_Rules.ExecuteMove(move);
                move.Score = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(m_Side).PlayerSide, depth - 1, -beta, -alpha);
                m_TotalMovesAnalyzed++;
                m_Rules.UndoMove(move);

                if (move.Score > alpha) {
                    BestMove = move;
                    alpha = move.Score;
                }
            }
        }
        
        return BestMove;
    }
    private int AlphaBeta(Side PlayerSide, int depth, int alpha, int beta) {
        int val;
        int R = (depth > 6) ? 3 : 2;

        if (depth >= 2 && !m_GameNearEnd && m_Rules.ChessGame.DoNullMovePruning) {
            val = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide, depth - R - 1, -beta, -beta + 1);
            if (val >= beta)
                return beta;
        }

        bool bFoundPv = false;
        if (depth <= 0) {

            if (m_Rules.ChessGame.DoQuiescentSearch)
                return QuiescentSearch(PlayerSide, alpha, beta);
            else
                return m_Rules.Evaluate(PlayerSide);
        }

        ArrayList TotalMoves = m_Rules.GenerateAllLegalMoves(PlayerSide);
        foreach (Move move in TotalMoves) {

            m_Rules.ExecuteMove(move);
            if (bFoundPv && m_Rules.ChessGame.DoPrincipleVariation) {
                val = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide, depth - 1, -alpha - 1, -alpha);
                if ((val > alpha) && (val < beta))
                    val = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide, depth - 1, -beta, -alpha);
            } else
                val = -AlphaBeta(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide, depth - 1, -beta, -alpha);

            m_TotalMovesAnalyzed++;
            m_Rules.UndoMove(move);
            if (val >= beta)
                return beta;

            if (val > alpha) {
                alpha = val;
                bFoundPv = true;
            }
        }
        return alpha;
    }

    int QuiescentSearch(Side PlayerSide, int alpha, int beta) {
        int val = m_Rules.Evaluate(PlayerSide);

        if (val >= beta)
            return beta;

        if (val > alpha)
            alpha = val;
        ArrayList TotalMoves = m_Rules.GenerateGoodCaptureMoves(PlayerSide);
        foreach (Move move in TotalMoves) {

            m_Rules.ExecuteMove(move);
            val = -QuiescentSearch(m_Rules.ChessGame.EnemyPlayer(PlayerSide).PlayerSide, -beta, -alpha);
            m_Rules.UndoMove(move);

            if (val >= beta)
                return beta;

            if (val > alpha)
                alpha = val;
        }

        return alpha;
    }

    public Type PlayerType {
        get {
            return m_Type;
        }
        set {
            m_Type = value;
        }
    }

    public Side PlayerSide {
        get {
            return m_Side;
        }
        set {
            m_Side = value;
        }
    }
}
