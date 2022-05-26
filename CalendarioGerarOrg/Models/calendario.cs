using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarioGerarOrg.Models
{
    public class calendario
    {
        [Key]
        public int idcalendario {get;set;}
        public string empresa { get; set; }
        public string aprendiz { get; set; }
        public DateTime datainicial { get; set; }
        public DateTime datafinal { get; set; }
        public int semanateoria { get; set; }
        public int cargahoraria { get; set; }
        public int cargateorica { get; set; }
        public int cargapratica { get; set; }
        public double cargainicial { get; set; }
        public double cargainicialhoras { get; set; }
        public int diafolga { get; set; } = (int)DayOfWeek.Saturday;

        [InverseProperty("idcalendarioNavigation")]
        public virtual ICollection<calendarioextra> extras { get; set; }

        [NotMapped] // montar parte base para isso !
        public ICollection<calendarioextra> iniciais { get; set; }

        public int idcidade { get; set; }
        [ForeignKey("idcidade")]
        //[InverseProperty("extra")]
        public virtual cidade idcidadeNavigation { get; set; }
        
        [NotMapped]
        public List<DateTime> teoricas { get; set; }

        [NotMapped]
        public List<DateTime> praticas { get; set; }

        [NotMapped]
        public List<DateTime> feriados { get; set; }
        
        [NotMapped]
        public List<calendarioextra> manuaisteorica { get; set; }

        [NotMapped]
        public List<calendarioextra> manuaispratica { get; set; }

        [NotMapped]
        public ICollection<calendarioextra> ferias { get; set; }
        
        [NotMapped]
        public List<DateTime> feriasdias { get; set; }
        [NotMapped]
        public List<calendarioextra> suspensao { get; set; }

        [NotMapped]
        public List<DateTime> suspensaodias { get; set; }
        public Double mesestotal { get; set; }

        [NotMapped]
        public List<recesso> recessos { get; set; }

        [NotMapped]
        public List<DateTime> recessosgerar { get; set; }

        [NotMapped]
        public List<dynamic> log { get; set; }

        [NotMapped]
        public int reducaoper { get; set; }

        [NotMapped]
        public List<DateTime> reducaodias { get; set; }
        [NotMapped]
        public List<calendarioextra> reducao { get; set; }

        [NotMapped]
        public List<calendarioextra> teoricodias { get; set; }
        [NotMapped]
        public List<calendarioextra> praticasdias { get; set; }
        [NotMapped]
        public List<calendarioextra> estabilidadepraticas { get; set; }
        [NotMapped]
        public List<calendarioextra> estabilidadeteoricas { get; set; }
        [NotMapped]
        public bool datafixa { get; set; }

        [NotMapped]
        public bool estabilidadesuspencao { get; set; }

        [NotMapped]
        public List<calendarioextra> estabilidadesuspencaodias { get; set; }
    }

    public class calendarioextra {
        [Key]
        public int idcalendarioextra { get; set; }
        public DateTime  datainicial { get; set; }
        public DateTime datafinal { get; set; }
        public int idcalendario { get; set; }
        public bool inicial { get; set; }
        [ForeignKey("idcalendario")]        
        [InverseProperty("extra")]
        public virtual calendario idcalendarioNavigation { get; set; }
    }
}
