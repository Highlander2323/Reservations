using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Media;
using System.Runtime.Remoting.Contexts;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Reservations
{
    

    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }


        private void Register_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Register Form created!");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // text from boxes;
            string user = boxUsername.Text;
            string pass = boxPassword.Text;
            string repeat = boxRepeat.Text;

            

            // create a database instance;
            Database db = Database.getInstance();
 
            // message to be shown in case of invalid credentials;
            string message = null;
            string caption = "Invalid credentials";
            if(user == null || pass == null)
            {
                message = "You must enter a username and a password!";
                boxUsername.Clear();
            }
            else if (!db.checkRegister(user))
            {
                message = "Username is already in use!";
                boxUsername.Clear();
            }
            else if (pass.Length < 8 || user.Length < 8)
            {
                message = "Username and password must be at least 8 characters long!";
                boxUsername.Clear();
            }
            else if (pass != repeat)
            {
                message = "The passwords do not match! Please make sure you enter the same password twice!";
            }
            else if (user.Equals(pass))
            {
                message = "The username can't be the same as your password!";
            }

            if(message != null)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show(message, caption, MessageBoxButtons.OK);
                boxRepeat.Clear();
                boxPassword.Clear();
                return;
            }


            if(db.insertCred(user, pass)) 
            {
                caption = "Registration success!";
                message = "Succesfully registered account!";
                SystemSounds.Exclamation.Play();
                MessageBox.Show(message, caption, MessageBoxButtons.OK);
                this.Close();
            }
            else
            {
                message = "An error occured while trying to insert data into the Database!";
                SystemSounds.Exclamation.Play();
                MessageBox.Show(message, "Fatal error", MessageBoxButtons.OK);
            }
            
        }

        private void boxUsername_TextChanged(object sender, EventArgs e)
        {
            // We remove whitespaces from text inserted 
            (sender as TextBox).Text = Regex.Replace((sender as TextBox).Text, @"\s+", "");
        }
        private void boxUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            // we don't accept whitespace characters
            if (char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void boxPassword_TextChanged(object sender, EventArgs e)
        {
            // We remove whitespaces from text inserted 
            (sender as TextBox).Text = Regex.Replace((sender as TextBox).Text, @"\s+", "");
        }
        private void boxPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            // we don't accept whitespace characters
            if (char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void boxRepeat_TextChanged(object sender, EventArgs e)
        {
            // We remove whitespaces from text inserted 
            (sender as TextBox).Text = Regex.Replace((sender as TextBox).Text, @"\s+", "");
        }
        private void boxRepeat_KeyPress(object sender, KeyPressEventArgs e)
        {
            // we don't accept whitespace characters
            if (char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
