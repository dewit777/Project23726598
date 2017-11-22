using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Net.Mail;
using System.IO;

namespace Webservice
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

        public string InsertBooking(BookingDatas data)
        {
            string msg = string.Empty;

            con.Open();
            MySqlCommand cmd = new MySqlCommand("insert into Bookings(UserID,Name_and_Surname,Date,Timeslot,Confirmed) values(@userid,@name,@date,@timeslot,@confirm)", con);
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddWithValue("@userid", data.UserID);
            cmd.Parameters.AddWithValue("@name", data.Name);

            cmd.Parameters.AddWithValue("@date", data.Date);
            cmd.Parameters.AddWithValue("@timeslot", data.Timeslot);

            cmd.Parameters.AddWithValue("@confirm", data.Confirm);
            int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {
                msg = data.Name + " Your booking is successful";
            }
            else
            {
                msg = "failed to create booking";
            }
            con.Close();
            return msg;
        }

        public string Createuser(UserData Udata)
        {
            string msg = string.Empty;
            con.Open();
            MySqlCommand cmd = new MySqlCommand("insert into Users(Name,Surname,UserType,Password,Email,CellNumber) values(@name,@surname,@usertype,@pwd,@email,@cell)", con);
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddWithValue("@name", Udata.Name);
            cmd.Parameters.AddWithValue("@surname", Udata.Surname);
            cmd.Parameters.AddWithValue("@usertype", Udata.Usertype);
            cmd.Parameters.AddWithValue("@pwd", Udata.Password);
            cmd.Parameters.AddWithValue("@email", Udata.Email);
            cmd.Parameters.AddWithValue("@cell", Udata.Cellnumber);

            int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {
                msg = Udata.Name + " Inserted successfully";
            }
            else
            {
                msg = "failed to create user";
            }
            con.Close();
            return msg;
        }


        /*  public Bookingdata GetBookings(string query)
          {

              using (MySqlCommand cmd = new MySqlCommand(query))
              {
                  using (MySqlDataAdapter da = new MySqlDataAdapter())
                  {
                      cmd.Connection = con;
                      da.SelectCommand = cmd;
                      using (DataTable dt = new DataTable())
                      {
                          Bookingdata bookings = new Bookingdata();
                          da.Fill(bookings.BookingTable);
                              return bookings;
                         }
                  }
              }
          }*/
        public List<BookingDatas> GetBookings(string query)
        {

            List<BookingDatas> Booking = new List<BookingDatas>();
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                con.Open();
                MySqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    BookingDatas book = new BookingDatas();
                    book.BookingID = Convert.ToInt32(rd["BookingID"]);
                    book.UserID = Convert.ToString(rd["UserID"]);
                    book.Name = Convert.ToString(rd["Name_and_Surname"]);

                    book.Date = Convert.ToDateTime(rd["Date"]);
                    book.Timeslot = Convert.ToString(rd["Timeslot"]);
                    book.Confirm = Convert.ToInt32(rd["Confirmed"]);
                    book.Reference = Convert.ToString(rd["Reference"]);
                    Booking.Add(book);
                }

            }
            return Booking.ToList();


        }

        public List<UserData> GetUsers(string query)
        {

            List<UserData> Users = new List<UserData>();

            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {

                con.Open();
                MySqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    UserData user = new UserData();
                    user.Userid = Convert.ToInt32(rd["UserID"]);
                    user.Name = Convert.ToString(rd["Name"]);
                    user.Surname = Convert.ToString(rd["Surname"]);

                    //  user.Usertype = Convert.ToString(rd["Usertype"]);
                    //    user.Password = Convert.ToString(rd["Password"]);
                    user.Email = Convert.ToString(rd["Email"]);
                    user.Cellnumber = Convert.ToString(rd["CellNumber"]);
                    Users.Add(user);
                }

            }
            return Users.ToList();
        }
        public string Sendemail(string date, string body, int userid,byte[] pdf)
        {

            string userEmail = "";
            string query = "Select Email from ITRW324.Users  where UserID = '" + userid + "'";

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
                    mm.Attachments.Add(new Attachment(new MemoryStream(pdf), "iTextSharpPDF.pdf"));
                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(mm);
                  
                }

            }
            return userEmail;
        }
    }
}
