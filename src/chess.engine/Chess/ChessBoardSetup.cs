﻿using System;
using chess.engine.Entities;
using chess.engine.Game;

namespace chess.engine.Chess
{
    public class ChessBoardSetup : IGameSetup
    {
        public void SetupPieces(ChessBoardEngine engine)
        {
            AddPawns(engine);
            AddMajorPieces(engine);
        }


        private void AddPawns(ChessBoardEngine engine)
        {
            foreach (var colour in new[] { Colours.White, Colours.Black })
            {
                foreach (var file in Enum.GetValues(typeof(ChessFile)))
                {
                    engine.AddPiece(ChessPieceEntityFactory.CreatePawn(colour),
                        BoardLocation.At((ChessFile)file, colour == Colours.White ? 2 : 7));
                }
            }
        }

        private void AddMajorPieces(ChessBoardEngine engine)
        {
            foreach (var rank in new[] { 1, 8 })
            {
                var colour = rank == 1 ? Colours.White : Colours.Black;

                engine.AddPiece(ChessPieceEntityFactory.CreateRook(colour), BoardLocation.At($"A{rank}"));
                engine.AddPiece(ChessPieceEntityFactory.CreateKnight(colour), BoardLocation.At($"B{rank}"));
                engine.AddPiece(ChessPieceEntityFactory.CreateBishop(colour), BoardLocation.At($"C{rank}"));
                engine.AddPiece(ChessPieceEntityFactory.CreateQueen(colour), BoardLocation.At($"D{rank}"));
                engine.AddPiece(ChessPieceEntityFactory.CreateKing(colour), BoardLocation.At($"E{rank}"));
                engine.AddPiece(ChessPieceEntityFactory.CreateBishop(colour), BoardLocation.At($"F{rank}"));
                engine.AddPiece(ChessPieceEntityFactory.CreateKnight(colour), BoardLocation.At($"G{rank}"));
                engine.AddPiece(ChessPieceEntityFactory.CreateRook(colour), BoardLocation.At($"H{rank}"));
            }
        }
    }
}