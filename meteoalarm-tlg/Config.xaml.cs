using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace meteoalarm_tlg
{
    /// <summary>
    /// Lógica de interacción para Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        public Config()
        {
            InitializeComponent();
            CargarValoresConfiguracion();
            this.Closing += Config_Closing;
        }

        private void CargarValoresConfiguracion()
        {
            txtTlgToken.Text = AppSettings.BotToken;
            txtChatId.Text = AppSettings.ChannelId;
            txtChatPrioId.Text = AppSettings.ChannelPrioId;
            txtMAurl.Text = AppSettings.UrlMeteoAlarm;
            txtMAzonas.Text = AppSettings.EmmaId;

            foreach (ComboBoxItem item in cmbIntervalo.Items)
            {
                if (item.Tag != null && int.Parse(item.Tag.ToString()) == AppSettings.IntervaloTiempo)
                {
                    cmbIntervalo.SelectedItem = item;
                    break;
                }
            }
        }

        private void Config_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AppSettings.BotToken = txtTlgToken.Text;
            AppSettings.ChannelId = txtChatId.Text;
            AppSettings.ChannelPrioId = txtChatPrioId.Text;
            AppSettings.UrlMeteoAlarm = txtMAurl.Text;
            AppSettings.EmmaId = txtMAzonas.Text;
            if (cmbIntervalo.SelectedItem is ComboBoxItem selectedItem)
            {
                AppSettings.IntervaloTiempo = int.Parse(selectedItem.Tag.ToString());
            }
        }
    }
}
