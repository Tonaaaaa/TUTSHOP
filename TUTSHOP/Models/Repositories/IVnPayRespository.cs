using TUTSHOP.Models.Entities.VNPay;

namespace TUTSHOP.Models.Repositories
{
    public interface IVnPayRespository
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
