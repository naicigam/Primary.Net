using Primary.Data;
using Primary.WebSockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Primary.WinFormsApp
{
    public partial class FrmMain : Form
    {
        private List<string> watchList;
        private Primary.Api api;
        private MarketDataWebSocket marketDataSocket;
        public delegate void MarketDataEventHandler(MarketData marketData);
        public event MarketDataEventHandler OnMarketData;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
        }

        private async void Login()
        {
            var login = new FrmLogin();

            if (login.ShowDialog() == DialogResult.OK)
            {
                this.api = new Api(Api.ProductionEndpoint);
                var success = await this.api.Login(login.UserName, login.Password);

                if (success == false)
                {
                    MessageBox.Show("Login Failed", "Login Failed", MessageBoxButtons.OK);
                    return;
                }

                InitWatchList();
                var allInstruments = await this.api.GetAllInstruments();
                var watchedInstruments = allInstruments.Where(ShouldWatch).ToArray();

                foreach (var watchInstrument in watchedInstruments)
                {
                    var frmMarketData = new FrmMarketData();
                    frmMarketData.SetInstrument(watchInstrument.Symbol);
                    this.OnMarketData += frmMarketData.OnMarketData;
                    frmMarketData.MdiParent = this;
                    frmMarketData.Show();
                    frmMarketData.FormClosing += MarketDataClosing;
                }

                // Subscribe to all entries
                this.marketDataSocket = this.api.CreateMarketDataSocket(watchedInstruments, Constants.AllEntries, 1, 5);

                this.marketDataSocket.OnData = OnReceiveMarketData;
                await this.marketDataSocket.Start();
            }
        }

        private void MarketDataClosing(object sender, FormClosingEventArgs e)
        {
            var frmMarketData = (FrmMarketData)sender;
            this.OnMarketData -= frmMarketData.OnMarketData;
        }

        private void OnReceiveMarketData(Api api, MarketData marketData)
        {
            Console.WriteLine(marketData.Instrument.Symbol + ": " + marketData.Data?.Last?.Price);
            this.OnMarketData(marketData);
        }

        private void InitWatchList()
        {
            //var bonds = new[] { "AL29", "AL30", "AL35", "AE38", "AL41", "GD29", "GD30", "GD35", "GD38", "GD41", "GD46" };
            var bonds = new[] { "AL30"};

            var allBonds = bonds.Concat(bonds.Select(x => x + "D"));

            this.watchList = new List<string>();
            foreach (var item in allBonds)
            {
                watchList.AddRange(ToMervalSymbol(item));
            }

        }

        private bool ShouldWatch(Instrument instrument)
        {
            return this.watchList.Contains(instrument.Symbol);
        }

        private static string[] ToMervalSymbol(string ticker)
        {
            return new[] { 
                $"MERV - XMEV - {ticker} - 48hs", 
                $"MERV - XMEV - {ticker} - 24hs", 
                $"MERV - XMEV - {ticker} - CI" 
            };
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login();
        }
    }
}
