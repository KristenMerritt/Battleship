using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Battleship.Config;
using Battleship.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace Battleship.Repos
{
    public class ChatRepo
    {
        private readonly DataContext _context;

        public ChatRepo(DataContext context)
        {
            _context = context;
        }

        // Retreives all of the chat in the database
        // RETURN: IEnumerable<db_Chat> 
        public IEnumerable<db_Chat> GetAllChat()
        {
            try
            {
                return _context.MySqlDb.Query<db_Chat>("SELECT * FROM chat;",
                    commandType: CommandType.Text);
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetAllChat");
                Debug.WriteLine(mysqlex.InnerException);
                return new List<db_Chat>();
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetAllChat");
                Debug.WriteLine(ioe.InnerException);
                return new List<db_Chat>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetAllChat");
                Debug.WriteLine(e.InnerException);
                return new List<db_Chat>();
            }          
        }

        // Adds a new chat object into the database
        public void AddNewChat(db_Chat chat)
        {
            Debug.WriteLine("ADDING: " + chat.Player_Id);
            _context.MySqlDb.Query<db_Chat>(
                "INSERT INTO chat (chat_id, player_id, message) VALUES ("+chat.Chat_Id+", "+chat.Player_Id+", '"+chat.Message+"');",
                commandType: CommandType.Text);
        }

        // Retreives all of the chat after a certain ID
        // RETURN: IEnumerable<db_Chat> 
        public IEnumerable<db_Chat> GetRecentChat(int chatId)
        {
            try
            {
                return _context.MySqlDb.Query<db_Chat>("SELECT * FROM chat WHERE chat_id > " + chatId + ";",
                    commandType: CommandType.Text).ToList();

            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetRecentChat");
                Debug.WriteLine(mysqlex.InnerException);
                return new List<db_Chat>();
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetRecentChat");
                Debug.WriteLine(ioe.InnerException);
                return new List<db_Chat>();
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetRecentChat");
                Debug.WriteLine(e.InnerException);
                return new List<db_Chat>();
            }
        }
    }
}
