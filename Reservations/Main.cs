using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Reservations
{
    public partial class Main : Form
    {
        // static variable that stores the name of the user;
        public static string USERNAME;
        // static variable that stores the selected ID of the element in the database;
        public static int CURR_ID;
        // static variable that stores the number of double rooms;
        public static int D_ROOMS = 4;
        // static variable that stores the number of triple rooms;
        public static int T_ROOMS = 4;

       
        private void clearBox()
        {
            boxName.Text = null;
            boxIn.Text = null;
            boxOut.Text = null;
            boxDouble.Text = null;
            boxTriple.Text = null;
        }

        private void clearBoxAv()
        {
            boxAvIn.Text = null;
            boxAvOut.Text = null;
        }

        // two dates compared, if first is smaller result is -1, if first is bigger it is 1, and if they are equal it is 0;
        private int compareDate(string d1, string d2)
        {
            if(d1.Length == 5)
            {
                d1 = addYear(d1);
            }

            if(d2.Length == 5)
            {
                d2 = addYear(d2);
            }
            try
            {
                if (int.Parse(d1.Substring(6, 4)) < int.Parse(d2.Substring(6, 4)))
                {
                    return -1;
                }
                else if (int.Parse(d1.Substring(6, 4)) > int.Parse(d2.Substring(6, 4)))
                {
                    return 1;
                }
                else
                {
                    if (int.Parse(d1.Substring(3, 2)) < int.Parse(d2.Substring(3, 2)))
                    {
                        return -1;
                    }
                    else if (int.Parse(d1.Substring(3, 2)) > int.Parse(d2.Substring(3, 2)))
                    {
                        return 1;
                    }
                    else
                    {
                        if (int.Parse(d1.Substring(0, 2)) < int.Parse(d2.Substring(0, 2)))
                        {
                            return -1;
                        }
                        else if (int.Parse(d1.Substring(3, 2)) > int.Parse(d2.Substring(3, 2)))
                        {
                            return 1;
                        }
                        else return 0;
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -2;
            }
        }

        // everytime a button is actioned, the data grid will be refreshed;
        private void refresh(Database db)
        {
            db.dt = new DataTable();
            SqlCommand cmd = new SqlCommand("select * from Reservations where Username = @user order by Date_in asc", db.connection);
            cmd.Parameters.AddWithValue("@user", USERNAME);
            try
            {
                db.dt.Load(cmd.ExecuteReader());
                gridData.DataSource = db.dt;
                db.connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteData(Database db)
        {
            db.dt = new DataTable();
            SqlCommand cmd = new SqlCommand("delete from Reservations where Id = @id", db.connection);
            cmd.Parameters.AddWithValue("@id", CURR_ID);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // adds the data form the text boxes to the database;
        private void addData(Database db)
        {
            // can't add if check-out date is earlier or in the same day as the check-in one;
            if (compareDate(boxIn.Text, boxOut.Text) != -1)
            {
                MessageBox.Show("Check-out date cannot be earlier that check-in date!");
                return;
            }

            if (boxIn.Text.Length == 5)
            {
                boxIn.Text = addYear(boxIn.Text);
            }
            if (boxOut.Text.Length == 5)
            {
                boxOut.Text = addYear(boxOut.Text);
            }

            db.dt = new DataTable();
            string query = "insert into Reservations (Name,Date_in,Date_out,Username,Rooms_double,Rooms_triple) values (@name,@in,@out,@user,@double,@triple)";

            SqlCommand comm = new SqlCommand(query, db.connection);
            try
            {
                comm.Parameters.AddWithValue("@name", boxName.Text);
                comm.Parameters.AddWithValue("@in", truncDateAdd(boxIn.Text));
                comm.Parameters.AddWithValue("@out", truncDateAdd(boxOut.Text));
                comm.Parameters.AddWithValue("@user", USERNAME);


                if (boxDouble.Text == "")
                {
                    comm.Parameters.AddWithValue("@double", 0);
                }
                else
                {
                    comm.Parameters.AddWithValue("@double", int.Parse(boxDouble.Text));
                }
                if (boxTriple.Text == "")
                {
                    comm.Parameters.AddWithValue("@triple", 0);
                }
                else
                {
                    comm.Parameters.AddWithValue("@triple", int.Parse(boxTriple.Text));
                }

                int result = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // enables dates to be specified by month and day only. THe added year will be the current in which the app is used.
        private string addYear(string s)
        {
            DateTime currentTime = DateTime.Now;
            int year;
            year = currentTime.Year;
            s += "/" + year.ToString();
            return s;
        }

        // removes the time from datetime;
        private string truncDateMod(string s)
        {
            string aux = null;
            for (int i = 0; i < 10; i++)
                aux += s[i];
            return aux;

        }

        // formats the data in with '-' instead of '/' to be accepted into the SQL database;
        private string truncDateAdd(string s)
        {
            string aux = null;
            for (int i = 6; i < 10; i++)
                aux += s[i];
            aux += "-" + s[3] + s[4] + "-" + s[0] + s[1];
            return aux;
        }
        public Main()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login f = new Login();
            f.ShowDialog();
            Console.WriteLine("Main Form was destroyed!");
            this.Close();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'reservationsdbDataSet.Reservations' table. You can move, or remove it, as needed.
            this.reservationsTableAdapter2.Fill(this.reservationsdbDataSet.Reservations);
            // TODO: This line of code loads data into the 'reservationDBDataSet11.Reservations' table. You can move, or remove it, as needed.
            this.reservationsTableAdapter1.Fill(this.reservationDBDataSet11.Reservations);
            lblUser.Text += USERNAME + "!";
            Database db = Database.getInstance();
            db.dt = new DataTable();
            SqlCommand cmd = new SqlCommand("select * from Reservations where Username = @user order by Date_in asc", db.connection);
            cmd.Parameters.AddWithValue("@user", USERNAME);
            try
            {
                db.dt.Load(cmd.ExecuteReader());
                gridData.DataSource = db.dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        // Button click actions;

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Database db = Database.getInstance();

            deleteData(db);
            refresh(db);
            clearBox();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Database db = Database.getInstance();

            addData(db);
            refresh(db);
            clearBox();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Database db = Database.getInstance();

            deleteData(db);
            addData(db);
            refresh(db);
            clearBox();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearBox();
        }

        private void gridData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                // we extract all rows from the DataGrid and insert them into text boxes for modification/deletion;
                DataGridViewRow row = this.gridData.Rows[e.RowIndex];
                CURR_ID = (int)row.Cells[0].Value;
                boxName.Text = row.Cells[1].Value.ToString();
                boxIn.Text = truncDateMod(row.Cells[2].Value.ToString());
                boxOut.Text = truncDateMod(row.Cells[3].Value.ToString());
                boxDouble.Text = row.Cells[5].Value.ToString();
                boxTriple.Text = row.Cells[6].Value.ToString();
            }
        }


        private void btnSearchAv_Click(object sender, EventArgs e)
        {
            int doubleR = D_ROOMS, tripleR = T_ROOMS;

            // can't add if check-out date is earlier or in the same day as the check-in one;
            if (compareDate(boxAvIn.Text, boxAvOut.Text) != -1)
            {
                MessageBox.Show("Check-out date cannot be earlier that check-in date!");
                return;
            }

            //can't add if number of rooms goes over the limit in the selected period;




            if (boxAvIn.Text.Length == 5)
            {
                boxAvIn.Text = addYear(boxAvIn.Text);
            }
            if (boxAvOut.Text.Length == 5)
            {
                boxAvOut.Text = addYear(boxAvOut.Text);
            }

            Database db = Database.getInstance();
            db.dt = new DataTable();
            string query = "select Rooms_double, Rooms_triple from Reservations where (((@in >= Date_in and @in < Date_out) or (@in < Date_in and @out > Date_out) or (@out > Date_in and @out <= Date_out)) and @user = Username)";
            SqlCommand comm = new SqlCommand(query, db.connection);
            comm.Parameters.AddWithValue("@in", truncDateAdd(boxAvIn.Text));
            comm.Parameters.AddWithValue("@out", truncDateAdd(boxAvOut.Text));
            comm.Parameters.AddWithValue("@user", USERNAME);
            Console.WriteLine(truncDateAdd(boxAvIn.Text) + ' ' + truncDateAdd(boxAvOut.Text));
            
            try
            {
                db.dt.Load(comm.ExecuteReader());
                foreach (DataRow row in db.dt.Rows)
                {
                    Console.WriteLine(row[0]) ;
                    doubleR -= int.Parse(row[0].ToString());
                    tripleR -= int.Parse(row[1].ToString());
                }
                lblAv.Text = "DOUBLE ROOMS AVAILABLE: " + doubleR.ToString() + "\n\nTRIPLE ROOMS AVAILABLE: " + tripleR.ToString();
                db.connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClearAv_Click(object sender, EventArgs e)
        {
            lblAv.Text = "";
            clearBoxAv();
        }

        
    }
}
