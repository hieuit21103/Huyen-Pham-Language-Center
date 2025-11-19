import { useState, useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router";
import {
  CreditCard, Building2, Smartphone, Check,
  AlertCircle, ArrowLeft, DollarSign, Calendar,
  Clock, BookOpen
} from "lucide-react";
import { getProfile } from "~/apis/Profile";
import { getDangKy, updateDangKy, getDangKys } from "~/apis/DangKy";
import { getThanhToanByDangKyId, createThanhToan, createVNPayUrl } from "~/apis/ThanhToan";
import { VaiTro } from "~/types/index";

interface DangKy {
  id?: string;
  ngayDangKy?: string;
  trangThai?: number;
  khoaHoc?: {
    tenKhoaHoc?: string;
    moTa?: string;
    hocPhi?: number;
    thoiLuong?: number;
    ngayKhaiGiang?: string;
  };
}

export default function PaymentPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const dangKyId = searchParams.get("dangKyId");

  const [loading, setLoading] = useState(true);
  const [processing, setProcessing] = useState(false);
  const [registration, setRegistration] = useState<DangKy | null>(null);
  const [paymentMethod, setPaymentMethod] = useState<"vnpay" | "cash">("vnpay");
  const [message, setMessage] = useState("");

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);

    const profileRes = await getProfile();
    if (!profileRes.success || !profileRes.data) {
      navigate("/dang-nhap");
      return;
    }

    if (profileRes.data.vaiTro !== VaiTro.HocVien) {
      navigate("/");
      return;
    }

    if (dangKyId) {
      const regRes = await getDangKy(dangKyId);


      if (regRes.success && regRes.data) {
        const reg = regRes.data as DangKy;

        if (reg.trangThai === 1) {
          const paymentRes = await getThanhToanByDangKyId(dangKyId);
          if (paymentRes.success && paymentRes.data?.trangThai === 1) {
            setMessage("Đăng ký này đã được thanh toán!");
            setLoading(false);
            setTimeout(() => navigate("/khoa-hoc-cua-toi"), 2000);
            return;
          }

          setRegistration(reg);
        } else {
          setMessage(`Đăng ký này không thể thanh toán! (Trạng thái: ${reg.trangThai})`);
          setLoading(false);
          setTimeout(() => navigate("/khoa-hoc-cua-toi"), 2000);
          return;
        }
      } else {
        setMessage("Không tìm thấy đăng ký!");
        setLoading(false);
        setTimeout(() => navigate("/khoa-hoc-cua-toi"), 2000);
        return;
      }
    } else {
      const dangKysRes = await getDangKys();

      if (dangKysRes.success && dangKysRes.data) {
        const dataArray = Array.isArray(dangKysRes.data)
          ? dangKysRes.data
          : dangKysRes.data.items || [];

        let foundRegistration = null;
        for (const reg of dataArray) {
          if (reg.trangThai === 1) {
            const paymentRes = await getThanhToanByDangKyId(reg.id!);
            if (!paymentRes.success || !paymentRes.data) {
              foundRegistration = reg;
              break;
            }
          }
        }

        if (foundRegistration) {
          setRegistration(foundRegistration);
        } else {
          setMessage("Không có đăng ký nào cần thanh toán!");
          setTimeout(() => navigate("/khoa-hoc-cua-toi"), 2000);
          return;
        }
      } else {
        navigate("/khoa-hoc-cua-toi");
        return;
      }
    }

    setLoading(false);
  };

  const formatCurrency = (amount?: number) => {
    if (!amount) return "0 ₫";
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return "—";
    return new Date(dateString).toLocaleDateString("vi-VN");
  };

  const handlePayment = async () => {
    if (!registration?.id) return;

    setProcessing(true);
    setMessage("");

    try {
      if (paymentMethod === "vnpay") {
        let paymentId = "";
        const existingPayment = await getThanhToanByDangKyId(registration.id);
          console.log("existingPayment", existingPayment.data);

        if (existingPayment.success && existingPayment.data?.id) {
          paymentId = existingPayment.data.id;
        } else {
          setMessage("Lỗi khi lấy thông tin thanh toán!");
          setProcessing(false);
          return;
        }

        const returnUrl = `${window.location.origin}/vnpay-return`;
        const vnpayRes = await createVNPayUrl(paymentId, returnUrl);

        if (vnpayRes.success && vnpayRes.data?.paymentUrl) {
          window.location.href = vnpayRes.data.paymentUrl;
        } else {
          setMessage(vnpayRes.message || "Không thể tạo URL thanh toán VNPay!");
          setProcessing(false);
        }
      }
    } catch (error) {
      setMessage("Có lỗi xảy ra. Vui lòng thử lại!");
      setProcessing(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
          <p className="text-gray-600">Đang tải thông tin thanh toán...</p>
        </div>
      </div>
    );
  }

  if (!registration) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <AlertCircle className="w-16 h-16 text-red-500 mx-auto mb-4" />
          <p className="text-gray-600">Không tìm thấy thông tin đăng ký</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-24 px-4 sm:px-6 lg:px-8">
      <div className="max-w-4xl mx-auto">
        {/* Back Button */}
        <button
          onClick={() => navigate("/khoa-hoc-cua-toi")}
          className="flex items-center text-gray-600 hover:text-gray-900 mb-6 transition-colors"
        >
          <ArrowLeft className="w-5 h-5 mr-2" />
          Quay lại khóa học của tôi
        </button>

        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">Thanh toán</h1>
          <p className="text-gray-600">Hoàn tất thanh toán để đăng ký khóa học</p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Payment Methods */}
          <div className="space-y-6">
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-bold text-gray-900 mb-4">Chọn phương thức thanh toán</h2>

              <div className="space-y-3">
                {/* VNPay */}
                <button
                  onClick={() => setPaymentMethod("vnpay")}
                  className={`w-full flex items-center p-4 rounded-lg border-2 transition-all ${paymentMethod === "vnpay"
                    ? "border-blue-600 bg-blue-50"
                    : "border-gray-200 hover:border-gray-300"
                    }`}
                >
                  <div className={`w-6 h-6 rounded-full border-2 mr-4 flex items-center justify-center ${paymentMethod === "vnpay"
                    ? "border-blue-600 bg-blue-600"
                    : "border-gray-300"
                    }`}>
                    {paymentMethod === "vnpay" && <Check className="w-4 h-4 text-white" />}
                  </div>
                  <Building2 className="w-8 h-8 text-gray-600 mr-3" />
                  <div className="text-left flex-1">
                    <p className="font-semibold text-gray-900">VNPay</p>
                    <p className="text-sm text-gray-600">Thanh toán qua cổng VNPay</p>
                  </div>
                </button>

                {/* Cash */}
                <button
                  onClick={() => setPaymentMethod("cash")}
                  className={`w-full flex items-center p-4 rounded-lg border-2 transition-all ${paymentMethod === "cash"
                    ? "border-green-600 bg-green-50"
                    : "border-gray-200 hover:border-gray-300"
                    }`}
                >
                  <div className={`w-6 h-6 rounded-full border-2 mr-4 flex items-center justify-center ${paymentMethod === "cash"
                    ? "border-green-600 bg-green-600"
                    : "border-gray-300"
                    }`}>
                    {paymentMethod === "cash" && <Check className="w-4 h-4 text-white" />}
                  </div>
                  <DollarSign className="w-8 h-8 text-green-600 mr-3" />
                  <div className="text-left flex-1">
                    <p className="font-semibold text-gray-900">Tiền mặt</p>
                    <p className="text-sm text-gray-600">Thanh toán trực tiếp tại trung tâm</p>
                  </div>
                </button>
              </div>
            </div>
              
            {/* Payment Instructions */}
            {paymentMethod === "vnpay" && (
              <div className="bg-blue-50 rounded-xl border border-blue-200 p-6">
                <h3 className="font-bold text-gray-900 mb-4">Thanh toán qua VNPay</h3>
                <div className="space-y-2 text-sm">
                  <p>✓ Hỗ trợ thanh toán qua ATM, Internet Banking, Visa, MasterCard</p>
                  <p>✓ Xác nhận thanh toán ngay lập tức</p>
                  <p className="text-blue-600 mt-4">
                    * Bạn sẽ được chuyển đến trang thanh toán VNPay để hoàn tất giao dịch
                  </p>
                </div>
              </div>
            )}

            {paymentMethod === "cash" && (
              <div className="bg-green-50 rounded-xl border border-green-200 p-6">
                <h3 className="font-bold text-gray-900 mb-4">Thanh toán tiền mặt</h3>
                <div className="space-y-2 text-sm">
                  <p><strong>Địa chỉ:</strong> 123 Đường ABC, Quận 1, TP.HCM</p>
                  <p><strong>Thời gian:</strong> 8:00 - 17:00 (Thứ 2 - Thứ 6)</p>
                </div>
              </div>
            )}
          </div>

          {/* Order Summary */}
          <div>
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 sticky top-24">
                <h2 className="text-xl font-bold text-gray-900 mb-4">Thông tin đơn hàng</h2>

                <div className="space-y-4 mb-6">
                  <div className="flex items-start">
                    <BookOpen className="w-5 h-5 text-gray-400 mr-3 mt-0.5" />
                    <div className="flex-1">
                      <p className="text-sm text-gray-600">Khóa học</p>
                      <p className="font-semibold text-gray-900">{registration.khoaHoc?.tenKhoaHoc}</p>
                    </div>
                  </div>

                  <div className="flex items-start">
                    <Calendar className="w-5 h-5 text-gray-400 mr-3 mt-0.5" />
                    <div className="flex-1">
                      <p className="text-sm text-gray-600">Ngày khai giảng</p>
                      <p className="font-semibold text-gray-900">{formatDate(registration.khoaHoc?.ngayKhaiGiang)}</p>
                    </div>
                  </div>

                  <div className="flex items-start">
                    <Clock className="w-5 h-5 text-gray-400 mr-3 mt-0.5" />
                    <div className="flex-1">
                      <p className="text-sm text-gray-600">Thời lượng</p>
                      <p className="font-semibold text-gray-900">{registration.khoaHoc?.thoiLuong} buổi</p>
                    </div>
                  </div>
                </div>

                <div className="border-t border-gray-200 pt-4 mb-6">
                  <div className="flex justify-between mb-2">
                    <span className="text-gray-600">Học phí</span>
                    <span className="font-semibold">{formatCurrency(registration.khoaHoc?.hocPhi)}</span>
                  </div>
                  <div className="flex justify-between mb-2">
                    <span className="text-gray-600">Phí dịch vụ</span>
                    <span className="font-semibold">0 ₫</span>
                  </div>
                  <div className="flex justify-between text-lg font-bold pt-2 border-t">
                    <span>Tổng cộng</span>
                    <span className="text-blue-600">{formatCurrency(registration.khoaHoc?.hocPhi)}</span>
                  </div>
                </div>

                {message && (
                  <div className={`mb-4 p-3 rounded-lg ${message.includes("thành công")
                    ? "bg-green-50 text-green-800 border border-green-200"
                    : "bg-red-50 text-red-800 border border-red-200"
                    }`}>
                    <p className="text-sm">{message}</p>
                  </div>
                )}

                <button
                  onClick={handlePayment}
                  disabled={processing}
                  className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                >
                  {processing ? (
                    <>
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                      Đang xử lý...
                    </>
                  ) : (
                    <>
                      <CreditCard className="w-5 h-5" />
                      Xác nhận thanh toán
                    </>
                  )}
                </button>

                <p className="text-xs text-gray-500 mt-4 text-center">
                  Bằng việc thanh toán, bạn đồng ý với điều khoản và điều kiện của chúng tôi
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
  );
}
