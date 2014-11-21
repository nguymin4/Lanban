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
    <div id="projectWindow" class="window view show" style="display: block;">
        <div class="title-bar">Project browser</div>
        <div class="window-content">
            <!-- List of all project -->
            <asp:Panel ID="projectbrowser" runat="server">
                <div id="projectdetail">
                    <div id="projectdetail-left">
                        <img id="screenshot" src="images/screenshot.jpg" />
                        <div id="projectdetail-panel">
                            <div class="project-detail-button" id="btnOpenProject">Open</div>
                            <div class="project-detail-button" id="btnDeleteProject">Edit</div>
                            <div class="project-detail-button" id="btnEditProject">Delete</div>
                        </div>
                        <div id="project-supervisor">
                            <div class="criteria project-field">Supervisor</div>
                            <div class="project-data"></div>
                        </div>
                    </div>
                    <div id="projectdetail-right">
                        <div id="projectdetail-name"></div>
                        <div id="projectdetail-description"></div>
                        <div id="project-date">
                            <div class="criteria project-field">Date</div>
                            From: <span id="txtProjectStartDate"></span>
                            <br />
                            <span id="txtProjectEndDate"></span>
                        </div>
                        <div id="project-owner">
                            <div class="criteria project-field">Owner</div>
                            <div class="project-data">
                                <div class="person">
                                    <img class="person-avatar" src="images/sidebar/profile.png" />
                                    <div class="person-name">Minh Son Nguyen</div>
                                </div>
                            </div>
                        </div>
                        <div class="clear"></div>
                    </div>
                    <div id="projectdetail-close" onclick="hideProjectDetail()"></div>
                </div>
            </asp:Panel>
            
            <!-- End list of all project -->
            <!-- Start - Project search and filter -->
            <div id="projectfilter" class="right-window-content show">
                <div id="txtSearch">
                    <div title="Back to Search" onclick="openAddProjectWindow(0)"></div>
                    <input name="txtSearch" type="text" placeholder="Search..." />
                </div>
                <div>
                    <h4>Search by: </h4>
                    <div class="criteria">Name</div>
                    <div class="criteria">Priority</div>
                    <div class="criteria">Date created</div>
                    <div class="criteria">Date modified</div>
                </div>
                <div>
                    <h4>Sort by: </h4>
                </div>
            </div>
            <!-- End - Project search and filter -->
            <!-- Start - Add new project - Hidden when filter project is shown -->
            <div id="addproject" class="right-window-content">
                <div id="backProjectSearch" onclick="openAddProjectWindow(0)"></div>
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
