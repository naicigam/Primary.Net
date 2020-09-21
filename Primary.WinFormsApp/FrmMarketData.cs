using Primary.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Primary.WinFormsApp
{
    public partial class FrmMarketData : Form
    {
        
        public FrmMarketData()
        {
            InitializeComponent();
        }

        public void OnMarketData(MarketData marketData)
        {
            try
            {
                this.Invoke(new Action(() => this.UpdateMarketData(marketData)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void UpdateMarketData(MarketData marketData)
        {
            if (marketData.Instrument.Symbol == this.Text)
            {
                txtPrice.Text = marketData.Data?.Last?.Price.ToString();
                txtChange.Text =  ((marketData.Data?.Last?.Price / marketData.Data?.Close?.Price - 1M) * 100).ToString();

                var dataTable = new DataTable();
                dataTable.Columns.Add("BidSize", typeof(decimal));
                dataTable.Columns.Add("BidPrice", typeof(decimal));
                dataTable.Columns.Add("OfferPrice", typeof(decimal));
                dataTable.Columns.Add("OfferSize", typeof(decimal));

                for (int i = 0; i < 5; i++)
                {
                    var row = dataTable.NewRow();
                    var bid = marketData.Data?.Bids?.ElementAtOrDefault(i);
                    if (bid != null)
                    {
                        row["BidSize"] = bid.Size;
                        row["BidPrice"] = bid.Price;
                    }
                    var offer = marketData.Data?.Offers?.ElementAtOrDefault(i);
                    if (offer != null)
                    {
                        row["OfferSize"] = offer.Size;
                        row["OfferPrice"] = offer.Price;
                    }
                    dataTable.Rows.Add(row);
                }

                grdBook.DataSource = dataTable;
            }

        }

        public void SetInstrument(string symbol)
        {
            this.Text = symbol;
        }

        private void grdBook_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmMarketData_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
