using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarioGerarOrg.Models
{
    public class cidade
    {
        [Key]
        public int idcidade { get; set; }
        public string nome { get; set; }
        public string estado { get; set; }
        [NotMapped]
        public int idsubsede { get; set; }
        [InverseProperty("idcidadeNavigation")]
        public virtual ICollection<feriado> feriados { get; set; }

        [InverseProperty("idcidadeNavigation")]
        public virtual ICollection<recesso> recessos { get; set; }
        
        //[ForeignKey("idcidade")]
        public virtual ICollection<subsede> subsedes { get; set; }

    }


    public class subsede
    {
        [Key]
        public int idsubsede { get; set; }
        public int idcidade { get; set; }
        public string nome { get; set; }
        [ForeignKey("idcidade")]
        //[InverseProperty("extra")]
        public virtual cidade idcidadeNavigation { get; set; }
    }

}
