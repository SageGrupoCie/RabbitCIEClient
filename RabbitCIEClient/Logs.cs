using System;
using System.Collections.Generic;
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
        public bool hayErrores(string tipolist)      
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
    }
}
