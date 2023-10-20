using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Core
{
    public interface IResolver
    {
        void Resolve();
        void Reject();
    }
}
