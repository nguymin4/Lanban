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
    <!-- Start - Kanban board -->
    <div id="kanbanWindow" class="window view show" style="display: block;">
        <div class="title-bar">Project 1</div>
        <div class="window-content">
            <table id="kanban" border="1">
                <colgroup></colgroup>
                <tr>
                    <asp:Panel ID="panelKanbanHeader" runat="server"></asp:Panel>
                </tr>
                <tr>
                    <asp:Panel ID="panelKanbanBody" runat="server"></asp:Panel>
                </tr>
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
        <div class="window-content">
            <div class="panelAdd-left">
                Title:
                <asp:TextBox CssClass="inputTitle inputBox" runat="server" ID="txtBacklogTitle"></asp:TextBox>
                Assignee:
                <div id="backlogAssign" class="boxAssign" onclick="$('#txtbacklogAssignee').trigger('focus')">
                    <input type="text" id="txtbacklogAssignee" class="inputAssignee" autocomplete="off"
                        onkeyup="searchAssignee(this, 'backlog')" onblur="clearResult(this)" />
                </div>
                <div id="assigneeSearchResult"></div>
                <br />
                Description:
                <asp:TextBox CssClass="inputDescription inputBox" runat="server" TextMode="MultiLine" ID="txtBacklogDescription"></asp:TextBox>

            </div>
            <div class="panelAdd-right">
                <table class="tblAddData">
                    <tr>
                        <td>Complexity:</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlBacklogComplexity">
                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Color</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlBacklogColor">
                                <asp:ListItem Text="Red" Value="Red"></asp:ListItem>
                                <asp:ListItem Text="White" Value="White"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Start date:</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtBacklogStart" CssClass="inputBox" Enabled="false"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>End date:</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtBacklogEnd" CssClass="inputBox" Enabled="false"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <input type="button" class="button medium btnSave" value="Add" onclick="insertBacklogItem()" />
                            <input type="button" class="button medium btnCancel" value="Close" onclick="hideWindow('backlogWindow')" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
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

    <asp:TextBox runat="server" ID="txtProjectID" CssClass="hidden"></asp:TextBox>
    <asp:TextBox runat="server" ID="txtSwimlaneID" CssClass="hidden"></asp:TextBox>
    <asp:TextBox runat="server" ID="txtSwimlanePosition" CssClass="hidden"></asp:TextBox>
    <asp:TextBox runat="server" ID="txtNoteIndex" CssClass="hidden"></asp:TextBox>
</asp:Content>
