import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import {
  FileCheck,
  Search,
  Calendar,
  Users,
  CheckCircle,
  XCircle,
  Eye,
  X,
  Filter,
  TrendingUp,
} from "lucide-react";
import { getProfile } from "~/apis/Profile";
import { getByTaiKhoanId as getGiaoVienByTaiKhoanId } from "~/apis/HocVien";
import { getLopHocs } from "~/apis/LopHoc";
import { getPhienLamBais } from "~/apis/PhienLamBai";
import { gradePhienLamBai } from "~/apis/PhienLamBai";
import type { LopHoc, PhienLamBai } from "~/types/index";
import Header from "~/components/Header";
import Footer from "~/components/Footer";

export default function TeacherGrading() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [classes, setClasses] = useState<LopHoc[]>([]);
  const [sessions, setSessions] = useState<PhienLamBai[]>([]);
  const [filteredSessions, setFilteredSessions] = useState<PhienLamBai[]>([]);
  const [selectedClass, setSelectedClass] = useState<string>("");
  const [searchTerm, setSearchTerm] = useState("");
  const [showGradeModal, setShowGradeModal] = useState(false);
  const [selectedSession, setSelectedSession] = useState<PhienLamBai | null>(null);
  const [gradeValue, setGradeValue] = useState("");

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    applyFilters();
  }, [sessions, selectedClass, searchTerm]);

  const loadData = async () => {
    setLoading(true);

    const profileRes = await getProfile();
    if (!profileRes.success || !profileRes.data) {
      navigate("/dang-nhap");
      return;
    }

    const teacherRes = await getGiaoVienByTaiKhoanId(profileRes.data.id!);
    if (teacherRes.success && teacherRes.data) {
      const classesRes = await getLopHocs();
      if (classesRes.success && classesRes.data) {
        setClasses(classesRes.data);
      }
    }

    const sessionsRes = await getPhienLamBais();
    if (sessionsRes.success && sessionsRes.data) {
      setSessions(sessionsRes.data);
    }

    setLoading(false);
  };

  const applyFilters = () => {
    let filtered = [...sessions];

    if (selectedClass) {
      filtered = filtered.filter(
        (session) => session.hocVien?.lopHocId === selectedClass
      );
    }

    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      filtered = filtered.filter(
        (session) =>
          session.hocVien?.hoTen?.toLowerCase().includes(term) ||
          session.kyThi?.tenKyThi?.toLowerCase().includes(term) ||
          session.deThi?.tenDe?.toLowerCase().includes(term)
      );
    }

    setFilteredSessions(filtered);
  };

  const handleGrade = (session: PhienLamBai) => {
    setSelectedSession(session);
    setGradeValue(session.diem !== null && session.diem !== undefined ? session.diem.toString() : "");
    setShowGradeModal(true);
  };

  const handleSaveGrade = async () => {
    if (!selectedSession || !gradeValue) return;

    const grade = parseFloat(gradeValue);
    if (isNaN(grade) || grade < 0 || grade > 10) {
      alert("Vui lòng nhập điểm hợp lệ từ 0 đến 10");
      return;
    }

    const result = await gradePhienLamBai(selectedSession.id!, {
      diem: grade,
    });

    if (result.success) {
      alert("Chấm điểm thành công!");
      setShowGradeModal(false);
      loadData();
    } else {
      alert(result.message || "Có lỗi xảy ra khi chấm điểm");
    }
  };

  const getGradeColor = (diem?: number) => {
    if (diem === null || diem === undefined) return "text-gray-500";
    if (diem >= 8) return "text-green-600";
    if (diem >= 6) return "text-blue-600";
    if (diem >= 5) return "text-yellow-600";
    return "text-red-600";
  };

  const ungradedCount = filteredSessions.filter(
    (s) => s.diem === null || s.diem === undefined
  ).length;
  const passedCount = filteredSessions.filter((s) => s.diem && s.diem >= 5).length;
  const failedCount = filteredSessions.filter((s) => s.diem && s.diem < 5).length;
  const averageScore =
    filteredSessions.filter((s) => s.diem !== null && s.diem !== undefined).length > 0
      ? (
          filteredSessions
            .filter((s) => s.diem !== null && s.diem !== undefined)
            .reduce((sum, s) => sum + (s.diem || 0), 0) /
          filteredSessions.filter((s) => s.diem !== null && s.diem !== undefined).length
        ).toFixed(2)
      : "N/A";

  return (
    <div className="min-h-screen bg-gray-50">
      <main className="pb-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-gray-900">Chấm điểm</h1>
            <p className="text-gray-600 mt-2">Quản lý và chấm điểm bài thi của học viên</p>
          </div>

          {/* Statistics Cards */}
          <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-600">Tổng bài thi</p>
                  <p className="text-3xl font-bold text-gray-900 mt-1">
                    {filteredSessions.length}
                  </p>
                </div>
                <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
                  <FileCheck className="w-6 h-6 text-blue-600" />
                </div>
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-600">Chưa chấm</p>
                  <p className="text-3xl font-bold text-orange-600 mt-1">{ungradedCount}</p>
                </div>
                <div className="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
                  <Calendar className="w-6 h-6 text-orange-600" />
                </div>
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-600">Đạt / Không đạt</p>
                  <p className="text-3xl font-bold mt-1">
                    <span className="text-green-600">{passedCount}</span>
                    <span className="text-gray-400 text-xl mx-1">/</span>
                    <span className="text-red-600">{failedCount}</span>
                  </p>
                </div>
                <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                  <Users className="w-6 h-6 text-green-600" />
                </div>
              </div>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-600">Điểm trung bình</p>
                  <p className="text-3xl font-bold text-gray-900 mt-1">{averageScore}</p>
                </div>
                <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
                  <TrendingUp className="w-6 h-6 text-purple-600" />
                </div>
              </div>
            </div>
          </div>

          {/* Filters */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Filter className="w-4 h-4 inline mr-2" />
                  Lọc theo lớp
                </label>
                <select
                  value={selectedClass}
                  onChange={(e) => setSelectedClass(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                >
                  <option value="">Tất cả lớp</option>
                  {classes.map((cls) => (
                    <option key={cls.id} value={cls.id}>
                      {cls.tenLop}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  <Search className="w-4 h-4 inline mr-2" />
                  Tìm kiếm
                </label>
                <input
                  type="text"
                  placeholder="Tìm theo tên học viên, kỳ thi, đề thi..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                />
              </div>
            </div>
          </div>

          {/* Table */}
          {loading ? (
            <div className="text-center py-12">
              <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
            </div>
          ) : (
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
              {filteredSessions.length === 0 ? (
                <div className="text-center py-12">
                  <FileCheck className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                  <p className="text-gray-600 text-lg">Không có bài thi nào</p>
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="w-full table-auto">
                    <thead className="bg-gray-50 border-b border-gray-200">
                      <tr>
                        <th className="px-3 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider w-12">
                          STT
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider min-w-[120px]">
                          Học viên
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider min-w-[100px]">
                          Lớp
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider min-w-[150px]">
                          Kỳ thi / Đề thi
                        </th>
                        <th className="px-4 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider w-28">
                          Ngày làm
                        </th>
                        <th className="px-4 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider w-20">
                          Câu đúng
                        </th>
                        <th className="px-4 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider w-20">
                          Điểm
                        </th>
                        <th className="px-4 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider w-28">
                          Kết quả
                        </th>
                        <th className="px-4 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider w-32">
                          Thao tác
                        </th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-200">
                      {filteredSessions.map((session, index) => (
                        <tr key={session.id} className="hover:bg-gray-50 transition-colors">
                          <td className="px-3 py-4 text-sm text-gray-900 text-center">
                            {index + 1}
                          </td>
                          <td className="px-4 py-4">
                            <div className="text-sm font-medium text-gray-900 truncate max-w-[150px]" title={session.hocVien?.hoTen || "—"}>
                              {session.hocVien?.hoTen || "—"}
                            </div>
                          </td>
                          <td className="px-4 py-4">
                            <div className="text-sm text-gray-600 truncate max-w-[120px]" title={session.hocVien?.lopHoc?.tenLop || "—"}>
                              {session.hocVien?.lopHoc?.tenLop || "—"}
                            </div>
                          </td>
                          <td className="px-4 py-4">
                            <div className="text-sm text-gray-900 truncate max-w-[180px]" title={session.kyThi?.tenKyThi || "—"}>
                              {session.kyThi?.tenKyThi || "—"}
                            </div>
                            <div className="text-xs text-gray-500 truncate max-w-[180px]" title={session.deThi?.tenDe || "—"}>
                              {session.deThi?.tenDe || "—"}
                            </div>
                          </td>
                          <td className="px-4 py-4 text-sm text-gray-600 text-center">
                            {session.ngayLam ? new Date(session.ngayLam).toLocaleDateString("vi-VN") : "—"}
                          </td>
                          <td className="px-4 py-4 text-center text-sm text-gray-900">
                            {session.soCauDung || 0} / {session.tongCauHoi}
                          </td>
                          <td className="px-4 py-4 text-center">
                            <span
                              className={`text-base font-bold ${getGradeColor(session.diem)}`}
                            >
                              {session.diem !== null && session.diem !== undefined ? session.diem.toFixed(2) : "—"}
                            </span>
                          </td>
                          <td className="px-4 py-4 text-center">
                            {session.diem !== null && session.diem !== undefined ? (
                              session.diem >= 5 ? (
                                <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">
                                  <CheckCircle className="w-3 h-3 mr-1" />
                                  Đạt
                                </span>
                              ) : (
                                <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-800">
                                  <XCircle className="w-3 h-3 mr-1" />
                                  Không đạt
                                </span>
                              )
                            ) : (
                              <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold bg-gray-100 text-gray-800">
                                Chưa chấm
                              </span>
                            )}
                          </td>
                          <td className="px-4 py-4 text-center">
                            <button
                              onClick={() => handleGrade(session)}
                              className="bg-gray-900 text-white px-3 py-1.5 rounded-lg hover:bg-gray-800 transition-colors text-sm inline-flex items-center space-x-1.5"
                            >
                              <Eye className="w-4 h-4" />
                              <span>Chấm điểm</span>
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}
        </div>
      </main>

      {/* Grade Modal */}
      {showGradeModal && selectedSession && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white p-6 border-b border-gray-200 flex justify-between items-center">
              <h2 className="text-2xl font-bold text-gray-900">Chấm điểm bài thi</h2>
              <button
                onClick={() => setShowGradeModal(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            <div className="p-6 space-y-6">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-600">Học viên</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.hocVien?.hoTen || "—"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Lớp</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.hocVien?.lopHoc?.tenLop || "—"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Kỳ thi</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.kyThi?.tenKyThi || "—"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Đề thi</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.deThi?.tenDe || "—"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Ngày làm</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.ngayLam ? new Date(selectedSession.ngayLam).toLocaleDateString("vi-VN") : "—"}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Số câu đúng</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.soCauDung || 0} / {selectedSession.tongCauHoi}
                  </p>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Điểm số (0-10)
                </label>
                <input
                  type="number"
                  step="0.01"
                  min="0"
                  max="10"
                  value={gradeValue}
                  onChange={(e) => setGradeValue(e.target.value)}
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent text-lg"
                  placeholder="Nhập điểm"
                />
              </div>

              <div className="flex space-x-4">
                <button
                  onClick={handleSaveGrade}
                  className="flex-1 bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors font-semibold"
                >
                  Lưu điểm
                </button>
                <button
                  onClick={() => setShowGradeModal(false)}
                  className="flex-1 bg-gray-200 text-gray-900 px-6 py-3 rounded-lg hover:bg-gray-300 transition-colors font-semibold"
                >
                  Hủy
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
