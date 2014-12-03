<%@ Page Title="Lanban - Projects" Language="C#" MasterPageFile="~/Lanban.Master" AutoEventWireup="true" CodeBehind="Project.aspx.cs" Inherits="Lanban.Project" Async="true" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentHead" runat="server">
    <script src="Scripts/jquery.Jcrop.min.js"></script>
    <link href="Styles/jquery.Jcrop.min.css" rel="stylesheet" />
    <script src="Scripts/project.js"></script>
    <link href="Styles/project.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="contentPanel" runat="server">
    <li class="viewIndicator show" style="background-image: url('images/logo.png')" data-view-indicator="0"></li>
    <li style="background-image: url('images/sidebar/add_project.png')" onclick="openAddProjectWindow(1)"></li>
    <li class="viewIndicator" style="background-image: url('images/sidebar/schedule.png')" data-view-indicator="1"></li>
    <li class="viewIndicator" style="display: none;" data-view-indicator="2"></li>
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
                            <img src="images/sidebar/open.png" class="project-detail-button" id="btnOpenProject" title="Open" />
                            <img src="images/sidebar/edit_note.png" class="project-detail-button" id="btnEditProject" title="Edit" />
                            <img src="images/sidebar/delete_note.png" class="project-detail-button" id="btnDeleteProject" title="Delete" />
                            <img src="images/sidebar/share.png" class="project-detail-button" id="btnShareProject" title="Share" />
                            <img src="images/sidebar/quit.png" class="project-detail-button" id="btnQuitProject" title="Quit" />
                        </div>
                        <div id="project-supervisor">
                            <div class="criteria project-field">Supervisor</div>
                            <div class="project-data"></div>
                        </div>
                    </div>
                    <div id="projectdetail-right">
                        <div id="projectdetail-name"></div>
                        <div id="projectdetail-description"></div>
                        <div id="project-owner">
                            <div class="criteria project-field">Owner</div>
                            <div class="project-data"></div>
                        </div>
                        <div id="project-date">
                            <div class="criteria project-field">Date</div>
                            From: <span id="projectStartDate"></span>
                            <br />
                            <span id="projectEndDate"></span>
                        </div>
                        <div class="clear"></div>
                    </div>
                    <div id="projectdetail-close" onclick="hideProjectDetail()"></div>
                </div>
            </asp:Panel>

            <!-- End list of all project -->
            <!-- Start - Project search and filter -->
            <div id="projectfilter" class="right-window-content show">
                <div class="txtSearch">
                    <div class="searchIcon"></div>
                    <input class="txtSearchBox" name="txtSearchProject" type="text"
                        placeholder="Search..." onkeyup="browseProject(this)" />
                </div>
                <div>
                    <h4>Search by: </h4>
                    <div class="criteria" data-criteria="1">Name</div>
                    <div class="criteria" data-criteria="2">Owner</div>
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
                Name:
                <input type="text" class="input-project" id="txtProjectName" />
                Description:
                <textarea class="input-project" id="txtProjectDescription" spellcheck="false"></textarea>
                Start date:
                <input type="text" class="input-project" id="txtProjectStartDate" placeholder="dd.mm.yyyy" /><br />
                Supervisor:
                <input type="text" class="input-project" id="txtSupervisor"
                    onkeyup="searchUser(this, 2)" onblur="clearResult(this)" />
                <div id="projectSupervisor"></div>
                <img src="images/sidebar/add_project.png" class="project-detail-button"
                    id="btnAddProject" title="Add new project" onclick="addProject()" />
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
    <div id="sharingWindow" class="window view">
        <div class="title-bar">Share</div>
        <div class="window-content">
            <div class="left-pane">
                <div id="userListOverlay"><span>Drop here</span></div>
                <div class="user-list-connected" id="userList"></div>
            </div>
            <div class="right-pane">
                <div class="txtSearch">
                    <div class="searchIcon"></div>
                    <input class="txtSearchBox" id="txtSearchMember" type="text" placeholder="Search..." onkeyup="searchUser(this, 1)" onblur="clearResult(this)" />
                </div>
                <div class="user-list-connected" id="tempUserList"></div>
            </div>
        </div>
    </div>
    <!-- End - The window for Share  -->

    <!-- Start - The window for Account management -->
    <div class="window view">
        <div class="title-bar">Account management</div>
        <div class="window-content">
            <div id="accountManagement">
                <table>
                    <tr>
                        <td>Full name:</td>
                        <td><input id="txtAccFullname" type="text" class="input-project" disabled /></td>
                    </tr>
                    <tr>
                        <td>Current password:</td>
                        <td><input id="txtCurrentPassword" type="password" class="input-project" /></td>
                    </tr>
                    <tr>
                        <td>New password:</td>
                        <td><input id="txtNewPassword" type="password" class="input-project" /></td>
                    </tr>
                    <tr>
                        <td>Repeat new password:</td>
                        <td><input id="txtRepeatPassword" type="password" class="input-project" /></td>
                    </tr>
                </table>
                <div id="fileUploadContainer">
                    <img src="images/sidebar/attach.png" />
                    <div id="inputFileName"></div>
                    <input id="inputUploadFile" type="file" onchange="uploadTempProfile(this)" />
                </div>
                <div class="loading-spinner" style="display: none"></div>
            </div>
            <img src="#" id="tempProfileImg"/>
            <div id="accManaPanel">
                <input type="button" class="button medium btnSave" value="Save" onclick="saveAccountChange()" />
                <input type="button" class="button medium btnCancel" value="Cancel" onclick="cancelAccountChange()" />
            </div>
        </div>
    </div>
    <!-- End - The window for Account management  -->

    <!-- Other elements  -->
    <div id="searchContainer"></div>

</asp:Content>
