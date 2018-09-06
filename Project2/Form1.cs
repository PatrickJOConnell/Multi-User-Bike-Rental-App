//
//  Multi-user  BikeHike  Windows  app,  using  transactions.
//
//  Patrick O'Connell (oconne16)
//  U.  of  Illinois,  Chicago
//  CS480,  Summer  2018
//  Project  #3
//

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

namespace Project2
{
    
    public partial class Form1 : Form
    {
        int lastButt = 0;
        int customer = 0;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            lastButt = 1;
            customer = 1;
            string filename, connectionInfo;
            SqlConnection db;
            filename = "BikeHike.mdf";
            connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
            db = new SqlConnection(connectionInfo);
            db.Open();
            string sql = string.Format(@"Select Cust_L_Name, Cust_F_Name From Customer Order by Cust_L_Name Asc, Cust_F_Name Asc;");
            SqlCommand commandSQL = new SqlCommand();
            commandSQL.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(commandSQL);
            DataSet dataset = new DataSet();
            commandSQL.CommandText = sql;
            adapter.Fill(dataset);
            String Fname;
            String Lname;
            List<string> X = new List<string>();
            string Custnames;
            foreach (DataRow row in dataset.Tables["TABLE"].Rows)
            {
                Lname = Convert.ToString(row["Cust_L_Name"]);
                Fname = Convert.ToString(row["Cust_F_Name"]);
                Custnames = string.Format(Lname + ", " + Fname);
                X.Add(Custnames);
            }
            listBox1.DataSource = X;
            db.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e){
            if (customer == 1){
                string name = this.listBox1.Text;
                // new string[] { "xx" }, StringSplitOptions.None, StringSplitOptions.None
                string[] names = name.Split(new string[] { ", " }, StringSplitOptions.None);
                
                string filename, connectionInfo;
                SqlConnection db;
                filename = "BikeHike.mdf";
                connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                db = new SqlConnection(connectionInfo);
                db.Open();
                string sql = string.Format(@"Select Customer.cid, Email, Bike_out,  Bike_In, Expected_Time  From CUSTOMER Left Join Rental on Rental.CID = Customer.CID Where Cust_L_Name = '{0}' AND Cust_F_Name = '{1}';", names[0], names[1]);
                SqlCommand commandSQL = new SqlCommand();
                commandSQL.Connection = db;
                SqlDataAdapter adapter = new SqlDataAdapter(commandSQL);
                DataSet dataset = new DataSet();
                commandSQL.CommandText = sql;
                adapter.Fill(dataset);
                String cid  = "";
                String email;
                string timeBack;
                String outWithRental = "No";
                List<string> X = new List<string>();
                int x = 0;            
                foreach (DataRow row in dataset.Tables["TABLE"].Rows)
                {
                    cid = Convert.ToString(row["cid"]);
                    email = Convert.ToString(row["Email"]);
                    timeBack = Convert.ToString(row["Expected_Time"]);
                    if ((Convert.ToString(row["Bike_Out"]) != "") && (Convert.ToString(row["Bike_In"]) == ""))
                    {
                        x++;
                    }
                    this.textBox5.Text = timeBack;
                    if (x == 0){
                        this.textBox3.Text = "No";
                        this.textBox5.Text = "N/A";
                    }
                    else{
                        this.textBox3.Text = "Yes";
                    }
                    this.textBox1.Text = cid;
                    this.textBox2.Text = email;
                    this.textBox4.Text = x.ToString();                   
                }
                DateTime outttime = DateTime.Now;
                string sql3 = string.Format(@"Select Bike_Out From Rental Where BID = (Select max(BID) From REntal Where CID = {0} and BIke_in is Null) And Bike_In is NULL;", cid);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql3;
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string date = result.ToString();
                    outttime = Convert.ToDateTime(date);
                    try
                    {
                        DateTime expec = outttime + TimeSpan.FromHours(Convert.ToDouble(this.textBox5.Text));              
                        this.textBox5.Text = expec.ToString();
                    }
                    catch
                    {
                       
                    }                   
                }
                db.Close();
            }
            else if (customer == 2)
            {
                string bid = this.listBox1.Text;
                string filename, connectionInfo;
                SqlConnection db;
                filename = "BikeHike.mdf";
                connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                db = new SqlConnection(connectionInfo);
                db.Open();
                string sql = string.Format(@"Select Bike.TID, Year_In, Rental_Status, Rental_Rate, BIKE_TYPE.Type_Description, Expected_Time From Bike with (Index(TID_Index)) Join BIKE_TYPE on Bike.TID = BIKE_TYPE.TID Left join Rental on Rental.BID = BIKE.BID
Where BIKE.BID = {0};", bid);
                SqlCommand commandSQL = new SqlCommand();
                commandSQL.Connection = db;
                SqlDataAdapter adapter = new SqlDataAdapter(commandSQL);
                DataSet dataset = new DataSet();
                commandSQL.CommandText = sql;
                adapter.Fill(dataset);
                string TID;
                string YearIn;
                string RentalStatus;
                string RentalRate;
                string TypeDescription;
                string ExpectedTime;                
                foreach (DataRow row in dataset.Tables["TABLE"].Rows)
                {
                    TID = Convert.ToString(row["TID"]);
                    YearIn = Convert.ToString(row["Year_In"]);
                    RentalStatus = Convert.ToString(row["Rental_Status"]);
                    RentalRate = Convert.ToString(row["Rental_Rate"]);
                    TypeDescription = Convert.ToString(row["Type_Description"]);
                    ExpectedTime = Convert.ToString(row["Expected_Time"]);
                    this.textBox10.Text = TID;
                    this.textBox9.Text = TypeDescription;
                    this.textBox8.Text = RentalStatus;
                    this.textBox7.Text = RentalRate;
                    this.textBox6.Text = ExpectedTime;
                    this.textBox11.Text = YearIn;
                    if (ExpectedTime == ""){
                        this.textBox6.Text = "N/A";
                    }
                }
                DateTime outttime = DateTime.Now;
                string sql3 = string.Format(@"Select Bike_Out From Rental Where BID = {0} and Bike_In is Null;", bid);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql3;
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string date = result.ToString();
                    outttime = Convert.ToDateTime(date);
                    DateTime expec = outttime + TimeSpan.FromHours(Convert.ToDouble(this.textBox6.Text));
                    this.textBox6.Text = expec.ToString();
                }
                db.Close();
            }
            else if (customer == 3)
            {
                string bid = this.listBox1.Text;
                string[] names = bid.Split(new string[] { ", " }, StringSplitOptions.None);
                string filename, connectionInfo;
                SqlConnection db;
                filename = "BikeHike.mdf";
                connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                db = new SqlConnection(connectionInfo);
                db.Open();
                string sql = string.Format(@"Select Bike.TID, Year_In, Rental_Status, Rental_Rate, BIKE_TYPE.Type_Description, Expected_Time From Bike Join BIKE_TYPE on Bike.TID = BIKE_TYPE.TID Left join Rental on Rental.BID = BIKE.BID
Where BIKE.BID = {0};", names[0]);
                SqlCommand commandSQL = new SqlCommand();
                commandSQL.Connection = db;
                SqlDataAdapter adapter = new SqlDataAdapter(commandSQL);
                DataSet dataset = new DataSet();
                commandSQL.CommandText = sql;
                adapter.Fill(dataset);
                string TID;
                string YearIn;
                string RentalStatus;
                string RentalRate;
                string TypeDescription;
                string ExpectedTime;
                foreach (DataRow row in dataset.Tables["TABLE"].Rows)
                {
                    TID = Convert.ToString(row["TID"]);
                    YearIn = Convert.ToString(row["Year_In"]);
                    RentalStatus = Convert.ToString(row["Rental_Status"]);
                    RentalRate = Convert.ToString(row["Rental_Rate"]);
                    TypeDescription = Convert.ToString(row["Type_Description"]);
                    ExpectedTime = Convert.ToString(row["Expected_Time"]);
                    this.textBox10.Text = TID;
                    this.textBox9.Text = TypeDescription;
                    this.textBox8.Text = RentalStatus;
                    this.textBox7.Text = RentalRate;
                    this.textBox6.Text = ExpectedTime;
                    this.textBox11.Text = YearIn;
                    if (ExpectedTime == "")
                    {
                        this.textBox6.Text = "N/A";
                    }
                }
                db.Close();
            }
        }
         
        private void button2_Click(object sender, EventArgs e)
        {
            lastButt = 2;
            customer = 2;
            string filename, connectionInfo;
            SqlConnection db;
            filename = "BikeHike.mdf";
            connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
            db = new SqlConnection(connectionInfo);
            db.Open();
            string sql = string.Format(@"Select BId From Bike Order by bid Asc;");
            SqlCommand commandSQL = new SqlCommand();
            commandSQL.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(commandSQL);
            DataSet dataset = new DataSet();
            commandSQL.CommandText = sql;
            adapter.Fill(dataset);
            String cid;
            List<string> X = new List<string>();
            foreach (DataRow row in dataset.Tables["TABLE"].Rows)
            {
                cid = Convert.ToString(row["bid"]);
                X.Add(cid);
            }
            listBox1.DataSource = X;
            db.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lastButt = 3;
            customer = 3;
            string filename, connectionInfo;
            SqlConnection db;
            filename = "BikeHike.mdf";
            connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
            db = new SqlConnection(connectionInfo);
            db.Open();

            string sql = string.Format(@"Select BID, Bike.TID, Type_Description, Year_In, Rental_Status From Bike join bike_Type on BIke_type.TID = Bike.TID Where Rental_Status = 0 Order By Tid Asc, Year_In Desc;");
            SqlCommand commandSQL = new SqlCommand();
            commandSQL.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(commandSQL);
            DataSet dataset = new DataSet();
            commandSQL.CommandText = sql;
            adapter.Fill(dataset);
            String bid, TypeDescription, Year_In;

            List<string> X = new List<string>();

            foreach (DataRow row in dataset.Tables["TABLE"].Rows)
            {
                bid = Convert.ToString(row["bid"]);
                TypeDescription = Convert.ToString(row["Type_Description"]);
                Year_In = Convert.ToString(row["Year_In"]);
                string r = String.Format(bid + ", " + TypeDescription + ", " + Year_In);
                X.Add(r);
            }
            listBox1.DataSource = X;
            db.Close();            
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void button4_Click(object sender, EventArgs e)
        {
            RentalCart CustRC = new RentalCart(this.rbid.Text, this.rcid.Text, this.ert.Text);      // Create Rental Cart Object


            string name = CustRC.BIDs;      // Get the string of bike IDs from the Rental Cart Object


            string[] names = name.Split(new string[] { ", " }, StringSplitOptions.None);

            customer = 4;
            int CustID = -1;
            int BikeID = -1;
            double rentalTime = -1.1;
            int gate = 1;
            foreach (string s in names)
            {

                SqlConnection db = null;
                SqlTransaction tx = null;
                int retries = 0;
                while (retries < 3) {
                    gate = 1;
                    try
                    {
                        CustID = Convert.ToInt32(CustRC.CID);
                        BikeID = Convert.ToInt32(s);
                        rentalTime = Convert.ToDouble(CustRC.ExpectedTime);
                        string filename, connectionInfo;
                        filename = "BikeHike.mdf";
                        connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                        db = new SqlConnection(connectionInfo);
                        db.Open();
                        tx = db.BeginTransaction(IsolationLevel.Serializable);
                        string sql = string.Format(@"Select CID From Customer Where Cid = {0};", CustID);
                        string sql2 = string.Format(@"Select BID From Bike where bid = {0} AND Rental_Status = 0;", BikeID);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = db;
                        cmd.Transaction = tx; 
                        cmd.CommandText = sql;
                        object result = cmd.ExecuteScalar();
                        cmd.CommandText = sql2;
                        object result2 = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            gate = 0;
                            retries = 3;
                            MessageBox.Show("Invalid Customer ID");
                            if (tx != null)
                            {
                                tx.Rollback();
                            }
                            if (db != null)
                            {
                                db.Close();
                            }
                            return;
                        }
                        if (result2 == null)
                        {
                            retries = 3;
                            gate = 0;
                            if (tx != null)
                            {
                                tx.Rollback();
                            }
                            if (db != null)
                            {
                                db.Close();
                            }
                            MessageBox.Show("Bike ID Entered Not Available For Rent");
                        }
                        if (gate == 1)
                        {
                            sql = string.Format(@"Insert Into Rental(CID, BID, Bike_Out, Expected_Time) Values({0}, {1},  GETDATE(), {2});
                    Update Bike Set Rental_Status = 1 where BID = {3};", CustID, BikeID, rentalTime, BikeID);
                            cmd.CommandText = sql;
                            int rowsModified = cmd.ExecuteNonQuery();

                            string sh = string.Format("Bike {0} rented for customer {1}.", BikeID, CustID);
                            retries = 3;
                            int delay = Convert.ToInt32(this.textBox13.Text);
                            System.Threading.Thread.Sleep(delay);
                            tx.Commit();
                            MessageBox.Show(sh);
                        }     
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205)
                        {
                            System.Threading.Thread.Sleep(1000);
                            Console.WriteLine("Retry\n");
                            retries++;
                        }
                        else
                        {
                            retries = 3;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (tx != null)
                                {
                                    tx.Rollback();
                                }
                                retries = 3;
                                MessageBox.Show("Bike Rental Failed");
                    }       
                }
                if (lastButt == 1)
                {
                    button1_Click(this, null);
                }
                else if (lastButt == 2)
                {
                    button2_Click(this, null);

                }
                else if (lastButt == 3)
                {
                    button3_Click(this, null);

                }
                else
                {

                }
            }
        }

        private void label9_Click(object sender, EventArgs e) { }

        private void label12_Click(object sender, EventArgs e) { }

        private void rbid_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            SqlTransaction tx = null;
            SqlConnection db = null;
            int retries = 0;

            customer = 0;
            while (retries < 3)
            {
                try
                {
                    string filename, connectionInfo;
                    filename = "BikeHike.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                    db = new SqlConnection(connectionInfo);
                    db.Open();
                    tx = db.BeginTransaction(IsolationLevel.Serializable);
                    string sql = string.Format(@"Select Rental.BID From Rental with (Index(BID_Index), Index(CID_Index)) left join Bike on rental.BID = Bike.Bid Where REntal.Bike_In is Null AND REntal.CID = {0};", this.textBox12.Text);
                    SqlCommand commandSQL = new SqlCommand();
                    commandSQL.Connection = db;
                    commandSQL.Transaction = tx;
                    SqlDataAdapter adapter = new SqlDataAdapter(commandSQL);
                    DataSet dataset = new DataSet();
                    commandSQL.CommandText = sql;
                    adapter.Fill(dataset);
                    String BID;
                    List<string> X = new List<string>();
                    foreach (DataRow row in dataset.Tables["TABLE"].Rows)
                    {
                        BID = Convert.ToString(row["BID"]);

                        X.Add(BID);
                    }
                    listBox1.DataSource = X;

                    string sql2 = string.Format(@"Update Bike Set Rental_Status = 0 Where BID in (Select Rental.BID From Rental left join Bike on rental.BID = Bike.Bid Where REntal.Bike_In is Null AND REntal.CID = {0});
                                          Update REntal Set Bike_In = GETDATE() Where BID in (Select Rental.BID From Rental with (Index(BID_Index), Index(CID_Index)) left join Bike on rental.BID = Bike.Bid Where REntal.Bike_In is Null AND REntal.CID = {0});
", this.textBox12.Text);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql2;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();
                    int x = 0;
                    foreach (string s in X)
                    {
                        string sql3 = string.Format(@"Update Rental Set Expected_Time = null, Rental_Cost = Round((DATEDIFF(second, REntal.Bike_out, Rental.Bike_in) / 3600.0 * (Select Distinct Rental_Rate From REntal with (Index(BID_Index)) join Bike on bike.BID = rental.bid join Bike_type on Bike_Type.TID = Bike.TID where Bike.Bid = {0})), 2) where BID = {0};", s);
                        SqlCommand cmd3 = new SqlCommand();
                        cmd3.Connection = db;
                        cmd3.CommandText = sql3;
                        cmd3.Transaction = tx;
                        int rowsM = cmd3.ExecuteNonQuery();
                        x++;
                    }
                    double totalCost = 0.0;
                    foreach (string s in X)
                    {
                        string sql4 = string.Format(@"Select Rental_Cost From Rental with (Index(CID_Index), Index(BID_Index)) Where BID = {0} and CID = {1} and Bike_Out = (Select max(Bike_Out) From REntal WHere  BID = {0} and CID = {1} );", s, this.textBox12.Text);
                        SqlCommand cmd12 = new SqlCommand();
                        cmd12.Connection = db;
                        cmd12.CommandText = sql4;
                        cmd12.Transaction = tx;
                       
                        object result5 = cmd12.ExecuteScalar();
                        if (result5 != null && result5.ToString() != "")
                        {
                            totalCost += Convert.ToDouble(result5);
                        }

                    }
                    string n = this.textBox12.Text;
                    string dsp = string.Format(@"{0} bike(s) returned for customer {1}. Total cost is: ${2}", x.ToString(), n, totalCost);
                    if (x == 0)
                    {
                        MessageBox.Show("No bikes to return for Customer {0}", n);
                    }
                    else
                    {
                        MessageBox.Show(dsp);
                    }          
                    int delay = Convert.ToInt32(this.textBox13.Text);
                    System.Threading.Thread.Sleep(delay);
                    tx.Commit();
                    retries = 3;

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                    {
                        System.Threading.Thread.Sleep(1000);
                        retries++;
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                        retries = 3;
                    }
                    
                    
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    retries = 3;
                    MessageBox.Show("Bike Rental Failed");
                }

                finally
                {
                    if (db != null)
                    {
                        db.Close();
                    }
                }

                if (lastButt == 1)
                {
                    button1_Click(this, null);
                }
                else if (lastButt == 2)
                {
                    button2_Click(this, null);
                }
                else if (lastButt == 3)
                {
                    button3_Click(this, null);
                }
                else
                {

                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e) {}

        private void label15_Click(object sender, EventArgs e) {}

        private void label17_Click(object sender, EventArgs e) {}

        private void textBox13_TextChanged(object sender, EventArgs e) {}

        private void button6_Click(object sender, EventArgs e)
        {
            SqlTransaction tx = null;
            SqlConnection db = null;
            int retries = 0;
            while (retries < 3)
            {
                try
                {
                    string sql = string.Format(@"Truncate Table Rental;
                                             UPDATE Bike SET Rental_Status = 0;");
                    string filename, connectionInfo;
                    filename = "BikeHike.mdf";
                    connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;", "MSSQLLocalDB", filename);
                    db = new SqlConnection(connectionInfo);
                    db.Open();
                    tx = db.BeginTransaction(IsolationLevel.Serializable);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = db;
                    cmd.CommandText = sql;
                    cmd.Transaction = tx;
                    int rowsModified = cmd.ExecuteNonQuery();
                    int delay = Convert.ToInt32(this.textBox13.Text);
                    System.Threading.Thread.Sleep(delay);
                    tx.Commit();
                    MessageBox.Show("Database Reset Successful");
                    retries = 3;                    
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205)
                    {
                        System.Threading.Thread.Sleep(500);
                        retries++;
                    }
                    else
                    {
                        retries = 3;
                    }
                }
                catch (Exception ex)
                {
                    if (tx != null)
                    {
                        tx.Rollback();
                    }
                    retries = 3;
                    MessageBox.Show("Database Reset Failed");
                }
                finally
                {
                    customer = 0;
                    listBox1.DataSource = null;

                    if (db != null)
                    {
                        db.Close();
                    }                  
                }
            }
        }
    }
}
