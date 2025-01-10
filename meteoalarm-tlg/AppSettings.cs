using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace meteoalarm_tlg
{
    public static class AppSettings
    {
        private const int DefaultIntervaloTiempo = 300; // Valor por defecto en segundos

        public static int IntervaloTiempo
        {
            get
            {
                int intervalo = Properties.Settings.Default.paramTiempo;
                return intervalo > 0 ? intervalo : DefaultIntervaloTiempo;
            }
            set
            {
                Properties.Settings.Default.paramTiempo = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string UrlMeteoAlarm
        {
            get => Properties.Settings.Default.paramUrlMA;
            set
            {
                Properties.Settings.Default.paramUrlMA = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string EmmaId
        {
            get => Properties.Settings.Default.paramEmmaId;
            set
            {
                Properties.Settings.Default.paramEmmaId = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string BotToken
        {
            get => Properties.Settings.Default.paramTokenTlg;
            set
            {
                Properties.Settings.Default.paramTokenTlg = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string ChannelId
        {
            get => Properties.Settings.Default.paramGrupo;
            set
            {
                Properties.Settings.Default.paramGrupo = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string ChannelPrioId
        {
            get => Properties.Settings.Default.paramGrupoPrio;
            set
            {
                Properties.Settings.Default.paramGrupoPrio = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
