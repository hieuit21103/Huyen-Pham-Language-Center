import { Users, BookOpen, GraduationCap, TrendingUp, FileText, CheckCircle } from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { getProfile } from "~/apis/Profile";
import { getHocViens } from "~/apis/HocVien";
import { getGiaoViens } from "~/apis/GiaoVien";
import { getKhoaHocs } from "~/apis/KhoaHoc";
import { getDangKys } from "~/apis/DangKy";
import { VaiTro, TrangThaiDangKy, type HocVien, type KhoaHoc, type LopHoc, type DangKy } from "~/types/index";
import { setLightTheme } from "./_layout";

interface DashboardStats {
  icon: any;
  label: string;
  value: string;
  color: string;
  loading?: boolean;
}

export default function AdminDashboard() {
  const navigate = useNavigate();
  const [userRole, setUserRole] = useState<VaiTro | null>(null);
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState<DashboardStats[]>([
    { icon: Users, label: "Tổng học viên", value: "0", color: "blue", loading: true },
    { icon: GraduationCap, label: "Giáo viên", value: "0", color: "green", loading: true },
    { icon: BookOpen, label: "Khóa học", value: "0", color: "purple", loading: true },
    { icon: FileText, label: "Đăng ký mới", value: "0", color: "orange", loading: true },
  ]);
  const [recentRegistrations, setRecentRegistrations] = useState<DangKy[]>([]);

  useEffect(() => {
    loadDashboardData();
    setLightTheme();
  }, []);

  const loadDashboardData = async () => {
    setLoading(true);
    
    const profileRes = await getProfile();
    if (profileRes.success && profileRes.data) {
      setUserRole(profileRes.data.vaiTro);
    }

    const [hocViensRes, giaoViensRes, khoaHocsRes, dangKysRes] = await Promise.all([
      getHocViens({ pageNumber: 1, pageSize: 1000 }),
      getGiaoViens({ pageNumber: 1, pageSize: 1000 }),
      getKhoaHocs({ pageNumber: 1, pageSize: 1000 }),
      getDangKys({ pageNumber: 1, pageSize: 10, sortBy: 'ngayDangKy', sortOrder: 'desc' }),
    ]);

    const newStats = [...stats];
    if (hocViensRes.success && Array.isArray(hocViensRes.data)) {
      newStats[0] = { ...newStats[0], value: hocViensRes.data.length.toString(), loading: false };
    }
    if (giaoViensRes.success && Array.isArray(giaoViensRes.data)) {
      newStats[1] = { ...newStats[1], value: giaoViensRes.data.length.toString(), loading: false };
    }
    if (khoaHocsRes.success && Array.isArray(khoaHocsRes.data)) {
      newStats[2] = { ...newStats[2], value: khoaHocsRes.data.length.toString(), loading: false };
    }
    if (dangKysRes.success && Array.isArray(dangKysRes.data)) {
      newStats[3] = { ...newStats[3], value: dangKysRes.data.length.toString(), loading: false };
      setRecentRegistrations(dangKysRes.data.slice(0, 5));
    }
    
    setStats(newStats);
    setLoading(false);
  };

  const getColorClasses = (color: string) => {
    const colors: any = {
      blue: "bg-blue-100 text-blue-600",
      green: "bg-green-100 text-green-600",
      purple: "bg-purple-100 text-purple-600",
      orange: "bg-orange-100 text-orange-600"
    };
    return colors[color] || colors.blue;
  };

  const getStatusText = (status?: TrangThaiDangKy) => {
    switch(status) {
      case TrangThaiDangKy.ChoDuyet: return "Chờ duyệt";
      case TrangThaiDangKy.DaDuyet: return "Đã duyệt";
      case TrangThaiDangKy.DaThanhToan: return "Đã thanh toán";
      case TrangThaiDangKy.DaXepLop: return "Đã xếp lớp";
      case TrangThaiDangKy.Huy: return "Đã hủy";
      default: return "Không xác định";
    }
  };

  const getStatusColor = (status?: TrangThaiDangKy) => {
    switch(status) {
      case TrangThaiDangKy.DaDuyet:
      case TrangThaiDangKy.DaXepLop:
        return "bg-green-100 text-green-800";
      case TrangThaiDangKy.ChoDuyet:
      case TrangThaiDangKy.DaThanhToan:
        return "bg-yellow-100 text-yellow-800";
      case TrangThaiDangKy.Huy:
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-1">
          Chào mừng trở lại, {userRole === VaiTro.Admin ? "Admin" : "Giáo vụ"}!
        </p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <div
              key={index}
              className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
            >
              <div className="flex items-center justify-between mb-4">
                <div className={`w-12 h-12 ${getColorClasses(stat.color)} rounded-lg flex items-center justify-center`}>
                  <Icon className="w-6 h-6" />
                </div>
              </div>
              <h3 className="text-gray-600 text-sm font-medium mb-1">{stat.label}</h3>
              {stat.loading ? (
                <div className="h-9 bg-gray-200 animate-pulse rounded mt-1"></div>
              ) : (
                <p className="text-3xl font-bold text-gray-900">{stat.value}</p>
              )}
            </div>
          );
        })}
      </div>

      {/* Recent Registrations */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        <div className="p-6 border-b border-gray-200">
          <h2 className="text-xl font-bold text-gray-900">Đăng ký gần đây</h2>
        </div>
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
            <p className="text-gray-600 mt-2">Đang tải dữ liệu...</p>
          </div>
        ) : recentRegistrations.length === 0 ? (
          <div className="p-8 text-center text-gray-600">
            Chưa có đăng ký nào
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b border-gray-200">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Học viên
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Email
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Khóa học
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Ngày đăng ký
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Trạng thái
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {recentRegistrations.map((registration) => (
                    <tr key={registration.id} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div className="w-10 h-10 bg-gray-900 rounded-full flex items-center justify-center text-white font-semibold">
                            {registration.hocVien?.hoTen?.charAt(0) || "?"}
                          </div>
                          <div className="ml-3">
                            <p className="text-sm font-medium text-gray-900">
                              {registration.hocVien?.hoTen || "Chưa có tên"}
                            </p>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                        {registration.hocVien?.taiKhoan?.email || "—"}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">
                        {registration.khoaHoc?.tenKhoaHoc || "—"}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                        {registration.ngayDangKy 
                          ? new Date(registration.ngayDangKy).toLocaleDateString('vi-VN')
                          : "—"}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${getStatusColor(registration.trangThai)}`}>
                          {registration.trangThai === TrangThaiDangKy.DaDuyet && (
                            <CheckCircle className="w-3 h-3 mr-1" />
                          )}
                          {getStatusText(registration.trangThai)}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            <div className="p-4 border-t border-gray-200">
              <button 
                onClick={() => navigate("/admin/dang-ky")}
                className="text-gray-900 font-semibold hover:text-gray-700 text-sm"
              >
                Xem tất cả →
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
