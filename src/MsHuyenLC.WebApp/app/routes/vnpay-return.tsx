import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router";
import { CheckCircle, XCircle, Clock, ArrowLeft } from "lucide-react";
import { type VNPayCallbackRequest, callback } from "~/apis/ThanhToan";

export default function VNPayReturnPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [loading, setLoading] = useState(true);
  const [paymentStatus, setPaymentStatus] = useState<"success" | "failed" | "pending">("pending");
  const [paymentInfo, setPaymentInfo] = useState({
    orderInfo: "",
    amount: "",
    transactionNo: "",
    bankCode: "",
    responseCode: "",
    message: "",
  });

  useEffect(() => {
    processVNPayCallback();
  }, []);

  const processVNPayCallback = async () => {
    setLoading(true);

    const vnp_Amount = searchParams.get("vnp_Amount");
    const vnp_BankCode = searchParams.get("vnp_BankCode");
    const vnp_BankTranNo = searchParams.get("vnp_BankTranNo");
    const vnp_CardType = searchParams.get("vnp_CardType");
    const vnp_OrderInfo = searchParams.get("vnp_OrderInfo");
    const vnp_PayDate = searchParams.get("vnp_PayDate");
    const vnp_ResponseCode = searchParams.get("vnp_ResponseCode");
    const vnp_TmnCode = searchParams.get("vnp_TmnCode");
    const vnp_TransactionNo = searchParams.get("vnp_TransactionNo");
    const vnp_TransactionStatus = searchParams.get("vnp_TransactionStatus");
    const vnp_TxnRef = searchParams.get("vnp_TxnRef");
    const vnp_SecureHash = searchParams.get("vnp_SecureHash");
    const vnp_SecureHashType = searchParams.get("vnp_SecureHashType");

    if (!vnp_ResponseCode || !vnp_TxnRef) {
      setPaymentStatus("failed");
      setPaymentInfo({
        ...paymentInfo,
        message: "Thông tin thanh toán không hợp lệ",
      });
      setLoading(false);
      return;
    }

    // Tạo callback request
    const callbackRequest: VNPayCallbackRequest = {
      vnp_TmnCode: vnp_TmnCode || undefined,
      vnp_Amount: vnp_Amount || undefined,
      vnp_BankCode: vnp_BankCode || undefined,
      vnp_BankTranNo: vnp_BankTranNo || undefined,
      vnp_CardType: vnp_CardType || undefined,
      vnp_OrderInfo: vnp_OrderInfo || undefined,
      vnp_TransactionNo: vnp_TransactionNo || undefined,
      vnp_TransactionStatus: vnp_TransactionStatus || undefined,
      vnp_PayDate: vnp_PayDate || undefined,
      vnp_ResponseCode: vnp_ResponseCode || undefined,
      vnp_TxnRef: vnp_TxnRef || undefined,
      vnp_SecureHash: vnp_SecureHash || undefined,
      vnp_SecureHashType: vnp_SecureHashType || undefined,
    };

    try {
      const response = await callback(callbackRequest);
      console.log("Callback response:", response);

      const isSuccess = response.success;
      
      setPaymentInfo({
        orderInfo: vnp_OrderInfo || "",
        amount: vnp_Amount ? (parseInt(vnp_Amount) / 100).toLocaleString("vi-VN") : "0",
        transactionNo: vnp_TransactionNo || "",
        bankCode: vnp_BankCode || "",
        responseCode: vnp_ResponseCode,
        message: isSuccess ? "Giao dịch thành công" : (response.message || getResponseMessage(vnp_ResponseCode)),
      });

      setPaymentStatus(isSuccess ? "success" : "failed");
    } catch (error) {
      console.error("Callback error:", error);
      setPaymentStatus("failed");
      setPaymentInfo({
        orderInfo: vnp_OrderInfo || "",
        amount: vnp_Amount ? (parseInt(vnp_Amount) / 100).toLocaleString("vi-VN") : "0",
        transactionNo: vnp_TransactionNo || "",
        bankCode: vnp_BankCode || "",
        responseCode: vnp_ResponseCode || "",
        message: "Lỗi khi xác thực giao dịch",
      });
    } finally {
      setLoading(false);
    }

    setTimeout(() => {
      navigate("/khoa-hoc-cua-toi");
    }, 5000);
  };

  const getResponseMessage = (responseCode: string): string => {
    const messages: { [key: string]: string } = {
      "00": "Giao dịch thành công",
      "07": "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
      "09": "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
      "10": "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
      "11": "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
      "12": "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
      "13": "Giao dịch không thành công do: Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).",
      "24": "Giao dịch không thành công do: Khách hàng hủy giao dịch",
      "51": "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
      "65": "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
      "75": "Ngân hàng thanh toán đang bảo trì.",
      "79": "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.",
    };
    return messages[responseCode] || "Giao dịch không thành công";
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center bg-white p-8 rounded-xl shadow-lg max-w-md">
          <Clock className="w-20 h-20 text-blue-500 mx-auto mb-4 animate-spin" />
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Đang xử lý...</h2>
          <p className="text-gray-600">Vui lòng chờ trong giây lát</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
      <div className="max-w-2xl w-full">
        <div className="bg-white rounded-xl shadow-lg overflow-hidden">
          {/* Header */}
          <div className={`p-8 text-center ${
            paymentStatus === "success" 
              ? "bg-gradient-to-r from-green-500 to-green-600" 
              : "bg-gradient-to-r from-red-500 to-red-600"
          }`}>
            {paymentStatus === "success" ? (
              <>
                <CheckCircle className="w-24 h-24 text-white mx-auto mb-4" />
                <h1 className="text-3xl font-bold text-white mb-2">Thanh toán thành công!</h1>
                <p className="text-green-100">Cảm ơn bạn đã đăng ký khóa học</p>
              </>
            ) : (
              <>
                <XCircle className="w-24 h-24 text-white mx-auto mb-4" />
                <h1 className="text-3xl font-bold text-white mb-2">Thanh toán không thành công</h1>
                <p className="text-red-100">Vui lòng thử lại sau</p>
              </>
            )}
          </div>

          {/* Content */}
          <div className="p-8">
            <div className="space-y-4">
              <div className="flex justify-between py-3 border-b border-gray-200">
                <span className="text-gray-600 font-medium">Trạng thái:</span>
                <span className={`font-bold ${
                  paymentStatus === "success" ? "text-green-600" : "text-red-600"
                }`}>
                  {paymentInfo.message}
                </span>
              </div>

              {paymentInfo.orderInfo && (
                <div className="flex justify-between py-3 border-b border-gray-200">
                  <span className="text-gray-600 font-medium">Nội dung:</span>
                  <span className="font-semibold text-gray-900 text-right">{paymentInfo.orderInfo}</span>
                </div>
              )}

              {paymentInfo.amount && (
                <div className="flex justify-between py-3 border-b border-gray-200">
                  <span className="text-gray-600 font-medium">Số tiền:</span>
                  <span className="font-bold text-blue-600 text-xl">{paymentInfo.amount} ₫</span>
                </div>
              )}

              {paymentInfo.transactionNo && (
                <div className="flex justify-between py-3 border-b border-gray-200">
                  <span className="text-gray-600 font-medium">Mã giao dịch:</span>
                  <span className="font-mono text-gray-900">{paymentInfo.transactionNo}</span>
                </div>
              )}

              {paymentInfo.bankCode && (
                <div className="flex justify-between py-3 border-b border-gray-200">
                  <span className="text-gray-600 font-medium">Ngân hàng:</span>
                  <span className="font-semibold text-gray-900">{paymentInfo.bankCode}</span>
                </div>
              )}

              <div className="flex justify-between py-3">
                <span className="text-gray-600 font-medium">Mã phản hồi:</span>
                <span className="font-mono text-gray-900">{paymentInfo.responseCode}</span>
              </div>
            </div>

            {/* Notification */}
            <div className={`mt-6 p-4 rounded-lg ${
              paymentStatus === "success" 
                ? "bg-green-50 border border-green-200" 
                : "bg-yellow-50 border border-yellow-200"
            }`}>
              <p className={`text-sm ${
                paymentStatus === "success" ? "text-green-800" : "text-yellow-800"
              }`}>
                {paymentStatus === "success" 
                  ? "✓ Giao dịch đã được xác nhận. Bạn có thể bắt đầu học ngay bây giờ!"
                  : "⚠ Nếu bạn gặp vấn đề, vui lòng liên hệ với bộ phận hỗ trợ để được trợ giúp."}
              </p>
            </div>

            {/* Auto redirect notice */}
            <div className="mt-6 text-center">
              <p className="text-sm text-gray-500 mb-4">
                Tự động chuyển hướng sau 5 giây...
              </p>
              <button
                onClick={() => navigate("/khoa-hoc-cua-toi")}
                className="inline-flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                <ArrowLeft className="w-4 h-4" />
                Quay lại khóa học của tôi
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
