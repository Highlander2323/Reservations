using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reservations
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            if (Reservations.Properties.Settings.Default.Username != String.Empty && Reservations.Properties.Settings.Default.Password != String.Empty)
            {
                checkRemember.CheckState = CheckState.Checked;
                boxUsername.Text = Reservations.Properties.Settings.Default.Username;
                boxPassword.Text = Reservations.Properties.Settings.Default.Password;
            }
        }

        private void lblRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Register f = new Register();
            f.ShowDialog();
            Console.WriteLine("Register Form was destroyed!");
            this.Show();
            
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (checkRemember.Checked)
            {
                Reservations.Properties.Settings.Default.Username = boxUsername.Text;
                Reservations.Properties.Settings.Default.Password = boxPassword.Text;
                Reservations.Properties.Settings.Default.Save();
            }
            else
            {
                Reservations.Properties.Settings.Default.Username = String.Empty;
                Reservations.Properties.Settings.Default.Password = String.Empty;
                Reservations.Properties.Settings.Default.Save();
            }
            Database db = Database.getInstance();
            if(db.checkLogin(boxUsername.Text, boxPassword.Text))
            {
                this.Hide();
                Main f = new Main();
                Console.WriteLine("Main Form was created!");
                Main.USERNAME = boxUsername.Text;
                f.ShowDialog();
                Console.WriteLine("Main Form was destroyed!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials or account does not exist!", "No account found", MessageBoxButtons.OK);
            }
            
            
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
    }
}
