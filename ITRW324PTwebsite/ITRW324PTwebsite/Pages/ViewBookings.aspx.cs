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
using System.Data;
using ITRW324PTwebsite.ServiceReference1;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;


namespace ITRW324PTwebsite.Pages
{
    public partial class ViewBookings : System.Web.UI.Page
    {
                                             
        private static string gFolder = System.Web.HttpContext.Current.Server.MapPath("");
      
        string userEmail = "";
        string usertype;
        ServiceReference1.Service1Client webservice = new ServiceReference1.Service1Client();

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ID"] != null)
                usertype = Session["Type"].ToString();
            Calendar2.SelectedDate = DateTime.Now;
            Label1.Text = Calendar2.SelectedDate.ToShortDateString();
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
            if (usertype!="Admin")
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

        public void Display()
        {
            string query;
            if (dd_paid.SelectedIndex==0)
            {
               query = "Select BookingID,UserID,Name_and_Surname,Date,Timeslot,Confirmed,Reference from ITRW324.Bookings where Confirmed = "+0+" OR Confirmed ="+1+"";
            }
            else if(dd_paid.SelectedIndex==1)
                 query = "Select BookingID,UserID,Name_and_Surname,Date,Timeslot,Confirmed,Reference from ITRW324.Bookings where Confirmed = '" + 1+ "'";
            else
                query = "Select BookingID,UserID,Name_and_Surname,Date,Timeslot,Confirmed,Reference from ITRW324.Bookings where Confirmed = '" + 0 + "'";



             if (tbx_nameOfClient.Text.Length > 0)
            {
                query += " and Name_and_Surname like '%" + tbx_nameOfClient.Text + "%'";
            }
            if (tbx_refNumber.Text.Length > 0)
            {
                query += " and Reference like '%" + tbx_refNumber.Text + "%'"; ;
            }
            if (DropDownList1.SelectedIndex == 1)
            {
                query += " and Date <=   '" + Calendar2.SelectedDate.ToString("yyyy/MM/dd") + "'";

            }
            if (DropDownList1.SelectedIndex == 2)
            {
                query += " and Date >=  '" + Calendar2.SelectedDate.ToString("yyyy/MM/dd") + "'";

            }
            try
            {

    

               List<ServiceReference1.BookingDatas> bookings = webservice.GetBookings(query);


                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("BookingID");
                dt.Columns.Add("UserID");
                dt.Columns.Add("Name_and_Surname");
                dt.Columns.Add("Date");
                dt.Columns.Add("Timeslot");
                dt.Columns.Add("Confirmed");
                dt.Columns.Add("Reference");

                foreach (var item in bookings)
                {

                    DataRow row = dt.NewRow();
                    row["BookingID"] = item.BookingID;
                    row["UserID"] = item.UserID;
                    row["Name_and_Surname"] = item.Name;
                    row["Date"] = item.Date;
                    row["Timeslot"] = item.Timeslot;
                    row["Confirmed"] = item.Confirm;
                    row["Reference"] = item.Reference;
                    dt.Rows.Add(row);
                }
                GridView1.DataSource = dt;
                GridView1.DataBind();
                if (GridView1.Rows.Count > 0)
                {
                    btn_confirmPayment.Visible = true;
                }
                else
                {
                    btn_confirmPayment.Visible = false;
                }
            }
            catch(Exception e)
            {
                Label1.Text = e.ToString();
            }
            /*(   string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
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
               }*/
        }

        protected void Calendar2_SelectionChanged(object sender, EventArgs e)
        {

            string date = Calendar2.SelectedDate.ToShortDateString();
            
            Label1.Text = date;
           
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
            Session["gridview1SelectedUserIdRow"] = row.Cells[2].Text;
            Session["gridview1SelectedBookingUserName"] = row.Cells[3].Text;
            Session["gridview1SelectedRowDate"] = row.Cells[4].Text;
            Session["gridview1SelectedRowTimeslot"] = row.Cells[5].Text;
            //MessageLabel.Text = "You selected " + row.Cells[2].Text + ".";
        }

        protected void btn_confirmPayment_Click(object sender, EventArgs e)
        {
            int bookingId = Convert.ToInt32(Session["gridview1SelectedBookingIdRow"]);
            
            if (bookingId != 0)
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

            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine("Good day " + name);
            sb1.AppendLine("");
            sb1.AppendLine("Your booking for " + Convert.ToDateTime(Session["gridview1SelectedRowDate"]).ToString("MMM-dd-yyyy") + " at " + Session["gridview1SelectedRowTimeslot"].ToString() + " Has been confirmed.");
            sb1.AppendLine("If this is a mistake please contact us at Ivan23726598@gmail.com");
            sb1.AppendLine("Payment has been confirmed, thank you.");
            sb1.AppendLine("");

            sb1.AppendLine("Kind regards");
            string body = sb1.ToString();

            //Create PDF
            DataTable dt2 = new DataTable();
            dt2.Columns.AddRange(new DataColumn[3] {
                                new DataColumn("Client Name"),
                                new DataColumn("Date"),
                                new DataColumn("Price")});
            dt2.Rows.Add(name, Convert.ToDateTime(Session["gridview1SelectedRowDate"]), "R200");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    string companyName = "Fitness Mad";
                    
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Invoice</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Booking No:</b>");
                    sb.Append(bookingId);
                    sb.Append("</td><td><b>Date: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Company Name :</b> ");
                    sb.Append(companyName);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt2.Columns)
                    {
                        sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>");
                        sb.Append(column.ColumnName);
                        sb.Append("</th>");
                    }
                    sb.Append("</tr>");
                    foreach (DataRow row in dt2.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt2.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(row[column]);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    StringReader sr = new StringReader(sb.ToString());

                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();

                        
                        //Send Complied 
                      
                        Sendemail(Session["gridview1SelectedRowDate"].ToString(), body,Convert.ToInt32(Session["gridview1SelectedUserIdRow"]), bytes);

                    }
                }

            }
        }
        public void Sendemail(string date, string body, int userid,byte[] pdf)
        {


           userEmail= webservice.Sendemail(date, body, userid, pdf);
       
            if(userEmail != string.Empty)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(),
 "confirm_msg",
 "alert('Confirmation has been sent!');",
 true);
            }

            /*  string userFirstName = name.Substring(0, name.IndexOf(" "));
              string userSurname = name.Substring(name.IndexOf(" ") + 1, name.Length - name.IndexOf(" ") - 1);

              string query = "Select Email from ITRW324.Users  where Name = '" + userFirstName + "' and Surname = '" + userSurname + "'";

              using (MySqlCommand cmd = new MySqlCommand(query, con))
              {
                  cmd.Connection = con;

                  using (MySqlDataAdapter adpt = new MySqlDataAdapter())
                  {
                      adpt.SelectCommand = cmd;

                      con.Open();
                      MySqlDataReader rd = cmd.ExecuteReader();

                      while (rd.Read())
                      {
                          userEmail = rd["Email"].ToString();
                      }

                      //conn.Open();
                      //int result = cmd.ExecuteNonQuery();
                      con.Close();

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
            */


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

            Event Event = service.Events.Insert(myEvent, "primary").Execute();


        }



        protected void Button1_Click1(object sender, EventArgs e)
        {
            
           
           

        }






    

    }
    }