<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Lanban.Login" ClientIDMode="Static" %>

<html>
<head runat="server">
    <title>Login - Welcome to Lanban</title>
    <script src="Scripts/jquery-2.1.1.min.js"></script>
    <link href="Styles/login.css" rel="stylesheet" />
</head>
<body>
    <form id="layer" runat="server">
        <div id="left-panel">
            <div id="login">
                <table>
                    <tr>
                        <td class="label">Username:</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtUsername"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUsername" CssClass="required"
                                SetFocusOnError="true" ErrorMessage="Required"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">Password:</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtPassword" TextMode="Password"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="required"
                                SetFocusOnError="true" ErrorMessage="Required"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <asp:Label runat="server" ID="lblMsg" CssClass="required"></asp:Label>
                <asp:Button runat="server" ID="btnLogin" CssClass="button" Text="Login" OnClick="btnLogin_Click" />
            </div>
        </div>
        <div id="right-panel">
            <div id="widget">
                <embed src="http://flash-clocks.com/free-flash-clocks-blog-topics/free-flash-clock-195.swf" width="300" height="150" wmode="transparent" type="application/x-shockwave-flash" />
            </div>
        </div>
    </form>
</body>
</html>
