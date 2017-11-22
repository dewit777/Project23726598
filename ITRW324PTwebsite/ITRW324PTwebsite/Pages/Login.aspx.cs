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
    public partial class Login : System.Web.UI.Page
    {
        string usertype;
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ID"] != null)
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

        protected void Button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(TextBox1.Text);
            SHA256Managed alg = new SHA256Managed();
            string pwd = BitConverter.ToString(alg.ComputeHash(bytes));
            
            MySqlConnection conn = new MySqlConnection(constr);
            MySqlCommand cmd = new MySqlCommand("Select * from Users where Email=@email and Password=@pwd", conn);

            cmd.Parameters.AddWithValue("@email", TextBox5.Text);
            cmd.Parameters.AddWithValue("@pwd", pwd);
            conn.Open();
            MySqlDataReader rd = cmd.ExecuteReader();

            if (rd.Read())
            {
                Session["ID"] = rd["UserID"];
                Session["email"] = rd["Email"];
                Session["Name"] = rd["Name"] + " "+ rd["Surname"];
                Session["Type"] = rd["UserType"];
                rd.Close();
                cmd.Dispose();
                conn.Close();
                Response.Redirect("Home.aspx");
            }
            else
            {
                Label1.Text = "Email or Password is incorrect";
                conn.Close();
            }

        }
    }
}