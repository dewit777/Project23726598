using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text;
using System.Net.Mail;
using Google.Apis.Calendar;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;


using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace ITRW324PTwebsite.Pages
{
    public partial class ViewBookings : System.Web.UI.Page
    {
                                             
        private static string gFolder = System.Web.HttpContext.Current.Server.MapPath("");
        DateTime dt;
        string userEmail = "";


        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                btn_confirmPayment.Visible = false;
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

        public void Display()
        {
            int hasPaid = 0;
            if (dd_paid.SelectedItem.ToString().ToLower() == "yes")
            {
                hasPaid = 1;
            }

            string query = "Select * from ITRW324.Bookings where Confirmed = " + hasPaid;
            if (tbx_nameOfClient.Text.Length > 0)
            {
                query += " and Name_and_Surname like '%" + tbx_nameOfClient.Text + "%'";
            }
            if (tbx_refNumber.Text.Length > 0)
            {
                query += " and Reference like '%" + tbx_refNumber.Text + "%'"; ;
            }
            if (DropDownList1.SelectedIndex == 0)
            {
                query += " and Date <=   '" + Calendar2.SelectedDate.ToString("yyyy/MM/dd") + "'";

            }
            if (DropDownList1.SelectedIndex == 1)
            {
                query += " and Date >=  '" + Calendar2.SelectedDate.ToString("yyyy/MM/dd") + "'";

            }

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(constr);

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Connection = conn;

                using (MySqlDataAdapter adpt = new MySqlDataAdapter())
                {
                    adpt.SelectCommand = cmd;

                    conn.Open();
                    MySqlDataReader rd = cmd.ExecuteReader();

                    GridView1.DataSource = rd;
                    GridView1.DataBind();
                    GridView1.Visible = true;

                    if (GridView1.Rows.Count > 0)
                    {
                        btn_confirmPayment.Visible = true;
                    }
                    else
                    {
                        btn_confirmPayment.Visible = false;
                    }

                    //conn.Open();
                    //int result = cmd.ExecuteNonQuery();
                    conn.Close();

                }
            }
        }

        protected void Calendar2_SelectionChanged(object sender, EventArgs e)
        {

            string date = Calendar2.SelectedDate.ToShortDateString();
            dt = Convert.ToDateTime(date);
            Label1.Text = dt.ToString();
           
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string uname = tbx_nameOfClient.Text;
            string refe = tbx_refNumber.Text;
            bool pay;

            if (dd_paid.SelectedIndex == 0)
            {
                pay = true;
            }
            if (dd_paid.SelectedIndex == 1)
            {
                pay = false;
            }

            Display();
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GridView1.SelectedRow;
            Session["gridview1SelectedBookingIdRow"] = row.Cells[1].Text;
            Session["gridview1SelectedBookingUserName"] = row.Cells[3].Text;
            Session["gridview1SelectedRowDate"] = row.Cells[4].Text;
            Session["gridview1SelectedRowTimeslot"] = row.Cells[5].Text;
            //MessageLabel.Text = "You selected " + row.Cells[2].Text + ".";
        }

        protected void btn_confirmPayment_Click(object sender, EventArgs e)
        {
            int bookingId = Convert.ToInt32(Session["gridview1SelectedBookingIdRow"]);
            
            if (bookingId != null)
            {
                string query = "Update ITRW324.Bookings set Confirmed = 1  where BookingID = " + bookingId;

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                MySqlConnection conn = new MySqlConnection(constr);

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Connection = conn;

                    using (MySqlDataAdapter adpt = new MySqlDataAdapter())
                    {
                        adpt.SelectCommand = cmd;

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        sendConfirmationEmail(bookingId);

                        string timeslot = Session["gridview1SelectedRowTimeslot"].ToString();
                        int hour = Convert.ToInt32(timeslot.Substring(0, 2));
                        
                       DateTime date= Convert.ToDateTime(Session["gridview1SelectedRowDate"]);
                        int year = date.Year;
                        int month = date.Month;
                        int day = date.Day;
                       

                        createGoogleCalenderevent(year,month,day,hour, userEmail);
                        Display();
                    }

                }

              
            }
        }

        public void sendConfirmationEmail(int bookingId)
        {

            string name = Session["gridview1SelectedBookingUserName"].ToString();


            using (MySqlCommand cmd = new MySqlCommand("Select Reference from Bookings where BookingID = @bookingId", con))
            {
                cmd.Parameters.AddWithValue("@bookingId", bookingId);
                con.Open();
                MySqlDataReader rd = cmd.ExecuteReader();

            }
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Good day " + name);
            sb.AppendLine("");
            sb.AppendLine("Your booking for " + Convert.ToDateTime(Session["gridview1SelectedRowDate"]).ToString("MMM-dd-yyyy") + " at " + Session["gridview1SelectedRowTimeslot"].ToString() + " Has been confirmed.");
            sb.AppendLine("If this is a mistake please contact us at Ivan23726598@gmail.com");
            sb.AppendLine("Payment has been confirmed, thank you.");
            sb.AppendLine("");
            
            sb.AppendLine("Kind regards");
            string body = sb.ToString();
            Sendemail(Session["gridview1SelectedRowDate"].ToString(), body);
          
        }


        public void Sendemail(string date, string body)
        {

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(constr);

            string userNameAndSurname = Session["gridview1SelectedBookingUserName"].ToString();


            string userFirstName = userNameAndSurname.Substring(0, userNameAndSurname.IndexOf(" "));
            string userSurname = userNameAndSurname.Substring(userNameAndSurname.IndexOf(" ") + 1, userNameAndSurname.Length - userNameAndSurname.IndexOf(" ") - 1);
          
            string query = "Select Email from ITRW324.Users  where Name = '" + userFirstName + "' and Surname = '" + userSurname + "'";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Connection = conn;

                using (MySqlDataAdapter adpt = new MySqlDataAdapter())
                {
                    adpt.SelectCommand = cmd;

                    conn.Open();
                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        userEmail = rd["Email"].ToString();
                    }

                    //conn.Open();
                    //int result = cmd.ExecuteNonQuery();
                    conn.Close();

                    MailMessage mm = new MailMessage("Ivan23726598@gmail.com", userEmail);
                    mm.Subject = "Booking CONFIRMED for " + date;
                    mm.Body = body;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(mm);


                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
          "confirm_msg",
          "alert('Confirmation has been sent!');",
          true);
                }
            }



        }

        public void createGoogleCalenderevent(int year, int month, int day, int hour,string email)
        {

            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "793074437757-u3r9hvoer0jtj6pcn1gi9qvpq08mmktg.apps.googleusercontent.com",
                    ClientSecret = "JhgmdCZ2lBD5RKexk2GjVFao",
                },
                new[] { CalendarService.Scope.Calendar },
                "ivan23726598@gmail.com",
                CancellationToken.None).Result;

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Calendar API Sample",
            });

           
            Event myEvent = new Event
            {
                Summary = "Training Session",
                Location = "Gym",
               
                Start = new EventDateTime()
                {
                    DateTime = new DateTime(year, month, day, hour, 0, 0),
                   TimeZone = "America/Los_Angeles"
                },
                End = new EventDateTime()
                {
                    DateTime = new DateTime(year, month, day, hour+2, 0, 0),
                    TimeZone = "America/Los_Angeles"
                },
                
                Attendees = new List<EventAttendee>()
      {
        new EventAttendee() { Email = "ivan23726598@gmail.com" },
         new EventAttendee() { Email = email }
      }
            };

            Event recurringEvent = service.Events.Insert(myEvent, "primary").Execute();


        }



        protected void Button1_Click1(object sender, EventArgs e)
        {
            
           
           

        }






    

    }
    }