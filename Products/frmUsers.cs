using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace Products
{
    public partial class frmUsers : frmInheritance
    {

        string strUserName;
        string strPassword;
        string strFirstName;
        string strLastName;
        
        bool boolUserExists = false;
        int intUserID = 0;

        string strAccessConnectionString = "Driver={Microsoft Access Driver (*.mdb)}; Dbq=Products.mdb; Uid=Admin; Pwd=;";


        public frmUsers()
        {
            InitializeComponent();
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            controlsLoad();
            loadUsers();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            frmMain frmMain = new frmMain();
            frmMain.Show();
            this.Hide();
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            if (btnCreateUser.Text == "Save")
            {
                if (txtUserName.Text == "")
                {
                    MessageBox.Show("User Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtPassword.Text == "")
                {
                    MessageBox.Show("Password field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtFirstName.Text == "")
                {
                    MessageBox.Show("First Name Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtLastName.Text == "")
                {
                    MessageBox.Show("Last Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    checkIfUserExists();
                    if (boolUserExists == false)
                    {
                        createUser();
                        controlsLoad();
                        clearTextBoxes();
                        loadUsers();
                    }
                    else if (boolUserExists == true)
                    {
                        MessageBox.Show("User Already Exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            else if (btnCreateUser.Text == "Create")
            {
                controlsCreate();
            }

        }

        private void btnEditUser_Click(object sender, EventArgs e)
        {
            controlsEdit();
            editUser();
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            updateUser();
            controlsLoad();
            clearTextBoxes();
            loadUsers();
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            deleteUser();
            controlsLoad();
            clearTextBoxes();
            loadUsers();
        }

        private void controlsLoad()
        {
            txtPassword.Enabled = false;
            txtLastName.Enabled = false;
            txtFirstName.Enabled = false;
            txtUserName.Enabled = false;

            cboUsers.Enabled = true;

            btnCreateUser.Enabled = true;
            btnDeleteUser.Enabled = false;
            btnEditUser.Enabled = true;
            btnUpdateUser.Enabled = false;
            btnReturn.Enabled = true;

            btnCreateUser.Text = "Create";
        }


        private void controlsCreate()
        {
            txtPassword.Enabled = true;
            txtUserName.Enabled = true;
            txtFirstName.Enabled = true;
            txtLastName.Enabled = true;

            cboUsers.Enabled = false;

            btnCreateUser.Enabled = true;
            btnDeleteUser.Enabled = false;
            btnEditUser.Enabled = false;
            btnUpdateUser.Enabled = false;
            btnReturn.Enabled = false;

            btnCreateUser.Text = "Save";
        }

        private void controlsEdit()
        {
            txtPassword.Enabled = true;
            txtUserName.Enabled = true;
            txtFirstName.Enabled = true;
            txtLastName.Enabled = true;

            cboUsers.Enabled = false;

            btnCreateUser.Enabled = false;
            btnEditUser.Enabled = false;
            btnDeleteUser.Enabled = true;
            btnUpdateUser.Enabled = true;
            btnReturn.Enabled = false;

        }

        private void clearTextBoxes()
        {
            txtPassword.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtUserName.Text = "";
        }

        private void loadUsers()
        {
            cboUsers.DataSource =null;
            cboUsers.Items.Clear();

            OdbcConnection ObdcConnection = new OdbcConnection();
            ObdcConnection.ConnectionString = strAccessConnectionString;

            string query = "select username from Users";

            OdbcCommand cmd = new OdbcCommand (query, ObdcConnection);

            ObdcConnection.Open();

            OdbcDataReader dr = cmd.ExecuteReader();
            AutoCompleteStringCollection UserCollection = new AutoCompleteStringCollection();

            while (dr.Read())
            {
                UserCollection.Add(dr.GetString(0));
            }

            ObdcConnection.Close();

            cboUsers.DataSource = UserCollection;

        }

        private void checkIfUserExists()
        {
            string query= "select * from Users where UserName='"+ txtUserName + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();

            cmd = new OdbcCommand (query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                boolUserExists = true;
            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }

        private void createUser()
        {
            string query = "select * from users where ID=0";

            OdbcConnection OdbcConnetion = new OdbcConnection();
            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnetion);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr;

            OdbcConnetion.ConnectionString = strAccessConnectionString;

            da.Fill(ds,"Users");
            dt = ds.Tables["Users"];

            try
            {
                dr=dt.NewRow();
                dr["UserName"] = txtUserName.Text;
                dr["Password"] = txtPassword.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"]= txtLastName.Text;

                dt.Rows.Add(dr);
                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);
                da.Update(ds, "Users");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                OdbcConnetion.Close();
                OdbcConnetion.Dispose();
            }
        }

        private void editUser()
        {
            string query = "select * from Users where username='" + cboUsers.Text + "'";

            OdbcConnection ObdcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            ObdcConnection.ConnectionString = strAccessConnectionString;

            ObdcConnection.Open();

            cmd = new OdbcCommand(query, ObdcConnection);

            dr = cmd.ExecuteReader();

            if(dr.Read())
            {
                intUserID = dr.GetInt32(0);
                txtUserName.Text = dr.GetString(1);
                txtPassword.Text = dr.GetString(2);
                txtFirstName.Text = dr.GetString(3);
                txtLastName.Text = dr.GetString(4);

            }

            dr.Close();
            ObdcConnection.Close();
            dr.Dispose();
            ObdcConnection.Dispose();
        }

        private void updateUser()
        {
            string query = "select * from users where ID=" + intUserID;

            OdbcConnection ObdcConnection = new OdbcConnection();

            ObdcConnection.ConnectionString = strAccessConnectionString;

            OdbcDataAdapter da = new OdbcDataAdapter(query, ObdcConnection);
            DataSet ds = new DataSet("Users");

            da.FillSchema(ds, SchemaType.Source, "Users");
            da.Fill(ds, "Users");
            DataTable dt;

            dt = ds.Tables["Users"];
            DataRow dr;
            dr = dt.NewRow();

            try
            {
                dr = dt.Rows.Find(intUserID);
                dr.BeginEdit();

                dr["UserName"] = txtUserName.Text;
                dr["Password"] = txtPassword.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"] = txtLastName.Text;

                dr.EndEdit();

                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);
                da.Update(ds, "Users");
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                ObdcConnection.Close();
                ObdcConnection.Dispose();
            }
        }

        private void deleteUser()
        {
            string query = "delete from users where ID=" + intUserID;

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;
            OdbcConnection.Open();

            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if(dr.Read())
            {

            }

            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }
        
    }
}
