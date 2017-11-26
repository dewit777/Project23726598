using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using System.Text;





namespace ITRW324PTwebsite.Pages
{
    public partial class MakeNewBooking : System.Web.UI.Page
    {
        ServiceReference1.Service1Client webservice = new ServiceReference1.Service1Client();
        string usertype;
        DateTime selecteddate;
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
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

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {

            selecteddate = Calendar1.SelectedDate;
            TextBox1.Text = selecteddate.ToString("yyyy/MM/dd");
            Label3.Text = "Date: " + TextBox1.Text + " Time: " + DropDownList1.SelectedValue.ToString();
            checkavailabletimeslots(selecteddate);

           
        }
        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.Date.DayOfWeek == DayOfWeek.Saturday || e.Day.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                e.Cell.Enabled = false;
                e.Day.IsSelectable = false;
                e.Cell.BackColor = System.Drawing.Color.Aqua;

            }
            else
                e.Day.IsSelectable = true;

            if (e.Day.Date < DateTime.Today)
            {
                e.Cell.Enabled = false;
                e.Day.IsSelectable = false;
                e.Cell.ForeColor = System.Drawing.Color.Gray;
            }

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
            if (Page.IsValid && TextBox1.Text!=string.Empty && DropDownList1.Text!=string.Empty)
            {
                string name = Session["Name"].ToString();
               string id = (Session["ID"]).ToString();
                string date = Calendar1.SelectedDate.ToShortDateString();
                string time = DropDownList1.SelectedValue.ToString();
                int pay = 0;
                string email = Session["email"].ToString();
                string refe = ""; 

                DateTime dt = Convert.ToDateTime(date);
                 

                try
                {
                    ServiceReference1.BookingDatas Booking = new ServiceReference1.BookingDatas();

                    string reference = dt.ToString("ddMMyyyy") + id.ToString() + createRandomSequence();
                    Booking.UserID = id;
                    Booking.Name = name;
                    Booking.Date = dt;
                    Booking.Timeslot = time;
                    Booking.Confirm = pay;
                    Booking.Reference = reference;


                        webservice.InsertBooking(Booking);
                }
                catch
                {
                    Label1.Text = "Failed to create booking";
                }
                finally
                {

                    using (MySqlCommand cmd = new MySqlCommand("Select Reference from Bookings where UserID=@user order by BookingID DESC LIMIT 1", con))
                    {
                        cmd.Parameters.AddWithValue("@user", id);
                        con.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            refe = rd["Reference"].ToString();
                        }
                        con.Close();
                    }
                        StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Good day " + name);
                    sb.AppendLine("");
                    sb.AppendLine("You've made a booking for " + date + " at " + time);
                    sb.AppendLine("If this is a mistake please contact us at Ivan23726598@gmail.com");
                    sb.AppendLine("Please make the payment to the following account to confirm the booking");
                    sb.AppendLine("");
                    sb.AppendLine("Banking Details:");
                    sb.AppendLine("Bank: Union Bank");
                    sb.AppendLine("account number: 65327522");
                    sb.AppendLine("Branch number: 422572");
                    sb.AppendLine("Reference number: " + refe);
                    string body = sb.ToString();
                    Sendemail(email, date, body);
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
"confirm_msg",
"alert('Booking has been made.');",
true);
                    checkavailabletimeslots(dt);
                }
            }
          else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
"confirm_msg",
"alert('Please pick a date and timeslot');",
true);
            }
           
        }
        public void checkavailabletimeslots(DateTime date)
        {
            DropDownList1.Items.Clear();
            DataTable dt = new DataTable();
            dt.Clear();


            string[] AllTimeslots = { "06:00", "08:00", "10:00", "12:00", "14:00", "16:00", "18:00", "20:00" };
            string[] NotAvailabletimeslots;
            string[] Availabletimeslots;

            


            dt.Columns.Add("Time");

           
            using (MySqlCommand cmd = new MySqlCommand("Select Date,Timeslot from Bookings where Date=@date", con))
            {
                cmd.Parameters.AddWithValue("@date", date);
                con.Open();
                MySqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    DataRow row = dt.NewRow();
                    row["Time"] = rd["Timeslot"];

                    dt.Rows.Add(row);

                }

            }

            var query = from row in dt.AsEnumerable()
                        select row["Time"].ToString();
            NotAvailabletimeslots = query.ToArray();

            Availabletimeslots = AllTimeslots.Except(NotAvailabletimeslots).ToArray();

          

            foreach (object time in Availabletimeslots)
            {
                DropDownList1.Items.Add(new ListItem(time.ToString(), time.ToString()));
            }
            Array.Clear(Availabletimeslots, 0, Availabletimeslots.Length);
            Array.Clear(NotAvailabletimeslots, 0, NotAvailabletimeslots.Length);
        }
    

       public void Sendemail(string email,string date,string body)
        {

            MailMessage mm = new MailMessage("Ivan23726598@gmail.com",email);
            mm.Subject = "Booking for " + date;
            mm.Body = body;
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mm);
        }

        public string createRandomSequence()
        {
            var randomNumber = new Random();
            string seq = "";
            for (int i = 0; i < 5; i++)
                seq += randomNumber.Next(0,9).ToString();

            return seq;
        }
    }
}