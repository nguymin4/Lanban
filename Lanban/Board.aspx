<%@ Page Title="" Language="C#" MasterPageFile="~/Lanban.Master" AutoEventWireup="true" CodeBehind="Board.aspx.cs" Inherits="Lanban.Board" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHead" runat="server">
    <script src="Scripts/board.js"></script>
    <link href="Styles/board.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPanel" runat="server">
    <li style="background-image: url('images/logo.png')" onclick="window.location.href = 'Project.aspx'"></li>
    <li class="viewIndicator show" style="background-image: url('images/sidebar/dashboard.png')" data-view-indicator="0"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/chart.png')" data-view-indicator="1"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/column.png')" data-view-indicator="2"></li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentMain" runat="server">
    <asp:ScriptManager runat="server"></asp:ScriptManager>
    <!-- Start - Kanban board -->
    <div id="kanbanWindow" class="window view show" style="display: block;">
        <div class="title-bar">Project 1</div>
        <div class="window-content">
            <table id="kanban" border="1">
                <colgroup></colgroup>
                <asp:UpdatePanel runat="server" ID="uplKanban">
                    <ContentTemplate>
                        <asp:Panel ID="panelKanban" runat="server"></asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </table>
        </div>
    </div>
    <!-- End - Kanban board -->

    <!-- Start - Chart -->
    <div class="window view">
        <div class="title-bar">Chart</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - Chart  -->

    <!-- Start - The window for Board layout-->
    <div class="window view">
        <div class="title-bar">Board layout</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - The window for Board layout  -->

    <!-- Start - Backlog item window-->
    <div id="backlogWindow" class="window view">
        <div class="title-bar">Add backlog item</div>
        <asp:UpdatePanel runat="server" ID="uplAddBacklog">
            <ContentTemplate>
                <div class="window-content">
                    <div class="panelAdd-left">
                        Title:
                        <asp:TextBox CssClass="inputTitle" runat="server" ID="txtBacklogTitle"></asp:TextBox>
                        <br />
                        Description:
                        <asp:TextBox CssClass="inputDescription" runat="server" TextMode="MultiLine" ID="txtBacklogDescription"></asp:TextBox>
                        
                    </div>
                    <div class="panelAdd-right">
                        <table class="tblAddData">
                            <tr>
                                <td>Complexity:</td>
                                <td><asp:DropDownList runat="server" ID="ddlBacklogComplexity" ClientIDMode="Static"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>Color</td>
                                <td><asp:DropDownList runat="server" ID="ddlBacklogColor" ClientIDMode="Static"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td>Start date:</td>
                                <td><asp:TextBox runat="server" ID="txtBacklogStart" Enabled="false" ClientIDMode="Static"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>End date:</td>
                                <td><asp:TextBox runat="server" ID="txtBacklogEnd" Enabled="false" ClientIDMode="Static"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td>
                                    <asp:Button runat="server" ID="btnAddBacklog" ClientIDMode="Static" CssClass="button medium btnSave" Text="Add" />
                                    <input type="button" class="button medium btnCancel" value="Close" onclick="hideWindow('backlogWindow')" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

    </div>
    <!-- End - Backlog item window -->

    <!-- Start - Task window -->
    <div id="taskWindow" class="window view">
        <div class="title-bar">Add new task</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - Task window  -->
   
     <!-- Error diaglog -->
    <div class="window diaglog">
        <div class="title-bar">Error</div>
        <div class="diaglog-content">
            Cannot drop that item because it is not the same type with the items in column.
            <input type="button" class="btnOK" value="OK" />
        </div>
    </div>
</asp:Content>
