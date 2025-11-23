import { Search, Calendar, User, Activity, Filter, Download, RefreshCw, FileText, Clock, AlertCircle } from "lucide-react";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { 
  getSystemLogs, 
  getSystemLogsByDateRange,
  searchSystemLogs 
} from "~/apis/SystemLogger";
import { formatDateTime } from "~/utils/date-utils";
import { setLightTheme } from "./_layout";
import Pagination from "~/components/Pagination";
import type { NhatKyHeThong } from "~/types/system.types";
import { getProfile } from "~/apis/Profile";
import { VaiTro } from "~/types/enums";


export default function AdminSystemLogger() {
  const navigate = useNavigate();
  const [logs, setLogs] = useState<NhatKyHeThong[]>([]);
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");
  const [isAdmin, setIsAdmin] = useState(false);
  
  const [searchTerm, setSearchTerm] = useState("");
  const [filterAction, setFilterAction] = useState("");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  useEffect(() => {
    setLightTheme();
    checkAdminAccess();
  }, []);

  const checkAdminAccess = async () => {
    const response = await getProfile();
    if (!response.success || !response.data) {
      navigate("/dang-nhap");
      return;
    }
    if (response.data.vaiTro !== VaiTro.Admin) {
      setIsAdmin(false);
      setLoading(false);
      return;
    }
    setIsAdmin(true);
    loadLogs();
  };

  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, filterAction, fromDate, toDate]);

  const loadLogs = async () => {
    setLoading(true);
    const response = await getSystemLogs();
    
    if (response.success && Array.isArray(response.data)) {
      const sortedLogs = response.data.sort((a: NhatKyHeThong, b: NhatKyHeThong) => {
        const dateA = new Date(a.thoiGian || 0).getTime();
        const dateB = new Date(b.thoiGian || 0).getTime();
        return dateB - dateA; 
      });
      setLogs(sortedLogs);
    } else if (response.success && response.data?.items) {
      const sortedLogs = response.data.items.sort((a: NhatKyHeThong, b: NhatKyHeThong) => {
        const dateA = new Date(a.thoiGian || 0).getTime();
        const dateB = new Date(b.thoiGian || 0).getTime();
        return dateB - dateA; 
      });
      setLogs(sortedLogs);
    }
    setLoading(false);
  };

  const handleSearch = async () => {
    setLoading(true);
    setCurrentPage(1);
    
    if (fromDate && toDate) {
      const response = await getSystemLogsByDateRange(fromDate, toDate);
      if (response.success && Array.isArray(response.data)) {
        const sortedLogs = response.data.sort((a: NhatKyHeThong, b: NhatKyHeThong) => {
          const dateA = new Date(a.thoiGian || 0).getTime();
          const dateB = new Date(b.thoiGian || 0).getTime();
          return dateB - dateA;
        });
        setLogs(sortedLogs);
      } else if (response.success && response.data?.items) {
        const sortedLogs = response.data.items.sort((a: NhatKyHeThong, b: NhatKyHeThong) => {
          const dateA = new Date(a.thoiGian || 0).getTime();
          const dateB = new Date(b.thoiGian || 0).getTime();
          return dateB - dateA;
        });
        setLogs(sortedLogs);
      }
    } else if (filterAction) {
      const response = await searchSystemLogs("", filterAction);
      if (response.success && Array.isArray(response.data)) {
        const sortedLogs = response.data.sort((a: NhatKyHeThong, b: NhatKyHeThong) => {
          const dateA = new Date(a.thoiGian || 0).getTime();
          const dateB = new Date(b.thoiGian || 0).getTime();
          return dateB - dateA;
        });
        setLogs(sortedLogs);
      } else if (response.success && response.data?.items) {
        const sortedLogs = response.data.items.sort((a: NhatKyHeThong, b: NhatKyHeThong) => {
          const dateA = new Date(a.thoiGian || 0).getTime();
          const dateB = new Date(b.thoiGian || 0).getTime();
          return dateB - dateA;
        });
        setLogs(sortedLogs);
      }
    } else {
      await loadLogs();
    }
    
    setLoading(false);
  };

  const handleReset = () => {
    setSearchTerm("");
    setFilterAction("");
    setFromDate("");
    setToDate("");
    setCurrentPage(1);
    loadLogs();
  };

  const handleExport = () => {
    const csv = [
      ["Thời gian", "Tài khoản", "Hành động", "Chi tiết", "IP Address", "Dữ liệu cũ", "Dữ liệu mới"].join(","),
      ...logs.map(log => [
        log.thoiGian || "",
        log.taiKhoan?.tenDangNhap || "",
        log.hanhDong || "",
        `"${(log.chiTiet || "").replace(/"/g, '""')}"`,
        log.ip || "",
        `"${(log.duLieuCu || "").replace(/"/g, '""')}"`,
        `"${(log.duLieuMoi || "").replace(/"/g, '""')}"`
      ].join(","))
    ].join("\n");

    const blob = new Blob(["\uFEFF" + csv], { type: "text/csv;charset=utf-8;" });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `system-logs-${new Date().toISOString().split('T')[0]}.csv`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
  };

  const getActionColor = (action?: string) => {
    if (!action) return "bg-gray-100 text-gray-800";
    
    const actionLower = action.toLowerCase();
    if (actionLower.includes("thêm") || actionLower.includes("create") || actionLower.includes("add")) return "bg-green-100 text-green-800";
    if (actionLower.includes("sửa") || actionLower.includes("update") || actionLower.includes("edit")) return "bg-blue-100 text-blue-800";
    if (actionLower.includes("xóa") || actionLower.includes("delete") || actionLower.includes("remove")) return "bg-red-100 text-red-800";
    if (actionLower.includes("login") || actionLower.includes("logout") || actionLower.includes("đăng nhập") || actionLower.includes("đăng xuất")) return "bg-purple-100 text-purple-800";
    if (actionLower.includes("view") || actionLower.includes("get") || actionLower.includes("xem")) return "bg-gray-100 text-gray-800";
    return "bg-yellow-100 text-yellow-800";
  };

  const getStatusColor = (statusCode?: number) => {
    if (!statusCode) return "text-gray-500";
    if (statusCode >= 200 && statusCode < 300) return "text-green-600";
    if (statusCode >= 400 && statusCode < 500) return "text-orange-600";
    if (statusCode >= 500) return "text-red-600";
    return "text-gray-500";
  };

  const uniqueActions = Array.from(new Set(logs.map(l => l.hanhDong).filter(Boolean)));

  const filteredLogs = logs.filter(log => {
    const matchSearch = !searchTerm || 
      (log.chiTiet || "").toLowerCase().includes(searchTerm.toLowerCase()) ||
      (log.hanhDong || "").toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchAction = !filterAction || 
      (log.hanhDong === filterAction);
    
    return matchSearch && matchAction;
  });

  const getPaginatedData = () => {
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    return filteredLogs.slice(startIndex, endIndex);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
          <p className="text-gray-600">Đang kiểm tra quyền truy cập...</p>
        </div>
      </div>
    );
  }

  if (!isAdmin) {
    return (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="bg-white rounded-xl shadow-lg border border-gray-200 p-8 max-w-md w-full text-center">
          <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <AlertCircle className="w-8 h-8 text-red-600" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Truy cập bị từ chối</h2>
          <p className="text-gray-600 mb-6">
            Chỉ Quản trị viên mới có quyền truy cập trang này.
          </p>
          <button
            onClick={() => navigate("/admin")}
            className="w-full bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
          >
            Quay lại Dashboard
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">{message && (
        <div className="bg-blue-100 border-blue-400 text-blue-700 border px-4 py-3 rounded-lg">
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Nhật ký hệ thống</h1>
          <p className="text-gray-600 mt-1">Theo dõi hoạt động</p>
        </div>
        <div className="flex gap-2">
          <button 
            onClick={handleReset}
            className="bg-gray-200 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-300 transition-colors flex items-center space-x-2"
          >
            <RefreshCw className="w-5 h-5" />
            <span>Làm mới</span>
          </button>
          <button 
            onClick={handleExport}
            className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2"
          >
            <Download className="w-5 h-5" />
            <span>Xuất CSV</span>
          </button>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <p className="text-blue-700 text-xs font-medium mb-1">Tổng số log</p>
          <p className="text-2xl font-bold text-blue-900">{filteredLogs.length}</p>
        </div>
        <div className="bg-purple-50 border border-purple-200 rounded-lg p-4">
          <p className="text-purple-700 text-xs font-medium mb-1">Người dùng</p>
          <p className="text-2xl font-bold text-purple-900">
            {new Set(filteredLogs.map(l => l.taiKhoanId).filter(Boolean)).size}
          </p>
        </div>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4 mb-4">
          <div className="relative lg:col-span-2">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Tìm kiếm chi tiết hoặc hành động..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>

          <select
            value={filterAction}
            onChange={(e) => setFilterAction(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
            <option value="">Tất cả hành động</option>
            {uniqueActions.map(action => (
              <option key={action} value={action}>{action}</option>
            ))}
          </select>

          <div className="relative">
            <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="date"
              value={fromDate}
              onChange={(e) => setFromDate(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              placeholder="Từ ngày"
            />
          </div>

          <div className="relative">
            <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
              placeholder="Đến ngày"
            />
          </div>
        </div>

        <div className="flex gap-2">
          <button
            onClick={handleSearch}
            className="bg-gray-900 text-white px-6 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
          >
            <Search className="w-5 h-5" />
            <span>Tìm kiếm</span>
          </button>
        </div>
      </div>

      {/* Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : filteredLogs.length === 0 ? (
          <div className="p-8 text-center">
            <FileText className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Không có log nào</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Thời gian
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Tài khoản
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Hành động
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Chi tiết
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    IP Address
                  </th>
                  <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Dữ liệu
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {getPaginatedData().map((log) => (
                  <tr key={log.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center text-sm text-gray-900">
                        <Clock className="w-4 h-4 mr-2 text-gray-400" />
                        {log.thoiGian ? formatDateTime(log.thoiGian) : "—"}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <User className="w-4 h-4 mr-2 text-gray-400" />
                        <span className="text-sm font-medium text-gray-900">
                          {log.taiKhoan?.tenDangNhap || "System"}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getActionColor(log.hanhDong)}`}>
                        {log.hanhDong || "Unknown"}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-sm text-gray-900 max-w-md" title={log.chiTiet}>
                        {log.chiTiet || "—"}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {log.ip || "—"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-center">
                      {(log.duLieuCu || log.duLieuMoi) && (
                        <details className="inline-block text-left">
                          <summary className="cursor-pointer text-blue-600 hover:text-blue-800 text-sm font-medium">
                            Xem chi tiết
                          </summary>
                          <div className="absolute z-10 mt-2 bg-white border border-gray-300 rounded-lg shadow-lg p-4 max-w-2xl">
                            {log.duLieuCu && (
                              <div className="mb-3">
                                <p className="text-xs font-semibold text-red-600 mb-1">Dữ liệu cũ:</p>
                                <pre className="text-xs bg-red-50 p-2 rounded overflow-x-auto max-h-40">
                                  {(() => {
                                    try {
                                      return JSON.stringify(JSON.parse(log.duLieuCu), null, 2);
                                    } catch {
                                      return log.duLieuCu;
                                    }
                                  })()}
                                </pre>
                              </div>
                            )}
                            {log.duLieuMoi && (
                              <div>
                                <p className="text-xs font-semibold text-green-600 mb-1">Dữ liệu mới:</p>
                                <pre className="text-xs bg-green-50 p-2 rounded overflow-x-auto max-h-40">
                                  {(() => {
                                    try {
                                      return JSON.stringify(JSON.parse(log.duLieuMoi), null, 2);
                                    } catch {
                                      return log.duLieuMoi;
                                    }
                                  })()}
                                </pre>
                              </div>
                            )}
                          </div>
                        </details>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Pagination */}
        {!loading && filteredLogs.length > pageSize && (
          <div className="mt-6">
            <Pagination
              currentPage={currentPage}
              totalCount={filteredLogs.length}
              pageSize={pageSize}
              onPageChange={handlePageChange}
            />
          </div>
        )}
      </div>
    </div>
  );
}
