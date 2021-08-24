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
        private IConnection connection;
        private IModel channel;
        private string folderPath;

        public Principal()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rellenaDatosIniForm();
        }

        private void rellenaDatosIniForm()
        {
            HostTB.Text = Funciones.obtenerValoresIni("HOST");
            VirtualHostTB.Text = Funciones.obtenerValoresIni("VIRTUALHOST");
            PuertoTB.Text = Funciones.obtenerValoresIni("PUERTO");
            UserTB.Text = Funciones.obtenerValoresIni("USUARIO");
            PassTB.Text = Funciones.obtenerValoresIni("CONTRASENA");
            colaTB.Text = Funciones.obtenerValoresIni("COLA");
            empSAGETB.Text = Funciones.obtenerValoresIni("EMPRESA_SAGE");
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
            Funciones.borrarValoresIni(@"C:\COMPARTIDA\ConfigCIERabbit.ini");
            Funciones.guardarValoresIni("HOST", HostTB.Text);
            Funciones.guardarValoresIni("VIRTUALHOST", VirtualHostTB.Text);
            Funciones.guardarValoresIni("PUERTO", PuertoTB.Text);
            Funciones.guardarValoresIni("USUARIO", UserTB.Text);
            Funciones.guardarValoresIni("CONTRASENA", PassTB.Text);
            Funciones.guardarValoresIni("COLA", colaTB.Text);
            Funciones.guardarValoresIni("EMPRESA_SAGE", empSAGETB.Text);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //Comprobamos si hay ficheros y los procesamos. Este paso se hace al principio por si hubiera algún archivo por procesar
            Funciones.procesar_ficheros();
            //Descargamos los mensajes y creamos lo ficheros
            RabbitConsumer cliente = new RabbitConsumer();
            cliente.Connect();
            cliente.ConsumeMessages();

            //Comprobamos si hay ficheros y los procesamos
            Funciones.procesar_ficheros();

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            BaseDatos.ConectarBD();
        }
    }
}
