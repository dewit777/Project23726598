using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace ITRW324PTwebsite.Pages
{
    public partial class CreateNewUser : System.Web.UI.Page
    {
        string usertype;
        string pwd;
        string Fname;
        string Sname;
        string spwd;
        string email;
        string cell;
        string type;
        ServiceReference1.Service1Client webservice = new ServiceReference1.Service1Client();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ID"]!=null)
            usertype = Session["Type"].ToString();
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
            if (usertype != "Admin")
            {
                System.Web.UI.WebControls.Menu menu = (System.Web.UI.WebControls.Menu)sender;
                SiteMapNode mapNode = (SiteMapNode)e.Item.DataItem;

                if (mapNode.Title == "Admin")
                {
                    System.Web.UI.WebControls.MenuItem itemToRemove = menu.FindItem(mapNode.Title);
                    menu.Items.Remove(itemToRemove);
                }

            }
        }




        public bool checkifexist(string email)
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            MySqlConnection conn = new MySqlConnection(constr);
            MySqlCommand cmd = new MySqlCommand("Select * from Users where Email = @email", conn);
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr.HasRows == true)
                {

                    return true;
                }

            }
            return false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Fname = TextBox5.Text;
                 Sname = TextBox1.Text;
                spwd = TextBox2.Text;
                 email = TextBox4.Text;
                cell = TextBox6.Text;
                type = "Customer";

               byte[] bytes = Encoding.UTF8.GetBytes(spwd);
                SHA256Managed alg = new SHA256Managed();
                pwd = BitConverter.ToString(alg.ComputeHash(bytes));
               


                if (checkifexist(email) == false)
                {
                    try
                    {


                        ServiceReference1.UserData user = new ServiceReference1.UserData();

                        user.Name = Fname;
                        user.Surname = Sname;
                        user.Usertype = type;
                        user.Password = pwd;
                        user.Email = email;
                        user.Cellnumber = cell;

                        webservice.Createuser(user);
                        Response.Redirect("Login.aspx");

                        


                    }
                    catch (Exception ex)
                    {
                        Label1.Text = "failed " + ex;
                    }

                }
                else
                    Label1.Text = "Email already in use.";
            }
        }
    }
}