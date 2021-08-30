using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RabbitCIEClient
{
    class Logs
    {
        private List<string> erroresPreviosProcesado;
        private List<string> erroresconsumoRabbit;
        private List<string> erroresProcesado;

        public Logs()
        {
            erroresPreviosProcesado = new List<string>();
            erroresconsumoRabbit = new List<string>();
            erroresProcesado = new List<string>();

        }
        public void addError(string tipolist, string error)
        {
            switch (tipolist)
            {
                case "erroresPreviosProcesado":
                    erroresPreviosProcesado.Add(error);
                    break;
                case "erroresconsumoRabbit":
                    erroresconsumoRabbit.Add(error);
                    break;
                case "erroresProcesado":
                    erroresProcesado.Add(error);
                    break;
            }
        }
        public bool hayErrores(string tipolist="")      
        {
            switch (tipolist)
            {
                case "erroresPreviosProcesado":
                    return (erroresPreviosProcesado == null);
                case "erroresconsumoRabbit":
                    return (erroresconsumoRabbit == null);
                case "erroresProcesado":
                    return (erroresProcesado == null);
                case "":
                    if ((erroresPreviosProcesado == null) || (erroresconsumoRabbit == null) || (erroresProcesado == null)){ return true; }
                    break;
            }
            return false;
        }

        public string construirCuerpoEmail()
        {
            string resultado = "";
            string resulprevioproc = "";
            for (int i = 0; i < erroresPreviosProcesado.Count; i++)
            {
                if (resulprevioproc == "")
                {
                    resulprevioproc = new string(' ', 10) + erroresPreviosProcesado[i];
                }
                else
                {
                    resulprevioproc += "\r\n" + new string(' ', 10) + erroresPreviosProcesado[i];
                }
            }
            if (resulprevioproc != "")
            {
                resulprevioproc = new string(' ', 5) + "Ficheros descargados con anterioridad pendientes de procesar:" + "\r\n" + resulprevioproc;
            }

            string resulconsum = "";
            for (int i = 0; i < erroresconsumoRabbit.Count; i++)
            {
                if (resulconsum == "")
                {
                    resulconsum = new string(' ', 10) + erroresconsumoRabbit[i];
                }
                else
                {
                    resulconsum += "\r\n" + new string(' ', 10) + erroresconsumoRabbit[i];
                }
            }
            if (resulconsum != "")
            {
                resulconsum = new string(' ', 5) + "En proceso de connexión y consumo de mensajes de RabbitMQ:" + "\r\n" + resulconsum;
            }

            string resulproces = "";
            for (int i = 0; i < erroresconsumoRabbit.Count; i++)
            {
                if (resulproces == "")
                {
                    resulproces = new string(' ', 10) + erroresPreviosProcesado[i];
                }
                else
                {
                    resulproces += "\r\n" + new string(' ', 10) + erroresPreviosProcesado[i];
                }
            }
            if (resulproces != "")
            {
                resulproces = new string(' ', 5) + "Ficheros que se consumen una vez descargados:" + "\r\n" + resulproces;
            }

            if ((resulprevioproc != "") || (resulconsum != "") || (resulproces != ""))
            {
                resultado = "Existen errores en la gestión de mensajes con RabbitMQ y Sage:";
            }
            else { return resultado; }

            if (resulprevioproc != "")
            {
                resultado += "\r\n" + resulprevioproc;
            }
            if (resulconsum != "")
            {
                resultado += "\r\n" + resulconsum;
            }
            if (resulproces != "")
            {
                resultado += "\r\n" + resulproces;
            }


            return resultado;
        }

        public void enviarLogEmail()
        {
            if (hayErrores())
            {
                var fromAddress = new MailAddress("tucorreode@gmail.com", "From Name");
                var toAddress = new MailAddress("to@example.com", "To Name");
                const string fromPassword = "fromPassword";
                const string subject = "Subject";
                
                string body = construirCuerpoEmail();
                if (body != "")
                {
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body
                    })
                    {
                        smtp.Send(message);
                    }
                }
            }
        }
    }
}
