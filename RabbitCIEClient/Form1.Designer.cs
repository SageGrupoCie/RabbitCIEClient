﻿
namespace RabbitCIEClient
{
    partial class Principal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ParamTB = new System.Windows.Forms.TabPage();
            this.colaTB = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.PuertoTB = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PassTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.UserTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.VirtualHostTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.HostTB = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.BDTB = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.empSAGETB = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.passBDTB = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.userBDTB = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.servidorTB = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sslEMTB = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.puertoEMTB = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.hostEMTB = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.asuntoEMTB = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.passEMTB = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.receptorEMTB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.emisorEMTB = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.ParamTB.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ParamTB);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(489, 319);
            this.tabControl1.TabIndex = 33;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // ParamTB
            // 
            this.ParamTB.Controls.Add(this.colaTB);
            this.ParamTB.Controls.Add(this.label7);
            this.ParamTB.Controls.Add(this.button3);
            this.ParamTB.Controls.Add(this.label5);
            this.ParamTB.Controls.Add(this.PuertoTB);
            this.ParamTB.Controls.Add(this.label4);
            this.ParamTB.Controls.Add(this.PassTB);
            this.ParamTB.Controls.Add(this.label3);
            this.ParamTB.Controls.Add(this.UserTB);
            this.ParamTB.Controls.Add(this.label2);
            this.ParamTB.Controls.Add(this.VirtualHostTB);
            this.ParamTB.Controls.Add(this.label1);
            this.ParamTB.Controls.Add(this.HostTB);
            this.ParamTB.Location = new System.Drawing.Point(4, 24);
            this.ParamTB.Name = "ParamTB";
            this.ParamTB.Padding = new System.Windows.Forms.Padding(3);
            this.ParamTB.Size = new System.Drawing.Size(481, 291);
            this.ParamTB.TabIndex = 0;
            this.ParamTB.Text = "Parámetros RabbitMQ";
            this.ParamTB.UseVisualStyleBackColor = true;
            // 
            // colaTB
            // 
            this.colaTB.Location = new System.Drawing.Point(87, 163);
            this.colaTB.Name = "colaTB";
            this.colaTB.Size = new System.Drawing.Size(265, 23);
            this.colaTB.TabIndex = 5;
            this.colaTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.colaTB_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(14, 166);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 15);
            this.label7.TabIndex = 54;
            this.label7.Text = "Cola";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(277, 217);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 53;
            this.button3.Text = "Guardar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(14, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 51;
            this.label5.Text = "VirtualHost";
            // 
            // PuertoTB
            // 
            this.PuertoTB.Location = new System.Drawing.Point(87, 134);
            this.PuertoTB.Name = "PuertoTB";
            this.PuertoTB.Size = new System.Drawing.Size(265, 23);
            this.PuertoTB.TabIndex = 4;
            this.PuertoTB.TextChanged += new System.EventHandler(this.PuertoTB_TextChanged);
            this.PuertoTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PuertoTB_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(14, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 15);
            this.label4.TabIndex = 49;
            this.label4.Text = "Contraseña";
            // 
            // PassTB
            // 
            this.PassTB.Location = new System.Drawing.Point(87, 105);
            this.PassTB.Name = "PassTB";
            this.PassTB.PasswordChar = '*';
            this.PassTB.Size = new System.Drawing.Size(265, 23);
            this.PassTB.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(14, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 15);
            this.label3.TabIndex = 47;
            this.label3.Text = "Usuario";
            // 
            // UserTB
            // 
            this.UserTB.Location = new System.Drawing.Point(87, 76);
            this.UserTB.Name = "UserTB";
            this.UserTB.Size = new System.Drawing.Size(265, 23);
            this.UserTB.TabIndex = 2;
            this.UserTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.UserTB_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(14, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 45;
            this.label2.Text = "Puerto";
            // 
            // VirtualHostTB
            // 
            this.VirtualHostTB.Location = new System.Drawing.Point(87, 47);
            this.VirtualHostTB.Name = "VirtualHostTB";
            this.VirtualHostTB.Size = new System.Drawing.Size(265, 23);
            this.VirtualHostTB.TabIndex = 1;
            this.VirtualHostTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.VirtualHostTB_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(14, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 15);
            this.label1.TabIndex = 43;
            this.label1.Text = "Host";
            // 
            // HostTB
            // 
            this.HostTB.Location = new System.Drawing.Point(87, 18);
            this.HostTB.Name = "HostTB";
            this.HostTB.Size = new System.Drawing.Size(265, 23);
            this.HostTB.TabIndex = 0;
            this.HostTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HostTB_KeyPress);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.BDTB);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.empSAGETB);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.passBDTB);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.userBDTB);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.servidorTB);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(481, 291);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Parámetros SAGE";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(81, 163);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(133, 23);
            this.button5.TabIndex = 63;
            this.button5.Text = "Comprobar Conexión";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label12.Location = new System.Drawing.Point(8, 103);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(79, 15);
            this.label12.TabIndex = 62;
            this.label12.Text = "Base de datos";
            // 
            // BDTB
            // 
            this.BDTB.Location = new System.Drawing.Point(98, 100);
            this.BDTB.Name = "BDTB";
            this.BDTB.Size = new System.Drawing.Size(248, 23);
            this.BDTB.TabIndex = 9;
            this.BDTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BDTB_KeyPress);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(271, 163);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 60;
            this.button4.Text = "Guardar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // empSAGETB
            // 
            this.empSAGETB.Location = new System.Drawing.Point(98, 129);
            this.empSAGETB.Name = "empSAGETB";
            this.empSAGETB.Size = new System.Drawing.Size(248, 23);
            this.empSAGETB.TabIndex = 10;
            this.empSAGETB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.empSAGETB_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label8.Location = new System.Drawing.Point(8, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 15);
            this.label8.TabIndex = 58;
            this.label8.Text = "Empresa SAGE";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label10.Location = new System.Drawing.Point(8, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 15);
            this.label10.TabIndex = 53;
            this.label10.Text = "Contraseña";
            // 
            // passBDTB
            // 
            this.passBDTB.Location = new System.Drawing.Point(81, 71);
            this.passBDTB.Name = "passBDTB";
            this.passBDTB.PasswordChar = '*';
            this.passBDTB.Size = new System.Drawing.Size(265, 23);
            this.passBDTB.TabIndex = 8;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(8, 45);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 15);
            this.label11.TabIndex = 51;
            this.label11.Text = "Usuario";
            // 
            // userBDTB
            // 
            this.userBDTB.Location = new System.Drawing.Point(81, 42);
            this.userBDTB.Name = "userBDTB";
            this.userBDTB.Size = new System.Drawing.Size(265, 23);
            this.userBDTB.TabIndex = 7;
            this.userBDTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.userBDTB_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label9.Location = new System.Drawing.Point(8, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 15);
            this.label9.TabIndex = 45;
            this.label9.Text = "Servidor";
            // 
            // servidorTB
            // 
            this.servidorTB.Location = new System.Drawing.Point(81, 13);
            this.servidorTB.Name = "servidorTB";
            this.servidorTB.Size = new System.Drawing.Size(265, 23);
            this.servidorTB.TabIndex = 6;
            this.servidorTB.TextChanged += new System.EventHandler(this.servidorTB_TextChanged);
            this.servidorTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.servidorTB_KeyPress);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.asuntoEMTB);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.passEMTB);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.receptorEMTB);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.emisorEMTB);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(481, 291);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Reporte Email";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(390, 250);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 61;
            this.button1.Text = "Guardar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sslEMTB);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.puertoEMTB);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.hostEMTB);
            this.groupBox1.Location = new System.Drawing.Point(10, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(374, 132);
            this.groupBox1.TabIndex = 52;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SMTP";
            // 
            // sslEMTB
            // 
            this.sslEMTB.AutoSize = true;
            this.sslEMTB.Location = new System.Drawing.Point(242, 55);
            this.sslEMTB.Name = "sslEMTB";
            this.sslEMTB.Size = new System.Drawing.Size(116, 19);
            this.sslEMTB.TabIndex = 49;
            this.sslEMTB.Text = "Requiere SSL/TLS";
            this.sslEMTB.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label17.Location = new System.Drawing.Point(20, 54);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(42, 15);
            this.label17.TabIndex = 47;
            this.label17.Text = "Puerto";
            // 
            // puertoEMTB
            // 
            this.puertoEMTB.Location = new System.Drawing.Point(93, 51);
            this.puertoEMTB.Name = "puertoEMTB";
            this.puertoEMTB.Size = new System.Drawing.Size(93, 23);
            this.puertoEMTB.TabIndex = 46;
            this.puertoEMTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.puertoEMTB_KeyPress);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label16.Location = new System.Drawing.Point(20, 25);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 15);
            this.label16.TabIndex = 45;
            this.label16.Text = "Host";
            // 
            // hostEMTB
            // 
            this.hostEMTB.Location = new System.Drawing.Point(93, 22);
            this.hostEMTB.Name = "hostEMTB";
            this.hostEMTB.Size = new System.Drawing.Size(265, 23);
            this.hostEMTB.TabIndex = 44;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label15.Location = new System.Drawing.Point(10, 105);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(45, 15);
            this.label15.TabIndex = 51;
            this.label15.Text = "Asunto";
            // 
            // asuntoEMTB
            // 
            this.asuntoEMTB.Location = new System.Drawing.Point(103, 102);
            this.asuntoEMTB.Name = "asuntoEMTB";
            this.asuntoEMTB.Size = new System.Drawing.Size(265, 23);
            this.asuntoEMTB.TabIndex = 50;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label14.Location = new System.Drawing.Point(10, 76);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 15);
            this.label14.TabIndex = 49;
            this.label14.Text = "Contraseña";
            // 
            // passEMTB
            // 
            this.passEMTB.Location = new System.Drawing.Point(103, 73);
            this.passEMTB.Name = "passEMTB";
            this.passEMTB.PasswordChar = '*';
            this.passEMTB.Size = new System.Drawing.Size(265, 23);
            this.passEMTB.TabIndex = 48;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label13.Location = new System.Drawing.Point(10, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(83, 15);
            this.label13.TabIndex = 47;
            this.label13.Text = "Email receptor";
            // 
            // receptorEMTB
            // 
            this.receptorEMTB.Location = new System.Drawing.Point(103, 44);
            this.receptorEMTB.Name = "receptorEMTB";
            this.receptorEMTB.Size = new System.Drawing.Size(265, 23);
            this.receptorEMTB.TabIndex = 46;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(10, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 15);
            this.label6.TabIndex = 45;
            this.label6.Text = "Email emisor";
            // 
            // emisorEMTB
            // 
            this.emisorEMTB.Location = new System.Drawing.Point(103, 15);
            this.emisorEMTB.Name = "emisorEMTB";
            this.emisorEMTB.Size = new System.Drawing.Size(265, 23);
            this.emisorEMTB.TabIndex = 44;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(382, 337);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 23);
            this.button2.TabIndex = 53;
            this.button2.Text = "Procesar datos";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 369);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tabControl1);
            this.Name = "Principal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enlace RabbitMQ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.ParamTB.ResumeLayout(false);
            this.ParamTB.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ParamTB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PuertoTB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PassTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UserTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox VirtualHostTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox HostTB;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox colaTB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox empSAGETB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox passBDTB;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox userBDTB;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox servidorTB;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox BDTB;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox sslEMTB;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox puertoEMTB;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox hostEMTB;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox asuntoEMTB;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox passEMTB;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox receptorEMTB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox emisorEMTB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

