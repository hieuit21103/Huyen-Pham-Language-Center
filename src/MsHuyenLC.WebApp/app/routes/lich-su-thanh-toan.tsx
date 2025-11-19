import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import {
  Receipt, Calendar, CreditCard, CheckCircle,
  XCircle, Clock, AlertCircle, ArrowLeft,
  Filter, Search, DollarSign
} from "lucide-react";
import { getProfile } from "~/apis/Profile";
import { getThanhToans, type ThanhToan } from "~/apis/ThanhToan";
import { VaiTro } from "~/types/index";
import Pagination from "~/components/Pagination";

export default function PaymentHistoryPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [payments, setPayments] = useState<ThanhToan[]>([]);
  const [filteredPayments, setFilteredPayments] = useState<ThanhToan[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [filterStatus, setFilterStatus] = useState<number | "all">("all");
  const [filterMethod, setFilterMethod] = useState<number | "all">("all");
  
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    applyFilters();
  }, [searchTerm, filterStatus, filterMethod, payments]);

  // Reset to page 1 when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, filterStatus, filterMethod]);

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

    // Lấy hết tất cả data từ backend
    const paymentsRes = await getThanhToans(1, 1000);
    if (paymentsRes.success && paymentsRes.data) {
      setPayments(paymentsRes.data);
    }

    setLoading(false);
  };

  const applyFilters = () => {
    let filtered = [...payments];

    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(
        (p) =>
          p.maThanhToan?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          p.tenKhoaHoc?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          p.tenHocVien?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Status filter
    if (filterStatus !== "all") {
      filtered = filtered.filter((p) => p.trangThai === filterStatus);
    }

    // Method filter
    if (filterMethod !== "all") {
      filtered = filtered.filter((p) => p.phuongThuc === filterMethod);
    }

    setFilteredPayments(filtered);
  };

  // Tính toán data cho trang hiện tại
  const getPaginatedData = () => {
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    return filteredPayments.slice(startIndex, endIndex);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
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
    return new Date(dateString).toLocaleDateString("vi-VN", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const getStatusInfo = (status?: number) => {
    switch (status) {
      case 0:
        return {
          label: "Chờ thanh toán",
          color: "text-yellow-700 bg-yellow-50 border-yellow-200",
          icon: Clock,
        };
      case 1:
        return {
          label: "Đã thanh toán",
          color: "text-green-700 bg-green-50 border-green-200",
          icon: CheckCircle,
        };
      case 2:
        return {
          label: "Đã hủy",
          color: "text-red-700 bg-red-50 border-red-200",
          icon: XCircle,
        };
      default:
        return {
          label: "Không xác định",
          color: "text-gray-700 bg-gray-50 border-gray-200",
          icon: AlertCircle,
        };
    }
  };

  const getMethodLabel = (method?: number) => {
    switch (method) {
      case 0:
        return "VNPay";
      case 1:
        return "MoMo";
      case 2:
        return "Tiền mặt";
      default:
        return "—";
    }
  };

  const getMethodColor = (method?: number) => {
    switch (method) {
      case 0:
        return "text-blue-700 bg-blue-50 border-blue-200";
      case 1:
        return "text-pink-700 bg-pink-50 border-pink-200";
      case 2:
        return "text-green-700 bg-green-50 border-green-200";
      default:
        return "text-gray-700 bg-gray-50 border-gray-200";
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
          <p className="text-gray-600">Đang tải lịch sử thanh toán...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-24 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
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
          <h1 className="text-4xl font-bold text-gray-900 mb-2 flex items-center gap-3">
            <Receipt className="w-10 h-10" />
            Lịch sử thanh toán
          </h1>
          <p className="text-gray-600">Quản lý và theo dõi các giao dịch thanh toán của bạn</p>
        </div>

        {/* Filters */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
          <div className="flex items-center gap-2 mb-4">
            <Filter className="w-5 h-5 text-gray-600" />
            <h2 className="text-lg font-semibold text-gray-900">Bộ lọc</h2>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Search */}
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                type="text"
                placeholder="Tìm kiếm theo mã, khóa học..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* Status Filter */}
            <select
              value={filterStatus}
              onChange={(e) =>
                setFilterStatus(e.target.value === "all" ? "all" : Number(e.target.value))
              }
              className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="all">Tất cả trạng thái</option>
              <option value="0">Chờ thanh toán</option>
              <option value="1">Đã thanh toán</option>
              <option value="2">Đã hủy</option>
            </select>

            {/* Method Filter */}
            <select
              value={filterMethod}
              onChange={(e) =>
                setFilterMethod(e.target.value === "all" ? "all" : Number(e.target.value))
              }
              className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              <option value="all">Tất cả phương thức</option>
              <option value="0">VNPay</option>
              <option value="1">MoMo</option>
              <option value="2">Tiền mặt</option>
            </select>
          </div>
        </div>

        {/* Statistics */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-gray-600">Tổng giao dịch</p>
              <Receipt className="w-5 h-5 text-gray-400" />
            </div>
            <p className="text-2xl font-bold text-gray-900">{payments.length}</p>
          </div>

          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-gray-600">Đã thanh toán</p>
              <CheckCircle className="w-5 h-5 text-green-500" />
            </div>
            <p className="text-2xl font-bold text-green-600">
              {payments.filter((p) => p.trangThai === 1).length}
            </p>
          </div>

          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-gray-600">Chờ thanh toán</p>
              <Clock className="w-5 h-5 text-yellow-500" />
            </div>
            <p className="text-2xl font-bold text-yellow-600">
              {payments.filter((p) => p.trangThai === 0).length}
            </p>
          </div>

          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div className="flex items-center justify-between mb-2">
              <p className="text-gray-600">Tổng tiền</p>
              <DollarSign className="w-5 h-5 text-blue-500" />
            </div>
            <p className="text-lg font-bold text-blue-600">
              {formatCurrency(
                payments.filter((p) => p.trangThai === 1).reduce((sum, p) => sum + (p.soTien || 0), 0)
              )}
            </p>
          </div>
        </div>

        {/* Payment List */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          {filteredPayments.length === 0 ? (
            <div className="text-center py-12">
              <Receipt className="w-16 h-16 text-gray-300 mx-auto mb-4" />
              <p className="text-gray-500 text-lg">Không có giao dịch nào</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50 border-b border-gray-200">
                  <tr>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Mã thanh toán
                    </th>
                    {/* <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Khóa học
                    </th> */}
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Số tiền
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Phương thức
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Trạng thái
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Ngày tạo
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Ngày thanh toán
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {getPaginatedData().map((payment) => {
                    const statusInfo = getStatusInfo(payment.trangThai);
                    const StatusIcon = statusInfo.icon;

                    return (
                      <tr key={payment.id} className="hover:bg-gray-50 transition-colors">
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="flex items-center gap-2">
                            <CreditCard className="w-4 h-4 text-gray-400" />
                            <span className="font-mono text-sm font-semibold text-gray-900">
                              {payment.id || "—"}
                            </span>
                          </div>
                        </td>
                        {/* <td className="px-6 py-4">
                          <p className="font-semibold text-gray-900 truncate max-w-xs">
                            {payment.tenKhoaHoc || "—"}
                          </p>
                          <p className="text-sm text-gray-500">{payment.tenHocVien || "—"}</p>
                        </td> */}
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className="font-bold text-gray-900">
                            {formatCurrency(payment.soTien)}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span
                            className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold border ${getMethodColor(
                              payment.phuongThuc
                            )}`}
                          >
                            {getMethodLabel(payment.phuongThuc)}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span
                            className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold border ${statusInfo.color}`}
                          >
                            <StatusIcon className="w-3.5 h-3.5" />
                            {statusInfo.label}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="flex items-center gap-2 text-sm text-gray-600">
                            <Calendar className="w-4 h-4" />
                            {payment.ngayLap}
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="flex items-center gap-2 text-sm text-gray-600">
                            <Calendar className="w-4 h-4" />
                            {payment.ngayThanhToan}
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}
        </div>

        {/* Additional Info */}
        {filteredPayments.length > 0 && (
          <>
            <div className="mt-4 text-sm text-gray-500 text-center">
              Hiển thị {((currentPage - 1) * pageSize) + 1} - {Math.min(currentPage * pageSize, filteredPayments.length)} / {filteredPayments.length} giao dịch
            </div>
            
            {/* Pagination */}
            <div className="mt-6 flex justify-center">
              <Pagination
                currentPage={currentPage}
                totalCount={filteredPayments.length}
                pageSize={pageSize}
                onPageChange={handlePageChange}
              />
            </div>
          </>
        )}
      </div>
    </div>
  );
}
