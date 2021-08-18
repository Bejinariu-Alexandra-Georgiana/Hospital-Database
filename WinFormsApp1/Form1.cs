using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        static string connectionString =  ConfigurationManager.ConnectionStrings["WinFormsApp1.Properties.Settings.Setting"].ConnectionString;
        DataSet ds = new DataSet();
        SqlDataAdapter adapter = new SqlDataAdapter();
        SqlCommandBuilder cmdbl;
        DataTable dt;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    MessageBox.Show(connection.State.ToString());
                    adapter.SelectCommand = new SqlCommand("SELECT * FROM Sectie", connection);
                    adapter.Fill(ds, "Sectie");
                    dataGridViewParent.DataSource = ds.Tables["Sectie"];
                    SqlDataAdapter childAdapter = new SqlDataAdapter("SELECT * FROM Doctor;", connection);
                    childAdapter.Fill(ds, "Doctor");
                    BindingSource parentBS = new BindingSource();
                    BindingSource childBS = new BindingSource();
                    parentBS.DataSource = ds.Tables["Sectie"];
                    dataGridViewParent.DataSource = parentBS;
                    DataColumn parentPK = ds.Tables["Sectie"].Columns["cod_s"];
                    DataColumn childFK = ds.Tables["Doctor"].Columns["cod_s"];
                    DataRelation relation = new DataRelation("fk_parent_child", parentPK, childFK);
                    ds.Relations.Add(relation);
                    childBS.DataSource = parentBS;
                    childBS.DataMember = "fk_parent_child";
                    dataGridViewChild.DataSource = childBS;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Updatebutton_Click(object sender, EventArgs e)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter childAdapter = new SqlDataAdapter(ConfigurationManager.AppSettings["populatingCT"], connection);
                    cmdbl = new SqlCommandBuilder(childAdapter);
                    childAdapter.Update(ds, ConfigurationManager.AppSettings["childTable"]);
                    MessageBox.Show("Information updated", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }
}
