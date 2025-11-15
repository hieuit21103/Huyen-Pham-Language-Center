import { Search, Plus, Edit, Trash2, Briefcase, X, User, Mail, Phone, MapPin, Calendar, Users as UsersIcon } from "lucide-react";
import { useState, useEffect } from "react";
import { 
  getGiaoVus, 
  createGiaoVu, 
  updateGiaoVu, 
  deleteGiaoVu,
  getGiaoVu
} from "~/apis/GiaoVu";
import { getTaiKhoans } from "~/apis/TaiKhoan";
import Pagination from "~/components/Pagination";
import type { GiaoVu, GiaoVuRequest } from "~/types/index";
import { setLightTheme } from "./_layout";

export default function AdminGiaoVu() {
  const [searchTerm, setSearchTerm] = useState("");
  const [giaoVus, setGiaoVus] = useState<GiaoVu[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingGiaoVu, setEditingGiaoVu] = useState<GiaoVu | null>(null);
  const [message, setMessage] = useState("");
  const [taiKhoans, setTaiKhoans] = useState<any[]>([]);
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(10);
  
  const [formData, setFormData] = useState<GiaoVuRequest>({
    hoTen: "",
    boPhan: "",
    taiKhoanId: "",
  });

  useEffect(() => {
    setLightTheme();
  }, []);

  useEffect(() => {
    loadGiaoVus();
    loadTaiKhoans();
  }, [currentPage, searchTerm]);

  const loadGiaoVus = async () => {
    setLoading(true);
    const response = await getGiaoVus({
      pageNumber: currentPage,
      pageSize: pageSize,
      sortBy: 'hoTen',
      sortOrder: 'asc'
    });
    
    if (response.success && Array.isArray(response.data)) {
      setGiaoVus(response.data);
      setTotalCount((response as any).totalCount || response.data.length);
    }
    setLoading(false);
  };

  const loadTaiKhoans = async () => {
    const response = await getTaiKhoans({ pageSize: 1000 });
    if (response.success && Array.isArray(response.data)) {
      // Filter only accounts without existing GiaoVu
      setTaiKhoans(response.data);
    }
  };

  const handleCreate = () => {
    setEditingGiaoVu(null);
    setFormData({
      hoTen: "",
      boPhan: "",
      taiKhoanId: "",
    });
    setShowModal(true);
  };

  const handleEdit = async (giaoVu: GiaoVu) => {
    setEditingGiaoVu(giaoVu);
    setFormData({
      hoTen: giaoVu.hoTen || "",
      boPhan: giaoVu.boPhan || "",
      taiKhoanId: giaoVu.taiKhoanId || "",
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (editingGiaoVu) {
      const response = await updateGiaoVu(editingGiaoVu.id, formData);
      setMessage(response.message || "");
      if (response.success) {
        loadGiaoVus();
        setShowModal(false);
        setTimeout(() => setMessage(""), 3000);
      }
    } else {
      const response = await createGiaoVu(formData);
      setMessage(response.message || "");
      if (response.success) {
        loadGiaoVus();
        setShowModal(false);
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa giáo vụ này?")) {
      const response = await deleteGiaoVu(id);
      setMessage(response.message || "");
      if (response.success) {
        loadGiaoVus();
        setTimeout(() => setMessage(""), 3000);
      }
    }
  };

  const filteredGiaoVus = giaoVus.filter(gv =>
    gv.hoTen?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    gv.boPhan?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {message && (
        <div className={`${message.includes("thành công") ? "bg-green-100 border-green-400 text-green-700" : "bg-red-100 border-red-400 text-red-700"} border px-4 py-3 rounded-lg`}>
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý giáo vụ</h1>
          <p className="text-gray-600 mt-1">Quản lý thông tin nhân viên giáo vụ</p>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm giáo vụ</span>
        </button>
      </div>
      
      {/* Search */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm theo tên, bộ phận..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          />
        </div>
      </div>

      {/* Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : filteredGiaoVus.length === 0 ? (
          <div className="p-8 text-center">
            <Briefcase className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Không tìm thấy giáo vụ nào</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Họ tên
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Bộ phận
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Tài khoản
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredGiaoVus.map((giaoVu) => (
                  <tr key={giaoVu.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                          <User className="w-5 h-5 text-blue-600" />
                        </div>
                        <div>
                          <p className="text-sm font-medium text-gray-900">{giaoVu.hoTen}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center text-sm text-gray-600">
                        <Briefcase className="w-4 h-4 mr-2 text-gray-400" />
                        {giaoVu.boPhan || "—"}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-600">
                        {giaoVu.taiKhoan ? (
                          <div>
                            <p className="font-medium">@{giaoVu.taiKhoan.tenDangNhap}</p>
                            <p className="text-xs text-gray-500">{giaoVu.taiKhoan.email}</p>
                          </div>
                        ) : (
                          "—"
                        )}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end space-x-2">
                        <button
                          onClick={() => handleEdit(giaoVu)}
                          className="text-blue-600 hover:text-blue-900 p-2 hover:bg-blue-50 rounded-lg transition-colors"
                          title="Chỉnh sửa"
                        >
                          <Edit className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDelete(giaoVu.id)}
                          className="text-red-600 hover:text-red-900 p-2 hover:bg-red-50 rounded-lg transition-colors"
                          title="Xóa"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Pagination */}
      {!loading && totalCount > 0 && (
        <div className="flex justify-center mt-6">
          <Pagination
            currentPage={currentPage}
            totalCount={totalCount}
            pageSize={pageSize}
            onPageChange={(page) => setCurrentPage(page)}
          />
        </div>
      )}

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingGiaoVu ? "Chỉnh sửa giáo vụ" : "Thêm giáo vụ mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Họ và tên *</label>
                  <input
                    type="text"
                    value={formData.hoTen}
                    onChange={(e) => setFormData({ ...formData, hoTen: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Bộ phận</label>
                  <input
                    type="text"
                    value={formData.boPhan}
                    onChange={(e) => setFormData({ ...formData, boPhan: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="VD: Quản lý, Hành chính, Kế toán..."
                  />
                </div>

                {!editingGiaoVu && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Tài khoản</label>
                    <select
                      value={formData.taiKhoanId}
                      onChange={(e) => setFormData({ ...formData, taiKhoanId: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="">-- Chọn tài khoản --</option>
                      {taiKhoans.map((tk) => (
                        <option key={tk.id} value={tk.id}>
                          {tk.tenDangNhap} - {tk.email}
                        </option>
                      ))}
                    </select>
                  </div>
                )}

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingGiaoVu ? "Cập nhật" : "Thêm mới"}
                  </button>
                  <button
                    type="button"
                    onClick={() => setShowModal(false)}
                    className="flex-1 bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors"
                  >
                    Hủy
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
