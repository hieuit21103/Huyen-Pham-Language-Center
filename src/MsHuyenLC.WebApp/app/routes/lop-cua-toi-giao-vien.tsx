import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { 
  Users, 
  Download, 
  Search, 
  User, 
  Mail, 
  Phone, 
  Calendar,
  GraduationCap,
  BookOpen,
  FileDown,
  ChevronDown,
  X
} from "lucide-react";
import { getProfile } from "~/apis/Profile";
import { getPhanCongByGiaoVien } from "~/apis/PhanCong";
import { getLopHocStudents } from "~/apis/LopHoc";
import { VaiTro } from "~/types";
import { getByTaiKhoanId} from "~/apis/GiaoVien";

export default function LopCuaToiGiaoVien() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [classes, setClasses] = useState<any[]>([]);
  const [selectedClassId, setSelectedClassId] = useState<string>("");
  const [students, setStudents] = useState<any[]>([]);
  const [loadingStudents, setLoadingStudents] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [showStudentModal, setShowStudentModal] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);

    const profileRes = await getProfile();
    if (!profileRes.success || !profileRes.data) {
      navigate("/dang-nhap");
      return;
    }

    if (profileRes.data.vaiTro !== VaiTro.GiaoVien) {
      navigate("/dashboard");
      return;
    }

    const giaoVienRes = await getByTaiKhoanId(profileRes.data.id!);
    if (!giaoVienRes.success || !giaoVienRes.data) {
      navigate("/dashboard");
      return;
    }
    const phanCongRes = await getPhanCongByGiaoVien(giaoVienRes.data.id!);
    if (phanCongRes.success && phanCongRes.data) {
      const classesData = Array.isArray(phanCongRes.data) 
        ? phanCongRes.data 
        : [phanCongRes.data];
      setClasses(classesData);
            if (classesData.length > 0 && classesData[0].lopHoc?.id) {
        setSelectedClassId(classesData[0].lopHoc.id);
      }
    }

    setLoading(false);
  };

  useEffect(() => {
    if (selectedClassId) {
      loadStudents(selectedClassId);
    } else {
      setStudents([]);
    }
  }, [selectedClassId]);

  const loadStudents = async (classId: string) => {
    setLoadingStudents(true);
    try {
      const result = await getLopHocStudents(classId);
      if (result.success && result.data) {
        const studentsList = Array.isArray(result.data) ? result.data : [];
        setStudents(studentsList);
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

  const filteredStudents = students.filter((student) => {
    if (!searchTerm) return true;
    const search = searchTerm.toLowerCase();
    return (
      student.hoTen?.toLowerCase().includes(search) ||
      student.email?.toLowerCase().includes(search) ||
      student.sdt?.includes(search)
    );
  });

  const selectedClass = classes.find(c => c.lopHoc?.id === selectedClassId)?.lopHoc;

  const exportToExcel = () => {
    if (filteredStudents.length === 0) {
      alert("Không có dữ liệu để xuất");
      return;
    }

    const headers = ["STT", "Họ tên", "Email", "Số điện thoại", "Ngày sinh", "Địa chỉ"];
    const csvContent = [
      headers.join(","),
      ...filteredStudents.map((student, index) => [
        index + 1,
        `"${student.hoTen || ""}"`,
        `"${student.taiKhoan?.email || student.email || ""}"`,
        `"${student.taiKhoan?.sdt || student.sdt || ""}"`,
        `"${student.ngaySinh ? new Date(student.ngaySinh).toLocaleDateString('vi-VN') : ""}"`,
        `"${student.diaChi || ""}"`
      ].join(","))
    ].join("\n");

    const BOM = "\uFEFF";
    const blob = new Blob([BOM + csvContent], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    
    link.setAttribute("href", url);
    link.setAttribute("download", `Danh_sach_hoc_vien_${selectedClass?.tenLop || 'lop_hoc'}_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleViewDetails = (student: any) => {
    alert(`Thông tin chi tiết học viên: ${student.hoTen}`);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
          <p className="text-gray-600 mt-4">Đang tải dữ liệu...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <main className="max-w-7xl mx-auto px-4 py-8">
        {/* Header */}
        <div className="bg-white rounded-xl shadow-sm p-6 mb-6">
          <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
                <GraduationCap className="w-8 h-8" />
                Lớp học của tôi
              </h1>
              <p className="text-gray-600 mt-2">
                Quản lý và xem danh sách học viên trong các lớp được phân công
              </p>
            </div>
            
            {selectedClassId && filteredStudents.length > 0 && (
              <button
                onClick={exportToExcel}
                className="bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 transition-colors flex items-center gap-2 shadow-sm"
              >
                <FileDown className="w-5 h-5" />
                Xuất danh sách
              </button>
            )}
          </div>
        </div>

        {/* Class Selection */}
        <div className="bg-white rounded-xl shadow-sm p-6 mb-6">
          <label className="block text-sm font-semibold text-gray-700 mb-3">
            <BookOpen className="w-5 h-5 inline mr-2" />
            Chọn lớp học
          </label>
          
          {classes.length === 0 ? (
            <div className="text-center py-8 text-gray-500">
              <Users className="w-16 h-16 mx-auto mb-3 text-gray-400" />
              <p className="text-lg">Bạn chưa được phân công lớp học nào</p>
              <p className="text-sm mt-2">Vui lòng liên hệ giáo vụ để được phân công</p>
            </div>
          ) : (
            <div className="relative">
              <select
                value={selectedClassId}
                onChange={(e) => setSelectedClassId(e.target.value)}
                className="w-full px-4 py-3 pr-10 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent appearance-none bg-white text-gray-900 font-medium"
              >
                <option value="">-- Chọn lớp học --</option>
                {classes.map((assignment) => (
                  <option key={assignment.id} value={assignment.lopHoc?.id}>
                    {assignment.lopHoc?.tenLop} - {assignment.lopHoc?.khoaHoc?.tenKhoaHoc} 
                    ({assignment.lopHoc?.siSoHienTai || 0}/{assignment.lopHoc?.siSoToiDa || 0} học viên)
                  </option>
                ))}
              </select>
              <ChevronDown className="absolute right-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400 pointer-events-none" />
            </div>
          )}
        </div>

        {/* Students List */}
        {selectedClassId && (
          <div className="bg-white rounded-xl shadow-sm overflow-hidden">
            <div className="p-6 border-b border-gray-200">
              <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
                <div>
                  <h2 className="text-2xl font-bold text-gray-900">
                    Danh sách học viên
                  </h2>
                  {selectedClass && (
                    <p className="text-gray-600 mt-1">
                      Lớp: <span className="font-semibold">{selectedClass.tenLop}</span> - {selectedClass.khoaHoc?.tenKhoaHoc}
                    </p>
                  )}
                </div>

                {/* Search */}
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                  <input
                    type="text"
                    placeholder="Tìm theo tên, email, SĐT..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent w-full lg:w-80"
                  />
                </div>
              </div>

              {loadingStudents && (
                <div className="mt-4 text-center text-gray-600">
                  <div className="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-gray-900"></div>
                  <span className="ml-2">Đang tải...</span>
                </div>
              )}
            </div>

            {!loadingStudents && (
              <div className="overflow-x-auto">
                {filteredStudents.length === 0 ? (
                  <div className="text-center py-12">
                    <User className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                    <p className="text-gray-600 text-lg">
                      {searchTerm ? "Không tìm thấy học viên nào" : "Chưa có học viên trong lớp"}
                    </p>
                  </div>
                ) : (
                  <>
                    <table className="w-full">
                      <thead className="bg-gray-50 border-b border-gray-200">
                        <tr>
                          <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                            STT
                          </th>
                          <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                            Họ tên
                          </th>
                          <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                            Email
                          </th>
                          <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                            Số điện thoại
                          </th>
                          <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                            Ngày sinh
                          </th>
                          <th className="px-6 py-4 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                            Địa chỉ
                          </th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-200">
                        {filteredStudents.map((student, index) => (
                          <tr key={student.id} className="hover:bg-gray-50 transition-colors">
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
                            <td className="px-6 py-4 text-sm text-gray-900">
                              <div className="flex items-center">
                                <Mail className="w-4 h-4 text-gray-400 mr-2" />
                                {student.taiKhoan?.email || student.email || "—"}
                              </div>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                              <div className="flex items-center">
                                <Phone className="w-4 h-4 text-gray-400 mr-2" />
                                {student.taiKhoan?.sdt || student.sdt || "—"}
                              </div>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                              <div className="flex items-center">
                                <Calendar className="w-4 h-4 text-gray-400 mr-2" />
                                {student.ngaySinh 
                                  ? new Date(student.ngaySinh).toLocaleDateString('vi-VN')
                                  : "—"}
                              </div>
                            </td>
                            <td className="px-6 py-4 text-sm text-gray-900 max-w-xs truncate">
                              {student.diaChi || "—"}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>

                    {/* Stats Footer */}
                    <div className="bg-gray-50 px-6 py-4 border-t border-gray-200">
                      <div className="flex items-center justify-between text-sm text-gray-600">
                        <span>
                          Tổng số học viên: <strong className="text-gray-900">{filteredStudents.length}</strong>
                          {searchTerm && students.length !== filteredStudents.length && (
                            <span className="ml-2">(Lọc từ {students.length} học viên)</span>
                          )}
                        </span>
                        {selectedClass && (
                          <span>
                            Sĩ số: <strong className="text-gray-900">
                              {selectedClass.siSoHienTai || 0}/{selectedClass.siSoToiDa || 0}
                            </strong>
                          </span>
                        )}
                      </div>
                    </div>
                  </>
                )}
              </div>
            )}
          </div>
        )}
      </main>
    </div>
  );
}
