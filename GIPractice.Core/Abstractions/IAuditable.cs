using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIPractice.Core.Abstractions
{
    public interface IAuditable
    {
        DateTime CreatedAtUtc { get; set; }
        DateTime? UpdatedAtUtc { get; set; }
        string? CreatedBy { get; set; }
        string? UpdatedBy { get; set; }
    }
}
