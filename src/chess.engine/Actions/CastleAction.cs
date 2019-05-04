﻿using chess.engine.Board;
using chess.engine.Game;
using chess.engine.Movement;

namespace chess.engine.Actions
{
    public class CastleAction : BoardAction
    {
        public CastleAction(IBoardActionFactory factory, IBoardState boardState) : base(factory, boardState)
        {
        }

        public override void Execute(ChessMove move)
        {
            ChessMove kingMove, rookMove;
            if (move.From.File < move.To.File) // King Side
            {
                kingMove = new ChessMove(move.From, BoardLocation.At($"G{move.From.Rank}"), ChessMoveType.MoveOnly);
                rookMove = new ChessMove(BoardLocation.At($"H{move.From.Rank}"), BoardLocation.At($"F{move.From.Rank}"), ChessMoveType.MoveOnly);
            }
            else
            {
                kingMove = new ChessMove(move.From, BoardLocation.At($"C{move.From.Rank}"), ChessMoveType.MoveOnly);
                rookMove = new ChessMove(BoardLocation.At($"A{move.From.Rank}"), BoardLocation.At($"D{move.From.Rank}"), ChessMoveType.MoveOnly);
            }

            var moveOnly = Factory.Create(DefaultActions.MoveOnly, BoardState);
            moveOnly.Execute(kingMove);
            moveOnly.Execute(rookMove);
        }
    }
}