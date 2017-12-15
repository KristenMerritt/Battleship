using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Battleship.Config;
using Battleship.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace Battleship.Repos
{
    public class GameRepo
    {
        private readonly DataContext _context;

        public GameRepo(DataContext context)
        {
            _context = context;
        }

        // Retreives a game in the database
        // RETURN: db_Game 
        public db_Game GetGame(int id)
        {
            try
            {
                return _context.MySqlDb.Query<db_Game>("SELECT * FROM game " +
                                                       "WHERE game_id = " + id + ";",
                    commandType: CommandType.Text).FirstOrDefault();
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetGame");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetGame");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetGame");
                Debug.WriteLine(e.InnerException);
                return null;
            }
        }

        // Retreives a game in the database
        // RETURN: db_Game 
        public db_Game GetActiveGameByPlayers(int player1Id, int player2Id)
        {
            try
            {
                return _context.MySqlDb.Query<db_Game>("SELECT * FROM game " +
                                                       "WHERE ((player_1_id = "+player1Id+" AND player_2_id = "+player2Id+") " +
                                                                "OR (player_1_id = "+player2Id+" AND player_2_id = "+player1Id+")) " +
                                                       "AND complete = 0;",
                    commandType: CommandType.Text).FirstOrDefault();
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetGameByPlayers");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetGameByPlayers");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetGameByPlayers");
                Debug.WriteLine(e.InnerException);
                return null;
            }
        }

        // Retreives all of the active games for a certain player
        // RETURN: IEnumerable<db_Game> 
        public IEnumerable<db_Game> GetAllActiveGamesForPlayer(int playerId)
        {
            return _context.MySqlDb.Query<db_Game>("SELECT * FROM game " +
                                                   "WHERE (player_1_id = "+playerId+" " +
                                                   "OR player_2_id = "+playerId+") " +
                                                   "AND (complete = false);", 
                   commandType: CommandType.Text);
        }

        // Creates a new game object in the database
        // RETURN: bool 
        public db_Game CreateNewGame(db_Game gameInfo)
        {
            Debug.WriteLine("===========================");
            try
            {
                // Ensure there is not already an active game between the two players
                if (GetActiveGameByPlayers(gameInfo.Player_1_Id, gameInfo.Player_2_Id) == null)
                {
                    Debug.WriteLine("No active games already between the two players.");

                    // Create the new game
                    _context.MySqlDb.Query<db_Game>("INSERT INTO game (" +
                                                    "player_1_id, " +
                                                    "player_2_id, " +
                                                    "complete, " +
                                                    "turn" +
                                                    ") VALUES (" +
                                                    gameInfo.Player_1_Id + ", " +
                                                    gameInfo.Player_2_Id + "," +
                                                    gameInfo.Complete + "," +
                                                    gameInfo.Turn + ");",
                        commandType: CommandType.Text);

                    Debug.WriteLine("Created new game with the following data:");
                    Debug.WriteLine("Player 1: " + gameInfo.Player_1_Id);
                    Debug.WriteLine("Player 2: " + gameInfo.Player_2_Id);
                    Debug.WriteLine("Complete: " + gameInfo.Complete);
                    Debug.WriteLine("Turn: " + gameInfo.Turn);

                    // Retrieve the game created
                    var gameCreated = _context.MySqlDb.Query<db_Game>("SELECT * FROM game " +
                                                                      "WHERE player_1_id = "+gameInfo.Player_1_Id+" " +
                                                                      "AND player_2_id = "+gameInfo.Player_2_Id+" " +
                                                                      "AND complete = "+gameInfo.Complete+" " +
                                                                      "AND turn = "+gameInfo.Turn+";",
                        commandType: CommandType.Text).FirstOrDefault();

                    Debug.WriteLine("Selected the new game with the following data:");
                    Debug.WriteLine("Game Id: " + gameCreated.Game_Id);
                    Debug.WriteLine("Player 1: " + gameCreated.Player_1_Id);
                    Debug.WriteLine("Player 2: " + gameCreated.Player_2_Id);
                    Debug.WriteLine("Complete: " + gameCreated.Complete);
                    Debug.WriteLine("Turn: " + gameCreated.Turn);

                    // Save the game's ID
                    var gameCreatedId = gameCreated.Game_Id;

                    // Create and set the first board
                    _context.MySqlDb.Query<db_Board>("INSERT INTO board (game_id) VALUES ("+gameCreatedId+");", commandType: CommandType.Text);
                    Debug.WriteLine("Created new board 1");

                    var board1 = _context.MySqlDb.Query<db_Board>("SELECT * FROM board WHERE game_id = "+gameCreatedId+";", commandType: CommandType.Text).LastOrDefault();
                    var board1Id = board1.Board_Id;
                    Debug.WriteLine("Selected the new board with the following data:");
                    Debug.WriteLine("Board ID: " + board1.Board_Id);
                    Debug.WriteLine("Game ID: " + board1.Game_Id);

                    _context.MySqlDb.Query<db_Game>("UPDATE game SET player_1_board_id = "+board1Id+";", commandType: CommandType.Text);
                    Debug.WriteLine("Updated Game with Board 1");

                    // Create and set the second board
                    _context.MySqlDb.Query<db_Board>("INSERT INTO board (game_id) VALUES ("+gameCreatedId+");", commandType: CommandType.Text);
                    Debug.WriteLine("Created new board 2");

                    var board2 = _context.MySqlDb.Query<db_Board>("SELECT * FROM board WHERE game_id = " + gameCreatedId + ";", commandType: CommandType.Text).LastOrDefault();
                    var board2Id = board2.Board_Id;
                    Debug.WriteLine("Selected the new board with the following data:");
                    Debug.WriteLine("Board ID: " + board1.Board_Id);
                    Debug.WriteLine("Game ID: " + board1.Game_Id);

                    _context.MySqlDb.Query<db_Game>("UPDATE game SET player_2_board_id = " + board2Id + ";", commandType: CommandType.Text);
                    Debug.WriteLine("Updated Game with Board 2");

                    // Re-retrieve the created game that has been updated
                    var updatedCreatedGame = _context.MySqlDb.Query<db_Game>("SELECT * FROM game WHERE game_id = "+gameCreatedId+";", 
                                            commandType: CommandType.Text).FirstOrDefault();
                    Debug.WriteLine("Selected the updated game with the following data:");
                    Debug.WriteLine("Game Id: " + updatedCreatedGame.Game_Id);
                    Debug.WriteLine("Player 1: " + updatedCreatedGame.Player_1_Id);
                    Debug.WriteLine("Player 2: " + updatedCreatedGame.Player_2_Id);
                    Debug.WriteLine("Player 1 Board ID: " + updatedCreatedGame.Player_1_Board_Id);
                    Debug.WriteLine("Player 1 Board ID: " + updatedCreatedGame.Player_2_Board_Id);
                    Debug.WriteLine("Complete: " + updatedCreatedGame.Complete);
                    Debug.WriteLine("Turn: " + updatedCreatedGame.Turn);

                    return updatedCreatedGame;
                }

                return null;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN CreateNewGame");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN CreateNewGame");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN CreateNewGame");
                Debug.WriteLine(e.InnerException);
                return null;
            }          
        }

        // Sets the complete status of a game in the database
        // RETURN: bool 
        public bool SetGameStatus(int gameId, bool status)
        {
            try
            {
                _context.MySqlDb.Query<db_Game>("UPDATE game " +
                                                "SET complete = "+status+" " +
                                                "WHERE game_id = "+gameId+";",
                commandType: CommandType.Text);
                return true;
            }            
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN SetGameStatus");
                Debug.WriteLine(mysqlex.InnerException);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN SetGameStatus");
                Debug.WriteLine(ioe.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN SetGameStatus");
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }

        // Sets the complete status of a game in the database
        // RETURN: bool 
        public db_Game SetGameBoards(db_Game game)
        {
            try
            {
                return _context.MySqlDb.Query<db_Game>("UPDATE game SET " +
                                                       "player_1_board_id = "+game.Player_1_Board_Id+"," +
                                                       "player_2_board_id = "+game.Player_2_Board_Id+" " +
                                                       "WHERE game_id = "+game.Game_Id+";",
                       commandType: CommandType.Text).FirstOrDefault();
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN SetGameBoards");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN SetGameBoards");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN SetGameBoards");
                Debug.WriteLine(e.InnerException);
                return null;
            }
        }
    }
}
