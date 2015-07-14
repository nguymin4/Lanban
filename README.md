Lanban
======
<h3>Kanban board web application for Lahti University of Applied Sciences (LAMK)</h3>
<p>This web application development is a visual management tool which was developed as OSS and dedicated to LAMK. It can be used to control the flow of teamwork. With the help of this tool, team is able to see who is working for which task, thus enable better visual communication over team’s overall work in a time box. Further the tool would be enabling better visibility to see how the work is progressing over time and illustrating received business value that is created in a project.</p></br>
<p><strong>Value of the application:</strong> LAMK project teams can utilize agile methodology with lean approach. Electronic Kanban board
would bring visibility to not only current work and team level but also to individual level: who is doing
what and how much “points” have been earned within a sprint. Team leaders or “scrum masters”
could see directly how efficiently the team is working and where to find the obstacles or barriers for
working. The board also works as basis for every day standup meeting content (Show-and-tell)</p>
<img src="https://github.com/nguymin4/Lanban/blob/master/Lanban/Uploads/Project_5/screenshot.jpg" />
<br />
<img src="https://github.com/nguymin4/Lanban/blob/master/Lanban/images/Chart-Demo.png" />
<br />
<img src="https://github.com/nguymin4/Lanban/blob/master/Lanban/images/project.png" />


<h2>Main Feature</h2>
1) Working kanban board (default 5 swimlanes). 
Possible to add custom swimlanes and modifying them.

2) Graphical charts and tracking data to sprint
- Illustrate Burndown data chart (Hours remaining)
- Illustrate tasks done by persons pie chart
- Illustrate Points earned by persons
- Illustrate Burn up data chart (Business value cumulated)
- Illustrate Team estimation factor

3) Attaching documents and comments to tasks in kanban board
- Multiple uploading files with various file types
- Real-time chatting in comments section
 
4) Supporting multiple project with multiple user

5) User's account management (Display name, Profile picture, Password)

<h2>Platform and Technology</h2>
The project is developed in ASP.NET with C#

<strong>Prerequisites - Server side:</strong></br>
<ul>
    <li>The server support ASP.NET with .NET framework 4 and later</li>
    <li>SQL Server or SQL Azure for the database.</li>
</ul>

<strong>Prerequisites - Client side:</strong></br>
<ul>
    <li>Client desktop/laptop with internet connection.</li>
    <li>Browsers: Internet Explorer 10+, Firefox 20+, Chrome 20+</li>
</ul>

<h2>Database instruction</h2>
<h4>List of tables</h4>
<table>
    <tr>
        <th>&nbsp;</th>
        <th>Name</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>1</td>
        <td>Users</td>
        <td>Stores information of the user</td>
    </tr>
    <tr>
        <td>2</td>
        <td>Project</td>
        <td>Stores information of the project</td>
    </tr>
    <tr>
        <td>3</td>
        <td>Swimlane</td>
        <td>Stores information of swimlane in a project</td>
    </tr>
    <tr>
        <td>4</td>
        <td>Backlog</td>
        <td>Stores information of backlog in a project</td>
    </tr>
    <tr>
        <td>5</td>
        <td>Task</td>
        <td>Stores information task in a project</td>
    </tr>
    <tr>
        <td>6</td>
        <td>Project_Supervisor</td>
        <td>Store supervisor list of project</td>
    </tr>
    <tr>
        <td>7</td>
        <td>Project_User</td>
        <td>Store member list of project</td>
    </tr>
    <tr>
        <td>8</td>
        <td>Backlog_User</td>
        <td>Store data of a user who is assigned to a backlog</td>
    </tr>
    <tr>
        <td>9</td>
        <td>Task_User</td>
        <td>Store data of a user who is assigned to a backlog</td>
    </tr>
    <tr>
        <td>10</td>
        <td>Task_Comment</td>
        <td>Store comment of a task</td>
    </tr>
    <tr>
        <td>11</td>
        <td>Task_File</td>
        <td>Store attachment’s information of a task</td>
    </tr>
</table>
		

<h4>Setup and configuration</h4>
<ul>
    <li>Check <a href="https://github.com/nguymin4/Lanban/blob/master/DatabaseSchema.sql" target="_blank">DatabaseSchema.sql</a> file for more details of the schema and script to create new database.</li>
    <li>Create your custom new account in <strong>Users</strong> table</li>
    <li>Setup the <strong>connection string</strong> accordingly in file Query.cs</li>
</ul>


<h2>Preferences</h2>
List of used C# libraries:
<ol>
    <li>JSON.Net</li>
    <li>SignalR 2.1.1 (Server)</li>
</ol>

List of used Javascript libraries
<ol>
    <li>JQuery 2.1.1</li>
    <li>JQuery UI</li>
    <li>SignalR 2.1.2 (Client)</li>
    <li>Perfect scrollbar</li>
    <li>Chart.JS</li>
    <li>JustGage (and Raphael)</li>
    <li>JCrop</li>
</ol>

<h2>Author</h2>
<ul>
    <li>Minh Son Nguyen (minh.son.nguyen.1209@gmail.com)</li>
</ul>
