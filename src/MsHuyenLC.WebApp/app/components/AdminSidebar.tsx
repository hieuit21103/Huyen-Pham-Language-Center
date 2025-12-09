import {
  LayoutDashboard,
  Users,
  BookOpen,
  GraduationCap,
  FileText,
  Settings,
  LogOut,
  Menu,
  X,
  ChevronRight,
  ClipboardList,
  Archive,
  School,
  Building2,
  FileCheck,
  Calendar,
  Database,
  CalendarDays,
  UserCog,
  UserCheck,
  Bell,
  Briefcase,
  BarChart,
  DollarSign,
  MessageSquare,
} from "lucide-react";
import { useState, useEffect } from "react";
import { Link, useLocation } from "react-router";
import { getProfile } from "~/apis/Profile";
import { VaiTro } from "~/types/index";
import { Asset } from "~/assets/Asset";
import { clearJwtToken, logout } from "~/apis/Auth";

interface AdminSidebarProps {
  onCollapsedChange?: (collapsed: boolean) => void;
}

export default function AdminSidebar({ onCollapsedChange }: AdminSidebarProps) {
  const [collapsed, setCollapsed] = useState(false);
  const [mobileOpen, setMobileOpen] = useState(false);
  const location = useLocation();
  const [role, setRole] = useState<VaiTro | null>(null);

  useEffect(() => {
    loadProfile();
  }, []);

  useEffect(() => {
    if (onCollapsedChange) {
      onCollapsedChange(collapsed);
    }
  }, [collapsed, onCollapsedChange]);

  const loadProfile = async () => {
    const res = await getProfile();
    if (res.success && res.data) {
      setRole(res.data.vaiTro);
    }
  };

  // Menu groups
  const menuGroups = [
    {
      title: "Tổng quan",
      items: [
        { icon: LayoutDashboard, label: "Dashboard", path: "/admin" },
      ]
    },
    {
      title: "Quản lý người dùng",
      items: [
        { icon: Users, label: "Học viên", path: "/admin/hoc-vien" },
        { icon: GraduationCap, label: "Giáo viên", path: "/admin/giao-vien" },
        ...(role === VaiTro.Admin ? [
          { icon: Briefcase, label: "Giáo vụ", path: "/admin/giao-vu" },
          { icon: UserCog, label: "Tài khoản", path: "/admin/tai-khoan" },
        ] : []),
        { icon: Bell, label: "Thông báo", path: "/admin/thong-bao" },
      ]
    },
    {
      title: "Quản lý đào tạo",
      items: [
        { icon: BookOpen, label: "Khóa học", path: "/admin/khoa-hoc" },
        { icon: School, label: "Lớp học", path: "/admin/lop-hoc" },
        { icon: CalendarDays, label: "Lịch học", path: "/admin/lich-hoc" },
        { icon: Building2, label: "Phòng học", path: "/admin/phong-hoc" },
        { icon: UserCheck, label: "Phân công", path: "/admin/phan-cong" },
      ]
    },
    {
      title: "Quản lý thi cử",
      items: [
        { icon: FileCheck, label: "Đề thi", path: "/admin/de-thi" },
        { icon: Calendar, label: "Kỳ thi", path: "/admin/ky-thi" },
        { icon: Database, label: "Ngân hàng câu hỏi", path: "/admin/cau-hoi" },
      ]
    },
    {
      title: "Quản lý đăng ký",
      items: [
        { icon: FileText, label: "Đăng ký khóa học", path: "/admin/dang-ky" },
        { icon: ClipboardList, label: "Đăng ký tư vấn", path: "/admin/dang-ky-khach" },
      ]
    },
    {
      title: "Quản lý tài chính",
      items: [
        { icon: DollarSign, label: "Thanh toán", path: "/admin/thanh-toan" },
      ]
    },
    {
      title: "Báo cáo & Thống kê",
      items: [
        { icon: BarChart, label: "Kết quả học tập", path: "/admin/bao-cao-ket-qua" },
        { icon: DollarSign, label: "Doanh thu", path: "/admin/bao-cao-doanh-thu" },
      ]
    },
    {
      title: "Hỗ trợ",
      items: [
        { icon: MessageSquare, label: "Phản hồi", path: "/admin/phan-hoi" },
      ]
    }
  ];

  const adminOnlyGroup = {
    title: "Hệ thống",
    items: [
      { icon: Archive, label: "Nhật ký hệ thống", path: "/admin/nhat-ky-he-thong" },
      { icon: Database, label: "Sao lưu dữ liệu", path: "/admin/sao-luu-du-lieu" },
      { icon: Settings, label: "Cài đặt", path: "/admin/cai-dat" },
    ]
  };

  const displayGroups = role === VaiTro.Admin 
    ? [...menuGroups, adminOnlyGroup] 
    : menuGroups;

  const handleLogout = async () => {
    if (typeof window !== "undefined") {
      await logout();
      clearJwtToken();
      window.location.href = "/";
    }
  };

  return (
    <>
      {/* Mobile toggle */}
      <button
        onClick={() => setMobileOpen(!mobileOpen)}
        className="lg:hidden fixed top-4 left-4 z-50 bg-gray-900 text-white p-2 rounded-lg shadow-lg"
      >
        {mobileOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
      </button>

      {/* Sidebar */}
      <aside
        className={`fixed left-0 top-0 h-screen bg-gray-900 text-white transition-all duration-300 z-40 ${
          mobileOpen ? "w-64" : "-translate-x-full"
        } lg:translate-x-0 ${collapsed ? "lg:w-24" : "lg:w-64"}`}
      >
        <div className="flex flex-col h-full">
          {/* Logo */}
          <div className="p-4 border-b border-gray-800">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className="w-10 h-10 bg-white rounded-lg flex items-center justify-center flex-shrink-0">
                  <img src={Asset.logo} alt="Logo" className="w-8 h-8" />
                </div>
                {!collapsed && (
                  <div>
                    <h1 className="font-bold text-lg">Admin Panel</h1>
                    <p className="text-xs text-gray-400">HPLC</p>
                  </div>
                )}
              </div>
              <button
                onClick={() => setCollapsed(!collapsed)}
                className="hidden lg:block text-gray-400 hover:text-white flex-shrink-0 ml-2"
              >
                {collapsed ? <ChevronRight className="w-5 h-5" /> : <X className="w-5 h-5" />}
              </button>
            </div>
          </div>

          {/* Menu Items */}
          <nav className="flex-1 overflow-y-auto py-4 scrollbar-thin">
            {displayGroups.map((group, groupIndex) => (
              <div key={groupIndex} className="mb-6">
                {!collapsed && !mobileOpen && (
                  <h3 className="px-6 mb-2 text-xs font-semibold text-gray-400 uppercase tracking-wider">
                    {group.title}
                  </h3>
                )}
                {(mobileOpen || !collapsed) && (
                  <>
                    {mobileOpen && (
                      <h3 className="lg:hidden px-6 mb-2 text-xs font-semibold text-gray-400 uppercase tracking-wider">
                        {group.title}
                      </h3>
                    )}
                    <ul className="space-y-1 px-2">
                      {group.items.map((item) => {
                        const Icon = item.icon;
                        const isActive = location.pathname === item.path;

                        return (
                          <li key={item.path}>
                            <Link
                              to={item.path}
                              className={`flex items-center space-x-3 px-3 py-3 rounded-lg transition-all group ${
                                isActive
                                  ? "bg-white text-gray-900"
                                  : "text-gray-300 hover:bg-gray-800 hover:text-white"
                              }`}
                            >
                              <Icon className={`w-5 h-5 ${collapsed && !mobileOpen ? "mx-auto" : ""}`} />
                              {(!collapsed || mobileOpen) && <span className="font-medium">{item.label}</span>}
                            </Link>
                          </li>
                        );
                      })}
                    </ul>
                  </>
                )}
                {collapsed && !mobileOpen && (
                  <ul className="space-y-1 px-2">
                    {group.items.map((item) => {
                      const Icon = item.icon;
                      const isActive = location.pathname === item.path;

                      return (
                        <li key={item.path}>
                          <Link
                            to={item.path}
                            className={`flex items-center justify-center px-3 py-3 rounded-lg transition-all group ${
                              isActive
                                ? "bg-white text-gray-900"
                                : "text-gray-300 hover:bg-gray-800 hover:text-white"
                            }`}
                            title={item.label}
                          >
                            <Icon className="w-5 h-5" />
                          </Link>
                        </li>
                      );
                    })}
                  </ul>
                )}
              </div>
            ))}
          </nav>

          {/* Logout */}
          <div className="p-2 border-t border-gray-800">
            <button
              onClick={handleLogout}
              className={`flex items-center space-x-3 px-3 py-3 rounded-lg text-red-400 hover:bg-red-900/20 hover:text-red-300 transition-all w-full ${
                collapsed && !mobileOpen ? "justify-center" : ""
              }`}
              title={collapsed && !mobileOpen ? "Đăng xuất" : ""}
            >
              <LogOut className="w-5 h-5" />
              {(!collapsed || mobileOpen) && <span className="font-medium">Đăng xuất</span>}
            </button>
          </div>
        </div>
      </aside>

      {/* Overlay for mobile */}
      {mobileOpen && (
        <div
          className="lg:hidden fixed inset-0 bg-black/50 z-30"
          onClick={() => setMobileOpen(false)}
        />
      )}
    </>
  );
}
