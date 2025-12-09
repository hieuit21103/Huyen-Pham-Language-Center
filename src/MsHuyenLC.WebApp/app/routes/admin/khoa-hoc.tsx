import { Search, Plus, Edit, Trash2, BookOpen, X } from "lucide-react";
import { useState, useEffect } from "react";
import { getKhoaHocs, createKhoaHoc, updateKhoaHoc, deleteKhoaHoc } from "~/apis/KhoaHoc";
import type { KhoaHoc, TrangThaiKhoaHoc } from "~/types/index";
import { formatDateForInput, formatDateForDisplay } from "~/utils/date-utils";
import { setLightTheme } from "./_layout";

export default function AdminCourses() {
  const [searchTerm, setSearchTerm] = useState("");
  const [courses, setCourses] = useState<KhoaHoc[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [editingCourse, setEditingCourse] = useState<KhoaHoc | null>(null);
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState<"success" | "error">("success");
  
  // Form state
  const [formData, setFormData] = useState({
    tenKhoaHoc: "",
    moTa: "",
    hocPhi: 0,
    thoiLuong: 0,
    ngayKhaiGiang: "",
  });

  useEffect(() => {
    setLightTheme();
    loadCourses();
  }, []);

  const loadCourses = async () => {
    setLoading(true);
    const response = await getKhoaHocs();
    if (response.success && response.data) {
      setCourses(response.data);
    }
    setLoading(false);
  };

  const handleCreate = () => {
    setEditingCourse(null);
    setMessage("");
    setMessageType("success");
    setFormData({
      tenKhoaHoc: "",
      moTa: "",
      hocPhi: 0,
      thoiLuong: 0,
      ngayKhaiGiang: "",
    });
    setShowModal(true);
  };

  const handleEdit = (course: KhoaHoc) => {
    setEditingCourse(course);
    setMessage("");
    setMessageType("success");
    setFormData({
      tenKhoaHoc: course.tenKhoaHoc || "",
      moTa: course.moTa || "",
      hocPhi: course.hocPhi || 0,
      thoiLuong: course.thoiLuong || 0,
      ngayKhaiGiang: formatDateForInput(course.ngayKhaiGiang),
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (editingCourse) {
      const response = await updateKhoaHoc(editingCourse.id!, formData);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadCourses();
        setShowModal(false);
      }
    } else {
      const response = await createKhoaHoc(formData);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadCourses();
        setShowModal(false);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa khóa học này?")) {
      const response = await deleteKhoaHoc(id);
      setMessage(response.message || "");
      setMessageType(response.success ? "success" : "error");
      if (response.success) {
        loadCourses();
      }
    }
  };


  const filteredCourses = courses.filter(course =>
    course.tenKhoaHoc?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    course.moTa?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      {/* Success Message - Above Table */}
      {message && messageType === "success" && (
        <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg flex items-center justify-between">
          <span>{message}</span>
          <button onClick={() => setMessage("")} className="text-green-700 hover:text-green-900">
            <X className="w-4 h-4" />
          </button>
        </div>
      )}

      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý khóa học</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả khóa học</p>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm khóa học</span>
        </button>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Tìm kiếm khóa học..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>
        </div>
      </div>

      {/* Loading */}
      {loading && (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
        </div>
      )}

      {/* Courses Grid */}
      {!loading && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredCourses.map((course) => (
            <div key={course.id} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              <div className="p-6">
                <div className="flex items-start justify-between mb-4">
                  <div className="w-12 h-12 bg-gray-900 rounded-lg flex items-center justify-center">
                    <BookOpen className="w-6 h-6 text-white" />
                  </div>
                </div>
                
                <h3 className="text-xl font-bold text-gray-900 mb-2">{course.tenKhoaHoc}</h3>
                <p className="text-sm text-gray-600 mb-4 line-clamp-2 min-h-[40px]">{course.moTa}</p>
                
                <div className="space-y-2 mb-4">
                  <div className="flex flex-col gap-1 min-h-[90px] justify-center">
                    <div className="flex items-center">
                      <span className="font-semibold text-gray-900 w-28">Thời lượng:</span>
                      <span>{course.thoiLuong} buổi</span>
                    </div>
                    <div className="flex items-center">
                      <span className="font-semibold text-gray-900 w-28">Học phí:</span>
                      <span>{course.hocPhi?.toLocaleString('vi-VN')}đ</span>
                    </div>
                    <div className="flex items-center">
                      <span className="font-semibold text-gray-900 w-28">Khai giảng:</span>
                      <span>{formatDateForDisplay(course.ngayKhaiGiang)}</span>
                    </div>
                  </div>
                </div>
                
                <div className="flex space-x-2 pt-4 border-t border-gray-200">
                  <button 
                    onClick={() => handleEdit(course)}
                    className="flex-1 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center space-x-2"
                  >
                    <Edit className="w-4 h-4" />
                    <span>Sửa</span>
                  </button>
                  <button 
                    onClick={() => handleDelete(course.id!)}
                    className="flex-1 bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors flex items-center justify-center space-x-2"
                  >
                    <Trash2 className="w-4 h-4" />
                    <span>Xóa</span>
                  </button>
                </div>
              </div>
            </div>
          ))}
          </div>
        </>
      )}

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingCourse ? "Chỉnh sửa khóa học" : "Thêm khóa học mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              {/* Error Message - Inside Modal */}
              {message && messageType === "error" && (
                <div className="mb-4 bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded-lg flex items-start">
                  <svg className="w-5 h-5 mr-2 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                  </svg>
                  <span className="flex-1">{message}</span>
                  <button onClick={() => setMessage("")} className="text-red-700 hover:text-red-900 ml-2">
                    <X className="w-4 h-4" />
                  </button>
                </div>
              )}

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tên khóa học</label>
                  <input
                    type="text"
                    value={formData.tenKhoaHoc}
                    onChange={(e) => setFormData({ ...formData, tenKhoaHoc: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Mô tả</label>
                  <textarea
                    value={formData.moTa}
                    onChange={(e) => setFormData({ ...formData, moTa: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    rows={4}
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Học phí (VNĐ)</label>
                    <input
                      type="number"
                      value={formData.hocPhi}
                      onChange={(e) => setFormData({ ...formData, hocPhi: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">Thời lượng (buổi)</label>
                    <input
                      type="number"
                      value={formData.thoiLuong}
                      onChange={(e) => setFormData({ ...formData, thoiLuong: Number(e.target.value) })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      required
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Ngày khai giảng</label>
                  <input
                    type="date"
                    value={formData.ngayKhaiGiang}
                    onChange={(e) => setFormData({ ...formData, ngayKhaiGiang: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  />
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingCourse ? "Cập nhật" : "Thêm mới"}
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
