using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ITRW324PTwebsite.Pages
{
    public partial class Home : System.Web.UI.Page
    {
        string usertype;
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

        protected void MultiView1_ActiveViewChanged(object sender, EventArgs e)
        {

        }

        protected void Menu_MenuItemClick(object sender, MenuEventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {

        }
    }
}