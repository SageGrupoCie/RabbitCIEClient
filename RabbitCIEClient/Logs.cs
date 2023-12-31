﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

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
                    return (erroresPreviosProcesado.Count != 0);
                case "erroresconsumoRabbit":
                    return (erroresconsumoRabbit.Count != 0);
                case "erroresProcesado":
                    return (erroresProcesado.Count != 0);
                case "":
                    if ((erroresPreviosProcesado.Count != 0) || (erroresconsumoRabbit.Count != 0) || (erroresProcesado.Count != 0)){ return true; }
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
            for (int i = 0; i < erroresProcesado.Count; i++)
            {
                if (resulproces == "")
                {
                    resulproces = new string(' ', 10) + erroresProcesado[i];
                }
                else
                {
                    resulproces += "\r\n" + new string(' ', 10) + erroresProcesado[i];
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

        public void enviarLogEmail(string emisor, string receptor, string pass, string asunto, string host, int puerto, bool ssl)
        {
            if (hayErrores())
            {

                try
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(receptor);
                    mail.From = new MailAddress(emisor);
                    mail.Subject = asunto;

                    mail.Body = construirCuerpoEmail();

                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = host; //Or Your SMTP Server Address
                    //
                    smtp.UseDefaultCredentials = false;
                    //
                    smtp.Credentials = new System.Net.NetworkCredential(emisor, pass); // ***use valid credentials***
                    smtp.Port = puerto;

                    //Or your Smtp Email ID and Password
                    smtp.EnableSsl = ssl;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void enviarLogEmail33(string emisor, string receptor, string pass, string asunto, string host, int puerto, bool ssl)
        {
            if (hayErrores())
            {

                Chilkat.MailMan mailman = new Chilkat.MailMan();

                // Datos servidor SMTP
                mailman.SmtpHost = host;

                mailman.SmtpUsername = emisor;
                mailman.SmtpPassword = pass;

                mailman.SmtpPort = puerto;

                mailman.StartTLS = ssl;

                // Create a new email object
                Chilkat.Email email = new Chilkat.Email();

                email.Subject = asunto;
                email.Body = construirCuerpoEmail();
                email.From = emisor;
                bool success = false;
                if (receptor.Contains(";"))
                {
                    string[] arrRecept = receptor.Split(';');
                    for (int i = 0; i < arrRecept.Length; i++)
                    {
                        success = email.AddTo("Receptor reporte Rabbit Grupo CIE", arrRecept[i]);
                    }
                }
                else
                {
                    success = email.AddTo("Receptor reporte Rabbit Grupo CIE", receptor);
                }

                success = mailman.SendEmail(email);
                if (success != true)
                {
                    MessageBox.Show(mailman.LastErrorText);
                    return;
                }

                success = mailman.CloseSmtpConnection();
                if (success != true)
                {
                    //Debug.WriteLine("Connection to SMTP server not closed cleanly.");
                    //return;
                }
            }
        }

    }
}
