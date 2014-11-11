using System.Collections.Generic;

namespace Lanban.Model
{
    /* Class object for charts */

    // Pie chart
    public class PieChart
    {
        public IList<PiePart> part { get; set; }

        public PieChart()
        {
            part = new List<PiePart>();
        }
    }

    public class PiePart
    {
        public int value { get; set; }
        public string color { get; set; }
        public string highlight { get; set; }
        public string label { get; set; }
    }

    // Bar chart
    public class BarChart
    {
        public IList<string> labels { get; set; }
        public IList<BarChartDataset> datasets { get; set; }

        public BarChart()
        {
            labels = new List<string>();
            datasets = new List<BarChartDataset>();
        }
    }

    public class BarChartDataset
    {
        public string label { get; set; }
        public string fillColor { get; set; }
        public string hightlightFill { get; set; }
        public IList<int> data { get; set; }

        public BarChartDataset(string fillColor, string highlightFill)
        {
            data = new List<int>();
            this.fillColor = fillColor;
            this.hightlightFill = highlightFill;
        }
    }

    // Line graph
    public class LineGraph
    {
        public IList<string> labels { get; set; }
        public IList<LineGraphDataset> datasets { get; set; }

        public LineGraph()
        {
            labels = new List<string>();
            datasets = new List<LineGraphDataset>();
        }
    }

    public class LineGraphDataset
    {
        public string label { get; set; }
        public string strokeColor { get; set; }
        public string pointColor { get; set; }
        public string pointStrokeColor { get; set; }
        public string pointHighlightFill { get; set; }
        public string fillColor { get; set; }
        public IList<int> data { get; set; }

        public LineGraphDataset(string strokeColor, string pointColor)
        {
            data = new List<int>();
            this.strokeColor = strokeColor;
            this.pointColor = pointColor;
            this.pointStrokeColor = pointColor;
            this.pointHighlightFill = "#fff";
            this.fillColor = "rgba(0,0,0,0.2)";
        }
    }
}