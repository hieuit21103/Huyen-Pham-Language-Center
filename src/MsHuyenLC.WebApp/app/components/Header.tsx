import { Menu, X, User, LogOut, Key, LayoutDashboard, ChevronDown, Bell, icons, CreditCard, Calendar, FileCheck, GraduationCap } from "lucide-react";
import { useState, useEffect } from "react";
import { Link } from "react-router";
import { Asset } from "~/assets/Asset";
import { clearJwtToken } from "~/apis/Auth";
import { getThongBaoByTaiKhoanId } from "~/apis/ThongBao";
import { formatDateTime } from "~/utils/date-utils";
import { getProfile } from "~/apis/Profile";
import type { ThongBaoResponse, VaiTro } from "~/types";
import type { LucideIcon } from "lucide-react";

type DropdownItem = {
  name: string;
  href: string;
  icon: LucideIcon;
  onClick?: () => void;
};

export default function Header() {
  const [jwtToken, setJwtToken] = useState<string | undefined>(undefined);
  const [currentUserId, setCurrentUserId] = useState<string | null>(null);
  const [userRole, setUserRole] = useState<VaiTro | null>(null);
  const [showDropdown, setShowDropdown] = useState(false);
  const [showNotifications, setShowNotifications] = useState(false);
  const [notifications, setNotifications] = useState<ThongBaoResponse[]>([]);
  const [showMobileMenu, setShowMobileMenu] = useState(false);
  const [activeSection, setActiveSection] = useState('');

  useEffect(() => {
    if (typeof window !== 'undefined') {
      const token = document.cookie.split('; ').find(row => row.startsWith('jwt_token='));
      setJwtToken(token);

      if (token) {
        const tokenValue = token.split('=')[1];
        try {
          const payload = JSON.parse(atob(tokenValue.split('.')[1]));
          const userId = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
          setCurrentUserId(userId);
          
          if (userId) {
            loadNotifications(userId);
            loadUserRole();
          }
        } catch (error) {
          console.error("Error decoding JWT:", error);
        }
      }
    }
  }, []);

  const loadNotifications = async (userId: string) => {
    try {
      const result = await getThongBaoByTaiKhoanId(userId);
      if (result.success && result.data) {
        setNotifications(result.data);
      }
    } catch (error) {
      console.error("Error loading notifications:", error);
    }
  };

  const loadUserRole = async () => {
    try {
      const result = await getProfile();
      if (result.success && result.data) {
        setUserRole(result.data.vaiTro);
      }
    } catch (error) {
      console.error("Error loading user role:", error);
    }
  };

  const handleLogout = () => {
    if (typeof window !== 'undefined') {
      clearJwtToken();
      window.location.href = '/';
    }
  };

  const getDropdownItems = (): DropdownItem[] => {
    const baseItems: DropdownItem[] = [
      { name: 'Dashboard', href: '/dashboard', icon: LayoutDashboard },
    ];

    if (userRole === 3) {
      baseItems.push(
        { name: 'Kỳ Thi Của Tôi', href: '/ky-thi-cua-toi', icon: Calendar }
      );
    }

    if (userRole === 2) {
      baseItems.push(
        { name: 'Lớp Của Tôi', href: '/lop-cua-toi-giao-vien', icon: GraduationCap },
        { name: 'Chấm Điểm', href: '/cham-diem-giao-vien', icon: FileCheck }
      );
    }

    baseItems.push(
      { name: 'Đổi Mật Khẩu', href: '/doi-mat-khau', icon: Key },
      { name: 'Đăng Xuất', href: '/', icon: LogOut, onClick: handleLogout }
    );

    return baseItems;
  };

  const navigation = jwtToken
    ? [
      { name: 'Trang Chủ', href: '/' },
      { name: 'Khóa Học', href: '/khoa-hoc' },
      { name: 'Luyện Đề', href: '/luyen-thi' },
      { name: 'Khóa Học Của Tôi', href: '/khoa-hoc-cua-toi' },
      { name: 'Phản Hồi', href: '/phan-hoi' },
    ]
    : [
      { name: 'Trang Chủ', href: '/' },
      { name: 'Khóa Học', href: '/khoa-hoc' },
      { name: 'Luyện Đề', href: '/luyen-thi' },
      { name: 'Đăng Nhập', href: '/dang-nhap' },
    ];

  const dropdown = getDropdownItems();

  return (
    <header className="bg-white text-gray-900 fixed w-full top-0 z-50 shadow-md border-b border-gray-200 backdrop-blur-sm bg-white/95">
      <nav className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          <Link to="/" className="flex items-center group transition-transform duration-300 hover:scale-105">
            <img src={Asset.logo} className="w-8 h-8 mr-2 text-gray-900 transition-transform duration-300 group-hover:rotate-12" />
            <span className="text-2xl font-bold text-gray-900">HPLC</span>
          </Link>

          {/* Desktop Menu */}
          <div className="hidden md:flex items-center space-x-6">
            {navigation.map((item) => (
              <Link to={item.href} key={item.name}
                className={`font-semibold transition-all duration-300 ease-in-out hover:text-gray-900 hover:scale-110 relative group ${activeSection === item.href ? 'text-gray-900' : 'text-gray-700'
                  }`}
                onClick={() => setActiveSection(item.href)}
              >
                {item.name}
                <span className={`absolute bottom-0 left-0 w-full h-0.5 bg-gray-900 transform transition-all duration-300 ${activeSection === item.href ? 'scale-x-100' : 'scale-x-0 group-hover:scale-x-100'
                  }`}></span>
              </Link>
            ))}

            {jwtToken && (
              <>

                {/* Notifications */}
                <div className="relative">
                  <button
                    onClick={() => setShowNotifications(!showNotifications)}
                    className="relative p-2 text-gray-700 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-all duration-300"
                  >
                    <Bell className="w-5 h-5" />
                  </button>

                  {showNotifications && (
                    <div className="absolute right-0 mt-2 w-80 bg-white rounded-lg shadow-xl border border-gray-200 py-2 z-50 max-h-96 overflow-y-auto">
                      <div className="px-4 py-2 border-b border-gray-200">
                        <h3 className="font-semibold text-gray-900">Thông báo</h3>
                      </div>
                      {notifications.length === 0 ? (
                        <div className="px-4 py-8 text-center text-gray-500">
                          Không có thông báo mới
                        </div>
                      ) : (
                        notifications.map((notif, index) => (
                          <div
                            key={index}
                            className={`px-4 py-3 hover:bg-gray-50 cursor-pointer border-l-4 border-blue-500`}
                          >
                            <div className="flex items-center justify-between mb-1">
                              <p className="font-medium text-gray-900 text-sm">{notif.tieuDe}</p>
                              <span className="text-xs text-gray-400">{formatDateTime(notif.ngayTao)}</span>
                            </div>
                            <p className="text-xs text-gray-600 mt-1">{notif.noiDung}</p>
                          </div>
                        ))
                      )}
                    </div>
                  )}
                </div>

                {/* User Dropdown */}
                <div className="relative">
                  <button
                    onClick={() => setShowDropdown(!showDropdown)}
                    className="flex items-center space-x-2 font-semibold text-gray-700 hover:text-gray-900 transition-all duration-300 group"
                  >
                    <div className="w-8 h-8 bg-gray-900 rounded-full flex items-center justify-center text-white group-hover:bg-gray-700 group-hover:scale-110 transition-all duration-300 shadow-md group-hover:shadow-lg">
                      <User className="w-5 h-5" />
                    </div>
                    <ChevronDown className={`w-4 h-4 transition-transform duration-300 ${showDropdown ? 'rotate-180' : ''}`} />
                  </button>

                  {showDropdown && (
                    <div className="absolute right-0 mt-2 w-56 bg-white rounded-lg shadow-xl border border-gray-200 py-2 z-50 animate-fadeIn">
                      {dropdown.map((item, index) => (
                        <Link
                          key={index}
                          to={item.href}
                          className="flex items-center px-4 py-3 text-gray-700 hover:bg-gray-50 hover:text-gray-900 transition-all duration-200 group"
                          onClick={() => {
                            setShowDropdown(false);
                            (item as any).onClick?.();
                          }}
                        >
                          <item.icon className="w-5 h-5 mr-3 group-hover:scale-110 transition-transform duration-200" />
                          <span className="font-medium">{item.name}</span>
                        </Link>
                      ))}
                    </div>
                  )}
                </div>
              </>
            )}
          </div>

          {/* Mobile Menu Button */}
          <button
            className="md:hidden transition-transform duration-300 hover:scale-110 active:scale-95"
            onClick={() => setShowMobileMenu(!showMobileMenu)}
          >
            {showMobileMenu ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
          </button>
        </div>

        {/* Mobile Menu */}
        {showMobileMenu && (
          <div className="md:hidden pb-4 bg-white animate-fadeIn">
            {navigation.map((item) => (
              <Link
                to={item.href}
                key={item.name}
                onClick={() => {
                  setActiveSection(item.href);
                  setShowMobileMenu(false);
                }}
                className="block w-full text-left py-3 px-4 hover:bg-gray-100 rounded-lg text-gray-900 font-semibold transition-all duration-200 hover:translate-x-2 active:scale-95"
              >
                {item.name}
              </Link>
            ))}

            {jwtToken && (
              <>
                <div className="border-t border-gray-200 my-2"></div>

                {/* Notifications Mobile */}
                <div className="px-4 py-2">
                  <button
                    onClick={() => setShowNotifications(!showNotifications)}
                    className="flex items-center justify-between w-full text-gray-900 hover:text-gray-700 font-semibold"
                  >
                    <div className="flex items-center">
                      <Bell className="w-5 h-5 mr-3" />
                      <span>Thông báo</span>
                    </div>
                  </button>

                  {showNotifications && (
                    <div className="mt-2 space-y-2 animate-fadeIn">
                      {notifications.length === 0 ? (
                        <p className="text-sm text-gray-500 text-center py-4">Không có thông báo mới</p>
                      ) : (
                        notifications.map((notif, index) => (
                          <div
                            key={index}
                            className="p-3 rounded-md bg-gray-50 border-l-4 border-blue-500"
                          >
                            <div className="flex items-center justify-between mb-1">
                              <p className="font-medium text-gray-900 text-sm">{notif.tieuDe}</p>
                              <span className="text-xs text-gray-400">{formatDateTime(notif.ngayTao)}</span>
                            </div>
                            <p className="text-xs text-gray-600 mt-1">{notif.noiDung}</p>
                          </div>
                        ))
                      )}
                    </div>
                  )}
                </div>

                <Link
                  to="/dashboard"
                  onClick={() => setShowMobileMenu(false)}
                  className="flex items-center py-3 px-4 hover:bg-gray-100 rounded-lg text-gray-900 font-semibold transition-all duration-200 hover:translate-x-2"
                >
                  <LayoutDashboard className="w-5 h-5 mr-3" />
                  Dashboard
                </Link>

                <Link
                  to="/doi-mat-khau"
                  onClick={() => setShowMobileMenu(false)}
                  className="flex items-center py-3 px-4 hover:bg-gray-100 rounded-lg text-gray-900 font-semibold transition-all duration-200 hover:translate-x-2"
                >
                  <Key className="w-5 h-5 mr-3" />
                  Đổi mật khẩu
                </Link>

                <div className="border-t border-gray-200 my-1"></div>

                <button
                  onClick={() => {
                    handleLogout();
                    setShowMobileMenu(false);
                  }}
                  className="flex items-center w-full py-3 px-4 hover:bg-red-50 rounded-lg text-red-600 font-semibold transition-all duration-200 hover:translate-x-2"
                >
                  <LogOut className="w-5 h-5 mr-3" />
                  Đăng xuất
                </button>
              </>
            )}
          </div>
        )}
      </nav>
    </header>
  );
}