using System;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Battleship.Config
{
	public class DataContext
	{
		private readonly IConfiguration _config;
		private MySqlConnection _mySqlDb;

        /// <summary>
        /// Creates a link between the application 
        /// and the database.
        /// </summary>
        /// <param name="config"></param>
		public DataContext(IConfiguration config)
		{
			_config = config;
		}

        /// <summary>
        /// Make the connection to the MySQL database
        /// </summary>
		public MySqlConnection MySqlDb => _mySqlDb ?? (_mySqlDb = GetDbConnection("Default"));

        /// <summary>
        /// Use the connection string to initiate connection
        /// </summary>
        /// <param name="connectionStringKey"></param>
        /// <returns>Connection</returns>
		private MySqlConnection GetDbConnection(string connectionStringKey)
		{
			var connstr = _config["ConnectionStrings:" + connectionStringKey];

			if (string.IsNullOrWhiteSpace(connstr))
			    throw new ArgumentException("Bad connection string");
		    var connection = new MySqlConnection(connstr);
		    return connection;
	    }
    }
}
