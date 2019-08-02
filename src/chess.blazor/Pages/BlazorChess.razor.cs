using System;
using System.Linq;
using System.Threading.Tasks;
using board.engine;
using chess.blazor.Shared.Chess;
using chess.webapi.client.csharp;
using Microsoft.AspNetCore.Components;

namespace chess.blazor.Pages
{
    public class BlazorChessComponent : ComponentBase
    {
        [Parameter] public string BoardString { get; set; }
        [Parameter] public bool WhiteIsHuman { get; set; } = true;
        [Parameter] public bool BlackIsHuman { get; set; } = false;

        public ChessBoardComponent ChessBoard { get; set; }
        public AvailableMoveListComponent MoveList { get; set; }
        [Inject] public IChessGameApiClient ApiClient { get; set; }

        private ChessWebApiResult _firstResult;
        private ChessWebApiResult _lastResult;

        private int _moveCount;
        private static readonly Random Random = new Random();

        protected override async Task OnInitAsync()
        {
            await ResetBoardAsync();
        }

        public async Task InitialiseState()
        {
            if (string.IsNullOrEmpty(BoardString))
            {
                _lastResult = await ApiClient.ChessGameAsync();
            }
            else
            {
                // NOTE: Blazor default routing gets confused by using '.' in the url and I couldn't work out how to fix it so...
                _lastResult = await ApiClient.ChessGameAsync(BoardString.Replace("_", "."));
            }

            if (_firstResult == null) _firstResult = _lastResult;

            Guard.NotNull(_lastResult, "Unable to initialise board");

            _moveCount = 0;
        }
        public async Task ResetBoardAsync()
        {

            if (_firstResult == null)
            {
                await InitialiseState();
            }
            else
            {
                _lastResult = _firstResult;
            }

            UpdateBoardAndMoves();
        }

        private void UpdateBoardAndMoves()
        {

            UpdateChessBoardComponent(_lastResult);

            UpdateMoveListComponent(_lastResult);
        }

        private void UpdateMoveListComponent(ChessWebApiResult result)
        {
            var title = string.IsNullOrEmpty(result.Message)
                ? $"{result.WhoseTurn} to play"
                : result.Message;

            MoveList.Update(title, result.AvailableMoves, !IsAiTurn(result));

        }

        private void UpdateChessBoardComponent(ChessWebApiResult result)
        {
            ChessBoard.Update(result.Board, result.AvailableMoves, result.WhoseTurn.ToLower().Contains("white"));
        }

        public async Task<bool> OnMoveSelectedAsync(string move)
        {
            ChessBoard.Message = "";

            try
            {
                // TODO: Temp hack to stop endless auto-games, need proper stalement & not-enough-material-left checks
                if (_moveCount > 150) throw new Exception("Move limit exceeded");
                _lastResult = await ApiClient.PlayMoveAsync(ChessBoard.Board, EncodeMove(move));
                UpdateBoardAndMoves();
                StateHasChanged();  // NOTE: We call StateHasChanged() because we are in a recursive method when handling AI players and therefore the state doesn't automatically get updated until the stack unwinds
                _moveCount++;
                if(!_lastResult.Message.ToLower().Contains("checkmate"))
                {
                    await HandleAiPlayer(_lastResult);
                }
            }
            catch (Exception e)
            {
                ChessBoard.Message = $"Error performing move;\n{e.Message}"; // TODO: Better exception handling and logging
                StateHasChanged();
            }
            return true;
        }

        private async Task HandleAiPlayer(ChessWebApiResult lastResult)
        {
            if (IsAiTurn(lastResult))
            {
                await PlayRandomMove(lastResult);
            }
        }

        private bool IsAiTurn(ChessWebApiResult lastResult) 
            => lastResult.WhoseTurn.ToLower() == "black" && !BlackIsHuman
            || lastResult.WhoseTurn.ToLower() == "white" && !WhiteIsHuman;

        private async Task PlayRandomMove(ChessWebApiResult lastResult)
        {
            if(!lastResult.AvailableMoves.Any()) return;

            MoveList.Title = $"{lastResult.WhoseTurn} is thinking...";
            MoveList.ShowMoveList = false;
            var rnd = Random.Next(lastResult.AvailableMoves.Length); // TODO: Ok so it's not really an AI ;)
            await OnMoveSelectedAsync(lastResult.AvailableMoves[rnd].SAN);
        }

        public string EncodeMove(string move) => move.Replace("+", ""); // NOTE: '+' breaks the urls!, it's only cosmetic so just remove it
    }
}