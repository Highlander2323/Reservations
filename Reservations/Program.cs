using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace Reservations
{

    public class Database
    {
        //Link for the database location
        static public string conLink = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\\reservationsdb.mdf; Integrated Security = True; Connect Timeout = 30";
        public SqlConnection connection = null;
        public DataTable dt;
        public SqlDataAdapter da;
        public SqlDataReader dr;
        private static Database instance = null;
        private Database()
        {
            try
            {
                connection = new SqlConnection(conLink);
                connection.Open();
                Console.WriteLine("Connection started succesfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ~Database()
        {
            try
            {
                connection.Dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void checkConnection()
        {
            if (connection != null && connection.State == ConnectionState.Closed)
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection started succesfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        public static Database getInstance()
        {
            if (instance == null) 
                instance = new Database();
            instance.checkConnection();
            return instance;
        }


        public void getDataTable()
        {
            da = new SqlDataAdapter("select Username from Accounts", connection);
            dt = new DataTable();
            da.Fill(dt);
        }

        private bool checkLoginDB(string user, string pass)
        {
            string query = "select * from Accounts where Username = " + " '" + user + "'" + "and pwdcompare ('" + pass + "', Password) = 1";
            SqlCommand com = new SqlCommand(query, connection);

            // try/catch for the case in which there is an error in executing the query;
            try
            {
                dr = com.ExecuteReader();

                // check if username and password are matching ones existing in the database
                if (dr.Read())
                {
                    com.Dispose();
                    dr.Dispose();
                    return true;
                }
                else
                {
                    com.Dispose();
                    dr.Dispose();
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                com.Dispose();
                dr.Dispose();
                return false;
            }
            
        }

        public bool insertCred(string user, string pass)
        {
            // hash passwords with PWDENCRYPT;
            string query = "INSERT INTO Accounts (Username,Password) VALUES (@user, PWDENCRYPT(@pass))";
            SqlCommand com = new SqlCommand(query, connection);
            com.Parameters.AddWithValue("@user", user);
            com.Parameters.AddWithValue("@pass", pass);


            int result = com.ExecuteNonQuery();

            if (result < 0)
            {
                MessageBox.Show("Error while inserting values in the database", "Error");
                com.Dispose();
                return false;
            }
            else
            {
                Console.WriteLine("Data inserted succesfully into the DB");
                com.Dispose();
                return true;
            }
        }

        public bool checkRegister(string user)
        {
            getDataTable();
            foreach (DataRow row in dt.Rows)
            {
                if ((string)row[0] == user)
                    return false;

            }
            return true;
        }
        public bool checkLogin(string user, string pass)
        {
            if (checkLoginDB(user, pass))
                return true;
            else
                return false;
        }
        

    }
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
