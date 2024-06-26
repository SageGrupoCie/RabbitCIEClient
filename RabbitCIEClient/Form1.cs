﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;

namespace RabbitCIEClient
{
    public partial class Principal : Form
    {
        public Principal(bool autom = false)
        {
            if (!autom)
            {
                InitializeComponent();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            rellenaDatosIniForm();
        }

        private void rellenaDatosIniForm()
        {
            //Parámetros RabbitMQ
            HostTB.Text = Funciones.obtenerValoresIni("HOST");
            VirtualHostTB.Text = Funciones.obtenerValoresIni("VIRTUALHOST");
            PuertoTB.Text = Funciones.obtenerValoresIni("PUERTO");
            UserTB.Text = Funciones.obtenerValoresIni("USUARIO");
            PassTB.Text = Funciones.obtenerValoresIni("CONTRASENA");
            colaTB.Text = Funciones.obtenerValoresIni("COLA");
            exchangeTB.Text = Funciones.obtenerValoresIni("EXCHANGE");
            if (Funciones.obtenerValoresIni("OPTION_CREATE") == "True")
            {
                createCHK.Checked = true;
            }
            else
            {
                createCHK.Checked = false;
            }
            if (Funciones.obtenerValoresIni("OPTION_UPDATE") == "True")
            {
                updateCHK.Checked = true;
            }
            else
            {
                updateCHK.Checked = false;
            }
            if (Funciones.obtenerValoresIni("OPTION_DELETE") == "True")
            {
                deleteCHK.Checked = true;
            }
            else
            {
                deleteCHK.Checked = false;
            }
            if ((!createCHK.Checked) && (!updateCHK.Checked) && (!deleteCHK.Checked))
            {           //Esto se hace para la primera vez que se abre
                createCHK.Checked = true;
                Funciones.guardarValoresIni("OPTION_CREATE", createCHK.Checked.ToString());
            }
            if (Funciones.obtenerValoresIni("OPTION_CLIENTE") == "True")
            {
                clienteCHK.Checked = true;
            }
            else
            {
                clienteCHK.Checked = false;
            }
            if (Funciones.obtenerValoresIni("OPTION_SEDE") == "True")
            {
                sedeCHK.Checked = true;
            }
            else
            {
                sedeCHK.Checked = false;
            }
            if (Funciones.obtenerValoresIni("OPTION_FACTURA") == "True")
            {
                facturaCHK.Checked = true;
            }
            else
            {
                facturaCHK.Checked = false;
            }
            if ((!clienteCHK.Checked) && (!sedeCHK.Checked) && (!facturaCHK.Checked))
            {           //Esto se hace para la primera vez que se abre
                clienteCHK.Checked = true;
                sedeCHK.Checked = true;
                Funciones.guardarValoresIni("OPTION_CLIENTE", clienteCHK.Checked.ToString());
                Funciones.guardarValoresIni("OPTION_SEDE", sedeCHK.Checked.ToString());
            }
            //Parámetros empresa
            servidorTB.Text = Funciones.obtenerValoresIni("SERVIDOR","BD");
            userBDTB.Text = Funciones.obtenerValoresIni("USUARIO", "BD");
            passBDTB.Text = Funciones.obtenerValoresIni("PASSWORD", "BD");
            BDTB.Text = Funciones.obtenerValoresIni("DATABASE", "BD");
            empSAGETB.Text = Funciones.obtenerValoresIni("EMPRESA_SAGE", "BD");
            diasLimitHistTB.Text = Funciones.obtenerValoresIni("DIAS_LIMITE_HISTORICO", "BD");      
            //Parámetros email
            emisorEMTB.Text = Funciones.obtenerValoresIni("EMISOR", "EMAIL");
            receptorEMTB.Text = Funciones.obtenerValoresIni("RECEPTOR", "EMAIL");
            passEMTB.Text = Funciones.obtenerValoresIni("PASSWORD", "EMAIL");
            asuntoEMTB.Text = Funciones.obtenerValoresIni("ASUNTO", "EMAIL");
            hostEMTB.Text = Funciones.obtenerValoresIni("HOST", "EMAIL");
            puertoEMTB.Text = Funciones.obtenerValoresIni("PUERTO", "EMAIL");
            if (Funciones.obtenerValoresIni("SSL", "EMAIL") == "True")
            {
                sslEMTB.Checked = true;
            }
            else
            {
                sslEMTB.Checked = false;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        

        


       


       



       

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Funciones.borrarValoresIni();
            Funciones.guardarValoresIni("HOST", HostTB.Text);
            Funciones.guardarValoresIni("VIRTUALHOST", VirtualHostTB.Text);
            Funciones.guardarValoresIni("PUERTO", PuertoTB.Text);
            Funciones.guardarValoresIni("USUARIO", UserTB.Text);
            Funciones.guardarValoresIni("CONTRASENA", PassTB.Text);
            Funciones.guardarValoresIni("COLA", colaTB.Text);
            Funciones.guardarValoresIni("EXCHANGE", exchangeTB.Text);
            Funciones.guardarValoresIni("EMPRESA_SAGE", empSAGETB.Text);
            Funciones.guardarValoresIni("OPTION_CREATE", createCHK.Checked.ToString());
            Funciones.guardarValoresIni("OPTION_UPDATE", updateCHK.Checked.ToString());
            Funciones.guardarValoresIni("OPTION_DELETE", deleteCHK.Checked.ToString());
            Funciones.guardarValoresIni("OPTION_CLIENTE", clienteCHK.Checked.ToString());
            Funciones.guardarValoresIni("OPTION_SEDE", sedeCHK.Checked.ToString());
            Funciones.guardarValoresIni("OPTION_FACTURA", facturaCHK.Checked.ToString());
        }
        public void procesarFichero(string soloDescarga = "N")
        {
            //Creamos fichero de logs
            Logs lg = new Logs();
            if (soloDescarga != "S")
            {
                //Comprobamos si hay ficheros y los procesamos. Este paso se hace al principio por si hubiera algún archivo por procesar
                Funciones.procesar_ficheros(lg, "SI");
            }
            //Descargamos los mensajes y creamos lo ficheros
            RabbitConsumer cliente = new RabbitConsumer();
            cliente.Connect(lg);
            cliente.ConsumeMessages(lg);

            if (soloDescarga != "S")
            {
                //Comprobamos si hay ficheros y los procesamos
                Funciones.procesar_ficheros(lg);
            }

            //Enviamos el log si fuera necesario
            string comprobParam = comprobarParamEmail();
            string[] arrComParam = comprobParam.Split('#');
            if (arrComParam[0] == "OK")
            {
                string xemisorEMTB = Funciones.obtenerValoresIni("EMISOR", "EMAIL");
                string xreceptorEMTB = Funciones.obtenerValoresIni("RECEPTOR", "EMAIL");
                string xpassEMTB = Funciones.obtenerValoresIni("PASSWORD", "EMAIL");
                string xasuntoEMTB = Funciones.obtenerValoresIni("ASUNTO", "EMAIL");
                string xhostEMTB = Funciones.obtenerValoresIni("HOST", "EMAIL");
                string xpuertoEMTB = Funciones.obtenerValoresIni("PUERTO", "EMAIL");
                int xintpuertoEMTB = 0;
                if (xpuertoEMTB != "")
                {
                    xintpuertoEMTB = int.Parse(xpuertoEMTB);
                }
                bool xsslEMTB = false;
                if (Funciones.obtenerValoresIni("SSL", "EMAIL") == "True")
                {
                    xsslEMTB = true;
                }
                //lg.addError("erroresProcesado", "Error de prueba Andreu");
                lg.enviarLogEmail(xemisorEMTB, xreceptorEMTB, xpassEMTB, xasuntoEMTB, xhostEMTB, xintpuertoEMTB, xsslEMTB);
            }
            int diasLimitHist;
            try
            {
                diasLimitHist = int.Parse(Funciones.obtenerValoresIni("DIAS_LIMITE_HISTORICO", "BD"));
            }
            catch 
            {
                diasLimitHist = 0;
            }
            Funciones.eliminar_ficherosAntiguos(diasLimitHist);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            

        }

        private string comprobarParamEmail()
        {
            string resultado = "OK#";
            if (Funciones.obtenerValoresIni("EMISOR", "EMAIL") == "")
            {
                resultado += "Dirección email del emisor";
            }
            if (Funciones.obtenerValoresIni("RECEPTOR", "EMAIL") == "")
            {
                string msg = "Dirección email del receptor";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("PASSWORD", "EMAIL") == "")
            {
                string msg = "Contraseña de logueo";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("HOST", "EMAIL") == "")
            {
                string msg = "Dirección del servidor de correo SMTP";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("PUERTO", "EMAIL") == "")
            {
                string msg = "Puerto de salida SMTP";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (resultado != "OK#")
            {
                resultado = "ERROR#" + resultado.Substring(3, resultado.Length - 3);
            }
            return resultado;



        }

        private string comprobarParametros()
        {
            string resultado = "OK#";
            if (Funciones.obtenerValoresIni("HOST") == "")
            {
                resultado += "Dirección Host de RAbbitMQ del proveedor de servicios";
            }
            if (Funciones.obtenerValoresIni("VIRTUALHOST") == "")
            {
                string msg = "Espacio VirtualHost de RAbbitMQ";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("PUERTO") == "")
            {
                string msg = "Puerto de connexión de RAbbitMQ";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("USUARIO") == "")
            {
                string msg = "Usuario de connexión de RAbbitMQ";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("CONTRASENA") == "")
            {
                string msg = "Contraseña asociada al usuario de connexión de RAbbitMQ";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("COLA") == "")
            {
                string msg = "Cola de exportación de RAbbitMQ";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }

            if (Funciones.obtenerValoresIni("SERVIDOR","BD") == "")
            {
                string msg = "Instancia del servidor SQL Server";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("USUARIO", "BD") == "")
            {
                string msg = "Usuario con acceso a la base de datos";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("PASSWORD", "BD") == "")
            {
                string msg = "Contraseña asociada al usuario de la base de datos";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("DATABASE", "BD") == "")
            {
                string msg = "Base de datos de SQL Server";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (Funciones.obtenerValoresIni("EMPRESA_SAGE", "BD") == "")
            {
                string msg = "Código de empresa asociado a SAGE";
                if (resultado == "OK#")
                {
                    resultado += msg;
                }
                else
                {
                    resultado += "#" + msg;
                }
            }
            if (resultado != "OK#")
            {
                resultado = "ERROR#" + resultado.Substring(3,resultado.Length-3);
            }
            return resultado;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Funciones.borrarValoresIni("BD");
            Funciones.guardarValoresIni("SERVIDOR", servidorTB.Text, "BD");
            Funciones.guardarValoresIni("USUARIO", userBDTB.Text, "BD");
            Funciones.guardarValoresIni("PASSWORD", passBDTB.Text, "BD");
            Funciones.guardarValoresIni("DATABASE", BDTB.Text, "BD");
            Funciones.guardarValoresIni("EMPRESA_SAGE", empSAGETB.Text,"BD");
            Funciones.guardarValoresIni("DIAS_LIMITE_HISTORICO", diasLimitHistTB.Text, "BD");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BaseDatos bd = new BaseDatos(servidorTB.Text, BDTB.Text, userBDTB.Text, passBDTB.Text);

            if (bd.estaConectado())
            {
                MessageBox.Show("Se ha realizado la conexión correctamente", "Conexión establecida");
            }
            else
            {
                MessageBox.Show("Parámetros de conexión incorrectos", "Error de conexión");
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PuertoTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void PuertoTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            
        }

        private void empSAGETB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            
        }

        private void servidorTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void servidorTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == Convert.ToChar(Keys.Space);
        }

        private void userBDTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == Convert.ToChar(Keys.Space);
        }

        private void BDTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == Convert.ToChar(Keys.Space);
        }

        private void HostTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == Convert.ToChar(Keys.Space);
        }

        private void VirtualHostTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == Convert.ToChar(Keys.Space);
        }

        private void UserTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == Convert.ToChar(Keys.Space);
        }

        private void colaTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == Convert.ToChar(Keys.Space);
        }

        private void puertoEMTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            Funciones.borrarValoresIni("EMAIL");
            Funciones.guardarValoresIni("EMISOR", emisorEMTB.Text, "EMAIL");
            Funciones.guardarValoresIni("RECEPTOR", receptorEMTB.Text, "EMAIL");
            Funciones.guardarValoresIni("PASSWORD", passEMTB.Text, "EMAIL");
            Funciones.guardarValoresIni("ASUNTO", asuntoEMTB.Text, "EMAIL");
            Funciones.guardarValoresIni("HOST", hostEMTB.Text, "EMAIL");
            Funciones.guardarValoresIni("PUERTO", puertoEMTB.Text, "EMAIL");
            Funciones.guardarValoresIni("SSL", sslEMTB.Checked.ToString(), "EMAIL");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Se va a iniciar el proceso de importación de entidades de IGEO, ¿Desea continuar?", "Inicio de proceso", MessageBoxButtons.YesNo,MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.Yes)
            {
                button2.Enabled = false;
                string comprobParam = comprobarParamEmail();
                string[] arrComParam = comprobParam.Split('#');
                if (arrComParam[0] != "OK")
                {
                    string msg = "Faltan parámetros email para el envío de logs (no son obligatorios, en tal caso el log no se enviará):";
                    for (int i = 1; i < arrComParam.Length; i++)
                    {
                        msg += "\r\n" + new string(' ', 5) + arrComParam[i];
                    }
                    MessageBox.Show(msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                comprobParam = comprobarParametros();
                arrComParam = comprobParam.Split('#');
                if (arrComParam[0] == "OK")
                {
                    procesarFichero();
                    MessageBox.Show("Se ha completado el proceso." + "\r\n" + "Compruebe la bandeja de entrada del correo electrónico por si hubiera incidencias.", "Proceso finalizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string msg = "Debe informar los siguientes datos correctamente:";
                    for (int i = 1; i < arrComParam.Length; i++)
                    {
                        msg += "\r\n" + new string(' ', 5) + arrComParam[i];
                    }
                    MessageBox.Show(msg, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                button2.Enabled = true;
            }            
        }

        private void verPassBTN_Click(object sender, EventArgs e)
        {
            if (verPassBTN.ImageIndex == 1) 
            { 
                verPassBTN.ImageIndex = 0;
                PassTB.PasswordChar = '*';
            }
            else 
            {
                verPassBTN.ImageIndex = 1;
                PassTB.PasswordChar = '\0';
            }
        }

        private void verPassEmailBTN_Click(object sender, EventArgs e)
        {
            if (verPassEmailBTN.ImageIndex == 1)
            {
                verPassEmailBTN.ImageIndex = 0;
                passEMTB.PasswordChar = '*';
            }
            else
            {
                /*  Desactivamos por el momento
                verPassEmailBTN.ImageIndex = 1;
                passEMTB.PasswordChar = '\0';
                */
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void diasLimitHistTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("Se va a iniciar únicamente el proceso de descarga de entidades de IGEO, ¿Desea continuar?", "Inicio de proceso", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == System.Windows.Forms.DialogResult.Yes)
            {
                string comprobParam = comprobarParametros();
                string[] arrComParam = comprobParam.Split('#');
                if (arrComParam[0] == "OK")
                {
                    procesarFichero("S");
                    MessageBox.Show("Se han descargado solo los mensajes." + "\r\n" + "Compruebe la bandeja de entrada del correo electrónico por si hubiera incidencias.", "Proceso finalizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void clienteCHK_CheckedChanged(object sender, EventArgs e)
        {
            if (clienteCHK.Checked) { sedeCHK.Checked = true; }
            else { sedeCHK.Checked = false; }
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            Logs lg = new Logs();
            lg.addError("erroresProcesado", "\r\n" + "Andreu Prueba: " + "\r\n" + "prueba" + "\r\n");
            string xemisorEMTB = Funciones.obtenerValoresIni("EMISOR", "EMAIL");
            string xreceptorEMTB = Funciones.obtenerValoresIni("RECEPTOR", "EMAIL");
            string xpassEMTB = Funciones.obtenerValoresIni("PASSWORD", "EMAIL");
            string xasuntoEMTB = Funciones.obtenerValoresIni("ASUNTO", "EMAIL");
            string xhostEMTB = Funciones.obtenerValoresIni("HOST", "EMAIL");
            string xpuertoEMTB = Funciones.obtenerValoresIni("PUERTO", "EMAIL");
            int xintpuertoEMTB = 0;
            if (xpuertoEMTB != "")
            {
                xintpuertoEMTB = int.Parse(xpuertoEMTB);
            }
            bool xsslEMTB = false;
            if (Funciones.obtenerValoresIni("SSL", "EMAIL") == "True")
            {
                xsslEMTB = true;
            }
            //lg.addError("erroresProcesado", "Error de prueba Andreu");
            lg.enviarLogEmail(xemisorEMTB, xreceptorEMTB, xpassEMTB, xasuntoEMTB, xhostEMTB, xintpuertoEMTB, xsslEMTB);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void VirtualHostTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void colaTB_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
