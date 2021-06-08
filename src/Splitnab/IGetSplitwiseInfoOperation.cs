using System.Threading.Tasks;
using Splitnab.Model;

namespace Splitnab
{
    /// <summary>
    /// Interface to get the required information from Splitwise for Splitnab
    /// </summary>
    public interface IGetSplitwiseInfoOperation
    {
        /// <summary>
        /// Invoke the operation to fetch the necessary Splitwise info.
        /// </summary>
        /// <param name="appSettings">The appsettings.json object</param>
        /// <returns>The required Splitwise info as <see cref="SplitwiseInfo"/>, otherwise null.</returns>
        public Task<SplitwiseInfo?> Invoke(AppSettings appSettings);
    }
}
