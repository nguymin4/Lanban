<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Lanban.WebForm1" %>

<html>
<head>
    <title></title>
    <script src="Scripts/jquery-2.1.1.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.mousewheel.js" type="text/javascript"></script>
    <script src="Scripts/perfect-scrollbar.js" type="text/javascript"></script>
    <link href="Styles/perfect-scrollbar.css" rel="stylesheet" />
    <script src="Scripts/main.js" type="text/javascript"></script>
    <link href="Styles/main.css" rel="stylesheet" />
    <style type="text/css">
        /*Project only*/
        .view {
            height: 90%;
        }

        #projectbrowser {
            width: 60%;
            float: left;
            border-right: 1px solid gray;
            margin-left: 2%;
            position: relative;
            overflow: hidden;
            height: 100%;
        }

        .right-window-content {
            margin-left: 2%;
            width: 33%;
            float: left;
            transition: left 1s ease;
            position: relative;
            left: 50%;
        }

            .right-window-content.show {
                left: 0;
            }

        #txtSearch {
            background: rgba(25, 15, 15, 0.5);
            height: 28px;
            min-width: 200px;
            padding: 2px 5px;
            border-radius: 6px;
            border: solid 1px rgb(128, 128, 128);
            margin-bottom: 15px;
        }

        input[name="txtSearch"] {
            position: relative;
            top: -10px;
            border: none;
            background: transparent;
            color: gray;
            font-size: 16px;
            font-style: italic;
        }

            input[name="txtSearch"]:focus {
                color: #e6e6e6;
            }

        .filter {
            display: inline-block;
            height: 30px;
            line-height: 30px;
            background: rgba(25, 15, 15, 0.5);
            border-radius: 5px;
            padding: 3px 15px;
            margin: 10px;
        }

        /*Project.html only*/
        .board {
            background-color: rgba(25, 15, 15, 0.79);
            width: 200px;
            height: 200px;
            border-radius: 6px;
            color: #e0dfdf;
            padding: 10px;
            cursor: pointer;
            margin: auto 40px 40px auto;
            float: left;
        }
    </style>
</head>
<body>
    <form id="container" runat="server">
        <div id="sidebar">
            <!-- Start - Icon can be changed -->
            <ul id="panel">
                <li class="viewIndicator show" style="background-image: url('images/logo.png')" data-view-indicator="0"></li>
                <li style="background-image: url('images/sidebar/add_project.png')" onclick="openAddProjectWindow(1)"></li>
                <li class="viewIndicator" style="background-image: url('images/sidebar/schedule.png')" data-view-indicator="1"></li>
                <li class="viewIndicator" style="background-image: url('images/sidebar/share.png')" data-view-indicator="2"></li>
                <li class="viewIndicator" style="background-image: url('images/sidebar/setting.png')" data-view-indicator="3"></li>
            </ul>
            <!-- End - Icon can be changed -->
            <div id="info">
                <img id="profile" src="images/sidebar/profile.png" title="constantine_lazarus" />
                <button title="Log out" class="sidebar_icon_med" style="display: block; background: url('images/sidebar/logout.png') no-repeat center transparent;"></button>
            </div>
        </div>
        <!-- Content Editable -->
        <div id="content">
            <!-- Start - The window for Project browser -->
            <div class="window view show" style="display: block;">
                <div class="title-bar">Project browser</div>
                <div class="window-content">
                    <!-- List of all project -->
                    <div id="projectbrowser">
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                        <div class="board" onclick="window.location.href = 'board.html'">
                            <strong>Project 1
                            </strong>
                        </div>
                    </div>
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
                        <img src="images/sidebar/back.png" style="width: 24px; height: 24px" onclick="openAddProjectWindow(0)" />
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
        </div>
        <!-- End content Editable -->
        <!-- recalibrate the panel => some part can be reused in other files => need refactoring -->
        <script type="text/javascript">
            function recalibrate() {
                var windowHeight = window.innerHeight || document.documentElement.clientHeight;
                var windowWidth = window.innerWidth || document.documentElement.clientWidth;
                /*Recalibrate sidebar*/
                document.getElementById("sidebar").style.height = windowHeight;
                document.getElementById("panel").style.height = parseInt(windowHeight) - 120;

                /*Recalibrate other elements*/
                document.getElementById("content").style.width = windowWidth - 61;
                document.getElementsByClassName("view")[0].style.width = 0.9 * (windowWidth - 61);
            }

            $(document).ready(function () {
                /*Add customized scroll bar*/
                $("#projectbrowser").perfectScrollbar({
                    wheelSpeed: 20,
                    wheelPropagation: false,
                });
            });

            function openAddProjectWindow(index) {
                showView(0);
                var rightContent = document.getElementsByClassName("right-window-content");
                if (index == 1) {
                    rightContent[0].setAttribute("class", "right-window-content");
                    setTimeout(function () {
                        rightContent[0].style.display = "none";
                        rightContent[1].setAttribute("class", "right-window-content show");
                    }, 500);
                }
                else {
                    rightContent[1].setAttribute("class", "right-window-content");
                    setTimeout(function () {
                        rightContent[0].style.display = "block";
                    }, 450);
                    setTimeout(function () {
                        rightContent[0].setAttribute("class", "right-window-content show");
                    }, 500);
                }
            }
        </script>
    </form>
</body>
</html>
