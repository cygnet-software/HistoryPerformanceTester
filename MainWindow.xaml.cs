using CxVhsLib;
using CygNet.API.Historian;
using CygNet.Data.Core;
using CygNet.Data.Historian;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HistoryPerformanceTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Name> _names = new List<Name>()
        {
            new Name() { ID = "C4P92.SVCMON.00000606:C4P92_ACS_SVMADQLC" },
            new Name() { ID = "C4P92.SVCMON.00000607:C4P92_ACS_SVMADQLT" },
            new Name() { ID = "C4P92.SVCMON.00000615:C4P92_ACS_SVMCPUCYC" },
            new Name() { ID = "C4P92.SVCMON.00000616:C4P92_ACS_SVMCPUT" },
        };

        private DomainSiteService _vhs = new DomainSiteService("[13131]C4P92.VHS");
        private Client _client;

        private DateTime _start = DateTime.Now.AddMonths(-1);
        private DateTime _end = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();

            _client = new Client(_vhs);
        }

        public void GetHistory_NET_Sync()
        {
            PostMessage("****************************************************");
            PostMessage("TEST: Get History");
            PostMessage("API: NET");
            PostMessage("Mode: Sync");

            Stopwatch sw = Stopwatch.StartNew();

            foreach (var name in _names)
            {
                GetHistory_NET(name);
            }

            PostMessage("Duration: " + sw.Elapsed.TotalSeconds.ToString() + " seconds");
        }

        public void GetHistory_NET_Async()
        {
            PostMessage("****************************************************");
            PostMessage("TEST: Get History");
            PostMessage("API: NET");
            PostMessage("Mode: Async");

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.ForEach(_names, (name) =>
            {
                GetHistory_NET(name);
            });

            PostMessage("Duration: " + sw.Elapsed.TotalSeconds.ToString() + " seconds");
        }

        public void GetHistory_NET_Async_Parallel()
        {
            PostMessage("****************************************************");
            PostMessage("TEST: Get History");
            PostMessage("API: NET");
            PostMessage("Mode: Async");

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.ForEach(_names, (name) =>
            {
                GetHistory_NET_Parallel(name);
            });

            PostMessage("Duration: " + sw.Elapsed.TotalSeconds.ToString() + " seconds");
        }

        public void GetHistory_COM_Sync()
        {
            PostMessage("****************************************************");
            PostMessage("TEST: Get History");
            PostMessage("API: COM");
            PostMessage("Mode: Sync");

            Stopwatch sw = Stopwatch.StartNew();

            foreach (var name in _names)
            {
                GetHistory_COM(name);
            }

            PostMessage("Duration: " + sw.Elapsed.TotalSeconds.ToString() + " seconds");
        }

        public void GetHistory_COM_Async()
        {
            PostMessage("****************************************************");
            PostMessage("TEST: Get History");
            PostMessage("API: COM");
            PostMessage("Mode: Async");

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.ForEach(_names, (name) =>
            {
                GetHistory_COM(name);
            });

            PostMessage("Duration: " + sw.Elapsed.TotalSeconds.ToString() + " seconds");
        }

        public void GetHistory_ODBC_Sync()
        {
            PostMessage("****************************************************");
            PostMessage("TEST: Get History");
            PostMessage("API: ODBC");
            PostMessage("Mode: Sync");

            Stopwatch sw = Stopwatch.StartNew();

            foreach (var name in _names)
            {
                GetHistory_ODBC(name);
            }

            PostMessage("Duration: " + sw.Elapsed.TotalSeconds.ToString() + " seconds");
        }

        public void GetHistory_ODBC_Async()
        {
            PostMessage("****************************************************");
            PostMessage("TEST: Get History");
            PostMessage("API: ODBC");
            PostMessage("Mode: Async");

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.ForEach(_names, (name) =>
            {
                GetHistory_ODBC(name);
            });

            PostMessage("Duration: " + sw.Elapsed.TotalSeconds.ToString() + " seconds");
        }

        #region HELPER METHODS

        private void GetHistory_NET(Name name)
        {
            var entries = _client.GetHistoricalEntries(name, _start, _end, false);

            foreach (var entry in entries)
            {
                var historyValue = entry.Value;
            }
        }

        private void GetHistory_NET_Parallel(Name name)
        {
            var entries = _client.GetHistoricalEntries(name, _start, _end, false);

            Parallel.ForEach(entries, (entry) =>
            {
                var historyValue = entry.Value;
            });
        }

        private void GetHistory_COM(Name name)
        {
            var iterator = new CxVhsLib.ValueIterator();
            iterator.Initialize(_vhs.SiteService.GetSiteService(), name.ID, _start, _end, 0);

            iterator.MoveFirst();

            ValueEntryEx value = new ValueEntryEx();
            while (iterator.GetForwardEx(value) != 0)
            {
                var historyValue = value.Value;
            }
        }

        private void GetHistory_ODBC(Name name)
        {
            // establish ODBC connection
            var siteService = _vhs.SiteService.GetSiteService();
            var table = ODBCTools.GetServiceNameAsOdbcTable(siteService);
            var odbcConnection = ODBCTools.GetOdbcConnection(siteService);
            odbcConnection.Open();

            var pointTag = new PointTag(name.ID);
            var startString = _start.ToString("yyyy-MM-dd");
            var endString = _end.ToString("yyyy-MM-dd");

            var command = odbcConnection.CreateCommand();
            var sql = $"SELECT Value,RecordTime FROM {table}.HistoricalValues WHERE PointIdLong='{pointTag.LongId}' AND RecordTime > '{startString}' AND RecordTime < '{endString}'";

            command.CommandText = sql;

            var adapter = new OdbcDataAdapter();
            adapter.SelectCommand = command;

            var tables = new DataSet();

            adapter.Fill(tables, String.Format("{0}.BS_HEADER_RECORD", table));

            odbcConnection.Close();
        }

        #endregion

        #region EVENT HANDLERS

        private void PostMessage(string message)
        {
            ResultsList.Items.Add(message);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetHistory_NET_Sync();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GetHistory_NET_Async();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GetHistory_COM_Sync();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            GetHistory_COM_Async();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            GetHistory_ODBC_Sync();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            GetHistory_ODBC_Async();
        }

        #endregion
    }
}
