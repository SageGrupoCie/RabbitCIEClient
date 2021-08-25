using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace RabbitCIEClient
{
    using System;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal class RabbitConsumer
    {
        private IConnection connection;
        private IModel channel;


        public void Connect(Logs lg)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = Funciones.obtenerValoresIni("USUARIO");
            factory.Password = Funciones.obtenerValoresIni("CONTRASENA");
            factory.VirtualHost = Funciones.obtenerValoresIni("VIRTUALHOST");
            factory.Port = Int32.Parse(Funciones.obtenerValoresIni("PUERTO"));
            factory.HostName = Funciones.obtenerValoresIni("HOST");

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueBind("exportaciones", "exportaciones_exchange", "", null);
        }

        

        public void ConsumeMessages(Logs lg)
        {

            string pathFicheros = Funciones.carpetaFicherosRabbit();
            while (true)
            {
                QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
                var data = channel.BasicGet(Funciones.obtenerValoresIni("COLA"), true);
                //channel.BasicConsume(ValoresCon(6), true, consumer);
                if (data == null) { break; }
                var message = Encoding.UTF8.GetString(data.Body);
                string[] nomFicGuardar = Funciones.obtenerContadorIni();
                Funciones.guardarValoresIni("CONTADOR", nomFicGuardar[0] + "#" + (int.Parse(nomFicGuardar[1]) + 1).ToString());
                string[] lines = message.Split("\r\n");
                using (StreamWriter outputFile = new StreamWriter(pathFicheros + "\\RabMQCIE_" + nomFicGuardar[0] + "_" + (int.Parse(nomFicGuardar[1]) + 1).ToString() + ".txt"))
                {
                    foreach (string line in lines)
                        outputFile.WriteLine(line);
                }
            }

            connection.Close();
            connection.Dispose();
            connection = null;
        }

        


          
    }
}
