import { useState, useEffect } from "react";
import { Key, Lock, CheckCircle } from "lucide-react";
import { changePassword } from "../apis/Auth";

export default function DoiMatKhauPage() {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [message, setMessage] = useState("");
    const [messageType, setMessageType] = useState<"success" | "error">("success");
    const [loading, setLoading] = useState(false);

    const handleChangePassword = async () => {
        // Validation
        if (!oldPassword || !newPassword || !confirmPassword) {
            setMessage("Vui lòng điền đầy đủ thông tin!");
            setMessageType("error");
            return;
        }

        if (newPassword !== confirmPassword) {
            setMessage("Mật khẩu mới không khớp!");
            setMessageType("error");
            return;
        }

        if (newPassword.length < 6) {
            setMessage("Mật khẩu mới phải có ít nhất 6 ký tự!");
            setMessageType("error");
            return;
        }

        setLoading(true);

        let result = await changePassword({
            matKhauCu: oldPassword,
            matKhauMoi: newPassword
        });

        setLoading(false);
        setMessage(result.message ?? "");
        setMessageType(result.success ? "success" : "error");
        if (result.success) {
            setOldPassword("");
            setNewPassword("");
            setConfirmPassword("");
            setTimeout(() => {
                window.location.href = "/";
            }, 2000);
        }

        useEffect(() => {
            if (message) {
                const timer = setTimeout(() => {
                    setMessage("");
                }, 5000);
                return () => clearTimeout(timer);
            }
        }, [message]);
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
            <div className="w-full max-w-md">
                <div className="text-center mb-8">
                    <div className="inline-flex items-center justify-center w-16 h-16 bg-gray-900 rounded-full mb-4">
                        <Key className="w-8 h-8 text-white" />
                    </div>
                    <h2 className="text-4xl font-bold text-gray-900 mb-2">
                        Đổi Mật Khẩu
                    </h2>
                    <p className="text-gray-600">Cập nhật mật khẩu mới cho tài khoản của bạn</p>
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
                                <Lock className="w-5 h-5 mr-2 flex-shrink-0" />
                            )}
                            <span className="font-medium">{message}</span>
                        </div>
                    )}

                    <div className="space-y-5">
                        <div>
                            <label className="block text-gray-700 font-semibold mb-2">
                                Mật khẩu cũ <span className="text-red-500">*</span>
                            </label>
                            <input
                                type="password"
                                placeholder="Nhập mật khẩu hiện tại"
                                className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                                value={oldPassword}
                                onChange={(e) => setOldPassword(e.target.value)}
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-semibold mb-2">
                                Mật khẩu mới <span className="text-red-500">*</span>
                            </label>
                            <input
                                type="password"
                                placeholder="Nhập mật khẩu mới (tối thiểu 6 ký tự)"
                                className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-semibold mb-2">
                                Xác nhận mật khẩu mới <span className="text-red-500">*</span>
                            </label>
                            <input
                                type="password"
                                placeholder="Nhập lại mật khẩu mới"
                                className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                            />
                        </div>

                        <button
                            onClick={handleChangePassword}
                            disabled={loading}
                            className="w-full bg-gray-900 text-white py-4 rounded-lg font-bold text-lg hover:bg-gray-800 hover:scale-105 transition-all shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {loading ? "Đang xử lý..." : "Đổi Mật Khẩu"}
                        </button>
                    </div>

                    <p className="text-center mt-6 text-gray-600">
                        <button
                            onClick={() => window.location.href = "/"}
                            className="text-gray-900 font-bold hover:text-gray-700 hover:underline"
                        >
                            ← Quay lại trang chủ
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
}
