import { useState, useEffect } from "react";
import { Bell, Plus, Edit, Trash2, Send, X, Search, Users, User } from "lucide-react";
import {
  getThongBaos,
  createThongBao,
  updateThongBao,
  deleteThongBao,
} from "~/apis/ThongBao";
import { getLopHocs, getLopHocStudents } from "~/apis/LopHoc";
import { formatDateTime } from "~/utils/date-utils";
import { setLightTheme } from "./_layout";
import Pagination from "~/components/Pagination";
import { getProfile } from "~/apis/Profile";
import { getTaiKhoans } from "~/apis";
import type { TaiKhoan } from "~/types";

export default function ThongBaoAdmin() {
  const [thongBaos, setThongBaos] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [message, setMessage] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [taiKhoans, setTaiKhoans] = useState<TaiKhoan[]>([]);

  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  const [classes, setClasses] = useState<any[]>([]);
  const [students, setStudents] = useState<any[]>([]);
  const [loadingStudents, setLoadingStudents] = useState(false);
  const [sendType, setSendType] = useState<'all' | 'class' | 'individual'>('all');
  const [selectedClassId, setSelectedClassId] = useState<string>("");
  const [selectedStudentIds, setSelectedStudentIds] = useState<string[]>([]);

  const [formData, setFormData] = useState({
    tieuDe: "",
    noiDung: "",
  });

  useEffect(() => {
    setLightTheme();
    loadThongBaos();
    loadClasses();
  }, []);

  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm]);

  useEffect(() => {
    if (selectedClassId) {
      loadStudents(selectedClassId);
    } else {
      setStudents([]);
      setSelectedStudentIds([]);
    }
  }, [selectedClassId]);

  const loadThongBaos = async () => {
    setLoading(true);
    try {
      const result = await getThongBaos();
      if (result.success && result.data) {
        setThongBaos(result.data);
      }
      const taiKhoanRes = await getTaiKhoans();
      if (taiKhoanRes.success && taiKhoanRes.data) {
        setTaiKhoans(taiKhoanRes.data);
      }
    } catch (error) {
      console.error("Error loading notifications:", error);
    } finally {
      setLoading(false);
    }
  };

  const loadClasses = async () => {
    try {
      const result = await getLopHocs();
      if (result.success && result.data) {
        setClasses(result.data);
      }
    } catch (error) {
      console.error("Error loading classes:", error);
    }
  };

  const loadStudents = async (lopHocId: string) => {
    setLoadingStudents(true);
    try {
      const result = await getLopHocStudents(lopHocId);
      if (result.success && result.data) {
        const danhSach = Array.isArray(result.data) ? result.data : [];
        setStudents(danhSach);
      } else {
        setStudents([]);
      }
    } catch (error) {
      console.error("Error loading students:", error);
      setStudents([]);
    } finally {
      setLoadingStudents(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage("");

    if (!formData.tieuDe.trim() || !formData.noiDung.trim()) {
      setMessage("Vui lòng nhập đầy đủ thông tin");
      return;
    }

    if (sendType === 'class' && !selectedClassId) {
      setMessage("Vui lòng chọn lớp học");
      return;
    }

    if (sendType === 'individual' && selectedStudentIds.length === 0) {
      setMessage("Vui lòng chọn ít nhất một học viên");
      return;
    }

    try {
      const profileRes = await getProfile();
      if (!profileRes.success || !profileRes.data) {
        setMessage("Không thể lấy thông tin người gửi");
        return;
      }
      const nguoiGuiId = profileRes.data.id;
      setMessage("Đang gửi thông báo...");

      if (sendType === 'all') {
        const requestData: any = {
          nguoiGuiId,
          tieuDe: formData.tieuDe,
          noiDung: formData.noiDung,
        };

        const result = await createThongBao(requestData);
        if (result.success) {
          setMessage("Gửi thông báo thành công đến tất cả người dùng");
          loadThongBaos();
          setTimeout(() => {
            setShowModal(false);
            resetForm();
          }, 2000);
        } else {
          setMessage(result.message || "Gửi thông báo thất bại");
        }
      } else {
        let recipientIds: string[] = [];

        if (sendType === 'class') {
          // Lấy taiKhoanId từ danh sách học viên trong lớp
          recipientIds = students
            .map(s => s.taiKhoanId)
            .filter(id => id); // Lọc bỏ undefined/null
        } else if (sendType === 'individual') {
          // selectedStudentIds đã là taiKhoanId
          recipientIds = selectedStudentIds;
        }

        if (recipientIds.length === 0) {
          setMessage("Không có người nhận nào được chọn");
          return;
        }

        let successCount = 0;
        let failCount = 0;

        for (const recipientId of recipientIds) {
          const requestData = {
            nguoiGuiId,
            nguoiNhanId: recipientId,
            tieuDe: formData.tieuDe,
            noiDung: formData.noiDung,
          };

          const result = await createThongBao(requestData);
          if (result.success) {
            successCount++;
          } else {
            failCount++;
          }
        }

        if (successCount > 0) {
          setMessage(`Gửi thông báo thành công đến ${successCount} người${failCount > 0 ? ` (${failCount} thất bại)` : ''}`);
          loadThongBaos();
          setTimeout(() => {
            setShowModal(false);
            resetForm();
          }, 2000);
        } else {
          setMessage("Gửi thông báo thất bại");
        }
      }
    } catch (error) {
      console.error("Error sending notification:", error);
      setMessage("Có lỗi xảy ra khi gửi thông báo");
    }
  };

  const handleEdit = (thongBao: any) => {
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
    setSendType('all');
    setSelectedClassId("");
    setSelectedStudentIds([]);
    setStudents([]);
  };

  const toggleStudentSelection = (studentId: string) => {
    setSelectedStudentIds(prev => {
      if (prev.includes(studentId)) {
        return prev.filter(id => id !== studentId);
      } else {
        return [...prev, studentId];
      }
    });
  };

  const toggleAllStudents = () => {
    if (selectedStudentIds.length === students.length) {
      setSelectedStudentIds([]);
    } else {
      setSelectedStudentIds(students.map(s => s.taiKhoanId).filter(id => id));
    }
  };

  const filteredThongBaos = thongBaos.filter(tb =>
    tb.tieuDe?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    tb.noiDung?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getPaginatedData = () => {
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    return filteredThongBaos.slice(startIndex, endIndex);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  return (
    <div className="space-y-6">
      {/* Message */}
      {message && (
        <div className={`px-4 py-3 rounded-lg ${message.includes("thành công")
            ? "bg-green-100 border border-green-400 text-green-700"
            : "bg-red-100 border border-red-400 text-red-700"
          }`}>
          {message}
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý thông báo</h1>
          <p className="text-gray-600 mt-1">Gửi và quản lý thông báo đến học viên</p>
        </div>
        <button
          onClick={() => {
            resetForm();
            setShowModal(true);
          }}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Gửi thông báo mới</span>
        </button>
      </div>

      {/* Search */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm theo tiêu đề hoặc nội dung..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          />
        </div>
      </div>

      {/* Loading state */}
      {loading && (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
        </div>
      )}

      {/* Table */}
      {!loading && (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Tiêu đề
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Nội dung
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Người gửi
                  </th>
                  <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Gửi từ
                  </th>
                  <th className="px-6 py-4 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {filteredThongBaos.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="px-6 py-12 text-center text-gray-500">
                      <Bell className="w-12 h-12 mx-auto mb-3 text-gray-400" />
                      <p>Không tìm thấy thông báo nào</p>
                    </td>
                  </tr>
                ) : (
                  getPaginatedData().map((tb, index) => (
                    <tr key={index} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4">
                        <p className="font-semibold text-gray-900">{tb.tieuDe}</p>
                      </td>
                      <td className="px-6 py-4">
                        <p className="text-sm text-gray-600 line-clamp-2">{tb.noiDung}</p>
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-600">
                        {taiKhoans.find(tk => tk.id === tb.nguoiGuiId)?.tenDangNhap || "—"}
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-600">
                        {tb.ngayTao ? formatDateTime(tb.ngayTao) : "—"}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex justify-end space-x-2">
                          <button
                            onClick={() => handleDelete(tb.id!)}
                            className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                            title="Xóa thông báo"
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
          {filteredThongBaos.length > pageSize && (
            <div className="px-6 py-4 border-t border-gray-200">
              <Pagination
                currentPage={currentPage}
                totalCount={filteredThongBaos.length}
                pageSize={pageSize}
                onPageChange={handlePageChange}
              />
            </div>
          )}
        </div>
      )}

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingId ? "Cập nhật thông báo" : "Gửi thông báo mới"}
                </h2>
                <button
                  onClick={() => {
                    setShowModal(false);
                    resetForm();
                  }}
                  className="text-gray-500 hover:text-gray-700"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                {/* Chọn loại gửi */}
                {!editingId && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-3">
                      Gửi đến <span className="text-red-500">*</span>
                    </label>
                    <div className="grid grid-cols-3 gap-3">
                      <button
                        type="button"
                        onClick={() => {
                          setSendType('all');
                          setSelectedClassId("");
                          setSelectedStudentIds([]);
                        }}
                        className={`p-4 rounded-lg border-2 transition-all ${sendType === 'all'
                            ? 'border-gray-900 bg-gray-50'
                            : 'border-gray-300 hover:border-gray-400'
                          }`}
                      >
                        <Users className={`w-6 h-6 mx-auto mb-2 ${sendType === 'all' ? 'text-gray-900' : 'text-gray-600'}`} />
                        <p className={`text-sm font-medium ${sendType === 'all' ? 'text-gray-900' : 'text-gray-700'}`}>
                          Tất cả
                        </p>
                      </button>

                      <button
                        type="button"
                        onClick={() => {
                          setSendType('class');
                          setSelectedStudentIds([]);
                        }}
                        className={`p-4 rounded-lg border-2 transition-all ${sendType === 'class'
                            ? 'border-gray-900 bg-gray-50'
                            : 'border-gray-300 hover:border-gray-400'
                          }`}
                      >
                        <Users className={`w-6 h-6 mx-auto mb-2 ${sendType === 'class' ? 'text-gray-900' : 'text-gray-600'}`} />
                        <p className={`text-sm font-medium ${sendType === 'class' ? 'text-gray-900' : 'text-gray-700'}`}>
                          Theo lớp
                        </p>
                      </button>

                      <button
                        type="button"
                        onClick={() => setSendType('individual')}
                        className={`p-4 rounded-lg border-2 transition-all ${sendType === 'individual'
                            ? 'border-gray-900 bg-gray-50'
                            : 'border-gray-300 hover:border-gray-400'
                          }`}
                      >
                        <User className={`w-6 h-6 mx-auto mb-2 ${sendType === 'individual' ? 'text-gray-900' : 'text-gray-600'}`} />
                        <p className={`text-sm font-medium ${sendType === 'individual' ? 'text-gray-900' : 'text-gray-700'}`}>
                          Chọn học viên
                        </p>
                      </button>
                    </div>
                  </div>
                )}

                {/* Chọn lớp */}
                {!editingId && (sendType === 'class' || sendType === 'individual') && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Chọn lớp học <span className="text-red-500">*</span>
                    </label>
                    <select
                      value={selectedClassId}
                      onChange={(e) => setSelectedClassId(e.target.value)}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    >
                      <option value="">-- Chọn lớp học --</option>
                      {classes.map((cls) => (
                        <option key={cls.id} value={cls.id}>
                          {cls.tenLop} ({cls.siSoHienTai || 0} học viên)
                        </option>
                      ))}
                    </select>
                  </div>
                )}

                {/* Chọn học viên */}
                {!editingId && sendType === 'individual' && selectedClassId && (
                  <div>
                    <div className="flex items-center justify-between mb-2">
                      <label className="block text-sm font-medium text-gray-700">
                        Chọn học viên <span className="text-red-500">*</span>
                      </label>
                      {students.length > 0 && (
                        <button
                          type="button"
                          onClick={toggleAllStudents}
                          className="text-sm text-gray-900 hover:text-gray-700 font-medium"
                        >
                          {selectedStudentIds.length === students.length ? 'Bỏ chọn tất cả' : 'Chọn tất cả'}
                        </button>
                      )}
                    </div>

                    <div className="border border-gray-300 rounded-lg max-h-64 overflow-y-auto">
                      {loadingStudents ? (
                        <div className="p-8 text-center">
                          <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
                          <p className="text-sm text-gray-600 mt-2">Đang tải danh sách học viên...</p>
                        </div>
                      ) : students.length === 0 ? (
                        <div className="p-8 text-center text-gray-500">
                          Không có học viên nào trong lớp
                        </div>
                      ) : (
                        <div className="divide-y divide-gray-200">
                          {students.map((student) => (
                            <label
                              key={student.id}
                              className="flex items-center p-3 hover:bg-gray-50 cursor-pointer"
                            >
                              <input
                                type="checkbox"
                                checked={selectedStudentIds.includes(student.taiKhoanId || "")}
                                onChange={() => toggleStudentSelection(student.taiKhoanId || "")}
                                className="w-4 h-4 text-gray-900 border-gray-300 rounded focus:ring-gray-900"
                              />
                              <div className="ml-3 flex-1">
                                <p className="text-sm font-medium text-gray-900">{student.hoTen}</p>
                                <p className="text-xs text-gray-500">{student.taiKhoan?.email || "—"}</p>
                              </div>
                            </label>
                          ))}
                        </div>
                      )}
                    </div>
                    {selectedStudentIds.length > 0 && (
                      <p className="text-xs text-gray-600 mt-2">
                        Đã chọn {selectedStudentIds.length} / {students.length} học viên
                      </p>
                    )}
                  </div>
                )}

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Tiêu đề <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="text"
                    value={formData.tieuDe}
                    onChange={(e) => setFormData({ ...formData, tieuDe: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="Nhập tiêu đề thông báo"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Nội dung <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    value={formData.noiDung}
                    onChange={(e) => setFormData({ ...formData, noiDung: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900 resize-none"
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
                      <strong>Lưu ý:</strong> {
                        sendType === 'all'
                          ? 'Thông báo sẽ được gửi đến tất cả học viên trong hệ thống.'
                          : sendType === 'class'
                            ? `Thông báo sẽ được gửi đến tất cả học viên trong lớp đã chọn.`
                            : `Thông báo sẽ được gửi đến ${selectedStudentIds.length} học viên đã chọn.`
                      }
                    </p>
                  </div>
                )}

                {message && (
                  <div className={`px-4 py-3 rounded-lg ${message.includes("thành công")
                      ? "bg-green-100 border border-green-400 text-green-700"
                      : "bg-red-100 border border-red-400 text-red-700"
                    }`}>
                    {message}
                  </div>
                )}

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingId ? "Cập nhật" : "Gửi thông báo"}
                  </button>
                  <button
                    type="button"
                    onClick={() => {
                      setShowModal(false);
                      resetForm();
                    }}
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
