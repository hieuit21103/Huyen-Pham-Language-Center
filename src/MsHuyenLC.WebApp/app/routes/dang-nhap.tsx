import { useState, useEffect, type KeyboardEventHandler } from "react";
import { getJwtToken, login } from "../apis/Auth";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error" | "">("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    var jwt_token = getJwtToken();
    if (jwt_token) {
      window.location.href = "/";
    }
  }, []);

  const handleForgotPassword = () => {
    window.location.href = "/quen-mat-khau";
  }
  const handleLogin = async () => {
    setLoading(true);
    const response = await login({
      tenDangNhap: username,
      matKhau: password
    });
    setLoading(false);
    setMessage(response.message || "???????");
    setMessageType(response.success ? "success" : "error");
    if (response.success) {
      setTimeout(() => {
        window.location.href = "/";
      }, 2000);
    }
  }

  const handleKeyDown: KeyboardEventHandler<HTMLButtonElement> = (e) => {
    if (e.key === 'Enter') {
      handleLogin();
    }
  };

  useEffect(() => {
    if (message) {
      const timer = setTimeout(() => {
        setMessage("")
      }, 3000);
      return () => clearTimeout(timer);
    }
  }, [message]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
      <div className="w-full max-w-md">
        <h2 className="text-4xl font-bold text-center text-gray-900 mb-8">
          Đăng Nhập
        </h2>
        <div className="bg-white rounded-2xl shadow-2xl p-8 border-2 border-gray-100">
          <div className="space-y-6">
            <div>
              <label className="block text-gray-700 font-semibold mb-2">
                Tài khoản
              </label>
              <input
                type="text"
                className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>

            <div>
              <label className="block text-gray-700 font-semibold mb-2">
                Mật khẩu
              </label>
              <input
                type="password"
                className="w-full px-4 py-3 border-2 border-gray-200 rounded-lg focus:border-gray-900 focus:outline-none transition-colors"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>
            <div className="text-right">
              <button
                onClick={() => {
                  handleForgotPassword();
                }}
                className="text-gray-400 hover:text-gray-700"
              >
                Quên mật khẩu
              </button>
            </div>
            {message && (
              <div className={`text-center text-sm ${message.includes('thành công') ? 'text-green-500' : 'text-red-500'}`}>
                {message}
              </div>
            )}
            <button
              onClick={handleLogin}
              onKeyDown={handleKeyDown}
              className="w-full bg-gray-900 text-white py-4 rounded-lg font-bold text-lg hover:bg-gray-800 hover:scale-105 transition-all"
            >
              Đăng Nhập
            </button>
          </div>

          <p className="text-center mt-6 text-gray-600">
            Chưa có tài khoản?{' '}
            <button
              onClick={() => {
                window.location.href = '/#register';
              }}
              className="text-gray-900 font-bold hover:text-gray-700"
            >
              Đăng ký ngay
            </button>
          </p>
        </div>
      </div>
    </div>
  );
}