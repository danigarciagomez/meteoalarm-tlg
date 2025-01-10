using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Telegram.Bot;

// awareness_level => 1 Verde, 2 Amarillo, 3 Naranja, 4 Rojo.
// awareness_type  => 1 Viento, 2 Lluvia, 3 Tormentas eléctricas, 4 Nieve/Hielo, 5 Temperatura extremadamente alta (Ola de calor), 6 Temperatura extremadamente baja (Frío extremo), 7 Incendios forestales o de pastizales, 8 Inundaciones, 9 Oleaje alto o marejada ciclónica, 10 Lluvia intensa

namespace meteoalarm_tlg
{
    internal class AvisosTlg
    {
        // Declarar los eventos ProcesarIniciado y ProcesarTerminado
        public event EventHandler ProcesarIniciado;
        public event EventHandler ProcesarTerminado;

        // Declarar los eventos de los Labels
        public event EventHandler<string> ProcesarLabelZonasAct;
        public event EventHandler<string> ProcesarLabelNumEjec;
        public event EventHandler<string> ProcesarLabelFecPrimeraEjec;
        public event EventHandler<string> ProcesarLabelNumAvisosAct;
        public event EventHandler<string> ProcesarLabelFecUltEjec;
        public event EventHandler<string> ProcesarLabelAvisosSent;

        public event EventHandler<string> ProcesarError;

        // Declarar la variable privada _botClient que es el cliente de Telegram
        private TelegramBotClient _botClient;

        // Variable para llevar el contador de ejecuciones del método Procesar
        private int contadorEjecuciones;
        private int contadorAvisos;
        private int contadorAvisosTlg;

        // Constructor de la clase AvisosTlg
        public AvisosTlg()
        {
        }





        /*
         *       
        public event EventHandler ProcesarLabelZonasAct;
        public event EventHandler ProcesarLabelNumEjec;
        public event EventHandler ProcesarLabelFecPrimeraEjec;
        public event EventHandler ProcesarLabelNumAvisosAct;
        public event EventHandler ProcesarLabelFecUltEjec;
        public event EventHandler ProcesarLabelAvisosSent;
                  
        <Label Name="lblZonasAct" Content="Zonas: " HorizontalAlignment="Left" Margin="5,1,0,0" Grid.Row="1" Grid.Column="0" VerticalAlignment="Top"/>
        <Label Name="lblNumEjec" Content="Nº Ejecuciones: " HorizontalAlignment="Left" Margin="5,1,0,0" Grid.Row="1" Grid.Column="1" VerticalAlignment="Top"/>
        
        <Label Name="lblFecPrimeraEjec" Content="Primera ejecución: " HorizontalAlignment="Left" Margin="5,1,0,0" Grid.Row="2" Grid.Column="0" VerticalAlignment="Top"/>
        <Label Name="lblNumAvisosAct" Content="Nº Avisos en XML (ult.ejec): " HorizontalAlignment="Left" Margin="5,1,0,0" Grid.Row="2" Grid.Column="1" VerticalAlignment="Top"/>
        
        <Label Name="lblFecUltEjec" Content="Última ejecución: " HorizontalAlignment="Left" Margin="5,1,0,0" Grid.Row="3" Grid.Column="0" VerticalAlignment="Top"/>
        <Label Name="lblNumAvisosSent" Content="Nº Avisos enviados (ult.ejec): " HorizontalAlignment="Left" Margin="5,1,0,0" Grid.Row="3" Grid.Column="1" VerticalAlignment="Top"/>

         * 
         * **/




        /// <summary>
        /// 
        /// </summary>

        // Método que procesa los avisos [Es el método principal]
        public void Procesar()
        {
            try
            {

                if (string.IsNullOrEmpty(AppSettings.BotToken))
                {
                    OnProcesarError("El token del bot no puede estar vacío.");
                    return;
                }
                if (string.IsNullOrEmpty(AppSettings.ChannelId))
                {
                    OnProcesarError("El Channel ID del bot no puede estar vacío.");
                    return;
                }
                if (string.IsNullOrEmpty(AppSettings.ChannelPrioId))
                {
                    OnProcesarError("El Channel ID Prioritario del bot no puede estar vacío.");
                    return;
                }
                if (string.IsNullOrEmpty(AppSettings.UrlMeteoAlarm))
                {
                    OnProcesarError("La URL de MeteoAlarm no puede estar vacío.");
                    return;
                }

                _botClient = new TelegramBotClient(AppSettings.BotToken);

                // Para saber cuantas veces se ha ejecutado el método Procesar
                contadorEjecuciones++;


                // Actualizar los Labels - El del número de ejecuciones
                OnProcesarLabelNumEjec("Nº Ejecuciones: " + contadorEjecuciones.ToString());

                // Activa el evento ProcesarIniciado, icono de estado a verde
                OnProcesarIniciado();

                // Lógica del método procesar
                Console.WriteLine("Empezando a procesar");


                // Crear la base de datos y la tabla si no existen
                CrearBaseDeDatos();

                // Obtener los avisos actuales (Se lee el ATOM y se obtiene el link con el detalle de cada aviso en XML que se devuelve en una lista de tipo Aviso)
                var avisosActuales = ObtenerAvisosActuales();

                // Leer los avisos previos desde la base de datos
                var avisosPrevios = LeerAvisosPreviosDesdeBaseDeDatos();

                var avisosNuevosOActualizados = avisosActuales
                    .Where(aviso => !avisosPrevios.Any(previo => previo.Identifier == aviso.Identifier && SonIguales(aviso, previo)))
                    .ToList();

                if (avisosNuevosOActualizados.Any())
                {
                    EnviarAvisosPorTelegram(avisosNuevosOActualizados);
                    GuardarAvisosEnBaseDeDatos(avisosNuevosOActualizados);
                }

                // Actualizar los Labels - El de última ejecución
                string fechaHoraActual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                OnProcesarLabelFecUltEjec("Última ejecución: " + fechaHoraActual);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el método Procesar: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Terminando de Procesar");

                // Activa el evento ProcesarTerminado, icono de estado a rojo
                OnProcesarTerminado();
            }
        }



        // Método que compara dos avisos para saber si son iguales
        private bool SonIguales(Aviso aviso1, Aviso aviso2)
        {
            return aviso1.Sender == aviso2.Sender &&
                   aviso1.Status == aviso2.Status &&
                   aviso1.MsgType == aviso2.MsgType &&
                   aviso1.Category == aviso2.Category &&
                   aviso1.Urgency == aviso2.Urgency &&
                   aviso1.Severity == aviso2.Severity &&
                   aviso1.Certainty == aviso2.Certainty &&
                   aviso1.Effective == aviso2.Effective &&
                   aviso1.Expires == aviso2.Expires &&
                   aviso1.Headline == aviso2.Headline &&
                   aviso1.Description == aviso2.Description &&
                   aviso1.AwarenessLevel == aviso2.AwarenessLevel &&
                   aviso1.AwarenessType == aviso2.AwarenessType &&
                   aviso1.AreaDescription == aviso2.AreaDescription &&
                   aviso1.EmmaId == aviso2.EmmaId;
        }




        // Método que obtiene el NamespaceManager para poder acceder a los nodos del XML
        private XmlNamespaceManager GetNamespaceManager(XmlDocument doc)
        {
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("cap", "urn:oasis:names:tc:emergency:cap:1.2");
            return nsmgr;
        }



        // Método que obtiene los avisos actuales, leyendo el ATOM y sacando el Link con el detalle de cada aviso en XML
        private List<Aviso> ObtenerAvisosActuales()
        {
            var avisos = new List<Aviso>();

            try
            {
                using (var reader = XmlReader.Create(AppSettings.UrlMeteoAlarm))
                {
                    var feed = SyndicationFeed.Load(reader);
                    foreach (var item in feed.Items)
                    {
                        // Acceder a la segunda URL con más detalles
                        var detalleUrl = item.Links.FirstOrDefault(link => link.MediaType == "application/cap+xml")?.Uri;
                        Console.WriteLine(detalleUrl);

                        if (detalleUrl != null)
                        {
                            var detalle = ObtenerDetalleAviso(detalleUrl);

                            if (detalle != null)
                            {
                                // Verificar si la fecha de expiración es válida y no ha expirado
                                if (DateTime.TryParse(detalle.Expires, out DateTime fechaExpiracion))
                                {
                                    if (fechaExpiracion.Date >= DateTime.Now.Date)
                                    {
                                        avisos.Add(detalle);
                                        Console.WriteLine("Resultado ->" + detalle.Identifier);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Aviso expirado: {detalle.Identifier}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Fecha de expiración no válida para el aviso: {detalle.Identifier}");
                                }

                                // Actualizar los Labels - El de número de avisos actuales
                                contadorAvisos++;
                                OnProcesarLabelNumAvisosAct("Nº Avisos en XML (ult.ejec): " + contadorAvisos.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener avisos actuales: {ex.Message}");
            }

            return avisos;
        }




        // Método que obtiene el detalle de un aviso, leyendo el XML y sacando los valores de los nodos
        private Aviso ObtenerDetalleAviso(Uri url)
        {
            try
            {
                using (var client = new System.Net.WebClient())
                {
                    // Configurar el cliente para que lea el XML en UTF-8
                    client.Encoding = Encoding.UTF8;

                    // Descargar el XML
                    var xml = client.DownloadString(url);

                    // Cargar el XML en un objeto XmlDocument
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);

                    // Obtener el NamespaceManager
                    var nsmgr = GetNamespaceManager(doc);

                    // Obtener valores del XML
                    var identifier = doc.SelectSingleNode("//cap:identifier", nsmgr)?.InnerText ?? "Sin identificador";
                    var sender = doc.SelectSingleNode("//cap:sender", nsmgr)?.InnerText ?? "Sin remitente";
                    var status = doc.SelectSingleNode("//cap:status", nsmgr)?.InnerText ?? "Sin estado";
                    var msgType = doc.SelectSingleNode("//cap:msgType", nsmgr)?.InnerText ?? "Sin tipo de mensaje";

                    var infoNode = doc.SelectSingleNode("//cap:info[cap:language='es-ES']", nsmgr);
                    var category = infoNode?.SelectSingleNode("cap:category", nsmgr)?.InnerText ?? "Sin categoría";
                    var urgency = infoNode?.SelectSingleNode("cap:urgency", nsmgr)?.InnerText ?? "Sin urgencia";
                    var severity = infoNode?.SelectSingleNode("cap:severity", nsmgr)?.InnerText ?? "Sin severidad";
                    var certainty = infoNode?.SelectSingleNode("cap:certainty", nsmgr)?.InnerText ?? "Sin certeza";
                    var effective = infoNode?.SelectSingleNode("cap:effective", nsmgr)?.InnerText ?? "Sin fecha de efectividad";
                    var expires = infoNode?.SelectSingleNode("cap:expires", nsmgr)?.InnerText ?? "Sin fecha de expiración";
                    var headline = infoNode?.SelectSingleNode("cap:headline", nsmgr)?.InnerText ?? "Sin titular";
                    var description = infoNode?.SelectSingleNode("cap:description", nsmgr)?.InnerText ?? "Sin descripción";

                    var awarenessLevel = infoNode?.SelectSingleNode("cap:parameter[cap:valueName='awareness_level']/cap:value", nsmgr)?.InnerText ?? "Sin nivel de alerta";
                    var awarenessType = infoNode?.SelectSingleNode("cap:parameter[cap:valueName='awareness_type']/cap:value", nsmgr)?.InnerText ?? "Sin tipo de alerta";

                    var areaNode = infoNode?.SelectSingleNode("cap:area", nsmgr);
                    var areaDesc = areaNode?.SelectSingleNode("cap:areaDesc", nsmgr)?.InnerText ?? "Sin descripción del área";
                    var emmaId = areaNode?.SelectSingleNode("cap:geocode[cap:valueName='EMMA_ID']/cap:value", nsmgr)?.InnerText ?? "Sin EMMA_ID";

                    Console.WriteLine(identifier);
                    Console.WriteLine(sender);
                    Console.WriteLine(status);
                    Console.WriteLine(msgType);
                    Console.WriteLine(infoNode);
                    Console.WriteLine(category);
                    Console.WriteLine(urgency);
                    Console.WriteLine(severity);
                    Console.WriteLine(certainty);
                    Console.WriteLine(effective);
                    Console.WriteLine(expires);
                    Console.WriteLine(headline);
                    Console.WriteLine(description);
                    Console.WriteLine(awarenessLevel);
                    Console.WriteLine(awarenessType);
                    Console.WriteLine(areaNode);
                    Console.WriteLine(areaDesc);
                    Console.WriteLine(emmaId);

                    return new Aviso
                    {
                        Identifier = identifier,
                        Sender = sender,
                        Status = status,
                        MsgType = msgType,
                        Category = category,
                        Urgency = urgency,
                        Severity = severity,
                        Certainty = certainty,
                        Effective = effective,
                        Expires = expires,
                        Headline = headline,
                        Description = description,
                        AwarenessLevel = awarenessLevel,
                        AwarenessType = awarenessType,
                        AreaDescription = areaDesc,
                        EmmaId = emmaId
                    };

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalle del aviso: {ex.Message}");
                return null;
            }
        }






        // Método que envía los avisos por Telegram
        private void EnviarAvisosPorTelegram(List<Aviso> avisos)
        {
            foreach (var aviso in avisos)
            {
                // Recuperamos los parámetros de los dos canales a los que se envía el aviso
                var chatId = AppSettings.ChannelId;
                var chatPrioId = AppSettings.ChannelPrioId;

                // Formateamos el mensaje que vamos a enviar (No se envía toda la info que tenemos en el aviso, solo la que nos interesa)
                var mensajeTlg = FormatearMensaje(aviso);


                // Enviamos el mensaje al canal principal
                try
                {
                    _botClient.SendMessage(chatId, mensajeTlg, Telegram.Bot.Types.Enums.ParseMode.Html).Wait();
                }
                catch (Telegram.Bot.Exceptions.ApiRequestException ex)
                {
                    Console.WriteLine($"Error al enviar mensaje: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    foreach (var innerEx in ex.InnerExceptions)
                    {
                        Console.WriteLine($"Error interno: {innerEx.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }


                // Enviamos el mensaje al canal de prioridad
                if (AppSettings.EmmaId.Split(',').Contains(aviso.EmmaId.Trim()))
                {
                    
                    try
                    {
                        _botClient.SendMessage(chatPrioId, mensajeTlg, Telegram.Bot.Types.Enums.ParseMode.Html).Wait();
                    }
                    catch (Telegram.Bot.Exceptions.ApiRequestException ex)
                    {
                        Console.WriteLine($"Error al enviar mensaje prio: {ex.Message}");
                    }
                    catch (AggregateException ex)
                    {
                        foreach (var innerEx in ex.InnerExceptions)
                        {
                            Console.WriteLine($"Error interno en prio: {innerEx.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error inesperado en prio: {ex.Message}");
                    }
                }

                // Actualizar los Labels - El de avisos enviados
                contadorAvisosTlg++;
                OnProcesarLabelAvisosSent("Nº Avisos enviados (ult.ejec): " + contadorAvisosTlg.ToString());
            }
        }

        private string FormatearMensaje(Aviso aviso)
        {
            // Variable para construir el mensaje
            var mensaje = new StringBuilder();

            // Agregar iconos (emojis) según la severidad
            string iconoSeveridad;
            switch (aviso.Severity.ToLower())
            {
                case "extreme":
                    iconoSeveridad = "🔴"; // Red Circle
                    break;
                case "severe":
                    iconoSeveridad = "🔶";
                    break;
                case "moderate":
                    iconoSeveridad = "⚠️";
                    break;
                case "minor":
                    iconoSeveridad = "🔷";
                    break;
                default:
                    iconoSeveridad = "⚪";
                    break;
            }

            // Construir el mensaje con formato Markdown
            mensaje.AppendLine($"{iconoSeveridad}<b>{aviso.Headline}</b> ");
            mensaje.AppendLine("");
            mensaje.AppendLine($"<b>Detalle:</b> {aviso.Description}");
            mensaje.AppendLine("");
            mensaje.AppendLine($"<b>Estado:</b> {aviso.Status}");
            mensaje.AppendLine($"<b>Tipo:</b> {aviso.MsgType}");
            mensaje.AppendLine($"<b>Tipo de Alerta:</b> {aviso.AwarenessType}");
            mensaje.AppendLine($"<b>Nivel:</b> {aviso.AwarenessLevel}");
            mensaje.AppendLine($"<b>Área:</b> {aviso.AreaDescription}");
            mensaje.AppendLine($"<b>EMMA ID:</b> {aviso.EmmaId}");
            mensaje.AppendLine("");
            mensaje.AppendLine($"<b>Categoría:</b> {aviso.Category}");
            mensaje.AppendLine($"<b>Urgencia:</b> {aviso.Urgency}");
            mensaje.AppendLine($"<b>Severidad:</b> {aviso.Severity}");
            mensaje.AppendLine($"<b>Certeza:</b> {aviso.Certainty}");
            mensaje.AppendLine("");
            mensaje.AppendLine($"<b>Efectivo:</b> {aviso.Effective}");
            mensaje.AppendLine($"<b>Expira:</b> {aviso.Expires}");
            //mensaje.AppendLine("");
            //mensaje.AppendLine($"<b>Identificador:</b> {aviso.Identifier}");

            // Devolver el mensaje formateado
            return mensaje.ToString();
        }









        // Método que crea la base de datos SQLite
        private void CrearBaseDeDatos()
        {
            try
            {
                using (var connection = new SQLiteConnection("Data Source=avisos.db"))
                {
                    connection.Open();
                    string sql = @"
                   CREATE TABLE IF NOT EXISTS Avisos (
                       Identifier TEXT PRIMARY KEY,
                       Sender TEXT,
                       Status TEXT,
                       MsgType TEXT,
                       Category TEXT,
                       Urgency TEXT,
                       Severity TEXT,
                       Certainty TEXT,
                       Effective TEXT,
                       Expires TEXT,
                       Headline TEXT,
                       Description TEXT,
                       AwarenessLevel TEXT,
                       AwarenessType TEXT,
                       AreaDescription TEXT,
                       EmmaId TEXT
                   )";
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la base de datos: {ex.Message}");
            }
        }




        // Método que lee los avisos de la base de datos SQLite
        private List<Aviso> LeerAvisosPreviosDesdeBaseDeDatos()
        {
            var avisos = new List<Aviso>();
            try
            {
                using (var connection = new SQLiteConnection("Data Source=avisos.db"))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Avisos";
                    using (var command = new SQLiteCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var aviso = new Aviso
                            {
                                Identifier = reader["Identifier"].ToString(),
                                Sender = reader["Sender"].ToString(),
                                Status = reader["Status"].ToString(),
                                MsgType = reader["MsgType"].ToString(),
                                Category = reader["Category"].ToString(),
                                Urgency = reader["Urgency"].ToString(),
                                Severity = reader["Severity"].ToString(),
                                Certainty = reader["Certainty"].ToString(),
                                Effective = reader["Effective"].ToString(),
                                Expires = reader["Expires"].ToString(),
                                Headline = reader["Headline"].ToString(),
                                Description = reader["Description"].ToString(),
                                AwarenessLevel = reader["AwarenessLevel"].ToString(),
                                AwarenessType = reader["AwarenessType"].ToString(),
                                AreaDescription = reader["AreaDescription"].ToString(),
                                EmmaId = reader["EmmaId"].ToString()
                            };
                            avisos.Add(aviso);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer avisos previos desde la base de datos: {ex.Message}");
            }
            return avisos;
        }




        // Método que guarda los avisos en la base de datos SQLite
        private void GuardarAvisosEnBaseDeDatos(List<Aviso> avisos)
        {
            try
            {
                using (var connection = new SQLiteConnection("Data Source=avisos.db"))
                {
                    connection.Open();
                    foreach (var aviso in avisos)
                    {
                        string sql = @"
                    INSERT OR REPLACE INTO Avisos (
                        Identifier, Sender, Status, MsgType, Category, Urgency, Severity, Certainty, Effective, Expires, Headline, Description, AwarenessLevel, AwarenessType, AreaDescription, EmmaId
                    ) VALUES (
                        @Identifier, @Sender, @Status, @MsgType, @Category, @Urgency, @Severity, @Certainty, @Effective, @Expires, @Headline, @Description, @AwarenessLevel, @AwarenessType, @AreaDescription, @EmmaId
                    )";
                        using (var command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Identifier", aviso.Identifier);
                            command.Parameters.AddWithValue("@Sender", aviso.Sender);
                            command.Parameters.AddWithValue("@Status", aviso.Status);
                            command.Parameters.AddWithValue("@MsgType", aviso.MsgType);
                            command.Parameters.AddWithValue("@Category", aviso.Category);
                            command.Parameters.AddWithValue("@Urgency", aviso.Urgency);
                            command.Parameters.AddWithValue("@Severity", aviso.Severity);
                            command.Parameters.AddWithValue("@Certainty", aviso.Certainty);
                            command.Parameters.AddWithValue("@Effective", aviso.Effective);
                            command.Parameters.AddWithValue("@Expires", aviso.Expires);
                            command.Parameters.AddWithValue("@Headline", aviso.Headline);
                            command.Parameters.AddWithValue("@Description", aviso.Description);
                            command.Parameters.AddWithValue("@AwarenessLevel", aviso.AwarenessLevel);
                            command.Parameters.AddWithValue("@AwarenessType", aviso.AwarenessType);
                            command.Parameters.AddWithValue("@AreaDescription", aviso.AreaDescription);
                            command.Parameters.AddWithValue("@EmmaId", aviso.EmmaId);
                            command.ExecuteNonQuery();
                        }
                        Console.WriteLine($"Aviso guardado: {aviso.Identifier}");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar avisos en la base de datos: {ex.Message}");
            }

        }






        // Método que dispara el evento ProcesarIniciado (para cambiar la imagen de estado, pasando de rojo a verde)
        protected virtual void OnProcesarIniciado()
        {
            try
            {
                ProcesarIniciado?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al lanzar el invoke del terminado: {ex.Message}");
            }
        }

        // Método que dispara el evento ProcesarTerminado (para cambiar la imagen de estado, pasando de verde a rojo)
        protected virtual void OnProcesarTerminado()
        {
            try
            {
                ProcesarTerminado?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al lanzar el invoke del terminado: {ex.Message}");
            }
        }


        protected virtual void OnProcesarLabelZonasAct(string texto)
        {
            ProcesarLabelZonasAct?.Invoke(this, texto);
        }
        protected virtual void OnProcesarLabelNumEjec(string texto)
        {
            ProcesarLabelNumEjec?.Invoke(this, texto);
        }

        protected virtual void OnProcesarLabelFecPrimeraEjec(string texto)
        {
            ProcesarLabelFecPrimeraEjec?.Invoke(this, texto);
        }

        protected virtual void OnProcesarLabelNumAvisosAct(string texto)
        {
            ProcesarLabelNumAvisosAct?.Invoke(this, texto);
        }

        protected virtual void OnProcesarLabelFecUltEjec(string texto)
        {
            ProcesarLabelFecUltEjec?.Invoke(this, texto);
        }

        protected virtual void OnProcesarLabelAvisosSent(string texto)
        {
            ProcesarLabelAvisosSent?.Invoke(this, texto);
        }


        protected virtual void OnProcesarError(string mensaje)
        {
            ProcesarError?.Invoke(this, mensaje);
        }



    }
}
