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
    public class ShotRepo
    {
        private readonly DataContext _context;

        public ShotRepo(DataContext context)
        {
            _context = context;
        }

        // Returns all of the shots for a specific board
        // PARAM: int boardId
        // RETURN: IEnumerable<db_Shot>
        public IEnumerable<db_Shot> GetAllShotsForBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_Shot>("SELECT * FROM shot WHERE board_id = " + boardId + ";",
                commandType: CommandType.Text);
        }

        // Returns all of the new shots for a specific board
        // PARAM: int boardId
        // PARAM: int shotId
        // RETURN: IEnumerable<db_Shot>
        public IEnumerable<db_Shot> GetNewShotsForBoard(int shotId, int board1Id, int board2Id)
        {
            return _context.MySqlDb.Query<db_Shot>("SELECT * FROM shot WHERE shot_id > " + shotId + " " +
                                                   "AND (board_id = " + board1Id + " OR board_id = "+board2Id+");",
                commandType: CommandType.Text);
        }

        // Returns all of the shots for a specific board where there was a hit
        // PARAM: int boardId
        // RETURN: IEnumerable<db_Shot>
        public IEnumerable<db_Shot> GetAllHitsForBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_Shot>("SELECT * FROM shot WHERE (board_id = " + boardId + ") AND (is_hit = 1);",
                commandType: CommandType.Text);
        }

        // Creates a new shot in the DB.
        // Switches whos turn it is in the DB.
        // PARAM: db_Shot
        // RETURN: bool
        public bool CreateNewShot(db_Shot shot)
        {
            try
            {
                Debug.WriteLine("Board ID: " + shot.Board_Id);
                Debug.WriteLine("Row: " + shot.Row);
                Debug.WriteLine("Col: " + shot.Col);
                Debug.WriteLine("Hit: " + shot.Is_Hit);

                _context.MySqlDb.Query<db_Shot>("INSERT INTO shot (board_id, row, col, is_hit) VALUES (" + shot.Board_Id + ", " + shot.Row + ", " + shot.Col + ", " + shot.Is_Hit + ");",
                    commandType: CommandType.Text);

                var game = _context.MySqlDb.Query<db_Game>("SELECT * FROM game WHERE player_1_board_id = "+shot.Board_Id+" " +
                                                           "OR player_2_board_id = "+shot.Board_Id+";",
                    commandType: CommandType.Text).FirstOrDefault();

                var nextTurn = shot.Board_Id == game.Player_1_Board_Id ? game.Player_1_Id : game.Player_2_Id;

                Debug.WriteLine("Setting turn: " + nextTurn);

                _context.MySqlDb.Query<db_Game>("UPDATE game " +
                                                "SET turn = "+nextTurn+" " +
                                                "WHERE game_id = "+game.Game_Id+";",
                    commandType: CommandType.Text);

                return true;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN CreateNewShot");
                Debug.WriteLine(mysqlex.InnerException);
                Debug.WriteLine(mysqlex.StackTrace);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN CreateNewShot");
                Debug.WriteLine(ioe.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN CreateNewShot");
                Debug.WriteLine(e.InnerException);
                return false;
            }           
        }
    }
}
