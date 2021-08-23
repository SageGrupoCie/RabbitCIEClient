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


        public void Connect()
        {
            /*
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "usr_vhost330",
                Password = "arbx330",
                VirtualHost = "pre_vhost_330",
                Port = 5672,
                HostName = "104.155.9.57"
               
            };
            */
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = Funciones.obtenerValoresIni("USUARIO");
            factory.Password = Funciones.obtenerValoresIni("CONTRASENA");
            factory.VirtualHost = Funciones.obtenerValoresIni("VIRTUALHOST");
            factory.Port = Int32.Parse(Funciones.obtenerValoresIni("PUERTO"));
            factory.HostName = Funciones.obtenerValoresIni("HOST");
            //factory.ClientProvidedName = "Pruebas Grupo CIE";




            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            //channel.QueueDeclare(ValoresCon(6), false, false, false, null);
            channel.QueueBind("exportaciones", "exportaciones_exchange", "", null);
        }

        

        public void ConsumeMessages()
        {
            
            //QueueingBasicConsumer consumer = MakeConsumer();
            
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
                using (StreamWriter outputFile = new StreamWriter("C:\\COMPARTIDA\\RabMQCIE_" + nomFicGuardar[0] + "_" + (int.Parse(nomFicGuardar[1]) + 1).ToString() + ".txt"))
                {
                    foreach (string line in lines)
                        outputFile.WriteLine(line);
                }
            }

            connection.Close();
            connection.Dispose();
            connection = null;
        }

        


        private QueueingBasicConsumer MakeConsumer()
        {
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
            channel.BasicGet(Funciones.obtenerValoresIni("COLA"), true);
            channel.BasicConsume(Funciones.obtenerValoresIni("COLA"), true, consumer);
            return consumer;
        }

        private bool WasQuitKeyPressed()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (Char.ToUpperInvariant(keyInfo.KeyChar) == 'Q')
                {
                    return true;
                }
            }

            return false;
        }

        private static void ReadAMessage(QueueingBasicConsumer consumer)
        {
            BasicDeliverEventArgs messageInEnvelope = DequeueMessage(consumer);
            if (messageInEnvelope == null)
            {
                return;
            }

            try
            {
                
                var message = Encoding.UTF8.GetString(messageInEnvelope.Body);
                StreamWriter sw = new StreamWriter("C:\\COMPARTIDA\\TeCIERabbir_" + new Random().Next(1, 2000) + ".txt");
                //Write a line of text
                sw.WriteLine(message);
                //object message = SerializationHelper.FromByteArray(messageInEnvelope.Body);
                //Console.WriteLine("Received {0} : {1}", message.GetType().Name, message);


                //object message = SerializationHelper.FromByteArray(messageInEnvelope.Body);
                //JObject prueba = JObject.Parse(message);
                //String titulo = (string)prueba["claseEntidadIgeo"];

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed message: {0}", ex);
            }
        }

        private static BasicDeliverEventArgs DequeueMessage(QueueingBasicConsumer consumer)
        {
            const int timeoutMilseconds = 400;
            object result;
            RabbitMQ.Client.Events.BasicDeliverEventArgs result2;

            consumer.Queue.Dequeue(timeoutMilseconds, out result2);
            return result2 as BasicDeliverEventArgs;
        }
    }
}
