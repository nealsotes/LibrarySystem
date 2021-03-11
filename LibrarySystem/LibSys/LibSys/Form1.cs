using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Diagnostics;

namespace LibSys
{
    public partial class form1 : Form
    {
        private OleDbConnection con;
        DateTime aDate = DateTime.Now;
        
        

        public form1()
        {
            InitializeComponent();
            con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Users\\nealf\\Desktop\\LibrarySystem\\LibSys.mdb");
            lblDate.Text = aDate.ToString("MM/dd/yyyy");
            lblTime.Text = aDate.ToString("H:mm");
        }

        private void form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'libSysDataSet.book' table. You can move, or remove it, as needed.
            this.bookTableAdapter.Fill(this.libSysDataSet.book);
            LoadDataGrid();
            retrieveId();
            
        }

        private void LoadDataGrid()
        {
            con.Open();
            OleDbCommand com = new OleDbCommand("Select * from book order by accession_number asc", con);
            com.ExecuteNonQuery();
            OleDbDataAdapter adap = new OleDbDataAdapter(com);
            DataTable tab = new DataTable();
            
            adap.Fill(tab);
            grid1.DataSource = tab;
            con.Close();
        }

        private void retrieveId() 
        {
            try
            {
                string query = "Select accession_number from book order by accession_number asc ";
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                OleDbCommand cmd = new OleDbCommand(query, con);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {  
                    int value = int.Parse(reader[0].ToString()) + 1;
                    txtno.Text = value.ToString("0000");
                    
                }
                Convert.IsDBNull(reader);
                

            }
            catch (Exception)
            {
                throw;
            }

            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

        
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand com = new OleDbCommand("Select * from book where title like '%" + txtSearch.Text +  "%' OR author like '%" + txtSearch.Text + "%' OR accession_number like '%" + txtSearch.Text + "%'  ", con);
            com.ExecuteNonQuery();
            OleDbDataAdapter adap = new OleDbDataAdapter(com);
            DataTable tab = new DataTable();

            adap.Fill(tab);
            grid1.DataSource = tab;
            con.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            con.Open();
           

            try
            {
                OleDbCommand com = new OleDbCommand("Insert into book values ('" + txtno.Text + "', '" +
                    txttitle.Text + "', '" + txtauthor.Text + "')", con);
                com.ExecuteNonQuery();

                if (ValidateChildren(ValidationConstraints.Enabled))
                {
                    MessageBox.Show("Successfully SAVED!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
               



            }
            catch (Exception ex) 
            {

                Debug.WriteLine(ex.Message);
            

            }
            clear_screen();


            con.Close();
            LoadDataGrid();
            retrieveId();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            con.Open();
            try
            {
                string num = txtno.Text;
                DialogResult dr = MessageBox.Show("Are you sure you want to delete this?", "Confirm Delation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    OleDbCommand com = new OleDbCommand("Delete from book where accession_number= " + num + "", con);
                    com.ExecuteNonQuery();
                    MessageBox.Show("Successfully DELETED!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clear_screen();
                }
                else
                {
                    MessageBox.Show("CANCELLED", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
           
            con.Close();
            LoadDataGrid();
        }

        private void grid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtno.Text = grid1.Rows[e.RowIndex].Cells["accession_number"].Value.ToString();
            txttitle.Text = grid1.Rows[e.RowIndex].Cells["title"].Value.ToString();
            txtauthor.Text = grid1.Rows[e.RowIndex].Cells["author"].Value.ToString();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            con.Open();

            try
            {
                string no;
                no = txtno.Text;
                OleDbCommand com = new OleDbCommand("Update book SET title= '" + txttitle.Text + "', author='" + txtauthor.Text + "' where accession_number= " + no + "", con);
                com.ExecuteNonQuery();

                MessageBox.Show("Successfully UPDATED!", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.Message);
            }




            con.Close();
            LoadDataGrid();
            clear_screen();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            clear_screen();
        }
        private string clear_screen() 
        {
            var clear_sc = Tuple.Create(txtno.Text = "", txttitle.Text = "", txtauthor.Text="");
            return clear_sc.Item1;
           
        }

       
    }
}
