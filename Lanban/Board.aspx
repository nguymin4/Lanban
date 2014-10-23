<%@ Page Title="" Language="C#" MasterPageFile="~/Lanban.Master" AutoEventWireup="true" CodeBehind="Board.aspx.cs" Inherits="Lanban.Board" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentHead" runat="server">
    <script src="Scripts/board.js"></script>
    <link href="Styles/board.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPanel" runat="server">
    <li style="background-image: url('images/logo.png')"></li>
    <li class="viewIndicator show" style="background-image: url('images/sidebar/dashboard.png')" data-view-indicator="0"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/chart.png')" data-view-indicator="1"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/column.png')" data-view-indicator="2"></li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentMain" runat="server">
    <!-- Start - Kanban board -->
    <div id="kanban" class="window view show" style="display: block;">
        <div class="title-bar">Project 1</div>
        <div class="window-content">
            <table border="1" style="border-collapse: collapse; color: white; margin: 0px 15px;">
                <colgroup style="min-width: 200px;">
                    <col />
                    <col />
                    <col />
                    <col />
                    <col />
                </colgroup>
                <tr>
                    <th>Column 1</th>
                    <th>Column 2</th>
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
                    <td id="3" class="connected" data-lane-type="1"></td>
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
    <div class="window view">
        <div class="title-bar">Board layout</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - Backlog item window -->

    <!-- Start - Task window -->
    <div class="window view">
        <div class="title-bar">Board layout</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - Task window  -->

    <!-- Error diaglog -->
    <div class="window diaglog">
        <div class="title-bar">Error</div>
        <div class="diaglog-content">
            Cannot drop that item because it is not the same type with the items in column.
                    <input type="button" class="btnOK" value="OK" onclick="document.getElementsByClassName('diaglog show')[0].setAttribute('class', 'window diaglog');" />
        </div>
    </div>
</asp:Content>
