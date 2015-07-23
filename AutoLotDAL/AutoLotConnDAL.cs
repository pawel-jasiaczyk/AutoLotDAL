using System;
using System.Collections.Generic;
using System.Text;

// I need to try write it as a ProviderFactory model
// Current version is targetet for MySQL database
using System.Data;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace AutoLotConnectionLayer
{
	public class InventoryDAL
	{
		// this variable will be used by every one methods
		private MySqlConnection sqlCn = null;

		public void OpenConnection(string connectionString)
		{
			sqlCn = new MySqlConnection ();
			sqlCn.ConnectionString = connectionString;
			sqlCn.Open ();
		}

		public void CloseConnection()
		{
			sqlCn.Clone ();
		}

		public void InsertAuto(int id, string color, string make, string petName)
		{
			string sql = string.Format ("INSERT INTO Inventory" +
			             "VALUES ({0}, '{1}', '{2}', '{3}')", id, make, color, petName);
			using (MySqlCommand cmd = new MySqlCommand (sql, this.sqlCn)) {
				cmd.ExecuteNonQuery ();
			}
		}

		public void InsertAuto(NewCar car)
		{
			string sql = string.Format ("INSERT INTO Inventory" +
				"VALUES ({0}, '{1}', '{2}', '{3}')", car.CarID, car.Make, car.Color, car.PetName);
			using (MySqlCommand cmd = new MySqlCommand (sql, this.sqlCn)) {
				cmd.ExecuteNonQuery ();
			}
		}

		public void DeleteCar(int id)
		{
			string sql = string.Format ("DELETE FROM Inventory WHERE CarID={0}",id);
			using (MySqlCommand cmd = new MySqlCommand (sql, this.sqlCn)) {
				try{
					cmd.ExecuteNonQuery();
				}
				catch(MySqlException ex) {
					Exception error = new Exception ("Sorry!, That car is on order!", ex);
					throw error;
				}
			}
		}

		public void UpdateCarPetName(int id, string newPetName)
		{
			string sql = string.Format ("UPDATE Inventory SET PetName='{0}' WHERE CarID={1}",
				newPetName, id);
			using(MySqlCommand cmd = new MySqlCommand(sql, this.sqlCn))
			{
				cmd.ExecuteNonQuery ();
			}
		}
	}

	public class NewCar
	{
		public int CarID{ get; set; }
		public string Color{ get; set; }
		public string Make{ get; set; }
		public string PetName{ get; set; }
	}
}

