<%@ Page Title="" Language="C#" MasterPageFile="~/Lanban.Master" AutoEventWireup="true" CodeBehind="Board.aspx.cs" Inherits="Lanban.Board" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHead" runat="server">
    <script src="Scripts/board.js"></script>
    <link href="Styles/board.css" rel="stylesheet" />
    <script src="Scripts/Chart.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPanel" runat="server">
    <li style="background-image: url('images/logo.png')" onclick="window.location.href = 'Project.aspx'"></li>
    <li class="viewIndicator show" style="background-image: url('images/sidebar/dashboard.png')" data-view-indicator="0"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/chart.png')" data-view-indicator="1" onclick="showChartWindow()"></li>
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
    <div id="chartWindow" class="window view">
        <div class="title-bar">Chart</div>
        <div class="window-content">
            <div class="chart-box">
                <h3>Number of Tasks assigned
                    <br />
                    by Person during Sprint</h3>
                <canvas class="chart" id="chartPie" width="300" height="300"></canvas>
            </div>

            <div class="chart-box">
                <h3>Number of estimation hour
                    <br />
                    by Person during Sprint</h3>
                <canvas class="chart" id="chartBar" width="500" height="300"></canvas>
            </div>

            <div class="chart-box">
                <h3>Number of estimation hour
                    <br />
                    by Person during Sprint</h3>
                <canvas class="chart" id="graphLine" width="500" height="300"></canvas>
            </div>
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
            <!-- Start - Window content page 1 -->
            <div class="page" style="display: block;">
                <div class="panelAdd-left">
                    Title:
                <asp:TextBox CssClass="inputTitle inputBox" runat="server" ID="txtBacklogTitle"></asp:TextBox>
                    Assignee:
                <div id="backlogAssign" class="boxAssign" onclick="$('#txtbacklogAssignee').trigger('focus')">
                    <input type="text" id="txtbacklogAssignee" class="inputAssignee" autocomplete="off"
                        onkeyup="searchAssignee(this, 'backlog')" onblur="clearResult(this)" />
                </div>
                    <br />
                    Description:
                <asp:TextBox CssClass="inputDescription inputBox" runat="server" TextMode="MultiLine" ID="txtBacklogDescription"></asp:TextBox>
                </div>
                <div class="panelAdd-right">
                    <table class="tblAddData">
                        <tr>
                            <td>Complexity:</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlBacklogComplexity"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>Color:</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlBacklogColor" Width="70"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>Start date:</td>
                            <td>
                                <asp:TextBox runat="server" ID="txtBacklogStart" CssClass="inputBox" Enabled="false"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>Completion date:</td>
                            <td>
                                <asp:TextBox runat="server" ID="txtBacklogComplete" CssClass="inputBox" Enabled="false"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <input type="button" class="button medium btnSave" value="Add" onclick="insertItem('backlog')" />
                                <input type="button" class="button medium btnCancel" value="Close" onclick="hideWindow('backlogWindow')" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="pageRibbon">
                    <img class="btnNextPage" src="images/sidebar/next.png" title="Next" onclick="changePageWindow('backlogWindow', 1)" />
                </div>
            </div>
            <!-- End - Window content page 1 -->
            <!-- Start - Window content page 2 -->
            <div class="page" style="left: 100%;">
                <div class="pageRibbon">
                    <img class="btnBackPage" src="images/sidebar/back.png" title="Back" onclick="changePageWindow('backlogWindow', 0)" />
                </div>
                <div class="panelAdd-left">
                    Title:
                </div>
                <div class="panelAdd-right">
                    History:
                </div>
            </div>
            <!-- End - Window content page 2 -->
        </div>
    </div>
    <!-- End - Backlog item window -->

    <!-- Start - Task window -->
    <div id="taskWindow" class="window view">
        <div class="title-bar">Add new task</div>
        <div class="window-content">
            <!-- Start - Window content page 1 -->
            <div class="page" style="display: block;">
                <div class="panelAdd-left">
                    Title:
                <asp:TextBox CssClass="inputTitle inputBox" runat="server" ID="txtTaskTitle"></asp:TextBox>
                    Assignee:
                <div id="taskAssign" class="boxAssign" onclick="$('#txttaskAssignee').trigger('focus')">
                    <input type="text" id="txttaskAssignee" class="inputAssignee" autocomplete="off"
                        onkeyup="searchAssignee(this, 'task')" onblur="clearResult(this)" />
                </div>
                    <br />
                    Description:
                <asp:TextBox CssClass="inputDescription inputBox" runat="server" TextMode="MultiLine" ID="txtTaskDescription"></asp:TextBox>
                </div>
                <div class="panelAdd-right">
                    <table class="tblAddData">
                        <tr>
                            <td>Backlog:</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlTaskBacklog" Width="200"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>Work estimation:</td>
                            <td>
                                <asp:TextBox CssClass="inputBox" runat="server" ID="txtTaskWorkEstimation"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>Color:</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlTaskColor"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>Due date:<br />
                                (dd.mm.yyyy)
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtTaskDueDate" CssClass="inputBox"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>Completion date:</td>
                            <td>
                                <asp:TextBox runat="server" ID="txtTaskCompletionDate" CssClass="inputBox" Enabled="false"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <input type="button" class="button medium btnSave" value="Add" onclick="insertItem('task')" />
                                <input type="button" class="button medium btnCancel" value="Close" onclick="hideWindow('taskWindow')" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="pageRibbon">
                    <img class="btnNextPage" src="images/sidebar/next.png" title="Next" onclick="changePageWindow('taskWindow', 1)" />
                </div>
            </div>
            <!-- End - Window content page 1 -->
            <!-- Start - Window content page 2 -->
            <div class="page" style="left: 100%;">
                <div class="pageRibbon">
                    <img class="btnBackPage" src="images/sidebar/back.png" title="Back" onclick="changePageWindow('taskWindow', 0)" />
                </div>
                <div class="panelAdd-left">
                    Title:
                </div>
                <div class="panelAdd-right">
                    History:
                </div>
            </div>
            <!-- End - Window content page 2 -->

        </div>
    </div>
    <!-- End - Task window  -->

    <!-- Error diaglog -->
    <div class="window diaglog error">
        <div class="title-bar">Error</div>
        <div class="diaglog-content">
            <div class="content-holder"></div>
            <input type="button" class="btnOK" value="OK" />
        </div>
    </div>

    <!-- Successful diaglog -->
    <div class="window diaglog success">
        <div class="title-bar">Operation successful</div>
        <div class="diaglog-content">
            <div class="content-holder"></div>
            <input type="button" class="btnOK" value="OK" />
        </div>
    </div>

    <!-- Box display search result of assignee for an item by name -->
    <div id="assigneeSearchResult"></div>

    <!-- Hidden field to store some temporary data -->
    <asp:TextBox runat="server" ID="txtProjectID" CssClass="hidden"></asp:TextBox>
    <asp:TextBox runat="server" ID="txtSwimlaneID" CssClass="hidden"></asp:TextBox>
    <asp:TextBox runat="server" ID="txtSwimlanePosition" CssClass="hidden"></asp:TextBox>
</asp:Content>
