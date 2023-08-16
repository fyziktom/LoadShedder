using LoadShedder.Common;
using System.Security.Cryptography.X509Certificates;

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

    public enum GamePlayStage
    {
        None,
        Start,
        LoadOfSources,
        LoadOfConsumers,
        BalancingOfNetwork,
        TimePenalty,
        End
    }

    public enum GameResponseActions
    {
        None,
        StartingWithoutSources,
        BlackOut,
        BlackoutRecovery,
        Overproduction,
        Overconsumption,
        EndOfTheGame_Success,
        EndOfTheGame_Loose,
    }

    public class GameResponseActionEventArgs
    {
        public GameResponseActions Action { get; set; } = GameResponseActions.None;
        public GamePlayStage Stage { get; set; } = GamePlayStage.None;
        public string PlayerId { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string BoardId { get; set; } = string.Empty;
        /// <summary>
        /// Rest of the time penalty in the seconds
        /// </summary>
        public double RestOfThePenalty { get; set; } = 0.0;
        /// <summary>
        /// Actual bilance of sources for the GameBoard
        /// </summary>
        public double ActualBilanceSources { get; set; } = 0.0;
        /// <summary>
        /// Actual bilance of consumers for the GameBoard
        /// </summary>
        public double ActualBilanceConsumers { get; set; } = 0.0;
        /// <summary>
        /// Actual total bilance for the GameBoard
        /// </summary>
        public double ActualBilance { get; set; } = 0.0;
        /// <summary>
        /// Elapsed time in seconds
        /// </summary>
        public double ActualElapsedGameTime { get; set; } = 0.0;
    }

    /// <summary>
    /// Instance of one game. It can be singleplayer or multiplayer
    /// </summary>
    public class Game : GameTime
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
        /// Data about the players progress in the game
        /// These objects contains also history of bilancing for each player
        /// </summary>
        public Dictionary<string, PlayerGameData> PlayersGameData { get; set; } = new Dictionary<string, PlayerGameData>();
        /// <summary>
        /// Indicates if the game is running
        /// </summary>
        public bool IsPlaying { get; set; } = false;
        /// <summary>
        /// Indicates when the game is already at the end
        /// </summary>
        public bool IsEndGame { get; set; } = false;

        /// <summary>
        /// When the game needs to tell other classes that happened something important
        /// </summary>
        public event EventHandler<GameResponseActionEventArgs> GameRespondingAction;

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
            if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
            {
                b.NewDataLoaded -= GameBoard_NewDataLoaded;
                b.NewDataLoaded += GameBoard_NewDataLoaded;
            }
            
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
            if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
                b.NewDataLoaded -= GameBoard_NewDataLoaded;

            return true;
        }

        /// <summary>
        /// Clear dictionary with the Players Game Data
        /// </summary>
        public void ClearGameData()
        {
            PlayersGameData.Clear();
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

            // add PlayerGameData for each board/player and change the GamePlayStage for them to Start
            foreach(var boardId in GameBoardIds)
            {
                if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
                {
                    b.NewDataLoaded -= GameBoard_NewDataLoaded;
                    b.NewDataLoaded += GameBoard_NewDataLoaded;
                    
                    if (!string.IsNullOrEmpty(b.PlayerId))
                    {
                        var pgm = new PlayerGameData()
                        {
                            StartTime = StartTime,
                            PlayerId = b.PlayerId,
                            GameId = Id,

                        };
                        pgm.ChangePlayStage(GamePlayStage.Start);

                        if (!PlayersGameData.ContainsKey(b.PlayerId))
                            PlayersGameData.Add(b.PlayerId, pgm);
                        else
                            PlayersGameData[b.PlayerId].ChangePlayStage(GamePlayStage.Start);

                        GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                        {
                            Action = GameResponseActions.StartingWithoutSources,
                            BoardId = boardId,
                            PlayerId = b.PlayerId,
                            GameId = Id,
                            Stage = pgm.ActualGamePlayStage,
                            ActualElapsedGameTime = ElapsedTime.TotalSeconds
                        });
                    }
                }
            }

            return "OK";
        }


        public string EndGame()
        {
            if (!IsPlaying) 
                return "Game is not running now.";

            foreach (var boardId in GameBoardIds)
            {
                if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
                    b.NewDataLoaded -= GameBoard_NewDataLoaded;
            }

            if (IsEndGame)
            {
                IsPlaying = false;
                EndTime = DateTime.UtcNow;
            }

            return "OK";
        }

        private void GameBoard_NewDataLoaded(object? sender, string e)
        {
            ProcessGameLogic(e);
        }

        private void ProcessGameLogic(string boardId)
        {
            if (!IsPlaying)
                return;
            if (IsEndGame)
                return;
            if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
            {
                if (!string.IsNullOrEmpty(b.PlayerId))
                {
                    if (PlayersGameData.TryGetValue(b.PlayerId, out var pgm))
                    {
                        switch (pgm.ActualGamePlayStage)
                        {
                            case GamePlayStage.None:
                                return;
                            case GamePlayStage.Start:
                                Stage_StartGame(b.PlayerId, boardId, pgm);
                                break;
                            case GamePlayStage.LoadOfSources:
                                Stage_LoadOfSources(b.PlayerId, boardId, pgm);
                                break;
                            case GamePlayStage.LoadOfConsumers:
                                Stage_LoadOfConsumers(b.PlayerId, boardId, pgm);
                                break;
                            case GamePlayStage.BalancingOfNetwork:
                                Stage_BalancingOfNetwork(b.PlayerId, boardId, pgm);
                                break;
                            case GamePlayStage.TimePenalty:
                                Stage_TimePenalty(b.PlayerId, boardId, pgm);
                                break;
                            case GamePlayStage.End:
                                Stage_EndOfGame(b.PlayerId, boardId, pgm);
                                break;
                        }
                    }
                }
            }
        }

        private void Stage_StartGame(string playerId, string boardId, PlayerGameData pgm)
        {
            pgm.ChangePlayStage(GamePlayStage.LoadOfSources);

            GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
            {
                Action = GameResponseActions.StartingWithoutSources,
                BoardId = boardId,
                PlayerId = playerId,
                GameId = Id,
                Stage = pgm.ActualGamePlayStage,
                ActualElapsedGameTime = ElapsedTime.TotalSeconds
            });
        }

        private void Stage_LoadOfSources(string playerId, string boardId, PlayerGameData pgm)
        {
            if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
            {
                var bilanceSources = b.GetActualBilanceForSources();
                var bilanceConsumers = b.GetActualBilanceForConsumers();
                var bilance = b.GetActualBilance();

                pgm.LoadNewBilances(bilanceSources, bilanceConsumers, bilance);

                var response = GetResponseAction(bilance);

                // if the player plug the consumer before the grid has 75MW it will cause the blackout
                if (bilanceConsumers > 0)
                {
                    // select the type of the penalty
                    pgm.ActualGameTimePenalty = GameTimePenalty.FIFTHTEEN_SECONDS;
                    // change the play stage of the player to Penalty
                    pgm.ChangePlayStage(GamePlayStage.TimePenalty);

                    // inform UI about the caused blackout for the player
                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = GameResponseActions.BlackOut,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });

                    return;
                }
                // when they reach the 75MW it will move them to the next level
                else if (bilanceSources >= 75000) 
                {
                    pgm.ChangePlayStage(GamePlayStage.LoadOfConsumers);

                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = response,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
                else
                {
                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = response,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        RestOfThePenalty = 0,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
            }

        }

        private void Stage_LoadOfConsumers(string playerId, string boardId, PlayerGameData pgm)
        {
            if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
            {
                var bilanceSources = b.GetActualBilanceForSources();
                var bilanceConsumers = b.GetActualBilanceForConsumers();
                var bilance = b.GetActualBilance();

                pgm.LoadNewBilances(bilanceSources, bilanceConsumers, bilance);

                var response = GetResponseAction(bilance);

                if (bilance <= 10000 && bilance >= 0) // next level is when they will plug enough of consumers to have just 10MW over production
                {
                    pgm.ChangePlayStage(GamePlayStage.BalancingOfNetwork);

                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = GameResponseActions.Overproduction,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
                else if (bilance < 0) // if the bilance will drop under zero you have a blackout
                {
                    pgm.ActualGameTimePenalty = GameTimePenalty.FIFTHTEEN_SECONDS;
                    pgm.ChangePlayStage(GamePlayStage.TimePenalty);

                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = GameResponseActions.BlackOut,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        RestOfThePenalty = (double)pgm.ActualGameTimePenalty,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
                else
                {
                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = response,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        RestOfThePenalty = 0,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
            }
        }

        private void Stage_BalancingOfNetwork(string playerId, string boardId, PlayerGameData pgm)
        {
            if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
            {
                var bilanceSources = b.GetActualBilanceForSources();
                var bilanceConsumers = b.GetActualBilanceForConsumers();
                var bilance = b.GetActualBilance();

                var response = GetResponseAction(bilance);

                pgm.LoadNewBilances(bilanceSources, bilanceConsumers, bilance);

                if (bilance == 0) // next level is when they will plug enough of consumers to have just 10MW over production
                {
                    pgm.ChangePlayStage(GamePlayStage.End);

                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = GameResponseActions.EndOfTheGame_Success,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
                else if (bilance <= -5000) // too large overconsumption
                {
                    pgm.ActualGameTimePenalty = GameTimePenalty.FIFTHTEEN_SECONDS;
                    pgm.ChangePlayStage(GamePlayStage.TimePenalty);

                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = GameResponseActions.BlackOut,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        RestOfThePenalty = (double)pgm.ActualGameTimePenalty,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });

                }
                else if (bilance >= 15000) // too large overproduction
                {
                    pgm.ActualGameTimePenalty = GameTimePenalty.FIFTHTEEN_SECONDS;
                    pgm.ChangePlayStage(GamePlayStage.TimePenalty);

                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = GameResponseActions.BlackOut,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        RestOfThePenalty = (double)pgm.ActualGameTimePenalty,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
                else
                {
                    // nothing extra out of the balance. Just inform the UI about the actual stage
                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = response,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
            }
        }

        private void Stage_TimePenalty(string playerId, string boardId, PlayerGameData pgm)
        {
            if (MainDataContext.GameBoards.TryGetValue(boardId, out var b))
            {
                var bilanceSources = b.GetActualBilanceForSources();
                var bilanceConsumers = b.GetActualBilanceForConsumers();
                var bilance = b.GetActualBilance();

                pgm.LoadNewBilances(bilanceSources, bilanceConsumers, bilance);

                if (pgm.GamePenaltyStartTime < DateTime.UtcNow && 
                    (DateTime.UtcNow - pgm.GamePenaltyStartTime).TotalSeconds < (double)pgm.ActualGameTimePenalty)
                {
                    // still waiting for the running out of penalty

                    // inform UI about the rest of the penalty time
                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = GameResponseActions.BlackoutRecovery,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        RestOfThePenalty = (double)pgm.ActualGameTimePenalty - (DateTime.UtcNow - pgm.GamePenaltyStartTime).TotalSeconds,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });
                }
                else
                {
                    // get stage which was before the penalty
                    var prevStage = pgm.GamePlayStagesHistory.OrderBy(s => s.Key).Skip(pgm.GamePlayStagesHistory.Count - 2).First().Value;

                    // change player stage
                    pgm.ChangePlayStage(prevStage);
                    pgm.ActualGameTimePenalty = GameTimePenalty.NONE;

                    var response = GetResponseAction(bilance);

                    GameRespondingAction?.Invoke(this, new GameResponseActionEventArgs()
                    {
                        Action = response,
                        BoardId = boardId,
                        PlayerId = playerId,
                        GameId = Id,
                        Stage = pgm.ActualGamePlayStage,
                        RestOfThePenalty = 0,
                        ActualBilanceSources = bilanceSources,
                        ActualBilanceConsumers = bilanceConsumers,
                        ActualBilance = bilance,
                        ActualElapsedGameTime = ElapsedTime.TotalSeconds
                    });

                }
            }

        }

        private void Stage_EndOfGame(string playerId, string boardId, PlayerGameData pgm)
        {
            IsEndGame = true;
            EndGame();
        }

        private GameResponseActions GetResponseAction(double bilance)
        {
            var response = GameResponseActions.Overproduction;

            if (bilance < 0)
                response = GameResponseActions.Overconsumption;

            return response;
        }

    }

}
