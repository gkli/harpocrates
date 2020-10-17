using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Harpocrates.Runtime.Tracking.SqlServer.Ef.Entities
{
    public class Attempt : Common.Contracts.Tracking.ProcessingAttempt
    {
        public Attempt()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttemptId { get; set; }

        public Transaction Transaction { get; set; }
        //public Common.Contracts.ProcessResult.ProcessingStatus StartingStatus { get; set; }
        //public Common.Contracts.ProcessResult.ProcessingStatus? EndingStatus { get; set; }

        //public DateTime StartTimeUtc { get; set; }
        //public DateTime? EndTimeUtc { get; set; }

        //public Common.Contracts.Tracking.ProcessingAttempt ToProcessingAttempt()
        //{
        //    return new Common.Contracts.Tracking.ProcessingAttempt()
        //    {
        //        StartingStatus = StartingStatus,
        //        EndingStatus = EndingStatus,
        //        EndTimeUtc = EndTimeUtc,
        //        StartTimeUtc = StartTimeUtc
        //    };
        //}
    }
}
