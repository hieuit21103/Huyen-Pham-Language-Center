import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router";
import {
  BookOpen, Clock, Users, Calendar, DollarSign,
  CheckCircle, XCircle, AlertCircle, CreditCard, FileText, Receipt
} from "lucide-react";
import { getProfile } from "~/apis/Profile";
import { getByTaiKhoanId } from "~/apis/HocVien";
import { getDangKysByHocVien } from "~/apis/DangKy";
import { VaiTro } from "~/types/index";
import type { DangKy } from "~/types/index";

export default function MyCoursesPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [registrations, setRegistrations] = useState<DangKy[]>([]);
  const [filter, setFilter] = useState<"all" | "pending" | "approved" | "paid" | "assigned" | "rejected">("all");

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

    const hocVienRes = await getByTaiKhoanId(profileRes.data.id!);
    if (hocVienRes.success && hocVienRes.data) {
      const dangKyRes = await getDangKysByHocVien(hocVienRes.data.id!);
      if (dangKyRes.success && Array.isArray(dangKyRes.data)) {
        setRegistrations(dangKyRes.data);
      }
    }

    setLoading(false);
  };

  const getTrangThaiText = (trangThai?: number) => {
    switch (trangThai) {
      case 0: return "Chờ duyệt";
      case 1: return "Đã duyệt";
      case 2: return "Đã thanh toán";
      case 3: return "Đã xếp lớp";
      case 4: return "Từ chối";
      default: return "Không xác định";
    }
  };

  const getTrangThaiColor = (trangThai?: number) => {
    switch (trangThai) {
      case 0: return "bg-yellow-100 text-yellow-800";
      case 1: return "bg-blue-100 text-blue-800";
      case 2: return "bg-green-100 text-green-800";
      case 3: return "bg-purple-100 text-purple-800";
      case 4: return "bg-red-100 text-red-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const getTrangThaiIcon = (trangThai?: number) => {
    switch (trangThai) {
      case 0: return <AlertCircle className="w-5 h-5" />;
      case 1: return <CreditCard className="w-5 h-5" />;
      case 2: return <CheckCircle className="w-5 h-5" />;
      case 3: return <CheckCircle className="w-5 h-5" />;
      case 4: return <XCircle className="w-5 h-5" />;
      default: return <AlertCircle className="w-5 h-5" />;
    }
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

  const filteredRegistrations = registrations.filter((reg) => {
    if (filter === "all") return true;
    if (filter === "pending") return reg.trangThai === 0;
    if (filter === "approved") return reg.trangThai === 1;
    if (filter === "paid") return reg.trangThai === 2;
    if (filter === "assigned") return reg.trangThai === 3;
    if (filter === "rejected") return reg.trangThai === 4;
    return true;
  });

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
          <p className="text-gray-600">Đang tải dữ liệu...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-24 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8 flex items-center justify-between">
          <div>
            <h1 className="text-4xl font-bold text-gray-900 mb-2">Khóa học của tôi</h1>
            <p className="text-gray-600">Quản lý các khóa học bạn đã đăng ký</p>
          </div>
          <button
            onClick={() => navigate("/lich-su-thanh-toan")}
            className="flex items-center gap-2 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors shadow-sm"
          >
            <Receipt className="w-5 h-5" />
            Lịch sử thanh toán
          </button>
        </div>

        {/* Filter Tabs */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 mb-6 p-2">
          <div className="flex flex-wrap gap-2">
            <button
              onClick={() => setFilter("all")}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${filter === "all"
                  ? "bg-gray-900 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
            >
              Tất cả ({registrations.length})
            </button>
            <button
              onClick={() => setFilter("pending")}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${filter === "pending"
                  ? "bg-gray-900 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
            >
              Chờ duyệt ({registrations.filter((r) => r.trangThai === 0).length})
            </button>
            <button
              onClick={() => setFilter("approved")}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${filter === "approved"
                  ? "bg-gray-900 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
            >
              Đã duyệt ({registrations.filter((r) => r.trangThai === 1).length})
            </button>
            <button
              onClick={() => setFilter("paid")}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${filter === "paid"
                  ? "bg-gray-900 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
            >
              Đã thanh toán ({registrations.filter((r) => r.trangThai === 2).length})
            </button>
            <button
              onClick={() => setFilter("assigned")}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${filter === "assigned"
                  ? "bg-gray-900 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
            >
              Đã xếp lớp ({registrations.filter((r) => r.trangThai === 3).length})
            </button>
            <button
              onClick={() => setFilter("rejected")}
              className={`px-4 py-2 rounded-lg font-medium transition-colors ${filter === "rejected"
                  ? "bg-gray-900 text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
            >
              Từ chối ({registrations.filter((r) => r.trangThai === 4).length})
            </button>
          </div>
        </div>

        {/* Courses Grid */}
        {filteredRegistrations.length === 0 ? (
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
            <BookOpen className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <h3 className="text-xl font-semibold text-gray-900 mb-2">Chưa có khóa học nào</h3>
            <p className="text-gray-600 mb-6">
              {filter === "all"
                ? "Bạn chưa đăng ký khóa học nào"
                : "Không có khóa học nào phù hợp với bộ lọc"}
            </p>
            <button
              onClick={() => navigate("/khoa-hoc")}
              className="bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
            >
              Khám phá khóa học
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {filteredRegistrations.map((registration) => (
              <div
                key={registration.id}
                className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow"
              >
                <div className="p-6">
                  {/* Status Badge */}
                  <div className="flex items-center justify-between mb-4">
                    <span className={`inline-flex items-center gap-2 px-3 py-1 rounded-full text-xs font-semibold ${getTrangThaiColor(registration.trangThai)}`}>
                      {getTrangThaiIcon(registration.trangThai)}
                      {getTrangThaiText(registration.trangThai)}
                    </span>
                  </div>

                  {/* Course Title */}
                  <h3 className="text-xl font-bold text-gray-900 mb-2">
                    {registration.khoaHoc?.tenKhoaHoc || "Tên khóa học không xác định"}
                  </h3>

                  {/* Course Info */}
                  <div className="space-y-2 mb-4">
                    <div className="flex items-center text-sm text-gray-600">
                      <Calendar className="w-4 h-4 mr-2" />
                      Ngày đăng ký: {formatDate(registration.ngayDangKy)}
                    </div>
                    <div className="flex items-center text-sm text-gray-600">
                      <Calendar className="w-4 h-4 mr-2" />
                      Ngày khai giảng: {formatDate(registration.khoaHoc?.ngayKhaiGiang)}
                    </div>
                    <div className="flex items-center text-sm text-gray-600">
                      <Clock className="w-4 h-4 mr-2" />
                      Thời lượng: {registration.khoaHoc?.thoiLuong} phút
                    </div>
                    <div className="flex items-center text-sm text-gray-600">
                      <DollarSign className="w-4 h-4 mr-2" />
                      Học phí: {formatCurrency(registration.khoaHoc?.hocPhi)}
                    </div>
                    {registration.lopHoc && (
                      <div className="flex items-center text-sm text-gray-600">
                        <Users className="w-4 h-4 mr-2" />
                        Lớp: {registration.lopHoc.tenLop} ({registration.lopHoc.siSoHienTai}/{registration.lopHoc.siSoToiDa})
                      </div>
                    )}
                  </div>

                  {/* Action Buttons */}
                  <div className="flex gap-2">
                    {registration.trangThai === 1 && ( // Đã duyệt, chưa thanh toán
                      <button
                        onClick={() => {
                          console.log('Navigating with ID:', registration.id);
                          navigate(`/thanh-toan?dangKyId=${registration.id}`);
                        }}
                        className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center gap-2"
                      >
                        <CreditCard className="w-4 h-4" />
                        Thanh toán ngay
                      </button>
                    )}
                    {registration.trangThai === 2 && ( // Đã thanh toán
                      <button
                        disabled
                        className="flex-1 bg-green-100 text-green-700 px-4 py-2 rounded-lg cursor-not-allowed flex items-center justify-center gap-2"
                      >
                        <CheckCircle className="w-4 h-4" />
                        Đã thanh toán
                      </button>
                    )}
                    {registration.trangThai === 0 && ( // Chờ duyệt
                      <button
                        disabled
                        className="flex-1 bg-gray-100 text-gray-600 px-4 py-2 rounded-lg cursor-not-allowed flex items-center justify-center gap-2"
                      >
                        <AlertCircle className="w-4 h-4" />
                        Chờ duyệt
                      </button>
                    )}
                    {registration.trangThai === 3 && ( // Đã xếp lớp
                      <button
                        disabled
                        className="flex-1 bg-red-100 text-purple-700 px-4 py-2 rounded-lg cursor-not-allowed flex items-center justify-center gap-2"
                      >
                        <CheckCircle className="w-4 h-4" />
                        Đã xếp lớp
                      </button>
                    )}
                    {registration.trangThai === 4 && ( // Từ chối
                      <button
                        disabled
                        className="flex-1 bg-red-100 text-red-700 px-4 py-2 rounded-lg cursor-not-allowed flex items-center justify-center gap-2"
                      >
                        <XCircle className="w-4 h-4" />
                        Đã từ chối
                      </button>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
