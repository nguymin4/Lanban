<%@ Page Title="Lanban - Projects" Language="C#" MasterPageFile="~/Lanban.Master" AutoEventWireup="true" CodeBehind="Project.aspx.cs" Inherits="Lanban.Project" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHead" runat="server">
    <script src="Scripts/project.js"></script>
    <link href="Styles/project.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPanel" runat="server">
    <li class="viewIndicator show" style="background-image: url('images/logo.png')" data-view-indicator="0"></li>
    <li style="background-image: url('images/sidebar/add_project.png')" onclick="openAddProjectWindow(1)"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/schedule.png')" data-view-indicator="1"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/share.png')" data-view-indicator="2"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/setting.png')" data-view-indicator="3"></li>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="contentMain" runat="server">
    <!-- Start - The window for Project browser -->
    <div class="window view show" style="display: block;">
        <div class="title-bar">Project browser</div>
        <div class="window-content">
            <!-- List of all project -->
            <asp:Panel ID="projectbrowser" runat="server"></asp:Panel>
            <!-- End list of all project -->

            <!-- Start - Project search and filter -->
            <div id="projectfilter" class="right-window-content show">
                <div id="txtSearch">
                    <img src="images/sidebar/search.png" width="30" height="30" title="Back to Search" onclick="openAddProjectWindow(0)" />
                    <input name="txtSearch" type="text" placeholder="Search..." />
                </div>
                <div>
                    <h4>Sort by: </h4>
                    <div class="filter">Name</div>
                    <div class="filter">Priority</div>
                    <div class="filter">Date created</div>
                    <div class="filter">Date modified</div>
                </div>
                <div>
                    <h4>Filter by: </h4>
                </div>
            </div>
            <!-- End - Project search and filter -->
            <!-- Start - Add new project - Hidden when filter project is shown -->
            <div id="addproject" class="right-window-content">
                <img src="images/sidebar/back.png" style="width: 24px; height: 24px; cursor: pointer;" onclick="openAddProjectWindow(0)" />
                <h3>Add new project</h3>
            </div>
            <!-- End - Add new project - -->
        </div>
    </div>
    <!-- End - The window for Project browser  -->
    <!-- Start - The window for Schedule-->
    <div class="window view">
        <div class="title-bar">Schedule</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - The window for Schedule  -->
    <!-- Start - The window for Share -->
    <div class="window view">
        <div class="title-bar">Share</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - The window for Setting  -->
    <!-- Start - The window for Setting -->
    <div class="window view">
        <div class="title-bar">Setting</div>
        <div class="window-content">
        </div>
    </div>
    <!-- End - The window for Setting  -->

</asp:Content>
