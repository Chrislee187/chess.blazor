using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using board.engine.Board;
using chess.engine.Entities;
using chess.engine.Extensions;
using chess.engine.Game;
using chess.engine.SAN;
using chess.webapi.client.csharp;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using PgnReader;

namespace chess.blazor
{
    public class Startup
    {
        public const bool UseExternalChessApi = false;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPgnSerialisationService, PgnSerialisationService>();

            // TODO: Need to hook up IConfiguration and pull host out to config
            // "https://chess-web-api.azurewebsites.net"
            //"https://localhost:5001"

            if (UseExternalChessApi)
            {
                services.AddTransient<IChessGameApiClient>(provider 
                    => new ChessGameApiClient(provider.GetService<HttpClient>(), "https://localhost:5001"));
            }
            else
            {

                services.AddTransient<IChessGameApiClient>(provider
                    => new InprocessChessGameApi());
            }
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }

    public class InprocessChessGameApi : IChessGameApiClient
    {
        // NOTE: These methods return proper tasks to run asynchronously (rather than running the code
        // and returning Task.FromResult() ) otherwise they run out of order in WASM which is single threaded
        // My assumption is without the extra task they get added to the queue before they should, noticed
        // when a property in a component was trying to be used before it was initialised using the Inproc client
        // but wasn't a problem when using a typical async http client.

        public Task<ChessWebApiResult> ChessGameAsync()
        {
            return Task.Run(() =>
            {
                var game = ChessFactory.NewChessGame();
                var items = game.BoardState.GetItems((int)game.CurrentPlayer).ToArray();

                var result = new ChessWebApiResult
                {
                    Board = ChessGameConvert.Serialise(game),
                    BoardText = new ChessBoardBuilder().FromChessGame(game).ToTextBoard(),
                    AvailableMoves = ToMoveList(game, items),
                    WhoseTurn = game.CurrentPlayer.ToString(),
                    Message = ""
                };

                return result;
            });
        }

        public Task<ChessWebApiResult> PlayMoveAsync(string board, string move)
        {
            return Task.Run(() =>
            {
                var game = ChessGameConvert.Deserialise(board);
                var msg = game.Move(move);
                var items = game.BoardState.GetItems((int) game.CurrentPlayer).ToArray();
                var result = new ChessWebApiResult
                {
                    Board = ChessGameConvert.Serialise(game),
                    BoardText = new ChessBoardBuilder().FromChessGame(game).ToTextBoard(),
                    AvailableMoves = ToMoveList(game, items),
                    WhoseTurn = game.CurrentPlayer.ToString(),
                    Message = msg
                };
                return result;
            });
        }

        private Move[] ToMoveList(ChessGame game, params LocatedItem<ChessPieceEntity>[] locatedItems)
        {
            return locatedItems
                .SelectMany(i => i.Paths.FlattenMoves())
                .Select(m => new Move
                {
                    Coord = $"{m.ToChessCoords()}",
                    SAN = StandardAlgebraicNotation.ParseFromGameMove(game.BoardState, m, true).ToNotation()
                }).ToArray();
        }

        public Task<ChessWebApiResult> ChessGameAsync(string customSerialisedBoard)
        {
            return Task.Run(() =>
            {
                var game = ChessGameConvert.Deserialise(customSerialisedBoard);

                var items = game.BoardState.GetItems((int)game.CurrentPlayer).ToArray();
                var result = new ChessWebApiResult
                {
                    Board = ChessGameConvert.Serialise(game),
                    BoardText = new ChessBoardBuilder().FromChessGame(game).ToTextBoard(),
                    AvailableMoves = ToMoveList(game, items),
                    WhoseTurn = game.CurrentPlayer.ToString(),
                    Message = ""
                };
                return result;
            });
        }
    }
}
