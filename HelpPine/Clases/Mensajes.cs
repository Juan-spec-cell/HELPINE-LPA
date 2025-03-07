using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpPine.Clases
{
    public class Mensajes
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int RemitenteId { get; set; }
        public int DestinatarioId { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
    }
}