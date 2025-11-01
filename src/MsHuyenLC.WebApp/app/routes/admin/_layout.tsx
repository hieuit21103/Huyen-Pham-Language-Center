import { Outlet, useNavigate } from "react-router";
import { useEffect, useState } from "react";
import AdminSidebar from "~/components/AdminSidebar";
import { Bell, Search, User, AlertCircle } from "lucide-react";
import { getProfile } from "~/apis/Profile";
import { VaiTro } from "~/types/index";

export default function AdminLayout() {
    const navigate = useNavigate();
    const [isAuthorized, setIsAuthorized] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [userName, setUserName] = useState("Admin");
    const [userRole, setUserRole] = useState<VaiTro | null>(null);
    const [sidebarCollapsed, setSidebarCollapsed] = useState(true);

    useEffect(() => {
        checkAuthorization();
        setLightTheme();
    }, []);

    const checkAuthorization = async () => {
        setIsLoading(true);
        const response = await getProfile();
        
        if (!response.success || !response.data) {
            navigate("/dang-nhap");
            return;
        }

        const userVaiTro = response.data.vaiTro;
        setUserRole(userVaiTro);
        
        if (response.data.tenDangNhap) {
            setUserName(response.data.tenDangNhap);
        }

        if (userVaiTro === VaiTro.Admin || userVaiTro === VaiTro.GiaoVu) {
            setIsAuthorized(true);
        } else {
            setIsAuthorized(false);
        }
        
        setIsLoading(false);
    };

    const getRoleText = () => {
        if (userRole === VaiTro.Admin) return "Quản trị viên";
        if (userRole === VaiTro.GiaoVu) return "Giáo vụ";
        return "Người dùng";
    };

    if (isLoading) {
        return (
            <div className="flex items-center justify-center min-h-screen bg-gray-50">
                <div className="text-center">
                    <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
                    <p className="text-gray-600">Đang kiểm tra quyền truy cập...</p>
                </div>
            </div>
        );
    }

    if (!isAuthorized) {
        return (
            <div className="flex items-center justify-center min-h-screen bg-gray-50">
                <div className="bg-white rounded-xl shadow-lg border border-gray-200 p-8 max-w-md w-full text-center">
                    <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <AlertCircle className="w-8 h-8 text-red-600" />
                    </div>
                    <h2 className="text-2xl font-bold text-gray-900 mb-2">Truy cập bị từ chối</h2>
                    <p className="text-gray-600 mb-6">
                        Bạn không có quyền truy cập vào trang quản trị. 
                        Chỉ Admin và Giáo vụ mới được phép truy cập.
                    </p>
                    <div className="space-y-3">
                        <button
                            onClick={() => navigate("/")}
                            className="w-full bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                        >
                            Về trang chủ
                        </button>
                        <button
                            onClick={() => navigate("/dang-nhap")}
                            className="w-full bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors"
                        >
                            Đăng nhập tài khoản khác
                        </button>
                    </div>
                </div>
            </div>
        );
    }
    return (
        <div className="min-h-screen bg-gray-50">
            <AdminSidebar onCollapsedChange={setSidebarCollapsed} />
            
            {/* Main Content */}
            <div 
                className={`transition-all duration-300 ${
                    sidebarCollapsed ? 'lg:pl-24' : 'lg:pl-64'
                }`}
            >
                {/* Top Header */}
                <header className="bg-white border-b border-gray-200 sticky top-0 z-20">
                    <div className="px-4 sm:px-6 lg:px-8 py-4">
                        <div className="flex items-center justify-between">
                            {/* Right Side */}
                            <div className="flex items-center space-x-4">
                                {/* Profile */}
                                <div className="flex items-center space-x-3 px-3 py-2 bg-gray-100 rounded-lg">
                                    <div className="w-8 h-8 bg-gray-900 rounded-full flex items-center justify-center">
                                        <User className="w-5 h-5 text-white" />
                                    </div>
                                    <div className="hidden md:block">
                                        <p className="text-sm font-semibold text-gray-900">{userName}</p>
                                        <p className="text-xs text-gray-600">{getRoleText()}</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </header>

                {/* Page Content */}
                <main className="p-4 sm:p-6 lg:p-8">
                    <Outlet />
                </main>
            </div>
        </div>
    );
}

export const setLightTheme = () => {
    const html = document.documentElement;
        html.style.colorScheme = 'light';
        html.classList.remove('dark');
        html.setAttribute('data-theme', 'light');
        try {
            localStorage.setItem('theme', 'light');
        } catch (e) {}
};
