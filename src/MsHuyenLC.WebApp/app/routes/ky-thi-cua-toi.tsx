import { useState, useEffect } from "react";
import { useNavigate } from "react-router";
import { Calendar, Clock, FileText, PlayCircle, CheckCircle, AlertCircle, Eye, X } from "lucide-react";
import { getProfile } from "~/apis/Profile";
import { getByTaiKhoanId } from "~/apis/HocVien";
import { getKyThiByHocVien } from "~/apis/KyThi";
import { getPhienLamBaiByHocVien } from "~/apis/PhienLamBai";
import type { KyThi, PhienLamBai, HocVien } from "~/types/index";
import Header from "~/components/Header";
import Footer from "~/components/Footer";
import { formatDateTime } from "~/utils/date-utils";

export default function MyExams() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [student, setStudent] = useState<HocVien>();
  const [exams, setExams] = useState<KyThi[]>([]);
  const [sessions, setSessions] = useState<PhienLamBai[]>([]);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [selectedSession, setSelectedSession] = useState<PhienLamBai | null>(null);
  const [activeTab, setActiveTab] = useState<"upcoming" | "completed">("upcoming");

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

    const studentRes = await getByTaiKhoanId(profileRes.data.id!);
    if (studentRes.success && studentRes.data) {
      setStudent(studentRes.data);

      const examsRes = await getKyThiByHocVien(studentRes.data.id!);
      if (examsRes.success && examsRes.data) {
        setExams(examsRes.data);
      }

      const sessionsRes = await getPhienLamBaiByHocVien(studentRes.data.id!);
      console.log("Loaded sessions:", sessionsRes);
      if (sessionsRes.success && sessionsRes.data) {
        setSessions(sessionsRes.data);
      }
    }

    setLoading(false);
  };

  const handleViewDetail = (session: PhienLamBai) => {
    setSelectedSession(session);
    setShowDetailModal(true);
  };

  const getTrangThaiText = (trangThai?: number) => {
    switch (trangThai) {
      case 0: return "Sắp diễn ra";
      case 1: return "Đang diễn ra";
      case 2: return "Kết thúc";
      case 3: return "Đã hủy";
      default: return "Không xác định";
    }
  };

  const getTrangThaiColor = (trangThai?: number) => {
    switch (trangThai) {
      case 0: return "bg-blue-100 text-blue-800";
      case 1: return "bg-green-100 text-green-800";
      case 2: return "bg-gray-100 text-gray-800";
      case 3: return "bg-red-100 text-red-800";
      default: return "bg-gray-100 text-gray-800";
    }
  };

  const getGradeColor = (diem?: number) => {
    if (diem === null || diem === undefined) return "text-gray-500";
    if (diem >= 8) return "text-green-600";
    if (diem >= 6) return "text-blue-600";
    if (diem >= 5) return "text-yellow-600";
    return "text-red-600";
  };

  const upcomingExams = exams.filter(exam => exam.trangThai === 0 || exam.trangThai === 1);
  const completedSessions = sessions.filter(session => session.diem !== null);

  const hasCompletedExam = (examId: string) => {
    return sessions.some(session => session.kyThiId === examId);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <main className="pb-16">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-gray-900">Kỳ thi của tôi</h1>
            <p className="text-gray-600 mt-2">Quản lý và theo dõi các kỳ thi của bạn</p>
          </div>

          {/* Tabs */}
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 mb-6">
            <div className="flex border-b border-gray-200">
              <button
                onClick={() => setActiveTab("upcoming")}
                className={`flex-1 px-6 py-4 text-center font-semibold transition-colors ${
                  activeTab === "upcoming"
                    ? "text-gray-900 border-b-2 border-gray-900"
                    : "text-gray-600 hover:text-gray-900"
                }`}
              >
                <Calendar className="w-5 h-5 inline mr-2" />
                Kỳ thi sắp diễn ra ({upcomingExams.length})
              </button>
              <button
                onClick={() => setActiveTab("completed")}
                className={`flex-1 px-6 py-4 text-center font-semibold transition-colors ${
                  activeTab === "completed"
                    ? "text-gray-900 border-b-2 border-gray-900"
                    : "text-gray-600 hover:text-gray-900"
                }`}
              >
                <CheckCircle className="w-5 h-5 inline mr-2" />
                Đã hoàn thành ({completedSessions.length})
              </button>
            </div>
          </div>

          {loading ? (
            <div className="text-center py-12">
              <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-gray-900"></div>
            </div>
          ) : (
            <>
              {/* Upcoming Exams Tab */}
              {activeTab === "upcoming" && (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  {upcomingExams.length === 0 ? (
                    <div className="col-span-full text-center py-12 bg-white rounded-xl border border-gray-200">
                      <Calendar className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                      <p className="text-gray-600 text-lg">Không có kỳ thi nào sắp diễn ra</p>
                    </div>
                  ) : (
                    upcomingExams.map((exam) => (
                      <div
                        key={exam.id}
                        className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow"
                      >
                        <div className="p-6">
                          <div className="flex items-start justify-between mb-4">
                            <div className="w-12 h-12 bg-gray-900 rounded-lg flex items-center justify-center">
                              <FileText className="w-6 h-6 text-white" />
                            </div>
                            <span
                              className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${getTrangThaiColor(
                                exam.trangThai
                              )}`}
                            >
                              {getTrangThaiText(exam.trangThai)}
                            </span>
                          </div>

                          <h3 className="text-xl font-bold text-gray-900 mb-2">{exam.tenKyThi}</h3>

                          <div className="space-y-2 mb-4">
                            <div className="flex items-center text-sm text-gray-600">
                              <Calendar className="w-4 h-4 mr-2" />
                              <span>
                                {formatDateTime(exam.ngayThi)}
                              </span>
                            </div>
                            <div className="flex items-center text-sm text-gray-600">
                              <Clock className="w-4 h-4 mr-2" />
                              <span>
                                {exam.gioBatDau} - {exam.gioKetThuc}
                              </span>
                            </div>
                            <div className="flex items-center text-sm text-gray-600">
                              <Clock className="w-4 h-4 mr-2" />
                              <span>Thời lượng: {exam.thoiLuong} phút</span>
                            </div>
                          </div>

                          {hasCompletedExam(exam.id!) && (
                            <div className="bg-green-50 border border-green-200 rounded-lg p-3 mb-4">
                              <div className="flex items-center text-green-800">
                                <CheckCircle className="w-4 h-4 mr-2" />
                                <span className="text-sm font-semibold">Đã hoàn thành</span>
                              </div>
                            </div>
                          )}

                          {exam.trangThai === 1 && !hasCompletedExam(exam.id!) && (
                            <button
                              onClick={() => navigate(`/thi?kyThiId=${exam.id}`)}
                              className="w-full bg-green-600 text-white px-4 py-3 rounded-lg hover:bg-green-700 transition-colors flex items-center justify-center space-x-2"
                            >
                              <PlayCircle className="w-5 h-5" />
                              <span className="font-semibold">Vào thi</span>
                            </button>
                          )}

                          {exam.trangThai === 0 && (
                            <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                              <div className="flex items-center text-blue-800">
                                <AlertCircle className="w-4 h-4 mr-2" />
                                <span className="text-sm">Chưa đến giờ thi</span>
                              </div>
                            </div>
                          )}
                        </div>
                      </div>
                    ))
                  )}
                </div>
              )}

              {/* Completed Tab */}
              {activeTab === "completed" && (
                <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                  {completedSessions.length === 0 ? (
                    <div className="text-center py-12">
                      <CheckCircle className="w-16 h-16 text-gray-400 mx-auto mb-4" />
                      <p className="text-gray-600 text-lg">Chưa có kỳ thi nào hoàn thành</p>
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
                              Kỳ thi
                            </th>
                            <th className="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">
                              Ngày làm
                            </th>
                            <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                              Tổng câu
                            </th>
                            <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                              Câu đúng
                            </th>
                            <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                              Điểm
                            </th>
                            <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                              Kết quả
                            </th>
                            <th className="px-6 py-3 text-center text-xs font-semibold text-gray-600 uppercase tracking-wider">
                              Thao tác
                            </th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                          {completedSessions.map((session, index) => (
                            <tr key={session.id} className="hover:bg-gray-50 transition-colors">
                              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                {index + 1}
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap">
                                <div className="text-sm font-medium text-gray-900">
                                  {session.kyThi?.tenKyThi || session.deThi?.tenDe || "—"}
                                </div>
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                                {formatDateTime(session.ngayLam!)}
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-900">
                                {session.tongCauHoi}
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-900">
                                {session.soCauDung || "—"}
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-center">
                                <span
                                  className={`text-lg font-bold ${getGradeColor(session.diem)}`}
                                >
                                  {session.diem !== null && session.diem !== undefined ? session.diem.toFixed(2) : "—"}
                                </span>
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-center">
                                {session.diem !== null && session.diem !== undefined ? (
                                  session.diem >= 5 ? (
                                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">
                                      <CheckCircle className="w-3 h-3 mr-1" />
                                      Đạt
                                    </span>
                                  ) : (
                                    <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-800">
                                      <X className="w-3 h-3 mr-1" />
                                      Không đạt
                                    </span>
                                  )
                                ) : (
                                  <span className="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-gray-100 text-gray-800">
                                    Chưa chấm
                                  </span>
                                )}
                              </td>
                              <td className="px-6 py-4 whitespace-nowrap text-center">
                                <button
                                  onClick={() => handleViewDetail(session)}
                                  className="bg-blue-600 text-white px-3 py-1 rounded-lg hover:bg-blue-700 transition-colors text-sm inline-flex items-center space-x-1"
                                >
                                  <Eye className="w-4 h-4" />
                                  <span>Chi tiết</span>
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
            </>
          )}
        </div>
      </main>

      {/* Detail Modal */}
      {showDetailModal && selectedSession && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white p-6 border-b border-gray-200 flex justify-between items-center z-10">
              <h2 className="text-2xl font-bold text-gray-900">Chi tiết kết quả</h2>
              <button
                onClick={() => setShowDetailModal(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>

            <div className="p-6 space-y-6">
              {/* Summary Section */}
              <div className="grid grid-cols-2 gap-4">
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
                    {formatDateTime(selectedSession.ngayLam!)}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Thời gian làm</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.thoiGianLam 
                      ? (() => {
                          const seconds = typeof selectedSession.thoiGianLam === 'number' 
                            ? selectedSession.thoiGianLam 
                            : 0;
                          const hours = Math.floor(seconds / 3600);
                          const minutes = Math.floor((seconds % 3600) / 60);
                          const secs = seconds % 60;
                          return hours > 0 
                            ? `${hours}h ${minutes}m ${secs}s`
                            : `${minutes}m ${secs}s`;
                        })()
                      : "—"
                    }
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Tổng câu hỏi</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.tongCauHoi}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Số câu đúng</p>
                  <p className="text-base font-medium text-gray-900">
                    {selectedSession.soCauDung || "—"}
                  </p>
                </div>
              </div>

              {/* Score Section */}
              <div className="bg-gray-50 rounded-lg p-6 text-center">
                <p className="text-sm text-gray-600 mb-2">Điểm số</p>
                <p className={`text-4xl font-bold ${getGradeColor(selectedSession.diem)}`}>
                  {selectedSession.diem !== null && selectedSession.diem !== undefined ? selectedSession.diem.toFixed(2) : "—"}
                </p>
                {selectedSession.diem !== null && selectedSession.diem !== undefined && (
                  <p className="text-sm text-gray-600 mt-2">
                    {selectedSession.diem >= 5 ? "✓ Đạt yêu cầu" : "✗ Chưa đạt yêu cầu"}
                  </p>
                )}
              </div>

              {/* Detailed Answers Section */}
              {selectedSession.cacCauTraLoi && selectedSession.cacCauTraLoi.length > 0 && (
                <div className="space-y-4">
                  <h3 className="text-lg font-bold text-gray-900 flex items-center">
                    <FileText className="w-5 h-5 mr-2" />
                    Chi tiết câu trả lời ({selectedSession.cacCauTraLoi.length} câu)
                  </h3>
                  <div className="space-y-4">
                    {selectedSession.cacCauTraLoi.map((answer: any, index: number) => {
                      const cauHoi = answer.cauHoi || {};
                      const cacDapAn = cauHoi.cacDapAn || [];
                      const dapAnDung = cacDapAn.find((da: any) => da.dung === true);
                      
                      return (
                        <div
                          key={answer.id || index}
                          className={`border rounded-lg p-4 ${
                            answer.dung
                              ? "bg-green-50 border-green-200"
                              : "bg-red-50 border-red-200"
                          }`}
                        >
                          <div className="flex items-start justify-between mb-3">
                            <div className="flex items-center space-x-2">
                              <span className="inline-flex items-center justify-center w-8 h-8 rounded-full bg-gray-900 text-white text-sm font-semibold">
                                {index + 1}
                              </span>
                              <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-semibold ${
                                answer.dung
                                  ? "bg-green-100 text-green-800"
                                  : "bg-red-100 text-red-800"
                              }`}>
                                {answer.dung ? (
                                  <>
                                    <CheckCircle className="w-3 h-3 mr-1" />
                                    Đúng
                                  </>
                                ) : (
                                  <>
                                    <X className="w-3 h-3 mr-1" />
                                    Sai
                                  </>
                                )}
                              </span>
                            </div>
                          </div>

                          <div className="space-y-3">
                            {/* Question */}
                            <div>
                              <p className="text-sm font-medium text-gray-700 mb-1">Câu hỏi:</p>
                              <p className="text-base text-gray-900">
                                {cauHoi.noiDungCauHoi || "Nội dung câu hỏi không có"}
                              </p>
                            </div>

                            {/* Answer Options */}
                            {cacDapAn.length > 0 && (
                              <div>
                                <p className="text-sm font-medium text-gray-700 mb-2">Các đáp án:</p>
                                <div className="space-y-2">
                                  {cacDapAn.map((dapAn: any, daIndex: number) => {
                                    const isUserAnswer = dapAn.nhan === answer.cauTraLoiText;
                                    const isCorrectAnswer = dapAn.dung === true;
                                    
                                    return (
                                      <div
                                        key={dapAn.id || daIndex}
                                        className={`flex items-start space-x-2 p-2 rounded ${
                                          isUserAnswer && isCorrectAnswer
                                            ? "bg-green-100 border border-green-300"
                                            : isUserAnswer && !isCorrectAnswer
                                            ? "bg-red-100 border border-red-300"
                                            : isCorrectAnswer
                                            ? "bg-green-50 border border-green-200"
                                            : "bg-white border border-gray-200"
                                        }`}
                                      >
                                        <span className="font-semibold text-gray-900 min-w-[24px]">
                                          {dapAn.nhan}.
                                        </span>
                                        <span className="text-gray-900 flex-1">
                                          {dapAn.noiDung}
                                        </span>
                                        {isUserAnswer && (
                                          <span className="text-xs font-semibold text-blue-600">
                                            (Bạn chọn)
                                          </span>
                                        )}
                                        {isCorrectAnswer && (
                                          <CheckCircle className="w-4 h-4 text-green-600 flex-shrink-0" />
                                        )}
                                      </div>
                                    );
                                  })}
                                </div>
                              </div>
                            )}

                            {/* User's Answer Summary */}
                            <div className="flex items-center justify-between pt-2 border-t border-gray-300">
                              <div>
                                <span className="text-sm text-gray-600">Bạn chọn: </span>
                                <span className="text-sm font-bold text-gray-900">
                                  {answer.cauTraLoiText || "Không trả lời"}
                                </span>
                              </div>
                              {!answer.dung && dapAnDung && (
                                <div>
                                  <span className="text-sm text-gray-600">Đáp án đúng: </span>
                                  <span className="text-sm font-bold text-green-600">
                                    {dapAnDung.nhan}
                                  </span>
                                </div>
                              )}
                            </div>

                            {/* Explanation */}
                            {dapAnDung?.giaiThich && (
                              <div className="bg-blue-50 border border-blue-200 rounded p-3">
                                <p className="text-sm font-medium text-blue-900 mb-1">💡 Giải thích:</p>
                                <p className="text-sm text-blue-800">{dapAnDung.giaiThich}</p>
                              </div>
                            )}
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}

              <button
                onClick={() => setShowDetailModal(false)}
                className="w-full bg-gray-900 text-white px-6 py-3 rounded-lg hover:bg-gray-800 transition-colors"
              >
                Đóng
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
