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
    public partial class Form6 : Form
    {
        private string connectionString;
        private SqlConnection connection;

        private string VisitorMail;
        private int VisitorAccessLevel;
        private SqlDataAdapter serviceDataAdapter;
        private DataSet dataSet;
        private int currentRow;

        public Form6(string str)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);

            InitializeComponent();
            button4.Click += button4_Click;
            GetUserMail();
            label11.Text = VisitorMail;
        }
        public void SetAccessLevel(int AccessLevel)
        {
            VisitorAccessLevel = AccessLevel;
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
                        command.Parameters.AddWithValue("@VisitorAccessLevel", VisitorAccessLevel);
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
                            "WHERE Book.AccessLevel <= @VisitorAccessLevel AND Genre.GenreName LIKE @Keyword";

                case "Author":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "WHERE Book.AccessLevel <= @VisitorAccessLevel AND Author.AuthorName LIKE @Keyword";

                case "Series":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "WHERE Book.AccessLevel <= @VisitorAccessLevel AND Series.SeriesName LIKE @Keyword";

                case "Access Restrictions":
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, " +
                            "Book.AccessLevel, AccessLevelDescription.AccessName AS AccessRestrictions, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "JOIN AccessLevelDescription ON Book.AccessLevel = AccessLevelDescription.AccessLevel " +
                            "WHERE Book.AccessLevel <= @VisitorAccessLevel AND AccessLevelDescription.AccessName LIKE @Keyword";
                case "Title": 
                    return "SELECT BookID, Title, Author.AuthorName, Genre.GenreName, Series.SeriesName, AccessLevel, Annotation, Located " +
                            "FROM Book " +
                            "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                            "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                            "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                            "WHERE Book.AccessLevel <= @VisitorAccessLevel AND Book.Title LIKE @Keyword";

                default:
                    return "";
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

        private void button4_Click(object sender, EventArgs e)
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(connectionString);
            form3.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form3.Show();
        }

        private void button2_Click(object sender, EventArgs e) //previous button
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

        private void button8_Click(object sender, EventArgs e)  // main menu button
        {
            Form3 form3 = new Form3(connectionString);
            form3.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form3.Show();
        }

        private void button6_Click(object sender, EventArgs e) // all books button
        {
            Form5 form5 = new Form5(connectionString);
            form5.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form5.Show();
        }

        private void button7_Click(object sender, EventArgs e) // my favorite button
        {
            Form7 form7 = new Form7(connectionString);
            form7.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form7.Show();
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

        private void button3_Click(object sender, EventArgs e) // add favorite button
        {
            try
            {
                int bookID = Convert.ToInt32(dataSet.Tables["Books"].Rows[currentRow]["BookID"]);
                string newRating = "Not decided yet";

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string checkExistingQuery = "SELECT COUNT(*) FROM Favorite WHERE VisitorEmail = @VisitorEmail AND BookID = @BookID";
                using (SqlCommand checkCommand = new SqlCommand(checkExistingQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@VisitorEmail", VisitorMail);
                    checkCommand.Parameters.AddWithValue("@BookID", bookID);

                    int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (existingCount > 0)
                    {
                        MessageBox.Show("The book is already in your favorites list.");
                    }
                    else
                    {
                        string updateRatingQuery = "INSERT INTO Favorite (VisitorEmail, BookID, Rating) VALUES (@VisitorEmail, @BookID, @Rating)";

                        using (SqlCommand command = new SqlCommand(updateRatingQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Rating", newRating);
                            command.Parameters.AddWithValue("@BookID", bookID);
                            command.Parameters.AddWithValue("@VisitorEmail", VisitorMail);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("The book has been added to favorites.");
                            }
                            else
                            {
                                MessageBox.Show("Failed to add the book. Please try again.");
                            }
                        }
                    }
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

        private void button9_Click(object sender, EventArgs e) // log out button
        {
            Form2 form2 = new Form2(connectionString);
            this.Close();
            form2.Show();
        }
    }
}
