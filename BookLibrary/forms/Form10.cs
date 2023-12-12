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
    public partial class Form10 : Form
    {
        private string connectionString;
        private SqlConnection connection;

        private SqlDataAdapter serviceDataAdapter;
        private DataSet dataSet;
        private int currentRow;

        private string adminName;

        public Form10(string str, string adminName)
        {
            InitializeComponent();
            connectionString = str;
            connection = new SqlConnection(connectionString);
            this.adminName = adminName;
            LoadAdmins();
        }
        private void LoadAdmins()
        {
            string queryVisitors = "SELECT * FROM AdminList";

            using (connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(queryVisitors, connection))
                {
                    connection.Open();

                    serviceDataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet();
                    serviceDataAdapter.Fill(dataSet, "AdminList");

                    currentRow = 0;
                    DisplayAdminList(currentRow);
                }
            }
        }
        private void DisplayAdminList(int row)
        {
            textBox1.Text = dataSet.Tables["AdminList"].Rows[row]["AdminID"].ToString();
            textBox2.Text = dataSet.Tables["AdminList"].Rows[row]["AdminEmail"].ToString();
            textBox3.Text = dataSet.Tables["AdminList"].Rows[row]["AdminName"].ToString();
            textBox4.Text = dataSet.Tables["AdminList"].Rows[row]["AdminPosition"].ToString();        
        }
        private void button4_Click(object sender, EventArgs e) // menu button
        {
            Form8 form8 = new Form8(connectionString, adminName);
            form8.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) // visitors button
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

        private void button5_Click(object sender, EventArgs e) // previous button
        {
            if (dataSet != null && currentRow > 0)
            {
                currentRow--;
                DisplayAdminList(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the beginning of the book list.");
            }
        }

        private void button6_Click(object sender, EventArgs e) // next button
        {
            if (dataSet != null && currentRow < dataSet.Tables["AdminList"].Rows.Count - 1)
            {
                currentRow++;
                DisplayAdminList(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the end of the book list.");
            }
        }

        private void button8_Click(object sender, EventArgs e) // save button
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                    command.CommandText = @"UPDATE AdminList SET AdminName = @AdminName, AdminPosition = @AdminPosition WHERE AdminID = @AdminID";

                    command.Parameters.Add("@AdminID", SqlDbType.Int).Value = textBox1.Text;
                    command.Parameters.Add("@AdminName", SqlDbType.NVarChar).Value = textBox3.Text;
                    command.Parameters.Add("@AdminPosition", SqlDbType.NVarChar).Value = textBox4.Text;

                    command.ExecuteNonQuery();

                    MessageBox.Show("Updated");
                    LoadAdmins();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error" + ex);
                }
            }
        }

        private void button11_Click(object sender, EventArgs e) // find book button
        {
            Form11 form11 = new Form11(connectionString, adminName);
            form11.Show();
            this.Close();
        }
    }
}
