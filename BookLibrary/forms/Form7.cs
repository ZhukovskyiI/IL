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
    public partial class Form7 : Form
    {
        private string connectionString;
        private SqlConnection connection;

        private int VisitorAccessLevel;
        private string VisitorMail;
        private SqlDataReader reader;

        private SqlDataAdapter serviceDataAdapter;
        private DataSet dataSet;
        private int currentRow;
        public Form7(string str)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);


            InitializeComponent();
            LoadFavBooks();
            GetUserMail();
            label11.Text = VisitorMail;
        }

        public void SetAccessLevel(int AccessLevel)
        {
            VisitorAccessLevel = AccessLevel;
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
        private void LoadFavBooks()
        {
            try
            {
                if(connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }


                //string projectFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                //string fileName = "userMail.txt";
                //string filePath = Path.Combine(projectFolderPath, fileName);
                //VisitorMail = ReadFromFile(filePath);
                GetUserMail();

                string queryFavBooks = "SELECT Favorite.ID, Book.Title, Favorite.Rating, Author.AuthorName, Genre.GenreName, Series.SeriesName, Book.Annotation, Book.Located " +
                                    "FROM Favorite " +
                                    "JOIN Book ON Favorite.BookID = Book.BookID " +
                                    "JOIN Author ON Book.AuthorID = Author.AuthorID " +
                                    "JOIN Genre ON Book.GenreID = Genre.GenreID " +
                                    "JOIN Series ON Book.SeriesID = Series.SeriesID " +
                                    "WHERE Favorite.VisitorEmail = @VisitorEmail";


                using(SqlCommand command = new SqlCommand(queryFavBooks, connection))
                {              
                    command.Parameters.AddWithValue("@VisitorEmail", VisitorMail);
                    serviceDataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet();
                    serviceDataAdapter.Fill(dataSet, "FavoriteBooks");

                    DisplayFavBookInfo(0);
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

        private void DisplayFavBookInfo(int row)
        {
            if (dataSet != null && dataSet.Tables["FavoriteBooks"].Rows.Count > 0)
            {
                textBox1.Text = dataSet.Tables["FavoriteBooks"].Rows[row]["Title"].ToString();
                textBox2.Text = dataSet.Tables["FavoriteBooks"].Rows[row]["AuthorName"].ToString();
                textBox3.Text = dataSet.Tables["FavoriteBooks"].Rows[row]["GenreName"].ToString();
                textBox4.Text = dataSet.Tables["FavoriteBooks"].Rows[row]["SeriesName"].ToString();
                textBox5.Text = dataSet.Tables["FavoriteBooks"].Rows[row]["Annotation"].ToString();
                textBox6.Text = dataSet.Tables["FavoriteBooks"].Rows[row]["Rating"].ToString();
                textBox8.Text = dataSet.Tables["FavoriteBooks"].Rows[row]["Located"].ToString();
            }
            else
            {
                ClearTextBoxes();
            }
        }

        private void button4_Click(object sender, EventArgs e) // main menu button
        {
            Form3 form3 = new Form3(connectionString);
            form3.SetAccessLevel(VisitorAccessLevel);
            this.Close();
            form3.Show();
        }

        private void button1_Click(object sender, EventArgs e) // all books button
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

        private void button3_Click(object sender, EventArgs e) // next button
        {
            if (dataSet != null && currentRow < dataSet.Tables["FavoriteBooks"].Rows.Count - 1)
            {
                currentRow++;
                DisplayFavBookInfo(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the end of the book list.");
            }
        }

        private void button2_Click(object sender, EventArgs e) // previous button
        {
            if (dataSet != null && currentRow > 0)
            {
                currentRow--;
                DisplayFavBookInfo(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the begginning of the book list.");
            }
        }

        private void button7_Click(object sender, EventArgs e) // update mark button
        {
            try
            {
                int ID = Convert.ToInt32(dataSet.Tables["FavoriteBooks"].Rows[currentRow]["ID"]);
                string newRating = textBox6.Text;

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string updateRatingQuery = "UPDATE Favorite SET Rating = @NewRating WHERE ID = @ID AND VisitorEmail = @VisitorEmail";

                using (SqlCommand command = new SqlCommand(updateRatingQuery, connection))
                {
                    command.Parameters.AddWithValue("@NewRating", newRating);
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@VisitorEmail", VisitorMail);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("The rating has been updated successfully.");
                        LoadFavBooks();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update rating.");
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
        private void ClearTextBoxes()
        {
            // Очистка вмісту текстових полів
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void button8_Click(object sender, EventArgs e) // delete from favorites button
        {
            try
            {
                int ID = Convert.ToInt32(dataSet.Tables["FavoriteBooks"].Rows[currentRow]["ID"]);

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string deleteFavoriteQuery = "DELETE FROM Favorite WHERE ID = @ID AND VisitorEmail = @VisitorEmail";

                using (SqlCommand command = new SqlCommand(deleteFavoriteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", ID);
                    command.Parameters.AddWithValue("@VisitorEmail", VisitorMail);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("The record has been successfully removed from favorites.");
                        ClearTextBoxes();
                        LoadFavBooks();
                        DisplayFavBookInfo(0);
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete record.");
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

        private void button9_Click(object sender, EventArgs e) 
        {

        }

        private void button9_Click_1(object sender, EventArgs e) // log out button
        {
            Form2 form2 = new Form2(connectionString);
            this.Close();
            form2.Show();
        }
    }
}
