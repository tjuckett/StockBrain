using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StockBrainModules.Caching
{
    public class StockHistory
    {
        public enum RangeType
        {
            Daily,
            Weekly,
            Monthly
        }

        public struct Slice
        {
            public DateTime Date { get; set; }
            public decimal Open { get; set; }
            public decimal High { get; set; }
            public decimal Low { get; set; }
            public decimal Close { get; set; }
            public long Volume { get; set; }
            public decimal AdjClose { get; set; }
        }

        public string Symbol { get; set; }
        public RangeType Range { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Slice> Slices { get; set; }

        public StockHistory(string symbol, RangeType range, DateTime startDate, DateTime endDate)
        {
            Symbol = symbol;
            Range = range;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void LoadPriceHistory()
        {
            string typeString = "d";
            switch(Range)
            {
                case RangeType.Daily: typeString = "d"; break;
                case RangeType.Weekly: typeString = "w"; break;
                case RangeType.Monthly: typeString = "m"; break;
            }

            string url = "http://real-chart.finance.yahoo.com/table.csv?s=" + Symbol + "&a=" + (StartDate.Month - 1) + "&b=" + StartDate.Day + "&c=" + StartDate.Year + "&d=" + (EndDate.Month - 1) + "&e=" + EndDate.Day + "&f=" + EndDate.Year + "&g=" + typeString + "&ignore=.csv";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream());
                string stockHistory = sr.ReadToEnd();
                sr.Close();

                List<string> lines = stockHistory.Split('\n').ToList();

                int dateIndex = 0;
                int openIndex = 1;
                int highIndex = 2;
                int lowIndex = 3;
                int closeIndex = 4;
                int volumeIndex = 5;
                int adjCloseIndex = 6;

                if (!string.IsNullOrWhiteSpace(lines[0]))
                {
                    List<string> headers = lines[0].Split(',').ToList();
                    lines.Remove(lines[0]);

                    for(int index = 0; index < headers.Count; ++index)
                    {
                        if (headers[index] == "Date") dateIndex = index;
                        if (headers[index] == "Open") openIndex = index;
                        if (headers[index] == "High") highIndex = index;
                        if (headers[index] == "Low") lowIndex = index;
                        if (headers[index] == "Close") closeIndex = index;
                        if (headers[index] == "Volume") volumeIndex = index;
                        if (headers[index] == "Adj Close") adjCloseIndex = index;
                    }
                }

                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        List<string> data = line.Split(',').ToList();

                        if(data.Count >= 0)
                        {
                            List<string> dateTimeStr = data[dateIndex].Split('-').ToList();
                            Slice slice = new Slice();
                            slice.Date = new DateTime(int.Parse(dateTimeStr[0]), int.Parse(dateTimeStr[1]), int.Parse(dateTimeStr[2]));
                            slice.Open = decimal.Parse(data[openIndex]);
                            slice.High = decimal.Parse(data[highIndex]);
                            slice.Low = decimal.Parse(data[lowIndex]);
                            slice.Close = decimal.Parse(data[closeIndex]);
                            slice.Volume = long.Parse(data[volumeIndex]);
                            slice.AdjClose = decimal.Parse(data[adjCloseIndex]);
                            Slices.Add(slice);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
