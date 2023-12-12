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
    public partial class Form4 : Form
    {
        string connectionString;
        private SqlConnection connection;

        public Form4(string str)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string email = textBox1.Text;
                string password = textBox2.Text;

                string query = "SELECT * FROM AdminList WHERE AdminEmail = @AdminEmail AND AdminPassword = @AdminPassword";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AdminEmail", email);
                    command.Parameters.AddWithValue("@AdminPassword", password);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string adminName = reader["AdminName"].ToString();

                        Form8 form8 = new Form8(connectionString, adminName);
                        form8.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Невірний email або пароль.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при роботі з базою даних: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) // back to previous
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }
    }
}