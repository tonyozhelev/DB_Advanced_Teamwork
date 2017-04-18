
namespace BetManager.Models
{
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    public class MatchesBets
    {
        [Key, Column(Order = 0)]
        public int BetId { get; set; }
        public virtual Bet Bet { get; set; }
        [Key,Column(Order = 1)]
        public int MatchId { get; set; }
        public virtual Match Match { get; set; }
        public string BetPrediction { get; set; }
        public string Result { get; set; }
    }
}
