using System;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Battleship.Config
{
	public class DataContext
	{
		private readonly IConfiguration _config;
		private MySqlConnection _mySqlDb;

		public DataContext(IConfiguration config)
		{
			_config = config;
		}

		public MySqlConnection MySqlDb => _mySqlDb ?? (_mySqlDb = GetDbConnection("Default"));

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
