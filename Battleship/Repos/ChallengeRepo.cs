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
    public class ChallengeRepo
    {
        private readonly DataContext _context;

        public ChallengeRepo(DataContext context)
        {
            _context = context;
        }

        public db_Challenge GetChallenge(int challengeId)
        {
            try
            {
                return _context.MySqlDb.Query<db_Challenge>("SELECT * FROM challenge " +
                                                            "WHERE challenge_id = "+challengeId+";",
                    commandType: CommandType.Text).FirstOrDefault();
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetAllPendingPlayerChallenges");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetAllPendingPlayerChallenges");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetAllPendingPlayerChallenges");
                Debug.WriteLine(e.InnerException);
                return null;
            }
        }

        // Retreives all of the pending challenges for a certain player
        // RETURN: IEnumerable<db_Challenge> 
        public IEnumerable<db_Challenge> GetAllPendingOrAcceptedChallenges(int playerId)
        {
            try
            {
                return _context.MySqlDb.Query<db_Challenge>("SELECT * FROM challenge " +
                                                            "WHERE (player_1 = "+playerId+" " +
                                                            "OR player_2 = "+playerId+") " +
                                                            "AND (accepted IS NULL OR accepted = 1);",
                    commandType: CommandType.Text);
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetAllPendingPlayerChallenges");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetAllPendingPlayerChallenges");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetAllPendingPlayerChallenges");
                Debug.WriteLine(e.InnerException);
                return null;
            }
        }

        // Retreives all of the pending challenges for a certain player after a certain id
        // RETURN: IEnumerable<db_Challenge> 
        public IEnumerable<db_Challenge> GetAllRecentPendingPlayerChallenges(int playerId, int challengeId)
        {
            try
            {
                return _context.MySqlDb.Query<db_Challenge>("SELECT * FROM challenge " +
                                                            "WHERE (player_1 = "+playerId+" " +
                                                            "OR player_2 = "+playerId+") " +
                                                            "AND (challenge_Id > "+challengeId+") " +
                                                            "AND (accepted IS NULL);",
                    commandType: CommandType.Text);
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetAllRecentPendingPlayerChallenges");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetAllRecentPendingPlayerChallenges");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetAllRecentPendingPlayerChallenges");
                Debug.WriteLine(e.InnerException);
                return null;
            }
        }

        // Inserts a new challenge into the challenge database
        // RETURN: bool
        public bool AddNewChallenge(db_Challenge challenge)
        {
            try
            {
                _context.MySqlDb.Query<db_Challenge>("INSERT INTO challenge " +
                                                            "(player_1, player_2) " +
                                                            "VALUES ("+challenge.Player_1+","+challenge.Player_2+");",
                    commandType: CommandType.Text);
                return true;
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN GetAllRecentPendingPlayerChallenges");
                Debug.WriteLine(mysqlex.InnerException);
                return false;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN GetAllRecentPendingPlayerChallenges");
                Debug.WriteLine(ioe.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN GetAllRecentPendingPlayerChallenges");
                Debug.WriteLine(e.InnerException);
                return false;
            }           
        }

        // Sets the status of a challenge in the challenge database
        // RETURN: bool
        public db_Challenge SetStatus(bool status, int challengeId)
        {
            try
            {
                _context.MySqlDb.Query<db_Challenge>("UPDATE challenge " +
                                                     "SET accepted = "+status+" " +
                                                     "WHERE challenge_id = "+challengeId+";",
                        commandType: CommandType.Text);

                return _context.MySqlDb.Query<db_Challenge>("SELECT * FROM challenge " +
                                                            "WHERE challenge_id = " + challengeId + ";",
                    commandType: CommandType.Text).FirstOrDefault();
            }
            catch (MySqlException mysqlex)
            {
                Debug.WriteLine("MYSQL EXCEPTION IN SetStatus");
                Debug.WriteLine(mysqlex.InnerException);
                return null;
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine("INVALID OPERATION EXCEPTION IN SetStatus");
                Debug.WriteLine(ioe.InnerException);
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXCEPTION IN SetStatus");
                Debug.WriteLine(e.InnerException);
                return null;
            }           
        }         
    }
}
