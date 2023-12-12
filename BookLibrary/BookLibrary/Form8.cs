using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookLibrary
{
    public partial class Form8 : Form
    {
        string connectionString;
        private SqlConnection connection;

        private string adminName;
        public Form8(string str, string adminName)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);

            InitializeComponent();
            this.adminName = adminName;
            label1.Text = "Welcome, " + adminName;

            DisplayStatistics();
        }

        private void DisplayStatistics()
        {
            try
            {
                connection.Open();

                SqlCommand visitorCommand = new SqlCommand("SELECT COUNT(*) FROM Visitor", connection);
                int totalVisitorCount = Convert.ToInt32(visitorCommand.ExecuteScalar());
                label6.Text = totalVisitorCount.ToString();

                SqlCommand bookCommand = new SqlCommand("SELECT COUNT(*) FROM Book", connection);
                int totalBookCount = Convert.ToInt32(bookCommand.ExecuteScalar());
                label7.Text = totalBookCount.ToString();

                SqlCommand adminCommand = new SqlCommand("SELECT COUNT(*) FROM AdminList", connection);
                int totalAdminCount = Convert.ToInt32(adminCommand.ExecuteScalar());
                label8.Text = totalAdminCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e) // visitor button
        {
            Form9 form9 = new Form9(connectionString, adminName);
            form9.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) // log out button
        {
            Form4 form4 = new Form4(connectionString);
            this.Close();
            form4.Show();
        }

        private void button3_Click(object sender, EventArgs e) // admins button
        {
            Form10 form10 = new Form10(connectionString, adminName);
            form10.Show();
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e) // find book button
        {
            Form11 form11 = new Form11(connectionString, adminName);
            form11.Show();
            this.Close();
        }
    }
}
