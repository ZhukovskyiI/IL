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
    public partial class Form11 : Form
    {
        string connectionString;
        private SqlConnection connection;

        private SqlDataAdapter serviceDataAdapter;
        private DataSet dataSet;
        private int currentRow;

        private string adminName;

        public Form11(string str, string adminName)
        {
            InitializeComponent();
            connectionString = str;
            connection = new SqlConnection(connectionString);
            this.adminName = adminName;
            button4.Click += button4_Click;

        }

        private void LoadBooks()
        {
            string filter = comboBox1.SelectedItem.ToString();
            string keyword = textBox7.Text;

            string queryBooks = GetQueryByFilter(filter);

            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                using (SqlCommand command = new SqlCommand(queryBooks, connection))
                {
                    command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    serviceDataAdapter = new SqlDataAdapter(command);

                    dataSet = new DataSet();
                    serviceDataAdapter.Fill(dataSet, "Books");

                    currentRow = 0;
                    DisplayBookInfo(currentRow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when working with the database: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void DisplayBookInfo(int row)
        {
            if (dataSet != null && dataSet.Tables["Books"].Rows.Count > 0)
            {
                textBox1.Text = dataSet.Tables["Books"].Rows[row]["Title"].ToString();
                textBox2.Text = dataSet.Tables["Books"].Rows[row]["AuthorName"].ToString();
                textBox3.Text = dataSet.Tables["Books"].Rows[row]["GenreName"].ToString();
                textBox4.Text = dataSet.Tables["Books"].Rows[row]["SeriesName"].ToString();
                textBox6.Text = dataSet.Tables["Books"].Rows[row]["Annotation"].ToString();
                textBox8.Text = dataSet.Tables["Books"].Rows[row]["Located"].ToString();

                int accessLevel = Convert.ToInt32(dataSet.Tables["Books"].Rows[row]["AccessLevel"]);
                textBox5.Text = GetAccessLevelName(accessLevel);
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
            }
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

        private string GetQueryByFilter(string filter)
        {
            switch (filter)
            {
                case "Genre":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "WHERE Genre.GenreName LIKE @Keyword";

                case "Author":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "WHERE Author.AuthorName LIKE @Keyword";

                case "Series":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "WHERE Series.SeriesName LIKE @Keyword";

                case "Access Restrictions":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, " +
                            "Book.AccessLevel, AccessLevelDescription.AccessName AS AccessRestrictions, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "JOIN AccessLevelDescription ON Book.AccessLevel = AccessLevelDescription.AccessLevel " +
                            "WHERE AccessLevelDescription.AccessName LIKE @Keyword";
                case "Title":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "WHERE Book.Title LIKE @Keyword";

                default:
                    return "";
            }
        }
        private void button4_Click(object sender, EventArgs e) // find by criteria button
        {
            if (comboBox1.SelectedItem != null)
            {

                LoadBooks();
            }
            else
            {
                MessageBox.Show("Please select a criteria before looking for books.");
            }
        }

        private void button2_Click(object sender, EventArgs e) // previous button
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

        private void button1_Click(object sender, EventArgs e) // next button
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

        private void button5_Click(object sender, EventArgs e) // menu button
        {
            Form8 form8 = new Form8(connectionString, adminName);
            form8.Show();
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e) // visitors button
        {
            Form9 form9 = new Form9(connectionString, adminName);
            form9.Show();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e) // admins button
        {
            Form10 form10 = new Form10(connectionString, adminName);
            form10.Show();
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e) // log out button
        {
            Form4 form4 = new Form4(connectionString);
            this.Close();
            form4.Show();
        }
    }
}
