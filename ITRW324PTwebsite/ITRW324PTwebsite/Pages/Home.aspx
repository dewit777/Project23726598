<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="ITRW324PTwebsite.Pages.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
    function OpenPopup() {

       window.open("ContactUs.aspx", "List", "toolbar=no, location=no,status=yes,menubar=no,scrollbars=yes,resizable=no, width=900,height=500,left=430,top=100");
        return false;
    }
</script>

        <style type="text/css">
    body
    {
              background:#ccc;
    align-content:center;
      background-image:url('/Images/Logo.png') ;
   background-repeat:no-repeat;
   background-position:center;
   background-position-y:250px;
        font-family: Arial;
        font-size: 10pt;
    }
    .main_menu
    {
        width: 100px;
        background-color: #8AE0F2;
        color: #000;
        text-align: center;
        height: 30px;
        line-height: 30px;
        margin-right: 5px;
    }
    .level_menu
    {
        width: 110px;
        background-color: #000;
        color: #fff;
        text-align: center;
        height: 30px;
        line-height: 30px;
        margin-top: 5px;
    }
    .selected
    {
        background-color: #852B91;
        color: #fff;
    }
    .label
    {
        float:right;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>

     <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" ShowStartingNode="false" />
&nbsp;
        <asp:LinkButton ID="LinkButton1" runat="server" Text="Contact Us" OnClientClick="OpenPopup();" OnClick="LinkButton1_Click"></asp:LinkButton>
        
<asp:Menu ID="Menu" runat="server" DataSourceID="SiteMapDataSource1" Orientation="Horizontal"
    OnMenuItemDataBound="OnMenuItemDataBound" >
 
    <LevelMenuItemStyles>
        <asp:MenuItemStyle CssClass="main_menu" />
        <asp:MenuItemStyle CssClass="level_menu" />
    </LevelMenuItemStyles>
   
</asp:Menu>
    
        <asp:Label ID="Label1" runat="server"></asp:Label>
        
    
        <br />
          <%   if (Session["ID"] == null)
              {  %>
         <h2>Please Login by clicking the link below</h2>
        <asp:HyperLink ID="HyperLink1" Text="Login" runat="server" NavigateUrl="~/Pages/Login.aspx"></asp:HyperLink>
   <%   }  %>
    </div>
    </form>
</body>
</html>
