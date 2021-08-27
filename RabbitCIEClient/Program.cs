using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RabbitCIEClient
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (!existeUnaInstanciaPrevia(args))
            {
                if (args.Length == 0)
                {
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Principal());
                }
                else
                {
                    Principal pr = new Principal(true);
                    pr.procesarFichero();
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private static bool existeUnaInstanciaPrevia(string[] args)
        {


            string currPrsName = Process.GetCurrentProcess().ProcessName;


            Process[] allProcessWithThisName
                         = Process.GetProcessesByName(currPrsName);

      
            if (allProcessWithThisName.Length > 1)
            {
                if (args.Length == 0)
                {
                    MessageBox.Show("La aplicación ya se encuentra en uso. Debe cerrar antes la instancia existente");
                }
                return true; 
            }
            else
            {
                return false; 
            }
        }
    }
}
