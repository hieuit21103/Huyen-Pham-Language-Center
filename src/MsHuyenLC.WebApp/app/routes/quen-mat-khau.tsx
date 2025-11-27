import { useState, useEffect } from "react";
import { Mail, CheckCircle, AlertCircle } from "lucide-react";
import { sendResetPasswordEmail } from "../apis/Auth";

export default function QuenMatKhauPage() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error">("success");
  const [loading, setLoading] = useState(false);
  const [emailSent, setEmailSent] = useState(false);

  const validateEmail = (email: string) => {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
  };

  const handleResetPassword = async () => {
    // Validation
    if (!email) {
      setMessage("Vui lòng nhập địa chỉ email!");
      setMessageType("error");
      return;
    }

    if (!validateEmail(email)) {
      setMessage("Email không hợp lệ!");
      setMessageType("error");
      return;
    }

    setLoading(true);

    let result = await sendResetPasswordEmail({
      email: email,
      returnUrl: window.location.origin + "/dat-lai-mat-khau"
    });
    setLoading(false);
    setMessage(result.message ?? "");
    setMessageType(result.success ? "success" : "error");
    if (result.success) {
      setEmailSent(true);
    }
  };

  useEffect(() => {
    if (message) {
      const timer = setTimeout(() => {
        setMessage("");
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [message]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
      <div className="w-full max-w-md">
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-gray-900 rounded-full mb-4">
            <Mail className="w-8 h-8 text-white" />
          </div>
          <h2 className="text-4xl font-bold text-gray-900 mb-2">
            Quên Mật Khẩu
          </h2>
          <p className="text-gray-600">
            {!emailSent
              ? "Nhập email của bạn để nhận link đặt lại mật khẩu"
              : "Kiểm tra email để tiếp tục"}
          </p>
        </div>

        <div className="bg-white rounded-2xl shadow-2xl p-8 border-2 border-gray-100">
          {message && (
            <div
              className={`mb-6 p-4 rounded-lg flex items-center ${messageType === "success"
                  ? "bg-green-50 text-green-800 border border-green-200"
                  : "bg-red-50 text-red-800 border border-red-200"
                }`}
            >
              {messageType === "success" ? (
                <CheckCircle className="w-5 h-5 mr-2 flex-shrink-0" />
              ) : (
                <AlertCircle className="w-5 h-5 mr-2 flex-shrink-0" />
              )}
              <span className="font-medium">{message}</span>
            </div>
          )}

          {!emailSent ? (
            <div className="space-y-6">
              <div>
                <label className="block text-gray-700 font-semibold mb-2">
                  Email <span className="text-red-500">*</span>
                </label>
                <input
                  type="email"
                  placeholder="Nhập địa chỉ email của bạn"
                  className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  onKeyPress={(e) => e.key === "Enter" && handleResetPassword()}
                />
              </div>

              <button
                onClick={handleResetPassword}
                disabled={loading}
                className="w-full bg-gray-900 text-white py-4 rounded-lg font-bold text-lg hover:bg-gray-800 hover:scale-105 transition-all shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? "Đang gửi..." : "Đặt Lại Mật Khẩu"}
              </button>
            </div>
          ) : (
            <div className="text-center space-y-4">
              <div className="bg-green-50 border border-green-200 rounded-lg p-6">
                <CheckCircle className="w-12 h-12 text-green-600 mx-auto mb-3" />
                <p className="text-gray-700 mb-2">
                  Email đã được gửi đến <strong>{email}</strong>
                </p>
                <p className="text-sm text-gray-600">
                  Vui lòng kiểm tra hộp thư và làm theo hướng dẫn để đặt lại mật khẩu.
                </p>
              </div>

              <button
                onClick={() => {
                  setEmailSent(false);
                  setEmail("");
                  setMessage("");
                }}
                className="text-gray-900 font-bold hover:text-gray-700 hover:underline"
              >
                Gửi lại email
              </button>
            </div>
          )}

          <div className="mt-6 pt-6 border-t border-gray-200">
            <p className="text-center text-gray-600">
              Đã nhớ mật khẩu?{" "}
              <button
                onClick={() => (window.location.href = "/dang-nhap")}
                className="text-gray-900 font-bold hover:text-gray-700 hover:underline"
              >
                Đăng nhập
              </button>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
