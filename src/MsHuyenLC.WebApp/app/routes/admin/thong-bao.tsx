import { useState, useEffect } from "react";
import { Bell, Plus, Edit, Trash2, Send, X, Search } from "lucide-react";
import { 
  getThongBaos, 
  createThongBao, 
  updateThongBao, 
  deleteThongBao,
  type ThongBaoNguoiNhanResponse 
} from "~/apis/ThongBao";
import { formatDateTime } from "~/utils/date-utils";
import { setLightTheme } from "./_layout";

export default function ThongBaoAdmin() {
  const [thongBaos, setThongBaos] = useState<ThongBaoNguoiNhanResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [message, setMessage] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [formData, setFormData] = useState({
    tieuDe: "",
    noiDung: "",
  });

  useEffect(() => {
    setLightTheme();
    loadThongBaos();
  }, []);

  const loadThongBaos = async () => {
    setLoading(true);
    try {
      const result = await getThongBaos(1, 100, "ngayTao", "desc");
      if (result.success && result.data) {
        setThongBaos(result.data);
      }
    } catch (error) {
      console.error("Error loading notifications:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");

    if (!formData.tieuDe.trim() || !formData.noiDung.trim()) {
      setMessage("Vui lòng nhập đầy đủ thông tin");
      return;
    }

    try {
      if (editingId) {
        // Update
        const result = await updateThongBao(editingId, formData);
        if (result.success) {
          setMessage("Cập nhật thông báo thành công");
          loadThongBaos();
          setTimeout(() => {
            setShowModal(false);
            resetForm();
          }, 1500);
        } else {
          setMessage(result.message || "Cập nhật thất bại");
        }
      } else {
        // Create
        const result = await createThongBao(formData);
        if (result.success) {
          setMessage("Gửi thông báo thành công");
          loadThongBaos();
          setTimeout(() => {
            setShowModal(false);
            resetForm();
          }, 1500);
        } else {
          setMessage(result.message || "Gửi thông báo thất bại");
        }
      }
    } catch (error) {
      setMessage("Có lỗi xảy ra");
    }
  };

  const handleEdit = (thongBao: ThongBaoNguoiNhanResponse) => {
    // Editing not available since GetAll doesn't return IDs
    // This function is kept for potential future use
    setFormData({
      tieuDe: thongBao.tieuDe || "",
      noiDung: thongBao.noiDung || "",
    });
    setShowModal(true);
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Bạn có chắc chắn muốn xóa thông báo này?")) return;

    try {
      const result = await deleteThongBao(id);
      if (result.success) {
        setMessage("Xóa thông báo thành công");
        loadThongBaos();
        setTimeout(() => setMessage(""), 3000);
      } else {
        setMessage(result.message || "Xóa thông báo thất bại");
      }
    } catch (error) {
      setMessage("Có lỗi xảy ra khi xóa");
    }
  };

  const resetForm = () => {
    setFormData({ tieuDe: "", noiDung: "" });
    setEditingId(null);
    setMessage("");
  };

  const filteredThongBaos = thongBaos.filter(tb =>
    tb.tieuDe?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    tb.noiDung?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900 flex items-center">
          <Bell className="w-8 h-8 mr-3 text-blue-600" />
          Quản lý thông báo
        </h1>
        <p className="text-gray-600 mt-2">
          Gửi và quản lý thông báo đến học viên
        </p>
      </div>

      {/* Message */}
      {message && (
        <div className={`mb-4 p-4 rounded-lg ${
          message.includes("thành công")
            ? "bg-green-50 text-green-800 border border-green-200"
            : "bg-red-50 text-red-800 border border-red-200"
        }`}>
          {message}
        </div>
      )}

      {/* Actions */}
      <div className="mb-6 flex flex-col sm:flex-row gap-4 justify-between">
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
          <input
            type="text"
            placeholder="Tìm kiếm thông báo..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
        <button
          onClick={() => {
            resetForm();
            setShowModal(true);
          }}
          className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Plus className="w-5 h-5 mr-2" />
          Gửi thông báo mới
        </button>
      </div>

      {/* Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  Tiêu đề
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  Nội dung
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  Người gửi
                </th>
                <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                  Gửi từ
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {loading ? (
                <tr>
                  <td colSpan={4} className="px-6 py-12 text-center">
                    <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
                  </td>
                </tr>
              ) : filteredThongBaos.length === 0 ? (
                <tr>
                  <td colSpan={4} className="px-6 py-12 text-center text-gray-500">
                    Không có thông báo nào
                  </td>
                </tr>
              ) : (
                filteredThongBaos.map((tb, index) => (
                  <tr key={index} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <p className="font-semibold text-gray-900">{tb.tieuDe}</p>
                    </td>
                    <td className="px-6 py-4">
                      <p className="text-sm text-gray-600 line-clamp-2">{tb.noiDung}</p>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {tb.tenNguoiGui}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {tb.ngayTao ? formatDateTime(tb.ngayTao) : "—"}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
              <h2 className="text-2xl font-bold text-gray-900 flex items-center">
                <Send className="w-6 h-6 mr-2 text-blue-600" />
                {editingId ? "Cập nhật thông báo" : "Gửi thông báo mới"}
              </h2>
              <button
                onClick={() => {
                  setShowModal(false);
                  resetForm();
                }}
                className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-6">
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Tiêu đề <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  value={formData.tieuDe}
                  onChange={(e) => setFormData({ ...formData, tieuDe: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  placeholder="Nhập tiêu đề thông báo"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Nội dung <span className="text-red-500">*</span>
                </label>
                <textarea
                  value={formData.noiDung}
                  onChange={(e) => setFormData({ ...formData, noiDung: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none"
                  rows={6}
                  placeholder="Nhập nội dung thông báo..."
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {formData.noiDung.length} ký tự
                </p>
              </div>

              {!editingId && (
                <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                  <p className="text-sm text-blue-800">
                    <strong>Lưu ý:</strong> Thông báo sẽ được gửi đến tất cả học viên trong hệ thống.
                  </p>
                </div>
              )}

              {message && (
                <div className={`p-4 rounded-lg ${
                  message.includes("thành công")
                    ? "bg-green-50 text-green-800 border border-green-200"
                    : "bg-red-50 text-red-800 border border-red-200"
                }`}>
                  {message}
                </div>
              )}

              <div className="flex justify-end space-x-3 pt-4 border-t border-gray-200">
                <button
                  type="button"
                  onClick={() => {
                    setShowModal(false);
                    resetForm();
                  }}
                  className="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                >
                  Hủy
                </button>
                <button
                  type="submit"
                  className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center"
                >
                  <Send className="w-4 h-4 mr-2" />
                  {editingId ? "Cập nhật" : "Gửi thông báo"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
