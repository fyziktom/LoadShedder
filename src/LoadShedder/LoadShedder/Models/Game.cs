using LoadShedder.Common;

namespace LoadShedder.Models
{
    /// <summary>
    /// Game types based on the amount of the players
    /// </summary>
    public enum GameType
    {
        SINGLEPLAYER_TIME_SCORE,
        SINGLEPLAYER_BALANCING_SCORE,
        MULTIPLAYER_TIME_SCORE,
        MULTIPLAYER_BALANCING_SCORE
    }

    /// <summary>
    /// Instance of one game. It can be singleplayer or multiplayer
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Unique ID of the game
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// Selected type of the game
        /// </summary>
        public GameType GameType { get; set; } = GameType.SINGLEPLAYER_TIME_SCORE;
        /// <summary>
        /// Game players based on the devices which are attached to this game
        /// </summary>
        public List<string> PlayersIds
        {
            get
            {
                var players = new List<string>();
                if (GameBoardIds != null && GameBoardIds.Count > 0)
                {
                    foreach(var board in GameBoardIds)
                    {
                        if (MainDataContext.GameBoards.TryGetValue(board, out var b))
                        {
                            if (!string.IsNullOrEmpty(b.PlayerId))
                                players.Add(b.PlayerId);
                            else
                                players.Add("UNKNOWN");
                        }
                    }
                }

                return players;
            }
        }

        /// <summary>
        /// List of GameBoards in this game. Each Game Board has device Id inside
        /// </summary>
        public List<string> GameBoardIds { get; set; } = new List<string>();

        /// <summary>
        /// Start time of the game
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// End time if the game has already ended. Otherwise there is the max value
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.MaxValue;
        /// <summary>
        /// Elapsed time of the game
        /// </summary>
        public TimeSpan ElapsedTime { get => DateTime.UtcNow - StartTime; }
        /// <summary>
        /// Indicates if the game is running
        /// </summary>
        public bool IsPlaying { get; set; } = false;
        /// <summary>
        /// Indicates when the game is already at the end
        /// </summary>
        public bool IsEndGame { get; set; } = false;

        /// <summary>
        /// Add board to the game
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public bool AddBoardToGame(string boardId)
        {
            if (boardId == null || string.IsNullOrEmpty(boardId))
                return false;

            if (GameBoardIds.Contains(boardId)) 
                return false;

            GameBoardIds.Add(boardId);

            return true;
        }

        /// <summary>
        /// Remove board from the game
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public bool RemoveBoardFromGame(string boardId)
        {
            if (boardId == null || string.IsNullOrEmpty(boardId))
                return false;
            if (!GameBoardIds.Contains(boardId))
                return false;
            GameBoardIds.Remove(boardId);
            return true;
        }

        public string StartGame()
        {
            if (GameBoardIds == null || GameBoardIds.Count == 0)
                return "No boards in the game. Please add boards first";

            if (GameBoardIds.Count == 1 && (GameType == GameType.MULTIPLAYER_TIME_SCORE ||
                                          GameType == GameType.MULTIPLAYER_BALANCING_SCORE))
                return "You have just one board in the game, but the game type is multiplayer. " +
                       "Please add another board to start the game or change game type to singleplayer";

            if (GameBoardIds.Count > 1 && (GameType == GameType.SINGLEPLAYER_TIME_SCORE ||
                                          GameType == GameType.SINGLEPLAYER_BALANCING_SCORE))
                return "You have more devices in the game, but the game type is singleplayer. " +
                       "Please keep just one board to start the game or change game type to singleplayer. " +
                       "Otherwise game will take just first board as main player.";

            IsEndGame = false;
            IsPlaying = true;

            // reset timers
            EndTime = DateTime.MaxValue;
            StartTime = DateTime.UtcNow;

            return "OK";
        }

        public string EndGame()
        {
            if (!IsPlaying) 
                return "Game is not running now.";

            if (!IsEndGame)
            {
                IsPlaying = false;
                EndTime = DateTime.UtcNow;
            }

            return "OK";
        }

    }

}
