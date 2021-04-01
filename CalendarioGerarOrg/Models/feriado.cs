using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarioGerarOrg.Models
{
    public class feriado
    {
        [Key]
        public int idferiado { get; set; }
        public int idcidade { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dia { get; set; }

        [ForeignKey("idcidade")]
        [InverseProperty("feriados")]
        public virtual cidade  idcidadeNavigation { get; set; }
    }
    public class recesso
    {
        [Key]
        public int idrecesso { get; set; }
        public int idcidade { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dia { get; set; }
        public int tipo { get; set; }

        [ForeignKey("idcidade")]
        [InverseProperty("recessos")]
        public virtual cidade idcidadeNavigation { get; set; }
    }

    public enum recessoTipo {
        Inicial=0,
        Extra =1,
        Regular =2,
        Recesso_Gerar_Inicial=4,
        Recesso_Gerar_Extra =5,
        Recesso_Gerar_Regular = 3
    }
}
