<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Lanban.Login" %>

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
                            <input type="text" /></td>
                    </tr>
                    <tr>
                        <td class="label">Password:</td>
                        <td>
                            <input type="text" /></td>
                    </tr>
                </table>
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
