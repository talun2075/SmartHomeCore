using SmartHome.Classes.SmartHome.Data;
using System.Threading.Tasks;

namespace SmartHome.Classes.Database
{
    public interface IDatabaseWrapper
    {
        void Close();
        Task<bool> Open();
        Task<bool> ReadButtons();
        Task<bool> UpdateButton(Button button);
    }
}