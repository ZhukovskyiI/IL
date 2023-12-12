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
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) // enter as visiter
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder["Data Source"] = "localhost";
            connectionStringBuilder["Initial Catalog"] = "BookLibrary";
            connectionStringBuilder["User ID"] = "UserTest";
            connectionStringBuilder["Password"] = "123";
            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    connection.Open();
                    Form2 form = new Form2(connectionStringBuilder.ConnectionString);
                    form.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Неправильні дані");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) // enter as admin
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder["Data Source"] = "localhost";
            connectionStringBuilder["Initial Catalog"] = "BookLibrary";
            connectionStringBuilder["User ID"] = "UserTest";
            connectionStringBuilder["Password"] = "123";
            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    connection.Open();
                    Form4 form = new Form4(connectionStringBuilder.ConnectionString);
                    form.Show();
                    this.Hide();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Неправильні дані");
                }
            }

        }
    }
}
