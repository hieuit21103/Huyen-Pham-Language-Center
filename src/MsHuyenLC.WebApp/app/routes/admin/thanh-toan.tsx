import { useEffect, useState } from "react";
import {
  CheckCircle,
  XCircle,
  Clock,
  Search,
  DollarSign,
  Calendar,
  Plus,
  Edit,
  Trash2,
  X,
} from "lucide-react";
import { getThanhToans, createThanhToan, updateThanhToan, deleteThanhToan, type ThanhToan } from "~/apis/ThanhToan";
import { getDangKys } from "~/apis/DangKy";
import type { ThanhToanResponse } from "~/types/finance.types"; 


export default function ThanhToanPage() {
  const [thanhToans, setThanhToans] = useState<ThanhToanResponse[]>([]);
  const [filteredThanhToans, setFilteredThanhToans] = useState<ThanhToanResponse[]>(
    []
  );
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedStatus, setSelectedStatus] = useState<number | null>(null);
  const [selectedPhuongThuc, setSelectedPhuongThuc] = useState<number | null>(
    null
  );
  const [currentPage, setCurrentPage] = useState(1);
  const [messageType, setMessageType] = useState<{
    type: "success" | "error";
    message: string;
  } | null>(null);

  // Modal state
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedThanhToan, setSelectedThanhToan] =
    useState<ThanhToanResponse | null>(null);
  const [modalLoading, setModalLoading] = useState(false);
  const [modalError, setModalError] = useState("");
  const [modalPhuongThuc, setModalPhuongThuc] = useState<number>(1);

  // Create/Edit modal
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [formData, setFormData] = useState({ dangKyId: "", soTien: 0, phuongThuc: 1 });
  const [dangKys, setDangKys] = useState<any[]>([]);

  const itemsPerPage = 10;

  useEffect(() => {
    loadThanhToans();
    loadDangKys();
  }, []);

  useEffect(() => {
    filterThanhToans();
  }, [thanhToans, searchTerm, selectedStatus, selectedPhuongThuc]);

  const loadThanhToans = async () => {
    try {
      setLoading(true);
      const response = await getThanhToans();
      if (response.success && response.data) {
        setThanhToans(response.data);
      } else {
        setThanhToans([]);
      }
    } catch (error) {
      console.error("Error loading thanh toans:", error);
      setThanhToans([]);
    } finally {
      setLoading(false);
    }
  };

  const loadDangKys = async () => {
    const response = await getDangKys();
    if (response.success && response.data) {
      setDangKys(response.data);
    }
  };

  const filterThanhToans = () => {
    let filtered = [...thanhToans];

    if (searchTerm) {
      filtered = filtered.filter(
        (tt) =>
          tt.maThanhToan?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          tt.tenHocVien?.toLowerCase().includes(searchTerm.toLowerCase()) ||
          tt.tenKhoaHoc?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (selectedStatus !== null) {
      filtered = filtered.filter((tt) => tt.trangThai === selectedStatus);
    }

    if (selectedPhuongThuc !== null) {
      filtered = filtered.filter((tt) => tt.phuongThuc === selectedPhuongThuc);
    }

    setFilteredThanhToans(filtered);
    setCurrentPage(1);
  };

  const handleUpdateStatus = async (trangThai: number) => {
    if (!selectedThanhToan || !selectedThanhToan.id) return;

    setModalLoading(true);
    setModalError("");

    try {
      const today = new Date();
      const dateOnly = `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, '0')}-${String(today.getDate()).padStart(2, '0')}`;
      
      const result = await updateThanhToan(selectedThanhToan.id, {
        trangThai,
        phuongThuc: modalPhuongThuc,
        ngayThanhToan: trangThai === 1 ? dateOnly : undefined,
      });

      if (result.success) {
        setMessageType({
          type: "success",
          message: "Cập nhật trạng thái thanh toán thành công!",
        });
        setIsModalOpen(false);
        loadThanhToans();
      } else {
        setModalError(result.message || "Lỗi khi cập nhật thanh toán");
      }
    } catch (error) {
      setModalError("Lỗi khi cập nhật thanh toán");
    } finally {
      setModalLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingId(null);
    setFormData({ dangKyId: "", soTien: 0, phuongThuc: 1 });
    setModalError("");
    setIsFormModalOpen(true);
  };

  const handleEdit = (thanhToan: ThanhToan) => {
    setEditingId(thanhToan.id || null);
    setFormData({
      dangKyId: thanhToan.dangKyId || "",
      soTien: thanhToan.soTien || 0,
      phuongThuc: thanhToan.phuongThuc || 1,
    });
    setModalError("");
    setIsFormModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setModalLoading(true);
    setModalError("");

    try {
      let result;
      if (editingId) {
        result = await updateThanhToan(editingId, formData);
      } else {
        result = await createThanhToan(formData);
      }

      if (result.success) {
        setMessageType({
          type: "success",
          message: editingId ? "Cập nhật thanh toán thành công!" : "Tạo thanh toán thành công!",
        });
        setIsFormModalOpen(false);
        loadThanhToans();
      } else {
        setModalError(result.message || "Lỗi khi lưu thanh toán");
      }
    } catch (error) {
      setModalError("Lỗi khi lưu thanh toán");
    } finally {
      setModalLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Bạn có chắc chắn muốn xóa thanh toán này?")) return;

    try {
      const result = await deleteThanhToan(id);
      if (result.success) {
        setMessageType({
          type: "success",
          message: "Xóa thanh toán thành công!",
        });
        loadThanhToans();
      } else {
        setMessageType({
          type: "error",
          message: result.message || "Lỗi khi xóa thanh toán",
        });
      }
    } catch (error) {
      setMessageType({
        type: "error",
        message: "Lỗi khi xóa thanh toán",
      });
    }
  };

  const getStatusBadge = (trangThai: number) => {
    switch (trangThai) {
      case 1: // Đã thanh toán
        return (
          <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800">
            <CheckCircle className="w-4 h-4" />
            Đã thanh toán
          </span>
        );
      case 2: // Thất bại
        return (
          <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium bg-red-100 text-red-800">
            <XCircle className="w-4 h-4" />
            Thất bại
          </span>
        );
      default: // Chưa thanh toán
        return (
          <span className="inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm font-medium bg-yellow-100 text-yellow-800">
            <Clock className="w-4 h-4" />
            Chưa thanh toán
          </span>
        );
    }
  };

  const getPhuongThucText = (phuongThuc: number) => {
    switch (phuongThuc) {
      case 0:
        return "Trực tuyến";
      case 1:
        return "Trực tiếp";
      case 2:
        return "Khác";
      default:
        return "Không xác định";
    }
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(amount);
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return "-";
    return new Date(dateString).toLocaleDateString("vi-VN");
  };

  // Pagination
  const totalPages = Math.ceil(filteredThanhToans.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentItems = filteredThanhToans.slice(startIndex, endIndex);

  // Statistics
  const tongDoanhThu = thanhToans
    .filter((tt) => tt.trangThai === 1)
    .reduce((sum, tt) => sum + (tt.soTien || 0), 0);
  const soThanhToanDaThanhCong = thanhToans.filter(
    (tt) => tt.trangThai === 1
  ).length;
  const soThanhToanCho = thanhToans.filter((tt) => tt.trangThai === 0).length;

  return (
    <div className="p-6 max-w-[1400px] mx-auto">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-3">
          <div className="p-3 bg-green-100 rounded-lg">
            <DollarSign className="w-8 h-8 text-green-600" />
          </div>
          <div>
            <h1 className="text-3xl font-bold text-gray-900">
              Quản lý thanh toán
            </h1>
            <p className="text-gray-500 mt-1">
              Quản lý và theo dõi các thanh toán từ học viên
            </p>
          </div>
        </div>
        <button
          onClick={handleCreate}
          className="flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-medium"
        >
          <Plus className="w-5 h-5" />
          Tạo thanh toán
        </button>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
        <div className="bg-gradient-to-br from-green-50 to-green-100 p-6 rounded-xl border border-green-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-green-600">
                Tổng doanh thu
              </p>
              <p className="text-2xl font-bold text-green-700 mt-1">
                {formatCurrency(tongDoanhThu)}
              </p>
            </div>
            <DollarSign className="w-12 h-12 text-green-600 opacity-50" />
          </div>
        </div>

        <div className="bg-gradient-to-br from-blue-50 to-blue-100 p-6 rounded-xl border border-blue-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-blue-600">
                Đã thanh toán
              </p>
              <p className="text-2xl font-bold text-blue-700 mt-1">
                {soThanhToanDaThanhCong}
              </p>
            </div>
            <CheckCircle className="w-12 h-12 text-blue-600 opacity-50" />
          </div>
        </div>

        <div className="bg-gradient-to-br from-yellow-50 to-yellow-100 p-6 rounded-xl border border-yellow-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-yellow-600">
                Chờ thanh toán
              </p>
              <p className="text-2xl font-bold text-yellow-700 mt-1">
                {soThanhToanCho}
              </p>
            </div>
            <Clock className="w-12 h-12 text-yellow-600 opacity-50" />
          </div>
        </div>
      </div>

      {messageType && (
        <div
          className={`mb-4 p-4 rounded-lg ${
            messageType.type === "success"
              ? "bg-green-50 text-green-800 border border-green-200"
              : "bg-red-50 text-red-800 border border-red-200"
          }`}
        >
          <div className="flex items-center justify-between">
            <span>{messageType.message}</span>
            <button
              onClick={() => setMessageType(null)}
              className="text-gray-500 hover:text-gray-700"
            >
              ×
            </button>
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="bg-white p-6 rounded-xl border border-gray-200 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              type="text"
              placeholder="Tìm theo mã, học viên, khóa học..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
            />
          </div>

          <select
            value={selectedStatus === null ? "" : selectedStatus}
            onChange={(e) =>
              setSelectedStatus(e.target.value ? Number(e.target.value) : null)
            }
            className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
          >
            <option value="">Tất cả trạng thái</option>
            <option value="0">Chưa thanh toán</option>
            <option value="1">Đã thanh toán</option>
            <option value="2">Thất bại</option>
          </select>

          <select
            value={selectedPhuongThuc === null ? "" : selectedPhuongThuc}
            onChange={(e) =>
              setSelectedPhuongThuc(
                e.target.value ? Number(e.target.value) : null
              )
            }
            className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
          >
            <option value="">Tất cả phương thức</option>
            <option value="0">Trực tuyến</option>
            <option value="1">Trực tiếp</option>
            <option value="2">Khác</option>
          </select>
        </div>
      </div>

      {/* Table */}
      <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
        {loading ? (
          <div className="flex items-center justify-center h-64">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-green-600"></div>
          </div>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Mã thanh toán
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Học viên
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Khóa học
                    </th>
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
                      Ngày thanh toán
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                      Thao tác
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {currentItems.length === 0 ? (
                    <tr>
                      <td
                        colSpan={8}
                        className="px-6 py-12 text-center text-gray-500"
                      >
                        Không tìm thấy thanh toán nào
                      </td>
                    </tr>
                  ) : (
                    currentItems.map((thanhToan) => (
                      <tr
                        key={thanhToan.id}
                        className="hover:bg-gray-50 transition-colors"
                      >
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className="font-medium text-gray-900">
                            {thanhToan.maThanhToan}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className="text-gray-900">
                            {thanhToan.tenHocVien || "-"}
                          </span>
                        </td>
                        <td className="px-6 py-4">
                          <span className="text-gray-900">
                            {thanhToan.tenKhoaHoc || "-"}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className="font-semibold text-gray-900">
                            {formatCurrency(thanhToan.soTien || 0)}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className="text-gray-700">
                            {getPhuongThucText(thanhToan.phuongThuc || 0)}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          {getStatusBadge(thanhToan.trangThai || 0)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-gray-700">
                          {formatDate(thanhToan.ngayThanhToan)}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <div className="flex items-center gap-2">
                            {thanhToan.trangThai === 0 && (
                              <button
                              onClick={() => {
                                setSelectedThanhToan(thanhToan);
                                setModalPhuongThuc(1);
                                setIsModalOpen(true);
                                setModalError("");
                              }}
                                className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                                title="Cập nhật trạng thái"
                              >
                                <Calendar className="w-5 h-5" />
                              </button>
                            )}
                            <button
                              onClick={() => handleEdit(thanhToan)}
                              className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                              title="Chỉnh sửa"
                            >
                              <Edit className="w-5 h-5" />
                            </button>
                            <button
                              onClick={() => handleDelete(thanhToan.id!)}
                              className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                              title="Xóa"
                            >
                              <Trash2 className="w-5 h-5" />
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="bg-gray-50 px-6 py-4 flex items-center justify-between border-t border-gray-200">
                <div className="text-sm text-gray-700">
                  Hiển thị {startIndex + 1} đến{" "}
                  {Math.min(endIndex, filteredThanhToans.length)} trong tổng số{" "}
                  {filteredThanhToans.length} thanh toán
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
                    disabled={currentPage === 1}
                    className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                  >
                    Trước
                  </button>
                  <span className="px-4 py-2 text-sm text-gray-700">
                    Trang {currentPage} / {totalPages}
                  </span>
                  <button
                    onClick={() =>
                      setCurrentPage((p) => Math.min(totalPages, p + 1))
                    }
                    disabled={currentPage === totalPages}
                    className="px-4 py-2 border border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                  >
                    Sau
                  </button>
                </div>
              </div>
            )}
          </>
        )}
      </div>

      {/* Update Status Modal */}
      {isModalOpen && selectedThanhToan && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full p-6">
            <h3 className="text-xl font-bold text-gray-900 mb-4">
              Cập nhật thanh toán
            </h3>

            {modalError && (
              <div className="mb-4 p-3 bg-red-50 text-red-700 rounded-lg text-sm border border-red-200">
                {modalError}
              </div>
            )}

            <div className="space-y-3 mb-6">
              <div>
                <span className="text-sm text-gray-600">Mã thanh toán:</span>
                <p className="font-medium text-gray-900">
                  {selectedThanhToan.maThanhToan}
                </p>
              </div>
              <div>
                <span className="text-sm text-gray-600">Học viên:</span>
                <p className="font-medium text-gray-900">
                  {selectedThanhToan.tenHocVien || "-"}
                </p>
              </div>
              <div>
                <span className="text-sm text-gray-600">Số tiền:</span>
                <p className="font-semibold text-green-600 text-lg">
                  {formatCurrency(selectedThanhToan.soTien || 0)}
                </p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Phương thức thanh toán <span className="text-red-500">*</span>
                </label>
                <select
                  value={modalPhuongThuc}
                  onChange={(e) => setModalPhuongThuc(Number(e.target.value))}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                >
                  <option value="0">Trực tuyến</option>
                  <option value="1">Trực tiếp</option>
                </select>
              </div>
            </div>

            <div className="flex gap-3">
              <button
                onClick={() => handleUpdateStatus(1)}
                disabled={modalLoading}
                className="flex-1 flex items-center justify-center gap-2 px-4 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
              >
                <CheckCircle className="w-5 h-5" />
                Xác nhận đã thanh toán
              </button>
              <button
                onClick={() => {
                  setIsModalOpen(false);
                  setSelectedThanhToan(null);
                  setModalError("");
                }}
                disabled={modalLoading}
                className="px-4 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
              >
                Hủy
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Create/Edit Form Modal */}
      {isFormModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full p-6">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-xl font-bold text-gray-900">
                {editingId ? "Chỉnh sửa thanh toán" : "Tạo thanh toán mới"}
              </h3>
              <button
                onClick={() => setIsFormModalOpen(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            {modalError && (
              <div className="mb-4 p-3 bg-red-50 text-red-700 rounded-lg text-sm border border-red-200">
                {modalError}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Đăng ký <span className="text-red-500">*</span>
                </label>
                <select
                  required
                  value={formData.dangKyId}
                  onChange={(e) =>
                    setFormData({ ...formData, dangKyId: e.target.value })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  disabled={!!editingId}
                >
                  <option value="">-- Chọn đăng ký --</option>
                  {dangKys.map((dk) => (
                    <option key={dk.id} value={dk.id}>
                      {dk.tenHocVien || dk.id} - {dk.tenKhoaHoc || "Khóa học"}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Số tiền <span className="text-red-500">*</span>
                </label>
                <input
                  type="number"
                  required
                  min="0"
                  value={formData.soTien}
                  onChange={(e) =>
                    setFormData({ ...formData, soTien: Number(e.target.value) })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  placeholder="Nhập số tiền"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Phương thức thanh toán <span className="text-red-500">*</span>
                </label>
                <select
                  required
                  value={formData.phuongThuc}
                  onChange={(e) =>
                    setFormData({ ...formData, phuongThuc: Number(e.target.value) })
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                >
                  <option value="0">Trực tuyến</option>
                  <option value="1">Trực tiếp</option>
                  <option value="2">Khác</option>
                </select>
              </div>

              <div className="flex gap-3 pt-4">
                <button
                  type="submit"
                  disabled={modalLoading}
                  className="flex-1 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
                >
                  {editingId ? "Cập nhật" : "Tạo mới"}
                </button>
                <button
                  type="button"
                  onClick={() => setIsFormModalOpen(false)}
                  disabled={modalLoading}
                  className="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
                >
                  Hủy
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
