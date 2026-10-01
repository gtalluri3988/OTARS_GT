<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="MTAoarsGeneral.Web.Reports.Display" %>
<%@ Register TagPrefix="Microsoft" Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <asp:ScriptManager ID="scriptManager" runat="server"></asp:ScriptManager>
    <div>
        <Microsoft:ReportViewer id="reportViewer" runat="server" Width="100%" Height="500"/>
    </div>
    </form>
</body>
</html>
