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
        public Principal(string[] args)
        {
            if (args.Length == 0)
            {
                InitializeComponent();
            }
            else
            {
                procesarFichero();
                Application.Exit();
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
            if (lg.hayErrores(""))
            {
                //Enviamos mail con reporte
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            procesarFichero();

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
    }
}
