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
    public partial class Form5 : Form
    {
        private string connectionString;
        private SqlConnection connection;

        private int VisitorAccessLevel;
        private string VisitorMail;
        private SqlDataReader reader;

        private SqlDataAdapter serviceDataAdapter;
        private DataSet dataSet;
        private int currentRow;

        public Form5(string str)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);


            InitializeComponent();

            GetUserMail();
            label11.Text = VisitorMail;
        }
        public void SetAccessLevel(int AccessLevel)
        {
            VisitorAccessLevel = AccessLevel;
            LoadBooks();
        }

        private void LoadBooks()
        {
            string queryBooks = "SELECT Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                                "FROM Book " +
                                "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                                "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                                "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                                "WHERE Book.AccessLevel <= @VisitorAccessLevel";

            using (connection)
            {
                using (SqlCommand command = new SqlCommand(queryBooks, connection))
                {
                    command.Parameters.AddWithValue("@VisitorAccessLevel", VisitorAccessLevel);
                    connection.Open();

                    serviceDataAdapter = new SqlDataAdapter(command);

                    dataSet = new DataSet();
                    serviceDataAdapter.Fill(dataSet, "Books");

                    currentRow = 0;
                    DisplayBookInfo(currentRow);
                }
            }
        }

        private void DisplayBookInfo(int row)
        {
            textBox1.Text = dataSet.Tables["Books"].Rows[row]["Title"].ToString();
            textBox2.Text = dataSet.Tables["Books"].Rows[row]["AuthorName"].ToString();
            textBox3.Text = dataSet.Tables["Books"].Rows[row]["GenreName"].ToString();
            textBox4.Text = dataSet.Tables["Books"].Rows[row]["SeriesName"].ToString();
            textBox6.Text = dataSet.Tables["Books"].Rows[row]["Annotation"].ToString();
            textBox7.Text = dataSet.Tables["Books"].Rows[row]["Located"].ToString();

            int accessLevel = Convert.ToInt32(dataSet.Tables["Books"].Rows[row]["AccessLevel"]);
            textBox5.Text = GetAccessLevelName(accessLevel);

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

        private string GetAccessLevelName(int accessLevel)
        {
            string accessName = "";
            string queryAccessLevel = "SELECT AccessName FROM AccessLevelDescription WHERE AccessLevel = @AccessLevel";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(queryAccessLevel, connection))
                {
                    command.Parameters.AddWithValue("@AccessLevel", accessLevel);
                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        accessName = result.ToString();
                    }
                }
            }

            return accessName;
        }

        private void button1_Click(object sender, EventArgs e)   //next button
        {
            if (dataSet != null && currentRow < dataSet.Tables["Books"].Rows.Count - 1)
            {
                currentRow++;
                DisplayBookInfo(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the end of the book list.");
            }

        }


        private void button2_Click_1(object sender, EventArgs e) //previous button
        {
            if (dataSet != null && currentRow > 0)
            {
                currentRow--;
                DisplayBookInfo(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the beginning of the book list.");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(connectionString);
            form3.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form3.Show();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e) // main menu button
        {
            Form3 form3 = new Form3(connectionString);
            form3.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form3.Show();
        }

        private void button6_Click(object sender, EventArgs e) // find book button
        {
            Form6 form6 = new Form6(connectionString);
            form6.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form6.Show();
        }

        private void button7_Click(object sender, EventArgs e) // my favorites button
        {
            Form7 form7 = new Form7(connectionString);
            form7.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form7.Show();
        }

        private void button3_Click_1(object sender, EventArgs e) // log out button
        {
            Form2 form2 = new Form2(connectionString);
            this.Close();
            form2.Show();
        }
    }
}
