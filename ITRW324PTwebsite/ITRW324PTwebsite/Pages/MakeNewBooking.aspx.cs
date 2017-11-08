using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;



namespace ITRW324PTwebsite.Pages
{
    public partial class MakeNewBooking : System.Web.UI.Page
    {
        public int  userid;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ID"] == null)
                // Response.Redirect("Login.aspx");
                Label1.Text = "please login";
            else
            {
                userid = Convert.ToInt32(Session["ID"]);
                string username = Session["Name"].ToString();
               

            }
        }
        protected void OnMenuItemDataBound(object sender, MenuEventArgs e)
        {
            if (SiteMap.CurrentNode != null)
            {
                if (e.Item.Text == SiteMap.CurrentNode.Title)
                {
                    if (e.Item.Parent != null)
                    {
                        e.Item.Parent.Selected = true;
                    }
                    else
                    {
                        e.Item.Selected = true;
                    }
                }
            }
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            string date = Calendar1.SelectedDate.ToLongDateString();
            TextBox1.Text = Calendar1.SelectedDate.ToLongDateString();
            Label3.Text = "Date: " + TextBox1.Text + " Time: " + DropDownList1.SelectedValue.ToString();

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {


        }

        protected void ImageButton1_Click1(object sender, ImageClickEventArgs e)
        {

            if (Calendar1.Visible)
                Calendar1.Visible = false;
            else
                Calendar1.Visible = true;
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TextBox1.Text != null)
                Label3.Text = "Date: " + TextBox1.Text + " Time: " + DropDownList1.SelectedValue.ToString();
            else
                Label3.Text = "Please select date.";
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {

        }
        public void checkavailabletimeslots()
        {



        }

        public void Book()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            try
            {
                MySqlConnection conn = new MySqlConnection(constr);

             



                string insert = "Insert into Bookings () values ()";
                using (MySqlCommand cmd = new MySqlCommand(insert, conn))
                {
                    cmd.Connection = conn;


                    using (MySqlDataAdapter adpt = new MySqlDataAdapter())
                    {
                        adpt.SelectCommand = cmd;
                     
                       


                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();

                    }
                }

            }
            catch (Exception ex)
            {
                Label1.Text = "Not Entered " + ex;
            }
        }
    }
}