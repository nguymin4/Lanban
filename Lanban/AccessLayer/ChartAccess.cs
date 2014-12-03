using Lanban.Model;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading;


namespace Lanban.AccessLayer
{
    /* Working with task comments */
    public class ChartAccess: Query
    {
        string[,] colorHex = {
                                {"#ff9898", "#ffc864", "#ffff95", "#98ff98", "#caffff", "#adadff", "#d598ff" },
                                { "#ff4b4b", "#ffa500", "#ffff4b", "#4bff4b", "#80ffff", "#6464ff", "#b64bff" }
                             };

        public string getPieChart(int projectID)
        {
            myCommand.CommandText = "SELECT [Count], [Name] FROM Users INNER JOIN " +
                "(SELECT Count(*) AS [Count], User_ID FROM Task_User INNER JOIN " +
                "(SELECT Task_ID FROM Task INNER JOIN (SELECT Backlog_ID FROM Backlog WHERE Project_ID=@projectID AND Status='Ongoing') AS A ON Task.Backlog_ID = A.Backlog_ID) " +
                "AS B ON Task_User.Task_ID = B.Task_ID GROUP BY User_ID) AS C ON C.User_ID = Users.User_ID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myReader = myCommand.ExecuteReader();

            // Create a pie chart instance and then use JSON Convert to serialize it to JSON string
            PieChart myPie = new PieChart();
            int index = 0;
            while (myReader.Read())
            {
                PiePart temp = new PiePart();
                temp.value = Convert.ToInt32(myReader.GetValue(0));
                temp.color = colorHex[1, index];
                temp.highlight = colorHex[0, index];
                temp.label = myReader.GetValue(1).ToString();
                myPie.part.Add(temp);
                index++;
            }
            myCommand.Parameters.Clear();

            //Serialize and return value
            return JsonConvert.SerializeObject(myPie.part);
        }

        public string getBarChart(int projectID)
        {
            myCommand.CommandText = "SELECT [Name], [Sum] FROM Users INNER JOIN " +
                "(SELECT User_ID, SUM(Work_estimation) AS [Sum] FROM Task_User INNER JOIN " +
                "(SELECT Task_ID, Work_estimation FROM Task INNER JOIN (SELECT Backlog_ID FROM Backlog WHERE Project_ID=@projectID AND Status='Ongoing') AS A " +
                "ON Task.Backlog_ID = A.Backlog_ID WHERE Task.Status='Done' OR Task.Status='Ongoing') " +
                "AS B ON Task_User.Task_ID = B.Task_ID GROUP BY User_ID) AS C ON C.User_ID = Users.User_ID";
            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myAdapter.Fill(myDataSet, "temp");
            var table = myDataSet.Tables["temp"];
            myCommand.Parameters.Clear();

            BarChart myBar = new BarChart();
            var labels = myBar.labels;

            // Random color - not necessary can be removed.
            int colorIndex = new Random().Next(0, 6);
            BarChartDataset barDataset = new BarChartDataset(colorHex[1, colorIndex], colorHex[0, colorIndex]);
            barDataset.label = "";

            // Name of the person and Work estimation
            foreach (DataRow temp in table.Rows)
            {
                labels.Add(temp["Name"].ToString());
                barDataset.data.Add(Convert.ToInt32(temp[1]));
            }
            myBar.datasets.Add(barDataset);
            return JsonConvert.SerializeObject(myBar);
        }

        public string getLineGraph(int projectID)
        {
            myCommand.CommandText = "SELECT [Completion_date], SUM(Work_estimation) FROM Task_User INNER JOIN " +
                "(SELECT Task_ID, Work_estimation, Completion_date FROM Task WHERE Project_ID=@projectID AND Task.Status='Done') AS A ON Task_User.Task_ID = A.Task_ID " +
                "GROUP BY [Completion_date] ORDER BY [Completion_date]";

            addParameter<int>("@projectID", SqlDbType.Int, projectID);
            myAdapter.Fill(myDataSet, "temp");
            var table = myDataSet.Tables["temp"];
            myCommand.Parameters.Clear();

            // Manipulate data - add missing date
            int i = 0;
            while (i < table.Rows.Count - 1)
            {
                DateTime date1 = (DateTime)table.Rows[i][0];
                DateTime date2 = (DateTime)table.Rows[i + 1][0];
                if (date1.AddDays(1) != date2)
                {
                    DataRow row = table.NewRow();
                    row[0] = date1.Date.AddDays(1);
                    row[1] = 0;
                    table.Rows.InsertAt(row, i + 1);
                }
                i++;
            }

            // Create graph
            LineGraph myLine = new LineGraph();
            var labels = myLine.labels;
            var datasets = myLine.datasets;
            int colorIndex = new Random().Next(0, 6);
            LineGraphDataset dataset = new LineGraphDataset(colorHex[1, colorIndex], colorHex[0, colorIndex]);
            dataset.label = "";

            int cumulated = 0;
            // Name of the person and Work estimation
            foreach (DataRow temp in table.Rows)
            {
                DateTime date = (DateTime)temp[0];
                labels.Add(date.ToString("dd.MM.yyyy"));
                cumulated += Convert.ToInt32(temp[1]);
                dataset.data.Add(cumulated);
            }
            datasets.Add(dataset);
            return JsonConvert.SerializeObject(myLine);
        }

        protected int[] getOptimumLine(int startPoint, int endPoint, int range)
        {
            int[] point = new int[range + 2];
            point[0] = startPoint;
            point[range + 1] = endPoint;
            double a = (endPoint - startPoint) / range;
            double b = startPoint;
            for (int i = 1; i <= range; i++)
                point[i] = Convert.ToInt32(a * i + b);
            return point;
        }
    }
}