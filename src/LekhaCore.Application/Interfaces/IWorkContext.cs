using Makuri.Core.Domain.User;

namespace Makuri.Core
{
    public interface IWorkContext
    {
        /// <summary>
        /// Gets the current user
        /// </summary>
        AppUser CurrentUser { get; }
    }
}
