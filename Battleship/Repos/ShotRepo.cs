using System;
using System.Collections;
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
    public class ShotRepo
    {
        private readonly DataContext _context;

        /// <summary>
        /// Repo for the shot table. Communicates
        /// directly with the database.
        /// </summary>
        /// <param name="context"></param>
        public ShotRepo(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all of the shots for a specific board
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns>IEnumerable<db_Shot></returns>
        public IEnumerable<db_Shot> GetAllShotsForBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_Shot>("SELECT * FROM shot WHERE board_id = " + boardId + ";",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Returns all of the new shots for a specific board
        /// </summary>
        /// <param name="shotId"></param>
        /// <param name="board1Id"></param>
        /// <param name="board2Id"></param>
        /// <returns>IEnumerable<db_Shot></returns>
        public IEnumerable<db_Shot> GetNewShotsForBoard(int shotId, int board1Id, int board2Id)
        {
            return _context.MySqlDb.Query<db_Shot>("SELECT * FROM shot WHERE shot_id > " + shotId + " " +
                                                   "AND (board_id = " + board1Id + " OR board_id = "+board2Id+");",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Returns all of the shots for a specific board where there was a hit.
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns>IEnumerable<db_Shot></returns>
        public IEnumerable<db_Shot> GetAllHitsForBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_Shot>("SELECT * FROM shot WHERE (board_id = " + boardId + ") AND (is_hit = 1);",
                commandType: CommandType.Text);
        }

        /// <summary>
        /// Creates a new shot in the DB.
        /// Switches whos turn it is in the DB.
        /// </summary>
        /// <param name="shot"></param>
        /// <returns>ArrayList</returns>
        public ArrayList CreateNewShot(db_Shot shot)
        {
            try
            {
                // Insert the new shot into the DB
                _context.MySqlDb.Query<db_Shot>("INSERT INTO shot (board_id, row, col, is_hit) VALUES (" + shot.Board_Id + ", " + shot.Row + ", " + shot.Col + ", " + shot.Is_Hit + ");",
                    commandType: CommandType.Text);

                // Get the game that we just made a shot in
                var game = _context.MySqlDb.Query<db_Game>("SELECT * FROM game WHERE player_1_board_id = "+shot.Board_Id+" " +
                                                           "OR player_2_board_id = "+shot.Board_Id+";",
                    commandType: CommandType.Text).FirstOrDefault();

                // Switch the turn in the game
                var nextTurn = shot.Board_Id == game.Player_1_Board_Id ? game.Player_1_Id : game.Player_2_Id;

                // Update the game with the turn
                _context.MySqlDb.Query<db_Game>("UPDATE game " +
                                                "SET turn = "+nextTurn+" " +
                                                "WHERE game_id = "+game.Game_Id+";",
                    commandType: CommandType.Text);

                var win = false;

                // If the shot was a hit, get all of the hits for the board
                if (shot.Is_Hit == 1)
                {
                    var hitsForBoard = GetAllHitsForBoard(shot.Board_Id);
                    var shipLocationsForBoard = _context.MySqlDb.Query<db_ShipLocation>("SELECT * FROM ship_location WHERE board_id = " + shot.Board_Id + ";",
                                        commandType: CommandType.Text);

                    // Checking for the win condition
                    if (hitsForBoard.Count() == shipLocationsForBoard.Count())
                    {
                        var validWin = false;
                        var requiredValidHits = shipLocationsForBoard.Count();
                        var validHits = 0;

                        foreach(var hit in hitsForBoard)
                        {
                            var hitRow = hit.Row;
                            var hitCol = hit.Col;

                            foreach (var shipLocation in shipLocationsForBoard)
                            {
                                var shipLocationRow = shipLocation.Row;
                                var shipLocationCol = shipLocation.Col;

                                if (shipLocationRow == hitRow)
                                {
                                    if (shipLocationCol == hitCol)
                                    {
                                        validHits++;
                                    }
                                }
                            }
                        }

                        if (validHits == requiredValidHits)
                        {
                            win = true;
                            _context.MySqlDb.Query<db_ShipLocation>("UPDATE game" +
                                                                    "SET complete = 1  " +
                                                                    "WHERE board_id = " + shot.Board_Id + ";",
                                commandType: CommandType.Text);
                        }
                    }
                }

                var returnVal = new ArrayList();
                returnVal.Add("Shot made");
                if (win)
                {
                    returnVal.Add("Win");
                }
                else
                {
                    returnVal.Add("No win");
                }

                return returnVal;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN CreateNewShot");
                Debug.WriteLine(mysqlex.InnerException);
                Debug.WriteLine(mysqlex.StackTrace);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN CreateNewShot");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN CreateNewShot");
                Debug.WriteLine(e.InnerException);
                return null;
            }           
        }
    }
}
