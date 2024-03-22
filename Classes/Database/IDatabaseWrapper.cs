using SmartHome.Classes.Receipt;
using SmartHome.Classes.SmartHome.Data;
using System.Threading.Tasks;

namespace SmartHome.Classes.Database
{
    public interface IDatabaseWrapper
    {
        Task<ReceiptDTO> ReadReceiptData(int receiptid);
        Task<bool> ReadButtons();
        Task<bool> UpdateButton(Button button);
    }
}