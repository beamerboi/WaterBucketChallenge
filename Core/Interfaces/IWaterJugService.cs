using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IWaterJugService
    {
        List<BucketState> Solve(int x, int y, int z);
    }
}
