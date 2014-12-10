<%@ Page Title="" Language="C#" MasterPageFile="~/Lanban.Master" AutoEventWireup="true" CodeBehind="Board.aspx.cs" Inherits="Lanban.Board" Async="true" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHead" runat="server">
    <script src="Scripts/html2canvas.min.js"></script>
    <script async="async" src="Scripts/Chart.min.js"></script>
    <script src="Scripts/board.js"></script>
    <link href="Styles/board.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPanel" runat="server">
    <li style="background-image: url('/images/logo.png')" runat="server" onclick="loadPageSpinner(); __doPostBack('RedirectProject','');"></li>
    <li class="viewIndicator show" style="background-image: url('/images/sidebar/dashboard.png')" data-view-indicator="0"></li>
    <li class="viewIndicator" style="background-image: url('/images/sidebar/chart.png')" data-view-indicator="1" onclick="showChartWindow()"></li>
    <li class="viewIndicator" style="background-image: url('/images/sidebar/column.png')" data-view-indicator="2" onclick="showSwimlaneWindow()"></li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentMain" runat="server">
    <!-- Start - Kanban board -->
    <div id="kanbanWindow" class="window view show" style="display: block;">
        <div class="title-bar">
            <asp:Label ID="lblProjectName" runat="server">Project</asp:Label>
        </div>
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

            <div id="gaugeEstimationFactor"></div>
            <table id="gaugeTable" border="1">
                <tr>
                    <td>Estimation (h)</td>
                    <td>Actual (h)</td>
                </tr>
                <tr>
                    <td id="txtGaugeEst"></td>
                    <td id="txtGaugeAct"></td>
                </tr>
            </table>

            <div class="chart-box">
                <h3>Number of Tasks assigned
                    <br />
                    by Person during Sprint</h3>
                <div class="loading-spinner"></div>
                <canvas class="chart" id="chartPie" width="300" height="300"></canvas>
            </div>

            <div class="chart-box">
                <h3>Number of estimation hour
                    <br />
                    by Person during Sprint</h3>
                <div class="loading-spinner"></div>
                <canvas class="chart" id="chartBar" width="500" height="300"></canvas>
            </div>

            <div class="chart-box">
                <h3>Burn-up Chart </h3>
                <div class="filter-container">
                    From:
                    <asp:TextBox runat="server" ID="txtBUFrom" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                    To:
                    <asp:TextBox runat="server" ID="txtBUTo" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                    <input type="button" id="btnBUFilter" class="medium button" value="Filter" onclick="filterBurnUp()" />
                </div>

                <div class="loading-spinner"></div>
                <canvas class="chart" id="burnupChart" width="500" height="300"></canvas>
            </div>

            <div class="chart-box">
                <h3>Burn-down Chart</h3>
                <div class="filter-container">
                    From:
                    <asp:TextBox runat="server" ID="txtBDFrom" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                    To:
                    <asp:TextBox runat="server" ID="txtBDTo" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                    <input type="button" id="btnBDFilter" class="medium button" value="Filter" onclick="filterBurnDown()" />
                </div>

                <div class="loading-spinner"></div>
                <canvas class="chart" id="burndownChart" width="500" height="300"></canvas>
            </div>
        </div>
    </div>
    <!-- End - Chart  -->

    <!-- Start - The window for Board layout-->
    <div id="boardLayout" class="window view">
        <div class="title-bar">Swimlane management</div>
        <div class="window-content">
            <table id="addSwimlane">
                <tr>
                    <td>Name</td>
                    <td>Data type</td>
                    <td>Data status</td>
                </tr>
                <tr>
                    <td>
                        <input class="input-sw txtName" type="text" /></td>
                    <td>
                        <select class="input-sw ddlType">
                            <option value="1">Backlog</option>
                            <option value="2">Task</option>
                        </select>
                    </td>
                    <td>
                        <select class="input-sw ddlDataStatus">
                            <option value="Standby">Standby</option>
                            <option value="Ongoing">Ongoing</option>
                        </select>
                    </td>
                </tr>
            </table>
            <input id="btnAddSw" type="button" class="button medium btnSave" value="Add" onclick="addSwimlane()" />
            <input id="btnCancelSw" type="button" class="button medium btnCancel" value="Close" onclick="showView(0); resetSwForm()" />
            <hr />
            <ul id="currentSwimlane"></ul>
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
                    <input type="text" id="txtbacklogAssignee" class="inputAssignee"
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
                                <asp:TextBox runat="server" ID="txtBacklogStart" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Completion date:</td>
                            <td>
                                <asp:TextBox runat="server" ID="txtBacklogComplete" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                            </td>
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
                <div class="panelAdd-left" style="width: 460px;">
                    <h3>Tasks in this backlog</h3>
                    <table id="tblTaskBacklog">
                        <colgroup>
                            <col style="width: 65px;" />
                            <col style="width: 298px;" />
                            <col style="width: 65px;" />
                            <col style="width: 32px;" />
                        </colgroup>
                        <thead>
                            <tr style="background: rgba(25, 15, 15, 0.49)">
                                <th>ID</th>
                                <th>Title</th>
                                <th>Status</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
                <div class="panelAdd-right" style="width: 300px;">
                    <h3>Other info</h3>
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
                            <td>Color:</td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlTaskColor"></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>Work estimation:</td>
                            <td>
                                <asp:TextBox CssClass="inputBox" runat="server" ID="txtTaskWorkEstimation"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Due date:</td>
                            <td>
                                <asp:TextBox runat="server" ID="txtTaskDueDate" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Completion date:</td>
                            <td>
                                <asp:TextBox runat="server" ID="txtTaskCompletionDate" CssClass="inputBox date" placeholder="dd.mm.yyyy"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>Actual hour:</td>
                            <td>
                                <asp:TextBox CssClass="inputBox" runat="server" ID="txtTaskActualWork"></asp:TextBox>
                            </td>
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
                <div class="panelAdd-left" style="width: 460px;">
                    <h3>Comment</h3>
                    <div id="commentBox"></div>
                    <!-- Add your comment -->
                    <div class="comment-box" style="padding: 10px;">
                        <div class="comment-panel">
                            <img src="images/sidebar/profile.png" class="comment-profile" id="myCommentProfile" />
                        </div>
                        <asp:TextBox CssClass="inputBox" runat="server" TextMode="MultiLine" ID="txtTaskComment"></asp:TextBox>
                        <input type="button" id="btnSubmitComment" value="Send" onclick="submitTaskComment()" />
                    </div>
                </div>

                <div class="panelAdd-right" style="width: 340px">
                    <h3>Document</h3>
                    <div id="fileUploadContainer">
                        <img src="images/sidebar/attach.png" />
                        <img title="Upload" src="images/sidebar/upload.png" onclick="startUploadFile()" />
                        <div id="inputFileName"></div>
                        <input id="inputUploadFile" type="file" multiple="multiple" onchange="getChosenFileName(this)" />
                    </div>
                    <hr />
                    <div id="uploadProgressContainer">
                        <div id="uploadProgress"></div>
                    </div>
                    <div id="fileList"></div>
                </div>
            </div>
            <!-- End - Window content page 2 -->

        </div>
    </div>
    <!-- End - Task window  -->

    <!-- Box display search result of assignee for an item by name -->
    <div id="assigneeSearchResult"></div>

    <!-- Box display statistic of all tasks belong to a backlog -->
    <div id="diaglogBacklogStat" style="display: none;">
        <canvas id="chartBacklogStat" width="300" height="200"></canvas>
    </div>

    <!-- Hidden field to store some temporary data -->
    <asp:TextBox runat="server" ID="txtSwimlaneID" CssClass="hidden"></asp:TextBox>
    <asp:TextBox runat="server" ID="txtSwimlanePosition" CssClass="hidden"></asp:TextBox>
</asp:Content>
