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

    public partial class Form3 : Form
    {
        private string connectionString;
        private SqlConnection connection;

        private int VisitorAccessLevel;
        private string VisitorMail;

        public Form3(string str)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);

            InitializeComponent();
            GetUserMail();
            label11.Text = VisitorMail;
        }
        static string ReadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);

                    return lines.Length > 0 ? lines[0] : string.Empty;
                }
                else
                {
                    Console.WriteLine("The file does not exist. The read value is not available.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from file: {ex.Message}");
                return string.Empty;
            }
        }
        private void GetUserMail()
        {
            string projectFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "userMail.txt";
            string filePath = Path.Combine(projectFolderPath, fileName);
            VisitorMail = ReadFromFile(filePath);
        }

        public void SetAccessLevel(int AccessLevel)
        {
            VisitorAccessLevel = AccessLevel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e) // all books button
        {
            Form5 form5 = new Form5(connectionString);
            form5.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form5.Show();
        }

        private void button6_Click(object sender, EventArgs e) // find book button
        {
            Form6 form6 = new Form6(connectionString);
            form6.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form6.Show();
        }

        private void button7_Click(object sender, EventArgs e) // my favorite button
        {
            Form7 form7 = new Form7(connectionString);
            form7.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form7.Show();
        }

        private void button1_Click_1(object sender, EventArgs e) // log out button
        {
            Form2 form2 = new Form2(connectionString);
            this.Close();
            form2.Show();

        }
    }
}
