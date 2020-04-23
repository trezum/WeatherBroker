using System;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IRunnableAsync : IDisposable
    {
        Task RunAsync();
    }
}
