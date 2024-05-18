using Microsoft.AspNetCore.Http;

namespace SmartHome.Classes.Receipt
{
    public class ImageUploadDTO
    {
        public long ReceiptID { get; set; }
        public IFormFile? file { get; set; }
        public string ImgName { get; set; }
        public UploadTypes UploadType { get; set; } = UploadTypes.Image;
    }

    public enum UploadTypes
    {
        Image
    }
}
