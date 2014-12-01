<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Lanban.Login" ClientIDMode="Static" %>

<html>
<head runat="server">
    <title>Login - Welcome to Lanban</title>
    <script src="Scripts/jquery-2.1.1.min.js"></script>
    <script src="Scripts/login.js"></script>
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
                <input type="button" runat="server" id="btnLRegister" class="button" value="Register" onclick="openRegister(true)" />
            </div>
            <div id="register" class="hidden">
                <table>
                    <tr>
                        <td class="label">Fullname:</td>
                        <td><asp:TextBox runat="server" ID="txtFullname"></asp:TextBox></td>
                        <td><span id="validateFullname" class="validator"></span></td>
                    </tr>
                    <tr>
                        <td class="label">Username:</td>
                        <td><asp:TextBox runat="server" ID="txtRUsername"></asp:TextBox></td>
                        <td><span id="checkUsername" class="validator"></span></td>
                    </tr>
                    <tr>
                        <td class="label">Password:</td>
                        <td><asp:TextBox runat="server" ID="txtRPassword" TextMode="Password"></asp:TextBox></td>
                        <td><span id="validatePassword" class="validator"></span></td>
                    </tr>
                    <tr>
                        <td class="label">Repeat Password:</td>
                        <td><asp:TextBox runat="server" ID="txtRPasswordRepeat" TextMode="Password"></asp:TextBox></td>
                        <td><span id="checkPassword" class="validator"></span></td>
                    </tr>
                </table>
                <asp:Label runat="server" ID="lblStatus" CssClass="required"></asp:Label>
                <input type="button" runat="server" class="button"  onclick="registerUser()" value="Register"/>
                <input type="button" class="button" onclick="openRegister(false)" value="Cancel" />
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
