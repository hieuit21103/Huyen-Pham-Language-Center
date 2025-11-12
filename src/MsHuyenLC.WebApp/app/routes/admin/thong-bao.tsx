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

export default function ThongBaoAdmin() {
  const [thongBaos, setThongBaos] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [message, setMessage] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  
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
      const result = await getThongBaos(1, 1000, "ngayTao", "desc");
      if (result.success && result.data) {
        setThongBaos(result.data);
      }
    } catch (error) {
      console.error("Error loading notifications:", error);
    } finally {
      setLoading(false);
    }
  };

  const loadClasses = async () => {
    try {
      const result = await getLopHocs({ 
        pageNumber: 1, 
        pageSize: 1000,
        sortBy: "tenLop",
        sortOrder: "asc"
      });
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
        const danhSach = result.data.danhSachHocVien || [];
        setStudents(Array.isArray(danhSach) ? danhSach : []);
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
      setMessage("Đang gửi thông báo...");

      if (sendType === 'all') {
        const requestData: any = {
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
          recipientIds = students.map(s => s.id);
        } else if (sendType === 'individual') {
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
      setSelectedStudentIds(students.map(s => s.id));
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
                getPaginatedData().map((tb, index) => (
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
        
        {/* Pagination */}
        {!loading && filteredThongBaos.length > pageSize && (
          <div className="mt-6">
            <Pagination
              currentPage={currentPage}
              totalCount={filteredThongBaos.length}
              pageSize={pageSize}
              onPageChange={handlePageChange}
            />
          </div>
        )}
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
              {/* Chọn loại gửi */}
              {!editingId && (
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-3">
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
                      className={`p-4 rounded-lg border-2 transition-all ${
                        sendType === 'all'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-300 hover:border-gray-400'
                      }`}
                    >
                      <Users className={`w-6 h-6 mx-auto mb-2 ${sendType === 'all' ? 'text-blue-600' : 'text-gray-600'}`} />
                      <p className={`text-sm font-medium ${sendType === 'all' ? 'text-blue-900' : 'text-gray-700'}`}>
                        Tất cả
                      </p>
                    </button>
                    
                    <button
                      type="button"
                      onClick={() => {
                        setSendType('class');
                        setSelectedStudentIds([]);
                      }}
                      className={`p-4 rounded-lg border-2 transition-all ${
                        sendType === 'class'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-300 hover:border-gray-400'
                      }`}
                    >
                      <Users className={`w-6 h-6 mx-auto mb-2 ${sendType === 'class' ? 'text-blue-600' : 'text-gray-600'}`} />
                      <p className={`text-sm font-medium ${sendType === 'class' ? 'text-blue-900' : 'text-gray-700'}`}>
                        Theo lớp
                      </p>
                    </button>
                    
                    <button
                      type="button"
                      onClick={() => setSendType('individual')}
                      className={`p-4 rounded-lg border-2 transition-all ${
                        sendType === 'individual'
                          ? 'border-blue-600 bg-blue-50'
                          : 'border-gray-300 hover:border-gray-400'
                      }`}
                    >
                      <User className={`w-6 h-6 mx-auto mb-2 ${sendType === 'individual' ? 'text-blue-600' : 'text-gray-600'}`} />
                      <p className={`text-sm font-medium ${sendType === 'individual' ? 'text-blue-900' : 'text-gray-700'}`}>
                        Chọn học viên
                      </p>
                    </button>
                  </div>
                </div>
              )}

              {/* Chọn lớp */}
              {!editingId && (sendType === 'class' || sendType === 'individual') && (
                <div>
                  <label className="block text-sm font-semibold text-gray-700 mb-2">
                    Chọn lớp học <span className="text-red-500">*</span>
                  </label>
                  <select
                    value={selectedClassId}
                    onChange={(e) => setSelectedClassId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
                    <label className="block text-sm font-semibold text-gray-700">
                      Chọn học viên <span className="text-red-500">*</span>
                    </label>
                    {students.length > 0 && (
                      <button
                        type="button"
                        onClick={toggleAllStudents}
                        className="text-sm text-blue-600 hover:text-blue-800 font-medium"
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
                              checked={selectedStudentIds.includes(student.id)}
                              onChange={() => toggleStudentSelection(student.id)}
                              className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                            />
                            <div className="ml-3 flex-1">
                              <p className="text-sm font-medium text-gray-900">{student.hoTen}</p>
                              <p className="text-xs text-gray-500">{student.email}</p>
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
