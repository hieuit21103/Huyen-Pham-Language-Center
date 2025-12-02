import { useState, useEffect } from "react";
import { Key, CheckCircle, AlertCircle, Lock } from "lucide-react";
import { useSearchParams } from "react-router";
import { confirmResetPassword } from "../apis/Auth";

export default function DatLaiMatKhauPage() {
    const [searchParams] = useSearchParams();
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");
    const [messageType, setMessageType] = useState<"success" | "error">("success");
    const [loading, setLoading] = useState(false);
    const [tokenValid, setTokenValid] = useState(false);
    const [checkingToken, setCheckingToken] = useState(true);
    
    const decoded = searchParams.get("token") || "";
    const token = decoded.replace(/ /g, "+");
    const email = searchParams.get("email") || "";

    useEffect(() => {
        const verifyToken = async () => {
            if (!token || !email || token.trim() === "" || email.trim() === "") {
                setMessage("Link đặt lại mật khẩu không hợp lệ!");
                setMessageType("error");
                setCheckingToken(false);
                return;
            }
            setTokenValid(true);
            setCheckingToken(false);
        };

        verifyToken();
    }, [token]);

    const handleResetPassword = async () => {
        // Validation
        if (!newPassword || !confirmPassword) {
            setMessage("Vui lòng điền đầy đủ thông tin!");
            setMessageType("error");
            return;
        }

        if (newPassword !== confirmPassword) {
            setMessage("Mật khẩu không khớp!");
            setMessageType("error");
            return;
        }

        if (newPassword.length < 6) {
            setMessage("Mật khẩu phải có ít nhất 6 ký tự!");
            setMessageType("error");
            return;
        }

        setLoading(true);

        let result = await confirmResetPassword({
            email: email,
            token: token,
            matKhauMoi: newPassword
        });

        setLoading(false);
        setMessage(result.message || "");
        setMessageType(result.success ? "success" : "error");

        if (result.success) {
            setNewPassword("");
            setConfirmPassword("");
            setTimeout(() => {
                window.location.href = "/dang-nhap";
            }, 2000);
        }
    };

    useEffect(() => {
        if (message && messageType === "error") {
            const timer = setTimeout(() => {
                setMessage("");
            }, 5000);
            return () => clearTimeout(timer);
        }
    }, [message, messageType]);

    // Loading state
    if (checkingToken) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50">
                <div className="text-center">
                    <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
                    <p className="text-gray-600 font-semibold">Đang xác thực...</p>
                </div>
            </div>
        );
    }

    // Invalid token
    if (!tokenValid) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
                <div className="w-full max-w-md">
                    <div className="bg-white rounded-2xl shadow-2xl p-8 border-2 border-gray-100 text-center">
                        <div className="inline-flex items-center justify-center w-16 h-16 bg-red-100 rounded-full mb-4">
                            <AlertCircle className="w-8 h-8 text-red-600" />
                        </div>
                        <h2 className="text-2xl font-bold text-gray-900 mb-4">
                            Link Không Hợp Lệ
                        </h2>
                        <p className="text-gray-600 mb-6">
                            Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.
                        </p>
                        <button
                            onClick={() => (window.location.href = "/quen-mat-khau")}
                            className="w-full bg-gray-900 text-white py-3 rounded-lg font-bold hover:bg-gray-800 transition-all"
                        >
                            Yêu cầu link mới
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
            <div className="w-full max-w-md">
                <div className="text-center mb-8">
                    <div className="inline-flex items-center justify-center w-16 h-16 bg-gray-900 rounded-full mb-4">
                        <Key className="w-8 h-8 text-white" />
                    </div>
                    <h2 className="text-4xl font-bold text-gray-900 mb-2">
                        Đặt Lại Mật Khẩu
                    </h2>
                    <p className="text-gray-600">Nhập mật khẩu mới cho tài khoản của bạn</p>
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

                    <div className="space-y-5">
                        <div>
                            <label className="block text-gray-700 font-semibold mb-2">
                                Mật khẩu mới <span className="text-red-500">*</span>
                            </label>
                            <div className="relative">
                                <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                                <input
                                    type="password"
                                    placeholder="Nhập mật khẩu mới (tối thiểu 6 ký tự)"
                                    className="w-full pl-10 pr-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                                    value={newPassword}
                                    onChange={(e) => setNewPassword(e.target.value)}
                                />
                            </div>
                        </div>

                        <div>
                            <label className="block text-gray-700 font-semibold mb-2">
                                Xác nhận mật khẩu mới <span className="text-red-500">*</span>
                            </label>
                            <div className="relative">
                                <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                                <input
                                    type="password"
                                    placeholder="Nhập lại mật khẩu mới"
                                    className="w-full pl-10 pr-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                                    value={confirmPassword}
                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                    onKeyPress={(e) => e.key === "Enter" && handleResetPassword()}
                                />
                            </div>
                        </div>

                        {/* Password strength indicator */}
                        {newPassword && (
                            <div className="space-y-2">
                                <div className="text-sm text-gray-600">Độ mạnh mật khẩu:</div>
                                <div className="flex gap-1">
                                    <div
                                        className={`h-1 flex-1 rounded ${newPassword.length >= 6 ? "bg-green-500" : "bg-gray-200"
                                            }`}
                                    ></div>
                                    <div
                                        className={`h-1 flex-1 rounded ${newPassword.length >= 8 ? "bg-green-500" : "bg-gray-200"
                                            }`}
                                    ></div>
                                    <div
                                        className={`h-1 flex-1 rounded ${newPassword.length >= 10 && /[A-Z]/.test(newPassword)
                                            ? "bg-green-500"
                                            : "bg-gray-200"
                                            }`}
                                    ></div>
                                </div>
                            </div>
                        )}

                        <button
                            onClick={handleResetPassword}
                            disabled={loading}
                            className="w-full bg-gray-900 text-white py-4 rounded-lg font-bold text-lg hover:bg-gray-800 hover:scale-105 transition-all shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {loading ? (
                                <span className="flex items-center justify-center">
                                    <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-2"></div>
                                    Đang xử lý...
                                </span>
                            ) : (
                                "Đặt Lại Mật Khẩu"
                            )}
                        </button>
                    </div>

                    <div className="mt-6 pt-6 border-t border-gray-200">
                        <p className="text-center text-gray-600">
                            <button
                                onClick={() => (window.location.href = "/dang-nhap")}
                                className="text-gray-900 font-bold hover:text-gray-700 hover:underline"
                            >
                                ← Quay lại đăng nhập
                            </button>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}
