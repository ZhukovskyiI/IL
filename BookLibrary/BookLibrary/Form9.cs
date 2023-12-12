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
    public partial class Form9 : Form
    {
        private string connectionString;
        private SqlConnection connection;


        private SqlDataAdapter serviceDataAdapter;
        private DataSet dataSet;
        private int currentRow;


        private string adminName;
        public Form9(string str, string adminName)
        {
            connectionString = str;
            connection = new SqlConnection(connectionString);

            InitializeComponent();
            this.adminName = adminName;
            InitializeComboBox();
            LoadVisitors();       
        }
        private void LoadVisitors()
        {
            string queryVisitors = "SELECT * FROM Visitor";

            using (connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(queryVisitors, connection))
                {
                    connection.Open();

                    serviceDataAdapter = new SqlDataAdapter(command);
                    dataSet = new DataSet();
                    serviceDataAdapter.Fill(dataSet, "Visitors");

                    currentRow = 0;
                    DisplayVisitorsInfo(currentRow);
                }
            }
        }

        private void DisplayVisitorsInfo(int row)
        {
            textBox1.Text = dataSet.Tables["Visitors"].Rows[row]["VisitorID"].ToString();
            textBox2.Text = dataSet.Tables["Visitors"].Rows[row]["VisitorEmail"].ToString();
            textBox3.Text = dataSet.Tables["Visitors"].Rows[row]["VisitorPassword"].ToString();
            textBox4.Text = dataSet.Tables["Visitors"].Rows[row]["VisitorName"].ToString();
            textBox5.Text = dataSet.Tables["Visitors"].Rows[row]["VisitorSurname"].ToString();
            textBox6.Text = dataSet.Tables["Visitors"].Rows[row]["VisitorBirhday"].ToString();
            textBox7.Text = dataSet.Tables["Visitors"].Rows[row]["VisitorWork"].ToString();
            comboBox1.SelectedValue = dataSet.Tables["Visitors"].Rows[row]["VisitorAccessLevel"];
        }

        private void InitializeComboBox()
        {
            var accessAdapted = new SqlDataAdapter("SELECT AccessLevel, AccessName FROM AccessLevelDescription", connectionString);
            var accessTable = new DataTable();
            accessAdapted.Fill(accessTable);
            comboBox1.DataSource = accessTable;
            comboBox1.DisplayMember = "AccessName";
            comboBox1.ValueMember = "AccessLevel";
        }

        private void button4_Click(object sender, EventArgs e) // menu button
        {
            Form8 form8 = new Form8(connectionString, adminName);
            form8.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e) // previous button
        {
            if (dataSet != null && currentRow > 0)
            {
                currentRow--;
                DisplayVisitorsInfo(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the beginning of the book list.");
            }
        }

        private void button6_Click(object sender, EventArgs e) // next button
        {
            if (dataSet != null && currentRow < dataSet.Tables["Visitors"].Rows.Count - 1)
            {
                currentRow++;
                DisplayVisitorsInfo(currentRow);
            }
            else
            {
                MessageBox.Show("You have reached the end of the book list.");
            }
        }

        private void button7_Click(object sender, EventArgs e) // add button
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Перевірка наявності користувача з вказаним email в базі даних
                    string checkUserQuery = "SELECT COUNT(*) FROM Visitor WHERE VisitorEmail = @VisitorEmail";
                    using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection))
                    {
                        checkUserCommand.Parameters.AddWithValue("@VisitorEmail", textBox2.Text);
                        int existingUserCount = Convert.ToInt32(checkUserCommand.ExecuteScalar());

                        if (existingUserCount > 0)
                        {
                            MessageBox.Show("User with the provided email already exists.");
                            return;
                        }
                    }

                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO Visitor VALUES (@VisitorID, @VisitorPassword, @VisitorName, @VisitorSurname, @VisitorEmail, @VisitorBirhday, @VisitorWork, @VisitorAccessLevel)";
                    command.Parameters.Add("@VisitorID", SqlDbType.Int);
                    command.Parameters.Add("@VisitorPassword", SqlDbType.NVarChar);
                    command.Parameters.Add("@VisitorName", SqlDbType.NVarChar);
                    command.Parameters.Add("@VisitorSurname", SqlDbType.NVarChar);
                    command.Parameters.Add("@VisitorEmail", SqlDbType.NVarChar);
                    command.Parameters.Add("@VisitorBirhday", SqlDbType.Date);
                    command.Parameters.Add("@VisitorWork", SqlDbType.NVarChar);
                    command.Parameters.Add("@VisitorAccessLevel", SqlDbType.Int);

                    command.Parameters["@VisitorID"].Value = textBox1.Text;
                    command.Parameters["@VisitorAccessLevel"].Value = comboBox1.SelectedIndex + 1;

                    command.Parameters["@VisitorPassword"].Value = textBox3.Text;
                    command.Parameters["@VisitorName"].Value = textBox4.Text;
                    command.Parameters["@VisitorSurname"].Value = textBox5.Text;
                    command.Parameters["@VisitorEmail"].Value = textBox2.Text;
                    command.Parameters["@VisitorBirhday"].Value = textBox6.Text;
                    command.Parameters["@VisitorWork"].Value = textBox7.Text;
                    command.ExecuteNonQuery();

                    MessageBox.Show("Added");
                    LoadVisitors();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error" + ex);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand();

                try
                {
                    command.Transaction = transaction;
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM Favorite WHERE VisitorEmail = @VisitorEmail; " +
                     "DELETE FROM Visitor WHERE VisitorID = @VisitorID";
                    command.Parameters.Add("@VisitorEmail", SqlDbType.NVarChar);
                    command.Parameters.Add("@VisitorID", SqlDbType.Int);


                    command.Parameters["@VisitorID"].Value = textBox1.Text;
                    command.Parameters["@VisitorEmail"].Value = textBox2.Text;
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    MessageBox.Show("Deleted");
                    LoadVisitors();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error"+ex);
                    transaction.Rollback();
                }
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

                    if (!string.IsNullOrEmpty(textBox9.Text) && !string.IsNullOrEmpty(textBox8.Text))
                    {
                        // Запит на оновлення Visitor та Favorite
                        command.CommandText = @"UPDATE Favorite SET VisitorEmail = @VisitorNewEmail WHERE VisitorEmail = @VisitorEmail;
                                        UPDATE Visitor SET VisitorID = @VisitorNewID, VisitorPassword = @VisitorPassword, VisitorName = @VisitorName, VisitorSurname = @VisitorSurname, VisitorEmail = @VisitorNewEmail, VisitorBirhday = @VisitorBirthday, VisitorWork = @VisitorWork, VisitorAccessLevel = @VisitorAccessLevel WHERE VisitorID = @VisitorID";
                    }
                    else if (!string.IsNullOrEmpty(textBox9.Text) && string.IsNullOrEmpty(textBox8.Text))
                    {
                        // Запит на оновлення Visitor
                        command.CommandText = @"UPDATE Visitor SET VisitorID = @VisitorNewID, VisitorPassword = @VisitorPassword, VisitorName = @VisitorName, VisitorSurname = @VisitorSurname, VisitorEmail = @VisitorEmail, VisitorBirhday = @VisitorBirthday, VisitorWork = @VisitorWork, VisitorAccessLevel = @VisitorAccessLevel WHERE VisitorID = @VisitorID";
                    }
                    else if (string.IsNullOrEmpty(textBox9.Text) && !string.IsNullOrEmpty(textBox8.Text))
                    {
                        // Запит на оновлення Favorite
                        command.CommandText = @"UPDATE Favorite SET VisitorEmail = @VisitorNewEmail WHERE VisitorEmail = @VisitorEmail;
                                        UPDATE Visitor SET VisitorID = @VisitorID, VisitorPassword = @VisitorPassword, VisitorName = @VisitorName, VisitorSurname = @VisitorSurname, VisitorEmail = @VisitorNewEmail, VisitorBirhday = @VisitorBirthday, VisitorWork = @VisitorWork, VisitorAccessLevel = @VisitorAccessLevel WHERE VisitorID = @VisitorID";
                    }
                    else
                    {
                        // Запит на оновлення Visitor
                        command.CommandText = @"UPDATE Visitor SET VisitorID = @VisitorID, VisitorPassword = @VisitorPassword, VisitorName = @VisitorName, VisitorSurname = @VisitorSurname, VisitorEmail = @VisitorEmail, VisitorBirhday = @VisitorBirthday, VisitorWork = @VisitorWork, VisitorAccessLevel = @VisitorAccessLevel WHERE VisitorID = @VisitorID";
                    }

                    command.Parameters.Add("@VisitorID", SqlDbType.Int).Value = textBox1.Text;
                    command.Parameters.Add("@VisitorPassword", SqlDbType.NVarChar).Value = textBox3.Text;
                    command.Parameters.Add("@VisitorName", SqlDbType.NVarChar).Value = textBox4.Text;
                    command.Parameters.Add("@VisitorSurname", SqlDbType.NVarChar).Value = textBox5.Text;
                    command.Parameters.Add("@VisitorEmail", SqlDbType.NVarChar).Value = textBox2.Text;
                    command.Parameters.Add("@VisitorBirthday", SqlDbType.Date).Value = textBox6.Text;
                    command.Parameters.Add("@VisitorWork", SqlDbType.NVarChar).Value = textBox7.Text;
                    command.Parameters.Add("@VisitorAccessLevel", SqlDbType.Int).Value = comboBox1.SelectedIndex + 1;
                    command.Parameters.Add("@VisitorNewEmail", SqlDbType.NVarChar).Value = textBox8.Text;
                    command.Parameters.Add("@VisitorNewID", SqlDbType.Int).Value = (!string.IsNullOrEmpty(textBox9.Text)) ? (object)textBox9.Text : DBNull.Value;

                    command.ExecuteNonQuery();

                    MessageBox.Show("Updated");
                    LoadVisitors();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error" + ex);
                }
            }
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
            this.Close();
            form10.Show();
        }

        private void button10_Click(object sender, EventArgs e) // clear button
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            comboBox1.SelectedIndex = -1;
        }

        private void button11_Click(object sender, EventArgs e) // find book button
        {
            Form11 form11 = new Form11(connectionString, adminName);
            form11.Show();
            this.Close();
        }
    }
}
