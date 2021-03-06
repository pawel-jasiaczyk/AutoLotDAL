﻿using System;
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

		/// <summary>
		/// Opens the connection to AutoLot database.
		/// </summary>
		/// <param name="connectionString">Connection string.</param>
		public void OpenConnection(string connectionString)
		{
			sqlCn = new MySqlConnection ();
			sqlCn.ConnectionString = connectionString;
			sqlCn.Open ();
		}
		/// <summary>
		/// Closes the connection.
		/// </summary>
		public void CloseConnection()
		{
			sqlCn.Clone ();
		}
		/// <summary>
		/// Inserts new auto to Inventory table.
		/// </summary>
		/// <param name="id">Identifier(CarID).</param>
		/// <param name="color">Color.</param>
		/// <param name="make">Make.</param>
		/// <param name="petName">Pet name(PetName).</param>
		public void InsertAuto(int id, string color, string make, string petName)
		{
			string sql = string.Format ("INSERT INTO Inventory" +
			             " VALUES (@CarID, @Make, @Color, @PetName)");
			using (MySqlCommand cmd = new MySqlCommand (sql, this.sqlCn)) {
				MySqlParameter param = new MySqlParameter ();
				param.ParameterName = "@CarID";
				param.Value = id;
				param.MySqlDbType = MySqlDbType.UInt32;
				cmd.Parameters.Add (param);

				param = new MySqlParameter ();
				param.ParameterName = "@Make";
				param.Value = make;
				param.MySqlDbType = MySqlDbType.VarChar;
				param.Size = 50;
				cmd.Parameters.Add (param);

				param = new MySqlParameter ();
				param.ParameterName = "@Color";
				param.Value = color;
				param.MySqlDbType = MySqlDbType.VarChar;
				param.Size = 50;
				cmd.Parameters.Add (param);

				param = new MySqlParameter ();
				param.ParameterName = "@PetName";
				param.Value = petName;
				param.MySqlDbType = MySqlDbType.VarChar;
				param.Size = 50;
				cmd.Parameters.Add (param);

				cmd.ExecuteNonQuery ();

			}
		}
		/// <summary>
		/// Inserts new auto using NewCar class.
		/// </summary>
		/// <param name="car">NewCar object to insert</param>
		public void InsertAuto(NewCar car)
		{
			string sql = string.Format ("INSERT INTO Inventory" +
				"VALUES ({0}, '{1}', '{2}', '{3}')", car.CarID, car.Make, car.Color, car.PetName);
			using (MySqlCommand cmd = new MySqlCommand (sql, this.sqlCn)) {
				cmd.ExecuteNonQuery ();
			}
		}
		// <summary>
		/// Deletes the car from Inventory table.
		/// </summary>
		/// <param name="id">Identifier.(CarID)</param>
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
		/// <summary>
		/// Updates PetName of the car.
		/// </summary>
		/// <param name="id">Identifier.(CarID)</param>
		/// <param name="newPetName">New pet name(PetName).</param>
		public void UpdateCarPetName(int id, string newPetName)
		{
			string sql = string.Format ("UPDATE Inventory SET PetName='{0}' WHERE CarID={1}",
				newPetName, id);
			using(MySqlCommand cmd = new MySqlCommand(sql, this.sqlCn))
			{
				cmd.ExecuteNonQuery ();
			}
		}
		/// <summary>
		/// Gets all invenory as list od NewCar objects.
		/// </summary>
		/// <returns>The all invenory as list.</returns>
		public List<NewCar> GetAllInvenoryAsList()
		{
			// lista przechowująca pobierane rekordy
			List<NewCar> inv = new List<NewCar> ();

			// polecenie
			string sql = "SELECT * FROM Inventory";
			using (MySqlCommand cmd = new MySqlCommand (sql, this.sqlCn)) {
				MySqlDataReader dr = cmd.ExecuteReader ();
				while (dr.Read ()) {
					inv.Add (new NewCar {
						CarID = (int)dr ["CarID"],
						Make = (string)dr ["Make"],
						Color = (string)dr ["Color"],
						PetName = (string)dr ["PetName"]
					});
				}
				dr.Close ();
			}
			return inv;
		}
		/// <summary>
		/// Gets all inventory as DataTable set.
		/// </summary>
		/// <returns>The all inventory as data table.</returns>
		public DataTable GetAllInventoryAsDataTable()
		{
			DataTable inv = new DataTable ();

			string sql = "SELECT * FROM Inventory";
			using (MySqlCommand cmd = new MySqlCommand (sql, this.sqlCn)) {
				MySqlDataReader dr = cmd.ExecuteReader ();
				inv.Load (dr);
				dr.Close ();
			}
			return inv;
		}
		/// <summary>
		/// Calls Stored Procedure from AutoLot and 
		/// returns the PetName of a Car with specified CarID.
		/// </summary>
		/// <returns>Car's PetName</returns>
		/// <param name="carID">CarID</param>
		public string LookUpPetName(int carID)
		{
			string carPetName = string.Empty;

			using (MySqlCommand cmd = new MySqlCommand ("GetPetName", this.sqlCn)) {
				cmd.CommandType = CommandType.StoredProcedure;

				// input parameters
				MySqlParameter param = new MySqlParameter ();
				param.ParameterName = "@carID";
				param.MySqlDbType = MySqlDbType.UInt32;
				param.Value = carID;

				param.Direction = ParameterDirection.Input;
				cmd.Parameters.Add (param);

				// output parameters
				param = new MySqlParameter();
				param.ParameterName = "@petName";
				param.MySqlDbType = MySqlDbType.VarChar;
				param.Size = 50;
				param.Direction = ParameterDirection.Output;
				cmd.Parameters.Add (param);

				// execute stored procedure
				cmd.ExecuteNonQuery();

				// return output parameter
				carPetName = (string)cmd.Parameters["@petName"].Value;
			}
			return carPetName;
		}
	}
	/// <summary>
	/// Class represents car object to insert to and select from Inventory table in AutoLot database.
	/// </summary>
	public class NewCar
	{
		public int CarID{ get; set; }
		public string Color{ get; set; }
		public string Make{ get; set; }
		public string PetName{ get; set; }
	}
}

