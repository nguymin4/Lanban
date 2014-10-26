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
                <tr>
                    <th>
                        <img src="images/sidebar/add_item.png" class="btnAddItem" onclick="showWindow('backlogWindow')" />
                        Column 1
                    </th>
                    <th>
                        <img src="images/sidebar/add_item.png" class="btnAddItem" onclick="showWindow('backlogWindow')" />
                        Column 2
                    </th>
                    <th>Column 3</th>
                    <th>Column 4</th>
                    <th>Column 5</th>
                </tr>
                <tr>
                    <td id="1" class="connected" data-lane-type="1">
                        <div class="note">1</div>
                        <div class="note">2</div>
                        <div class="note">3</div>
                        <div class="note">4</div>
                        <div class="note">5</div>
                        <div class="note">6</div>
                    </td>
                    <td id="2" class="connected" data-lane-type="1"></td>
                    <td id="3" class="connected" data-lane-type="0"></td>
                    <td id="4" class="connected" data-lane-type="0"></td>
                    <td id="5" class="connected" data-lane-type="0"></td>
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
    <div class="window view">
        <div class="title-bar">Add new task</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - Task window  -->

    <%-- Hidden textboxes contains Project ID and Swimlane ID 
    when you create a new sticky note for a swimlane or click on a sticky note --%>
    <asp:TextBox CssClass="inputDescription" runat="server" Visible="false" ID="txtProjectID"></asp:TextBox>
    <asp:TextBox CssClass="inputDescription" runat="server" Visible="false" ID="txtSwimlaneID"></asp:TextBox>
   
     <!-- Error diaglog -->
    <div class="window diaglog">
        <div class="title-bar">Error</div>
        <div class="diaglog-content">
            Cannot drop that item because it is not the same type with the items in column.
            <input type="button" class="btnOK" value="OK" />
        </div>
    </div>
</asp:Content>
