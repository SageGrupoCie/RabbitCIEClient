using System;
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
            //Parámetros empresa
            servidorTB.Text = Funciones.obtenerValoresIni("SERVIDOR","BD");
            userBDTB.Text = Funciones.obtenerValoresIni("USUARIO", "BD");
            passBDTB.Text = Funciones.obtenerValoresIni("PASSWORD", "BD");
            BDTB.Text = Funciones.obtenerValoresIni("DATABASE", "BD");
            empSAGETB.Text = Funciones.obtenerValoresIni("EMPRESA_SAGE", "BD");
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
            Funciones.guardarValoresIni("EMPRESA_SAGE", empSAGETB.Text);
        }
        public void procesarFichero()
        {
            //Creamos fichero de logs
            Logs lg = new Logs();
            //Comprobamos si hay ficheros y los procesamos. Este paso se hace al principio por si hubiera algún archivo por procesar
            Funciones.procesar_ficheros(lg, "SI");
            //Descargamos los mensajes y creamos lo ficheros
            RabbitConsumer cliente = new RabbitConsumer();
            cliente.Connect(lg);
            cliente.ConsumeMessages(lg);

            //Comprobamos si hay ficheros y los procesamos
            Funciones.procesar_ficheros(lg);

            //Enviamos el log si fuera necesario
            lg.enviarLogEmail();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string comprobParam = comprobarParametros();
            string[] arrComParam = comprobParam.Split('#');
            if (arrComParam[0] == "OK")
            {
                procesarFichero();
            }
            else
            {
                string msg = "Debe informar los siguientes datos correctamente:";
                for (int i = 1; i < arrComParam.Length; i++)
                {
                    msg += "\r\n" + new string(' ', 5) + arrComParam[i];
                }
                MessageBox.Show(msg,"ERROR",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
    }
}
