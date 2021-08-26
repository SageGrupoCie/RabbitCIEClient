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
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Principal(args));
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
                    MessageBox.Show("La aplicaci�n ya se encuentra en uso. Debe cerrar antes la instancia existente");
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
