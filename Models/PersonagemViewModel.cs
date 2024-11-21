using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgMvc.Models
{
    public class PersonagemViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int PontosVida { get; set; }
        public int Forca { get; set; }
        public int Defesa { get; set; }
        public int Inteligencia { get; set; }
        public int Classe { get; set; }
        public string FotoPersonagem { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public object Usuario { get; set; }  = string.Empty;
        public object Arma { get; set; }  = string.Empty;  
        public int Disputas { get; set; }
        public int Vitorias { get; set; }
        public int Derrotas { get; set; }
    }
}