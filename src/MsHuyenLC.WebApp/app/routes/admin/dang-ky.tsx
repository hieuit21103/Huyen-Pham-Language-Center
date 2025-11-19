import { Search, CheckCircle, XCircle, Clock, Eye, AlertCircle, Users, X, Trash } from "lucide-react";
import { useState, useEffect } from "react";
import { deleteDangKy, getDangKys, updateDangKy } from "~/apis/DangKy";
import { getKhoaHocs } from "~/apis/KhoaHoc";
import { getLopHocs } from "~/apis/LopHoc";
import { TrangThaiDangKy, type DangKy, type HocVien, type KhoaHoc, type LopHoc } from "~/types/index";
import { setLightTheme } from "./_layout";
import Pagination from "~/components/Pagination";


interface Course {
  id?: string;
  tenKhoaHoc?: string;
}


export default function AdminRegistrations() {
  const [searchTerm, setSearchTerm] = useState("");
  const [registrations, setRegistrations] = useState<DangKy[]>([]);
  const [courses, setCourses] = useState<Course[]>([]);
  const [classes, setClasses] = useState<LopHoc[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedCourse, setSelectedCourse] = useState("");
  const [selectedStatus, setSelectedStatus] = useState("");
  const [message, setMessage] = useState("");
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [selectedRegistration, setSelectedRegistration] = useState<DangKy | null>(null);
  const [selectedClassId, setSelectedClassId] = useState("");

  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  useEffect(() => {
    setLightTheme();
    loadData();
  }, []);

  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, selectedCourse, selectedStatus]);

  const loadData = async () => {
    setLoading(true);
    const [registrationsRes, coursesRes, classesRes] = await Promise.all([
      getDangKys(),
      getKhoaHocs(),
      getLopHocs()
    ]);

    if (registrationsRes.success && registrationsRes.data) {
      const dataArray = Array.isArray(registrationsRes.data) 
        ? registrationsRes.data 
        : registrationsRes.data.items || [];
      setRegistrations(dataArray);
    }
    
    if (coursesRes.success && coursesRes.data) {
      const coursesArray = Array.isArray(coursesRes.data)
        ? coursesRes.data
        : coursesRes.data.items || [];
      setCourses(coursesArray);
    }

    if (classesRes.success && classesRes.data) {
      const classesArray = Array.isArray(classesRes.data)
        ? classesRes.data
        : classesRes.data.items || [];
      setClasses(classesArray);
    }

    setLoading(false);
  };

  const handleApprove = async (id: string) => {
    if (!confirm("Bạn có chắc chắn muốn duyệt đăng ký này?")) return;
    
    const response = await updateDangKy(id, { trangThai: TrangThaiDangKy.DaDuyet });
    setMessage(response.message || "");
    if (response.success) {
      loadData();
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const handleReject = async (id: string) => {
    if (!confirm("Bạn có chắc chắn muốn từ chối đăng ký này?")) return;
    
    const response = await updateDangKy(id, { trangThai: TrangThaiDangKy.Huy });
    setMessage(response.message || "");
    if (response.success) {
      loadData();
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const handleOpenAssignModal = (registration: DangKy) => {
    setSelectedRegistration(registration);
    setSelectedClassId("");
    setShowAssignModal(true);
  };

  const handleAssignClass = async () => {
    if (!selectedRegistration || !selectedClassId) {
      setMessage("Vui lòng chọn lớp học");
      return;
    }

    const response = await updateDangKy(selectedRegistration.id!, {
      lopHocId: selectedClassId,
      trangThai: TrangThaiDangKy.DaXepLop
    });

    setMessage(response.message || "");
    if (response.success) {
      setShowAssignModal(false);
      setSelectedRegistration(null);
      setSelectedClassId("");
      loadData();
      setTimeout(() => setMessage(""), 3000);
    }
  };

  const handleDelete = async (registration: DangKy) => {
    if (!confirm("Bạn có chắc chắn muốn xóa đăng ký này?")) return;

    const response = await deleteDangKy(registration.id!);
    setMessage(response.message || "");
    if (response.success) {
      loadData();
      setTimeout(() => setMessage(""), 3000);
    }
  };

  // Lấy danh sách lớp học theo khóa học đã chọn
  const availableClasses = selectedRegistration
    ? classes.filter(cls => cls.khoaHocId === selectedRegistration.khoaHocId)
    : [];

  const getStatusBadge = (status?: TrangThaiDangKy) => {
    switch (status) {
      case TrangThaiDangKy.DaDuyet:
        return (
          <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">
            <CheckCircle className="w-3 h-3 mr-1" />
            Đã duyệt
          </span>
        );
      case TrangThaiDangKy.Huy:
        return (
          <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-800">
            <XCircle className="w-3 h-3 mr-1" />
            Đã hủy
          </span>
        );
      case TrangThaiDangKy.DaXepLop:
        return (
          <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800">
            <CheckCircle className="w-3 h-3 mr-1" />
            Đã xếp lớp
          </span>
        );
      case TrangThaiDangKy.DaThanhToan:
        return (
          <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-purple-100 text-purple-800">
            <Clock className="w-3 h-3 mr-1" />
            Đã thanh toán
          </span>
        );
      default:
        return (
          <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-yellow-100 text-yellow-800">
            <Clock className="w-3 h-3 mr-1" />
            Chờ xử lý
          </span>
        );
    }
  };

  const filteredRegistrations = registrations.filter(reg => {
    // Nếu không có search term, trả về true
    const matchSearch = !searchTerm || 
      reg.hocVien?.hoTen?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      reg.hocVien?.taiKhoan?.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      reg.hocVien?.taiKhoan?.sdt?.includes(searchTerm);
    
    const matchCourse = !selectedCourse || reg.khoaHocId === selectedCourse;
    const matchStatus = !selectedStatus || reg.trangThai?.toString() === selectedStatus;
    
    return matchSearch && matchCourse && matchStatus;
  });

  const getPaginatedData = () => {
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    return filteredRegistrations.slice(startIndex, endIndex);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const getStatusCount = (status: TrangThaiDangKy) => {
    return filteredRegistrations.filter(r => r.trangThai === status).length;
  };

  return (
    <div className="space-y-6">
      {message && (
        <div className={`${message.includes("thất bại") || message.includes("Lỗi") ? "bg-red-100 border-red-400 text-red-700" : "bg-green-100 border-green-400 text-green-700"} border px-4 py-3 rounded-lg`}>
          {message}
        </div>
      )}

      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Quản lý đăng ký</h1>
        <p className="text-gray-600 mt-1">Danh sách đăng ký khóa học</p>
      </div>

      {/* Stats */}
      {loading ? (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {[1, 2, 3].map((i) => (
            <div key={i} className="bg-gray-50 border border-gray-200 rounded-xl p-6">
              <div className="h-20 bg-gray-200 animate-pulse rounded"></div>
            </div>
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-yellow-50 border border-yellow-200 rounded-xl p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-yellow-700 text-sm font-medium mb-1">Chờ duyệt</p>
                <p className="text-3xl font-bold text-yellow-900">{getStatusCount(TrangThaiDangKy.ChoDuyet)}</p>
              </div>
              <div className="w-12 h-12 bg-yellow-100 rounded-lg flex items-center justify-center">
                <Clock className="w-6 h-6 text-yellow-700" />
              </div>
            </div>
          </div>
          <div className="bg-green-50 border border-green-200 rounded-xl p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-green-700 text-sm font-medium mb-1">Đã duyệt</p>
                <p className="text-3xl font-bold text-green-900">{getStatusCount(TrangThaiDangKy.DaDuyet)}</p>
              </div>
              <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                <CheckCircle className="w-6 h-6 text-green-700" />
              </div>
            </div>
          </div>
          <div className="bg-red-50 border border-red-200 rounded-xl p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-red-700 text-sm font-medium mb-1">Từ chối</p>
                <p className="text-3xl font-bold text-red-900">{getStatusCount(TrangThaiDangKy.Huy)}</p>
              </div>
              <div className="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
                <XCircle className="w-6 h-6 text-red-700" />
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Tìm kiếm theo tên, email, SĐT..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
            />
          </div>
          <select 
            value={selectedCourse}
            onChange={(e) => setSelectedCourse(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
            <option value="">Tất cả khóa học</option>
            {courses.map((course) => (
              <option key={course.id} value={course.id}>
                {course.tenKhoaHoc}
              </option>
            ))}
          </select>
          <select 
            value={selectedStatus}
            onChange={(e) => setSelectedStatus(e.target.value)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
            <option value="">Tất cả trạng thái</option>
            <option value={TrangThaiDangKy.ChoDuyet.toString()}>Chờ duyệt</option>
            <option value={TrangThaiDangKy.DaDuyet.toString()}>Đã duyệt</option>
            <option value={TrangThaiDangKy.Huy.toString()}>Đã hủy</option>
            <option value={TrangThaiDangKy.DaThanhToan.toString()}>Đã thanh toán</option>
            <option value={TrangThaiDangKy.DaXepLop.toString()}>Đã xếp lớp</option>
          </select>
        </div>
      </div>

      {/* Registrations Table */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        {loading ? (
          <div className="p-8 text-center">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900 mb-4"></div>
            <p className="text-gray-600">Đang tải dữ liệu...</p>
          </div>
        ) : filteredRegistrations.length === 0 ? (
          <div className="p-8 text-center">
            <AlertCircle className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <p className="text-gray-600 text-lg">Không tìm thấy đăng ký nào</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-200">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Học viên
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Liên hệ
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Khóa học
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Ngày đăng ký
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Trạng thái
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-semibold text-gray-600 uppercase tracking-wider">
                    Thao tác
                  </th>
                </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
                {getPaginatedData().map((registration) => (
                  <tr key={registration.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="w-10 h-10 bg-gray-900 rounded-full flex items-center justify-center text-white font-semibold">
                          {registration.hocVien?.hoTen?.charAt(0) || "?"}
                        </div>
                        <div className="ml-3">
                          <p className="text-sm font-medium text-gray-900">
                            {registration.hocVien?.hoTen || "Chưa có tên"}
                          </p>
                          {registration.lopHoc && (
                            <p className="text-xs text-gray-500 italic">Lớp: {registration.lopHoc.tenLop}</p>
                          )}
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <p className="text-sm text-gray-900">{registration.hocVien?.taiKhoan?.email || "—"}</p>
                      <p className="text-sm text-gray-600">{registration.hocVien?.taiKhoan?.sdt || "—"}</p>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 font-medium">
                      {registration.khoaHoc?.tenKhoaHoc || "—"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {registration.ngayDangKy 
                        ? new Date(registration.ngayDangKy).toLocaleDateString('vi-VN')
                        : "—"}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {getStatusBadge(registration.trangThai)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end space-x-2">
                        {registration.trangThai === TrangThaiDangKy.ChoDuyet && (
                          <>
                            <button 
                              onClick={() => handleApprove(registration.id!)}
                              className="text-green-600 hover:text-green-900 px-3 py-1 hover:bg-green-50 rounded-lg transition-colors font-semibold"
                            >
                              Duyệt
                            </button>
                            <button 
                              onClick={() => handleReject(registration.id!)}
                              className="text-red-600 hover:text-red-900 px-3 py-1 hover:bg-red-50 rounded-lg transition-colors font-semibold"
                            >
                              Từ chối
                            </button>
                          </>
                        )}
                        {(registration.trangThai === TrangThaiDangKy.DaDuyet || registration.trangThai === TrangThaiDangKy.DaThanhToan) && (
                          <button
                            onClick={() => handleOpenAssignModal(registration)}
                            className="text-blue-600 hover:text-blue-900 px-3 py-1 hover:bg-blue-50 rounded-lg transition-colors font-semibold inline-flex items-center"
                          >
                            <Users className="w-4 h-4 mr-1" />
                            Xếp lớp
                          </button>
                        )}
                        <button
                            onClick={() => handleDelete(registration)}
                            className="text-red-600 hover:text-red-900 px-3 py-1 hover:bg-red-50 rounded-lg transition-colors font-semibold inline-flex items-center"
                          >
                            <Trash className="w-4 h-4 mr-1" />
                            Xóa
                          </button>
                        <button className="text-gray-600 hover:text-gray-900 p-2 hover:bg-gray-50 rounded-lg transition-colors">
                          <Eye className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Pagination */}
        {!loading && filteredRegistrations.length > pageSize && (
          <div className="flex justify-center p-6 border-t border-gray-200">
            <Pagination
              currentPage={currentPage}
              totalCount={filteredRegistrations.length}
              pageSize={pageSize}
              onPageChange={handlePageChange}
            />
          </div>
        )}
      </div>

      {/* Modal xếp lớp */}
      {showAssignModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-xl max-w-md w-full mx-4">
            <div className="flex items-center justify-between p-6 border-b border-gray-200">
              <h3 className="text-xl font-bold text-gray-900">Xếp lớp học</h3>
              <button
                onClick={() => setShowAssignModal(false)}
                className="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            <div className="p-6 space-y-4">
              <div>
                <p className="text-sm text-gray-600 mb-1">Học viên</p>
                <p className="font-semibold text-gray-900">
                  {selectedRegistration?.hocVien?.hoTen || "Chưa có tên"}
                </p>
              </div>

              <div>
                <p className="text-sm text-gray-600 mb-1">Khóa học</p>
                <p className="font-semibold text-gray-900">
                  {selectedRegistration?.khoaHoc?.tenKhoaHoc || "—"}
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Chọn lớp học <span className="text-red-500">*</span>
                </label>
                {availableClasses.length === 0 ? (
                  <p className="text-sm text-gray-500 italic">
                    Không có lớp học nào cho khóa học này
                  </p>
                ) : (
                  <select
                    value={selectedClassId}
                    onChange={(e) => setSelectedClassId(e.target.value)}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                    <option value="">-- Chọn lớp học --</option>
                    {availableClasses.map((cls) => (
                      <option key={cls.id} value={cls.id}>
                        {cls.tenLop} ({cls.siSoHienTai || 0}/{cls.siSoToiDa || 0})
                      </option>
                    ))}
                  </select>
                )}
              </div>
            </div>

            <div className="flex items-center justify-end gap-3 p-6 border-t border-gray-200">
              <button
                onClick={() => setShowAssignModal(false)}
                className="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Hủy
              </button>
              <button
                onClick={handleAssignClass}
                disabled={!selectedClassId}
                className="px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Xếp lớp
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
