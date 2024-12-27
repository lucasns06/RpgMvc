using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpgMvc.Models
{
    public class ArmaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Dano { get; set; }
        public PersonagemViewModel Personagem { get; set; } = new PersonagemViewModel();
        public int PersonagemId { get; set; }
    }
}