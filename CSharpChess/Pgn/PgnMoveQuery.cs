﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CSharpChess.System.Extensions;
using CSharpChess.TheBoard;

namespace CSharpChess.Pgn
{
    /*
     * Pgn notation requires current board state to determine the actual move
     * as it is not explicit about the piece being moved only what happened and
     * where the destination was.
     * 
     * We will parse the raw text to create a moveQuery to apply against the board
     * that can be used to determine the missing details
     * 
     */

    public class PgnMoveQuery
    {
        public Chess.Board.ChessFile FromFile { get; }
        public MoveType MoveType { get; }
        public ChessPiece Piece { get; }
        public BoardLocation Destination { get; }

        private PgnMoveQuery(ChessPiece piece, BoardLocation destination, MoveType moveType, Chess.Board.ChessFile fromFile = Chess.Board.ChessFile.None)
        {
            Destination = destination;
            Piece = piece;
            MoveType = moveType;
            FromFile = fromFile;
        }

        private static readonly IDictionary<char, Chess.PieceNames> PieceNameMap = new Dictionary<char, Chess.PieceNames>
        {
            {'P', Chess.PieceNames.Pawn },
            {'N', Chess.PieceNames.Knight },
            {'B', Chess.PieceNames.Bishop },
            {'R', Chess.PieceNames.Rook },
            {'Q', Chess.PieceNames.Queen },
            {'K', Chess.PieceNames.King },
        };

        public static bool TryParse(Chess.Colours turn, string move, out PgnMoveQuery moveQuery)
        {
            MoveType moveType = MoveType.Unknown;
            Chess.Board.ChessFile fromFile = Chess.Board.ChessFile.None;
            
            string dest = "";
            Chess.PieceNames pn = GetPieceName(move);

            moveType = MoveType.Move;

            if (BasicPawnMove(move))
            {
                dest = move;
            }
            else
            {
                if (move[1] == 'x')
                {
                    moveType = MoveType.Take;
                    if (move.Length == 4)
                    {
                        dest = move.Substring(2, 2);
                        fromFile = BoardLocation.At($"{move[0]}1").File;
                    }
                }
                else
                {
                    dest = move.Substring(1, 2);
                }


            }
            var piece = new ChessPiece(turn, pn);
            var destination = BoardLocation.At(dest);

            moveQuery = new PgnMoveQuery(piece, destination, moveType, fromFile);
            return true;
        }

        private static bool BasicPawnMove(string move)
        {
            return move.Length == 2;
        }

        private static Chess.PieceNames GetPieceName(string move)
        {
            Chess.PieceNames result = Chess.PieceNames.Blank;
            
            var firstChar = move[0];
            if (char.IsUpper(firstChar))
            {
                var pieceChar = char.ToUpper(firstChar);
                if (!PieceNameMap.ContainsKey(pieceChar))
                {
                    throw new NotImplementedException($"'{pieceChar}' is not a valid SAN piece.");
                }
                result = PieceNameMap[pieceChar];
            }
            else
            {
                result = Chess.PieceNames.Pawn;
            }
            return result;
        }
    }
}