using System;
using System.IO;
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
    public partial class Form2 : Form
    {
        string connectionString;
        private SqlConnection connection;

        public Form2(string str)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);

            InitializeComponent();
        }

        static void SaveToFile(string filePath, string value)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, string.Empty);
                }
                else
                {
                    File.Create(filePath).Close();
                }

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(value);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when saving to file: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string VisitorEmail = textBox1.Text;
            string VisitorPassword = textBox2.Text;

            if (!DoesEmailExist(VisitorEmail))
            {
                MessageBox.Show("There is no user with this email address.");
                return;
            }

            if (!IsPasswordCorrect(VisitorEmail, VisitorPassword))
            {
                MessageBox.Show("Incorrect password.");
                return;
            }

            string query = "SELECT VisitorAccessLevel FROM Visitor WHERE VisitorEmail = @VisitorEmail AND VisitorPassword = @VisitorPassword";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VisitorEmail", VisitorEmail);
                    command.Parameters.AddWithValue("@VisitorPassword", VisitorPassword);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        int VisitorAccessLevel = Convert.ToInt32(result);
                        Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
                        string projectFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                        string fileName = "userMail.txt";
                        string filePath = Path.Combine(projectFolderPath, fileName);
                        SaveToFile(filePath, VisitorEmail);

                        OpenForm3(VisitorAccessLevel);
                    }
                }
            }
        }
        private bool DoesEmailExist(string email)
        {
            string query = "SELECT COUNT(*) FROM Visitor WHERE VisitorEmail = @VisitorEmail";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VisitorEmail", email);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private bool IsPasswordCorrect(string email, string password)
        {
            string query = "SELECT COUNT(*) FROM Visitor WHERE VisitorEmail = @VisitorEmail AND VisitorPassword = @VisitorPassword";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VisitorEmail", email);
                    command.Parameters.AddWithValue("@VisitorPassword", password);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }
        private void OpenForm3(int accessLevel)
        {
            this.Close();

            Form3 form3 = new Form3(connectionString);
            form3.SetAccessLevel(accessLevel);
            form3.ShowDialog();
        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }
    }
}
