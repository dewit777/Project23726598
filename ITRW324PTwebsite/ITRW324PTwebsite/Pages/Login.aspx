<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ITRW324PTwebsite.Pages.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <style type="text/css">
    body
    {
             background:#ccc;
    align-content:center;
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
            .auto-style1 {
                height: 29px;
            }
        </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
             <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" ShowStartingNode="false" />
<asp:Menu ID="Menu" runat="server" DataSourceID="SiteMapDataSource1" Orientation="Horizontal"
    OnMenuItemDataBound="OnMenuItemDataBound" >
 
    <LevelMenuItemStyles>
        <asp:MenuItemStyle CssClass="main_menu" />
        <asp:MenuItemStyle CssClass="level_menu" />
    </LevelMenuItemStyles>
   
</asp:Menu>
             <br />
             <br />
       <table>
           <tr>
               <td class="auto-style1">Enter Email</td>
               <td><asp:TextBox ID="TextBox5" runat="server"></asp:TextBox>
                   <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox5" ErrorMessage="Name Required"></asp:RequiredFieldValidator>
               </td>
           </tr>
           <tr>
               <td class="auto-style1">Enter Password</td>
               <td><asp:TextBox ID="TextBox1" runat="server" TextMode="Password"></asp:TextBox>
                   <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox1" ErrorMessage="Password Required"></asp:RequiredFieldValidator>
               </td>
           </tr>

           <tr>
               <td class="auto-style1">
                   <asp:Button ID="Button1" runat="server" Text="Login" OnClick="Button1_Click" />
               &nbsp;
               </td>
           </tr>
           

           <tr>
               <td class="auto-style2">
                   <asp:Label ID="Label1" runat="server"></asp:Label>
               </td>
           </tr>
           

           <tr>
               <td class="auto-style1">
                   &nbsp;</td>
           </tr>
           

       </table>
    
    </div>
    </form>
</body>
</html>
