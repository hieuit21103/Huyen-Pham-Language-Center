import { Calendar, DollarSign, TrendingUp, FileText, Download, Filter, CreditCard, Clock, CheckCircle, XCircle } from "lucide-react";
import { useState, useEffect } from "react";
import { getThanhToans } from "~/apis/ThanhToan";
import { getDangKys } from "~/apis/DangKy";
import { setLightTheme } from "./_layout";
import type { ThanhToan } from "~/apis/ThanhToan";
import type { DangKy } from "~/types/course.types";
import type { DoanhThuStats, DoanhThuTheoNgay, ThanhToanResponse } from "~/types/finance.types";

export default function BaoCaoDoanhThu() {
  const [loading, setLoading] = useState(true);
  const [thanhToans, setThanhToans] = useState<ThanhToanResponse[]>([]);
  const [dangKys, setDangKys] = useState<DangKy[]>([]);
  const [message, setMessage] = useState("");

  // Filters
  const [tuNgay, setTuNgay] = useState("");
  const [denNgay, setDenNgay] = useState("");
  const [khoaHocFilter, setKhoaHocFilter] = useState("");
  const [trangThaiFilter, setTrangThaiFilter] = useState<number>(-1); // -1: Tất cả, 0: Chưa TT, 1: Đã TT

  useEffect(() => {
    setLightTheme();
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    
    // Load thanh toán
    const ttResponse = await getThanhToans();
    if (ttResponse.success && ttResponse.data) {
      setThanhToans(ttResponse.data);
    }

    // Load đăng ký để lấy thông tin học viên và khóa học
    const dkResponse = await getDangKys();
    if (dkResponse.success && dkResponse.data) {
      setDangKys(dkResponse.data);
    }

    setLoading(false);
  };

  // Helper function để lấy thông tin từ đăng ký
  const getDangKyInfo = (dangKyId?: string) => {
    if (!dangKyId) return { tenHocVien: "—", tenKhoaHoc: "—", khoaHocId: "" };
    const dangKy = dangKys.find(dk => dk.id === dangKyId);
    return {
      tenHocVien: dangKy?.hocVien?.hoTen || dangKy?.hocVien?.hoTen || "—",
      tenKhoaHoc: dangKy?.khoaHoc?.tenKhoaHoc || "—",
      khoaHocId: dangKy?.khoaHocId || "",
    };
  };

  // Danh sách khóa học unique để filter
  const uniqueKhoaHocs = Array.from(
    new Map(
      dangKys
        .filter(dk => dk.khoaHoc?.tenKhoaHoc)
        .map(dk => [dk.khoaHoc?.tenKhoaHoc, { tenKhoaHoc: dk.khoaHoc?.tenKhoaHoc }])
    ).values()
  );

  // Filter dữ liệu
  const filteredThanhToans = thanhToans.filter((tt) => {
    // Filter theo ngày
    if (tuNgay && tt.ngayLap) {
      const ngayLap = new Date(tt.ngayLap);
      const tuNgayDate = new Date(tuNgay);
      if (ngayLap < tuNgayDate) return false;
    }
    if (denNgay && tt.ngayLap) {
      const ngayLap = new Date(tt.ngayLap);
      const denNgayDate = new Date(denNgay);
      denNgayDate.setHours(23, 59, 59, 999);
      if (ngayLap > denNgayDate) return false;
    }

    if (khoaHocFilter && tt.dangKyId) {
      const dangKyInfo = getDangKyInfo(tt.dangKyId);
      if (dangKyInfo.tenKhoaHoc !== khoaHocFilter) return false;
    }

    if (trangThaiFilter !== -1 && tt.trangThai !== trangThaiFilter) return false;

    return true;
  });

  const stats: DoanhThuStats = {
    tongDoanhThu: filteredThanhToans.reduce((sum, tt) => sum + (tt.soTien || 0), 0),
    tongDaThanhToan: filteredThanhToans
      .filter(tt => tt.trangThai === 1)
      .reduce((sum, tt) => sum + (tt.soTien || 0), 0),
    tongChuaThanhToan: filteredThanhToans
      .filter(tt => tt.trangThai === 0)
      .reduce((sum, tt) => sum + (tt.soTien || 0), 0),
    soLuongGiaoDich: filteredThanhToans.length,
    soLuongDaThanhToan: filteredThanhToans.filter(tt => tt.trangThai === 1).length,
    soLuongChuaThanhToan: filteredThanhToans.filter(tt => tt.trangThai === 0).length,
  };

  const doanhThuTheoNgay: DoanhThuTheoNgay[] = [];
  const ngayMap = new Map<string, DoanhThuTheoNgay>();

  filteredThanhToans.forEach((tt) => {
    if (!tt.ngayLap) return;
    const ngay = new Date(tt.ngayLap).toISOString().split('T')[0];
    
    if (!ngayMap.has(ngay)) {
      ngayMap.set(ngay, {
        ngay,
        soLuong: 0,
        tongTien: 0,
        daThanhToan: 0,
        chuaThanhToan: 0,
      });
    }

    const item = ngayMap.get(ngay)!;
    item.soLuong++;
    item.tongTien += tt.soTien || 0;
    if (tt.trangThai === 1) {
      item.daThanhToan += tt.soTien || 0;
    } else {
      item.chuaThanhToan += tt.soTien || 0;
    }
  });

  doanhThuTheoNgay.push(...Array.from(ngayMap.values()).sort((a, b) => b.ngay.localeCompare(a.ngay)));

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
    }).format(amount);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return "—";
    return new Date(dateString).toLocaleDateString('vi-VN');
  };

  const getTrangThaiText = (trangThai?: number) => {
    switch(trangThai) {
      case 0: return "Chưa thanh toán";
      case 1: return "Đã thanh toán";
      default: return "N/A";
    }
  };

  const getTrangThaiColor = (trangThai?: number) => {
    switch(trangThai) {
      case 0: return "bg-yellow-100 text-yellow-800";
      case 1: return "bg-green-100 text-green-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const handleExportExcel = () => {
    // Tạo CSV data
    const headers = ["Ngày", "Mã thanh toán", "Khóa học", "Học viên", "Số tiền", "Trạng thái", "Ngày thanh toán"];
    const rows = filteredThanhToans.map(tt => {
      const dangKyInfo = getDangKyInfo(tt.dangKyId);
      return [
        formatDate(tt.ngayLap),
        tt.maThanhToan || "",
        dangKyInfo.tenKhoaHoc,
        dangKyInfo.tenHocVien,
        tt.soTien || 0,
        getTrangThaiText(tt.trangThai),
        formatDate(tt.ngayThanhToan),
      ];
    });

    const csvContent = [
      headers.join(","),
      ...rows.map(row => row.join(",")),
    ].join("\n");

    const blob = new Blob(["\uFEFF" + csvContent], { type: "text/csv;charset=utf-8;" });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = `bao-cao-doanh-thu-${new Date().getTime()}.csv`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);

    setMessage("Xuất báo cáo Excel thành công");
    setTimeout(() => setMessage(""), 3000);
  };

  const resetFilters = () => {
    setTuNgay("");
    setDenNgay("");
    setKhoaHocFilter("");
    setTrangThaiFilter(-1);
  };

  return (
    <div className="space-y-6">
      {message && (
        <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg">
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Báo cáo doanh thu</h1>
          <p className="text-gray-600 mt-1">Thống kê và phân tích doanh thu</p>
        </div>
        <button
          onClick={handleExportExcel}
          className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2"
        >
          <Download className="w-5 h-5" />
          <span>Xuất Excel</span>
        </button>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-semibold text-gray-900 flex items-center space-x-2">
            <Filter className="w-5 h-5" />
            <span>Bộ lọc</span>
          </h2>
          <button
            onClick={resetFilters}
            className="text-sm text-blue-600 hover:text-blue-800 font-medium"
          >
            Đặt lại
          </button>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Từ ngày</label>
            <input
              type="date"
              value={tuNgay}
              onChange={(e) => setTuNgay(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Đến ngày</label>
            <input
              type="date"
              value={denNgay}
              onChange={(e) => setDenNgay(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Khóa học</label>
            <select
              value={khoaHocFilter}
              onChange={(e) => setKhoaHocFilter(e.target.value)}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            >
              <option value="">Tất cả khóa học</option>
              {uniqueKhoaHocs.map((kh) => (
                <option key={kh.tenKhoaHoc} value={kh.tenKhoaHoc}>
                  {kh.tenKhoaHoc}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Trạng thái</label>
            <select
              value={trangThaiFilter}
              onChange={(e) => setTrangThaiFilter(Number(e.target.value))}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            >
              <option value={-1}>Tất cả</option>
              <option value={0}>Chưa thanh toán</option>
              <option value={1}>Đã thanh toán</option>
            </select>
          </div>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div className="bg-white border border-blue-200 rounded-xl shadow-sm p-6">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-blue-600 mb-2">Tổng giao dịch</p>
              <p className="text-3xl font-bold text-gray-900">{formatCurrency(stats.tongDoanhThu)}</p>
            </div>
            <div className="bg-blue-50 p-3 rounded-lg">
              <FileText className="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div className="bg-white border border-green-200 rounded-xl shadow-sm p-6">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-green-600 mb-2">Đã thanh toán</p>
              <p className="text-3xl font-bold text-gray-900">{formatCurrency(stats.tongDaThanhToan)}</p>
            </div>
            <div className="bg-green-50 p-3 rounded-lg">
              <CheckCircle className="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div className="bg-white border border-orange-200 rounded-xl shadow-sm p-6">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <p className="text-sm font-medium text-orange-600 mb-2">Chưa thanh toán</p>
              <p className="text-3xl font-bold text-gray-900">{formatCurrency(stats.tongChuaThanhToan)}</p>
            </div>
            <div className="bg-orange-50 p-3 rounded-lg">
              <Clock className="w-6 h-6 text-orange-600" />
            </div>
          </div>
        </div>
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 gap-6">
        {/* Doanh thu theo ngày */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center space-x-2">
            <Calendar className="w-5 h-5" />
            <span>Doanh thu theo ngày</span>
          </h2>
          {loading ? (
            <div className="text-center py-8">
              <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
            </div>
          ) : doanhThuTheoNgay.length === 0 ? (
            <p className="text-gray-500 text-center py-8">Không có dữ liệu</p>
          ) : (
            <div className="space-y-3 max-h-96 overflow-y-auto">
              {doanhThuTheoNgay.slice(0, 10).map((item) => (
                <div key={item.ngay} className="border-b border-gray-100 pb-3">
                  <div className="flex justify-between items-center mb-2">
                    <span className="font-medium text-gray-900">{formatDate(item.ngay)}</span>
                    <span className="text-sm text-gray-600">{item.soLuong} giao dịch</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-green-600">Đã TT: {formatCurrency(item.daThanhToan)}</span>
                    <span className="text-yellow-600">Chưa TT: {formatCurrency(item.chuaThanhToan)}</span>
                  </div>
                  <div className="mt-2">
                    <div className="w-full bg-gray-200 rounded-full h-2">
                      <div
                        className="bg-green-500 h-2 rounded-full"
                        style={{ width: `${item.tongTien > 0 ? (item.daThanhToan / item.tongTien) * 100 : 0}%` }}
                      ></div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Transaction List */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        <div className="p-6 border-b border-gray-200">
          <h2 className="text-lg font-semibold text-gray-900 flex items-center space-x-2">
            <FileText className="w-5 h-5" />
            <span>Chi tiết giao dịch ({filteredThanhToans.length})</span>
          </h2>
        </div>
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : filteredThanhToans.length === 0 ? (
          <div className="p-8 text-center">
            <CreditCard className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Không có giao dịch nào</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Mã GD
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Ngày lập
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Học viên
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Khóa học
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Số tiền
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Ngày TT
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredThanhToans.slice(0, 50).map((tt) => {
                  const dangKyInfo = getDangKyInfo(tt.dangKyId);
                  return (
                    <tr key={tt.id} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="text-sm font-medium text-gray-900">{tt.maThanhToan || "—"}</span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                        {formatDate(tt.ngayLap)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {dangKyInfo.tenHocVien}
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-900">
                        <span className="line-clamp-2">{dangKyInfo.tenKhoaHoc}</span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-blue-600">
                        {formatCurrency(tt.soTien || 0)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getTrangThaiColor(tt.trangThai)}`}>
                          {getTrangThaiText(tt.trangThai)}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                        {formatDate(tt.ngayThanhToan)}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
