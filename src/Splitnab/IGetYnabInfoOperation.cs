using System.Threading.Tasks;
using Splitnab.Model;

namespace Splitnab
{
    /// <summary>
    /// Interface to get the required information from YNAB for Splitnab
    /// </summary>
    public interface IGetYnabInfoOperation
    {
        /// <summary>
        /// Invoke the operation to fetch the necessary YNAB info.
        /// </summary>
        /// <param name="appSettings">The appsettings.json object</param>
        /// <returns>The required YNAB info as <see cref="YnabInfo"/>, otherwise null.</returns>
        public Task<YnabInfo?> Invoke(AppSettings appSettings);
    }
}
