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
                string Fname = TextBox5.Text;
                string Sname = TextBox1.Text;
                string spwd = TextBox2.Text;
                string email = TextBox4.Text;
                string cell = TextBox6.Text;
                string type = "Customer";

               byte[] bytes = Encoding.UTF8.GetBytes(spwd);
                SHA256Managed alg = new SHA256Managed();
                string pwd = BitConverter.ToString(alg.ComputeHash(bytes));
                Label1.Text = pwd;


                if (checkifexist(email) == false)
                {
                    try
                    {
                              string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                               MySqlConnection conn = new MySqlConnection(constr);
                               string insert = "Insert into Users (Name,Surname,UserType,Password,Email,CellNumber) values (@Name,@Surname,@UserType,@Pass,@email,@Cellnumber)";
                               using (MySqlCommand cmd2 = new MySqlCommand(insert, conn))
                               {
                                   using (MySqlDataAdapter adpt = new MySqlDataAdapter())
                                   {
                                       adpt.SelectCommand = cmd2;
                                       cmd2.Connection = conn;
                                cmd2.Parameters.Add("@Name", MySqlDbType.VarChar, 50).Value = Fname;
                                cmd2.Parameters.Add("@Surname", MySqlDbType.VarChar, 50).Value = Sname;
                                cmd2.Parameters.Add("@UserType", MySqlDbType.VarChar, 50).Value = type;
                                cmd2.Parameters.Add("@Pass", MySqlDbType.VarChar, 50).Value = pwd;                   
                                cmd2.Parameters.Add("@email", MySqlDbType.VarChar, 50).Value = email;
                                cmd2.Parameters.Add("@Cellnumber", MySqlDbType.VarChar, 50).Value = cell;

                                conn.Open();
                                       cmd2.ExecuteNonQuery();
                                       conn.Close();


                     } 

                     }
                         Response.Redirect("Home.aspx");
                         



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