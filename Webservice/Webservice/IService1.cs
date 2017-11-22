using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace Webservice
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string InsertBooking(BookingDatas bdata);

        [OperationContract]
        string Createuser(UserData udata);

          [OperationContract]
          List<BookingDatas> GetBookings(string query);

      //  [OperationContract]
      //  Bookingdata GetBookings(string query);



        [OperationContract]
        List<UserData> GetUsers(string query);

        [OperationContract]
       string Sendemail(string date, string body,int userid,byte[] pdf);
        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.

        [DataContract]
        public class Bookingdata
    {
        public Bookingdata()
        {
            this.BookingTable = new DataTable("Bookings");
        }
        [DataMember]
        public DataTable BookingTable { get; set; }
    }


    [DataContract]
    public class BookingDatas
    {
        int bookingID = 0;
        string userID = string.Empty;
        string name = string.Empty;
        DateTime date = DateTime.MinValue;
        string timeslot = string.Empty;
        int confirm =0;
        string reference = string.Empty;

        [DataMember]
        public int BookingID
        {
            get { return bookingID; }
            set { bookingID = value; }
        }
        [DataMember]
        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [DataMember]
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

      
        [DataMember]
        public string Timeslot
        {
            get { return timeslot; }
            set { timeslot = value; }
        }
        [DataMember]
        public int Confirm
        {
            get { return confirm; }
            set { confirm = value; }
        }
        [DataMember]
        public string Reference
        {
            get { return reference; }
            set { reference = value; }
        }

    }

    [DataContract]
    public class UserData
    {
        int userid = 0;
        string name = string.Empty;
        string surname = string.Empty;
        string usertype = string.Empty;
        string password = string.Empty;
        string email = string.Empty;
        string cellnumber = string.Empty;
        [DataMember]
        public int Userid
        {
            get { return userid; }
            set { userid = value; }
        }
        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [DataMember]
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }
        [DataMember]
        public string Usertype
        {
            get { return usertype; }
            set { usertype = value; }
        }
        [DataMember]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        [DataMember]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        [DataMember]
        public string Cellnumber
        {
            get { return cellnumber; }
            set { cellnumber = value; }
        }
    }
}

