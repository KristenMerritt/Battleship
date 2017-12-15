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
    public class BoardRepo
    {
        private readonly DataContext _context;

        public BoardRepo(DataContext context)
        {
            _context = context;
        }

        // Retreives all of the boards in the database
        // RETURN: IEnumerable<db_Board> 
        public IEnumerable<db_Board> GetAllBoards()
        {
            return _context.MySqlDb.Query<db_Board>("SELECT * FROM board;", commandType: CommandType.Text);
        }

        // Retreives a board in the database
        // RETURN: db_Board
        public db_Board GetBoard(int boardId)
        {
            return _context.MySqlDb.Query<db_Board>("SELECT * FROM board WHERE board_id = "+boardId+";", commandType: CommandType.Text).FirstOrDefault();
        }

        // Retreives all of the boards for a player in the database
        // RETURN: IEnumerable<db_Board> 
        public IEnumerable<db_Board> GetAllBoardsForGame(int gameId)
        {
            try
            {
                return _context.MySqlDb.Query<db_Board>("SELECT * FROM board " +
                                                        "WHERE game_id = " + gameId + ";",
                    commandType: CommandType.Text);
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetAllBoardsForGame");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetAllBoardsForGame");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetAllBoardsForGame");
                Debug.WriteLine(e.InnerException);
                return null;
            }
        }

        // Creates a new board
        // RETURN: db_Board 
        public db_Board CreateBoard(int gameId)
        {
            try
            {
                return _context.MySqlDb.Query<db_Board>("INSERT INTO board " +
                                                        "(game_id) " +
                                                        "VALUES " +
                                                        "("+gameId+");",
                       commandType: CommandType.Text).FirstOrDefault();
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN CreateBoard");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN CreateBoard");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN CreateBoard");
                Debug.WriteLine(e.InnerException);
                return null;
            }           
        }
    }
}
