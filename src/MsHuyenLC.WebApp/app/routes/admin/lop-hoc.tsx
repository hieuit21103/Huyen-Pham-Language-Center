import { Search, Plus, Edit, Trash2, Users, X, Eye, UserCheck } from "lucide-react";
import { useState, useEffect } from "react";
import { getLopHocs, createLopHoc, updateLopHoc, deleteLopHoc, getLopHocStudents } from "~/apis/LopHoc";
import { getKhoaHocs } from "~/apis/KhoaHoc";
import { getPhanCongByLopHoc, createPhanCong, deletePhanCong } from "~/apis/PhanCong";
import { getGiaoViens } from "~/apis/GiaoVien";
import type { LopHoc, TrangThaiLopHoc, PhanCong, GiaoVien, PhanCongResponse } from "~/types/index";
import { setLightTheme } from "./_layout";
import Pagination from "~/components/Pagination";

export default function AdminClasses() {
  const [searchTerm, setSearchTerm] = useState("");
  const [classes, setClasses] = useState<LopHoc[]>([]);
  const [courses, setCourses] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [showStudentsModal, setShowStudentsModal] = useState(false);
  const [showTeachersModal, setShowTeachersModal] = useState(false);
  const [editingClass, setEditingClass] = useState<LopHoc | null>(null);
  const [selectedClass, setSelectedClass] = useState<LopHoc | null>(null);
  const [students, setStudents] = useState<any[]>([]);
  const [loadingStudents, setLoadingStudents] = useState(false);
  const [teachers, setTeachers] = useState<GiaoVien[]>([]);
  const [assignedTeachers, setAssignedTeachers] = useState<PhanCongResponse[]>([]);
  const [loadingTeachers, setLoadingTeachers] = useState(false);
  const [selectedTeacherId, setSelectedTeacherId] = useState("");
  const [message, setMessage] = useState("");
  
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;
  
  const [formData, setFormData] = useState({
    tenLop: "",
    khoaHocId: "",
    siSoToiDa: 30,
  });

  useEffect(() => {
    setLightTheme();
    loadClasses();
    loadCourses();
    loadTeachers();
  }, []);

  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm]);

  const loadClasses = async () => {
    setLoading(true);
    const response = await getLopHocs();
    if (response.success && response.data) {
      setClasses(response.data);
    }
    setLoading(false);
  };

  const loadCourses = async () => {
    const response = await getKhoaHocs();
    if (response.success && response.data) {
      setCourses(response.data);
    }
  };

  const loadTeachers = async () => {
    const response = await getGiaoViens();
    if (response.success && response.data) {
      setTeachers(response.data);
    }
  };

  const handleCreate = () => {
    setEditingClass(null);
    setFormData({
      tenLop: "",
      khoaHocId: "",
      siSoToiDa: 30,
    });
    setShowModal(true);
  };

  const handleEdit = (cls: LopHoc) => {
    setEditingClass(cls);
    setFormData({
      tenLop: cls.tenLop || "",
      khoaHocId: cls.khoaHocId || "",
      siSoToiDa: cls.siSoToiDa || 30,
    });
    setShowModal(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (editingClass) {
      const response = await updateLopHoc(editingClass.id!, formData);
      setMessage(response.message || "");
      if (response.success) {
        loadClasses();
        setShowModal(false);
      }
    } else {
      const response = await createLopHoc(formData);
      setMessage(response.message || "");
      if (response.success) {
        loadClasses();
        setShowModal(false);
      }
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm("Bạn có chắc chắn muốn xóa lớp học này?")) {
      const response = await deleteLopHoc(id);
      setMessage(response.message || "");
      if (response.success) {
        loadClasses();
      }
    }
  };

  const handleViewStudents = async (cls: LopHoc) => {
    setSelectedClass(cls);
    setShowStudentsModal(true);
    setLoadingStudents(true);
    
    const response = await getLopHocStudents(cls.id!);
    if (response.success && response.data) {
      const danhSach = response.data.danhSachHocVien || [];
      setStudents(Array.isArray(danhSach) ? danhSach : []);
    } else {
      setStudents([]);
    }
    setLoadingStudents(false);
  };

  const handleViewTeachers = async (cls: LopHoc) => {
    setSelectedClass(cls);
    setShowTeachersModal(true);
    setLoadingTeachers(true);
    setSelectedTeacherId("");
    
    const response = await getPhanCongByLopHoc(cls.id!);
    if (response.success && response.data) {
      const dataArray = Array.isArray(response.data) ? response.data : [response.data];
      setAssignedTeachers(dataArray);
    } else {
      setAssignedTeachers([]);
    }
    setLoadingTeachers(false);
  };

  const handleAssignTeacher = async () => {
    if (!selectedTeacherId || !selectedClass) return;
    
    const response = await createPhanCong({
      giaoVienId: selectedTeacherId,
      lopHocId: selectedClass.id,
    });
    
    setMessage(response.message || "");
    if (response.success) {
      setSelectedTeacherId("");
      handleViewTeachers(selectedClass);
    }
  };

  const handleRemoveTeacher = async (phanCongId: string) => {
    if (!confirm("Bạn có chắc chắn muốn xóa phân công này?")) return;
    
    const response = await deletePhanCong(phanCongId);
    setMessage(response.message || "");
    if (response.success && selectedClass) {
      handleViewTeachers(selectedClass);
    }
  };

  const getTrangThaiText = (trangThai?: TrangThaiLopHoc) => {
    switch (trangThai) {
      case 0: return "Chưa mở";
      case 1: return "Đang học";
      case 2: return "Tạm nghỉ";
      case 3: return "Kết thúc";
      default: return "Không xác định";
    }
  };

  const getTrangThaiColor = (trangThai?: TrangThaiLopHoc) => {
    switch (trangThai) {
      case 0: return "bg-gray-100 text-gray-800";
      case 1: return "bg-green-100 text-green-800";
      case 2: return "bg-yellow-100 text-yellow-800";
      case 3: return "bg-red-100 text-red-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const filteredClasses = classes.filter(cls =>
    cls.tenLop?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getPaginatedData = () => {
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    return filteredClasses.slice(startIndex, endIndex);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  return (
    <div className="space-y-6">
      {message && (
        <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded-lg">
          {message}
        </div>
      )}

      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Quản lý lớp học</h1>
          <p className="text-gray-600 mt-1">Danh sách tất cả lớp học</p>
        </div>
        <button 
          onClick={handleCreate}
          className="bg-gray-900 text-white px-4 py-2 rounded-lg hover:bg-gray-800 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Thêm lớp học</span>
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Tìm kiếm lớp học..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          />
        </div>
      </div>

      {loading && (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
        </div>
      )}

      {!loading && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {getPaginatedData().map((cls) => (
            <div key={cls.id} className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              <div className="p-6">
                <div className="flex items-start justify-between mb-4">
                  <div className="w-12 h-12 bg-gray-900 rounded-lg flex items-center justify-center">
                    <Users className="w-6 h-6 text-white" />
                  </div>
                  <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${getTrangThaiColor(cls.trangThai)}`}>
                    {getTrangThaiText(cls.trangThai)}
                  </span>
                </div>
                
                <h3 className="text-xl font-bold text-gray-900 mb-2">{cls.tenLop}</h3>
                <p className="text-sm text-gray-600 mb-4">{cls.khoaHoc?.tenKhoaHoc}</p>
                
                <div className="space-y-2 mb-4">
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Sĩ số hiện tại:</span>
                    <span className="font-semibold text-gray-900">{cls.siSoHienTai || 0}</span>
                  </div>
                  <div className="flex justify-between text-sm">
                    <span className="text-gray-600">Sĩ số tối đa:</span>
                    <span className="font-semibold text-gray-900">{cls.siSoToiDa}</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-2">
                    <div 
                      className="bg-blue-600 h-2 rounded-full" 
                      style={{ width: `${((cls.siSoHienTai || 0) / (cls.siSoToiDa || 1)) * 100}%` }}
                    ></div>
                  </div>
                </div>
                
                <div className="flex flex-col space-y-2 pt-4 border-t border-gray-200">
                  <div className="flex space-x-2">
                    <button 
                      onClick={() => handleViewStudents(cls)}
                      className="flex-1 bg-green-600 text-white px-3 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center justify-center space-x-1"
                    >
                      <Eye className="w-4 h-4" />
                      <span className="text-sm">DS HV</span>
                    </button>
                    <button 
                      onClick={() => handleViewTeachers(cls)}
                      className="flex-1 bg-purple-600 text-white px-3 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center justify-center space-x-1"
                    >
                      <UserCheck className="w-4 h-4" />
                      <span className="text-sm">DS GV</span>
                    </button>
                  </div>
                  <div className="flex space-x-2">
                    <button 
                      onClick={() => handleEdit(cls)}
                      className="flex-1 bg-blue-600 text-white px-3 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center space-x-1"
                    >
                      <Edit className="w-4 h-4" />
                      <span className="text-sm">Sửa</span>
                    </button>
                    <button 
                      onClick={() => handleDelete(cls.id!)}
                      className="flex-1 bg-red-600 text-white px-3 py-2 rounded-lg hover:bg-red-700 transition-colors flex items-center justify-center space-x-1"
                    >
                      <Trash2 className="w-4 h-4" />
                      <span className="text-sm">Xóa</span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          ))}
          </div>

          {/* Pagination */}
          {filteredClasses.length > pageSize && (
            <div className="flex justify-center mt-6">
              <Pagination
                currentPage={currentPage}
                totalCount={filteredClasses.length}
                pageSize={pageSize}
                onPageChange={handlePageChange}
              />
            </div>
          )}
        </>
      )}

      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold text-gray-900">
                  {editingClass ? "Chỉnh sửa lớp học" : "Thêm lớp học mới"}
                </h2>
                <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tên lớp</label>
                  <input
                    type="text"
                    value={formData.tenLop}
                    onChange={(e) => setFormData({ ...formData, tenLop: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Khóa học</label>
                  <select
                    value={formData.khoaHocId}
                    onChange={(e) => setFormData({ ...formData, khoaHocId: e.target.value })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                  >
                    <option value="">-- Chọn khóa học --</option>
                    {courses.map((course) => (
                      <option key={course.id} value={course.id}>
                        {course.tenKhoaHoc}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Sĩ số tối đa</label>
                  <input
                    type="number"
                    value={formData.siSoToiDa}
                    onChange={(e) => setFormData({ ...formData, siSoToiDa: Number(e.target.value) })}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    required
                    min="1"
                  />
                </div>

                <div className="flex space-x-4 pt-4">
                  <button
                    type="submit"
                    className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    {editingClass ? "Cập nhật" : "Thêm mới"}
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

      {showStudentsModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col">
            <div className="p-6 border-b border-gray-200">
              <div className="flex justify-between items-center">
                <div>
                  <h2 className="text-2xl font-bold text-gray-900">
                    Danh sách học viên
                  </h2>
                  <p className="text-gray-600 mt-1">
                    Lớp: {selectedClass?.tenLop} - {selectedClass?.khoaHoc?.tenKhoaHoc}
                  </p>
                </div>
                <button 
                  onClick={() => setShowStudentsModal(false)} 
                  className="text-gray-500 hover:text-gray-700"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>
            </div>

            <div className="flex-1 overflow-y-auto p-6">
              {loadingStudents ? (
                <div className="text-center py-12">
                  <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
                  <p className="text-gray-600 mt-4">Đang tải danh sách học viên...</p>
                </div>
              ) : students.length === 0 ? (
                <div className="text-center py-12">
                  <Users className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                  <p className="text-gray-600 text-lg">Chưa có học viên nào trong lớp</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-gray-50 border-b border-gray-200">
                      <tr>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          STT
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          Họ tên
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          Email
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          Số điện thoại
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          Giới tính
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-200">
                      {students.map((student, index) => (
                        <tr key={student.id || index} className="hover:bg-gray-50 transition-colors">
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {index + 1}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="flex items-center">
                              <div className="w-10 h-10 bg-gray-900 rounded-full flex items-center justify-center text-white font-semibold">
                                {student.hoTen?.charAt(0) || "?"}
                              </div>
                              <div className="ml-3">
                                <p className="text-sm font-medium text-gray-900">
                                  {student.hoTen || "Chưa có tên"}
                                </p>
                              </div>
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {student.email || "—"}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {student.sdt || "—"}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                            <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                              student.gioiTinh === 0 ? 'bg-blue-100 text-blue-800' : 'bg-pink-100 text-pink-800'
                            }`}>
                              {student.gioiTinh === 0 ? 'Nam' : 'Nữ'}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>

            <div className="p-6 border-t border-gray-200">
              <div className="flex justify-between items-center">
                <p className="text-sm text-gray-600">
                  Tổng số học viên: <span className="font-semibold text-gray-900">{students.length}</span>
                </p>
                <button
                  onClick={() => setShowStudentsModal(false)}
                  className="bg-gray-900 text-white px-6 py-2 rounded-lg hover:bg-gray-800 transition-colors"
                >
                  Đóng
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {showTeachersModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-4xl w-full max-h-[90vh] overflow-hidden flex flex-col">
            <div className="p-6 border-b border-gray-200">
              <div className="flex justify-between items-center">
                <div>
                  <h2 className="text-2xl font-bold text-gray-900">
                    Phân công giáo viên
                  </h2>
                  <p className="text-gray-600 mt-1">
                    Lớp: {selectedClass?.tenLop} - {selectedClass?.khoaHoc?.tenKhoaHoc}
                  </p>
                </div>
                <button 
                  onClick={() => setShowTeachersModal(false)} 
                  className="text-gray-500 hover:text-gray-700"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>
            </div>

            <div className="flex-1 overflow-y-auto p-6">
              <div className="mb-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Thêm giáo viên</h3>
                <div className="flex space-x-3">
                  <select
                    value={selectedTeacherId}
                    onChange={(e) => setSelectedTeacherId(e.target.value)}
                    className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                    <option value="">-- Chọn giáo viên --</option>
                    {teachers
                      .filter(t => !assignedTeachers.some(at => at.giaoVienId === t.id))
                      .map((teacher) => (
                        <option key={teacher.id} value={teacher.id}>
                          {teacher.hoTen}
                        </option>
                      ))}
                  </select>
                  <button
                    onClick={handleAssignTeacher}
                    disabled={!selectedTeacherId}
                    className="bg-gray-900 text-white px-6 py-2 rounded-lg hover:bg-gray-800 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed"
                  >
                    Thêm
                  </button>
                </div>
              </div>

              <h3 className="text-lg font-semibold text-gray-900 mb-4">Danh sách giáo viên đã phân công</h3>
              
              {loadingTeachers ? (
                <div className="text-center py-12">
                  <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
                  <p className="text-gray-600 mt-4">Đang tải danh sách giáo viên...</p>
                </div>
              ) : assignedTeachers.length === 0 ? (
                <div className="text-center py-12">
                  <UserCheck className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                  <p className="text-gray-600 text-lg">Chưa có giáo viên được phân công</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full">
                    <thead className="bg-gray-50 border-b border-gray-200">
                      <tr>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          STT
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          Họ tên
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          Ngày phân công
                        </th>
                        <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                          Thao tác
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-200">
                      {assignedTeachers.map((assignment, index) => (
                        <tr key={assignment.id || index} className="hover:bg-gray-50 transition-colors">
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {index + 1}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap">
                            <div className="flex items-center">
                              <div className="w-10 h-10 bg-gray-900 rounded-full flex items-center justify-center text-white font-semibold">
                                {assignment.tenGiaoVien?.charAt(0) || "?"}
                              </div>
                              <div className="ml-3">
                                <p className="text-sm font-medium text-gray-900">
                                  {assignment.tenGiaoVien || "Chưa có tên"}
                                </p>
                              </div>
                            </div>
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                            {assignment.ngayPhanCong 
                              ? new Date(assignment.ngayPhanCong).toLocaleDateString('vi-VN')
                              : "—"}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-center">
                            <button
                              onClick={() => handleRemoveTeacher(assignment.id!)}
                              className="bg-red-600 text-white px-3 py-1 rounded-lg hover:bg-red-700 transition-colors text-sm"
                            >
                              Xóa
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>

            <div className="p-6 border-t border-gray-200">
              <div className="flex justify-between items-center">
                <p className="text-sm text-gray-600">
                  Tổng số giáo viên: <span className="font-semibold text-gray-900">{assignedTeachers.length}</span>
                </p>
                <button
                  onClick={() => setShowTeachersModal(false)}
                  className="bg-gray-900 text-white px-6 py-2 rounded-lg hover:bg-gray-800 transition-colors"
                >
                  Đóng
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
